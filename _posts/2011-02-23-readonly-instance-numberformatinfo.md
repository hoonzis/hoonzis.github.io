---
layout: post
title: ReadOnly instance of NumberFormatInfo
date: '2011-02-23T14:18:00.000-08:00'
author: Jan Fajfr
tags:
- .NET
- C#
modified_time: '2014-06-27T02:55:19.237-07:00'
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-7285824319203390560
blogger_orig_url: http://hoonzis.blogspot.com/2011/02/i-was-running-traditional-ms-visual.html
---
When treating cultures, symbols and unites *NumberFormatInfo.InvariantInfo* is an easy way to get the *Invariant culture*. This instance however is read-only and can't be modified. If you attempt to modify it you will obtain *System.InvalidOperationException: Instance is read-only* exception. This took me by surprise and caused my unit tests to fail and so I dug a bit deeper into the issue to understand how the read-only instance is created.

The following method was causing the issue:

```csharp
public String GetBalance(String globalBalance){
  NumberFormatInfo formatInfo = NumberFormatInfo.InvariantInfo;
  formatInfo.CurrencySymbol = "$";
  return globalBalance.ToString("C", formatInfo);
}
```

After looking at my code it was clear that this is buggy. I was trying to change the InvariantFormat which surely was not allowed. The correct way to do things would be to create new culture based on the current cultures thread name.

I decided to check to look how such readonly instance is implemented in .NET. The stack trace gives a bit of info where to look.

```
at System.Globalization.NumberFormatInfo.VerifyWritable()
   at System.Globalization.NumberFormatInfo.set_CurrencySymbol(String value)
   at
```

Lets take a look at the source code of the NumberFormatInfo class and search for the CurrencySymbol property, for example [at this site](http://reflector.webtropy.com/default.aspx/Net/Net/3@5@50727@3053/DEVDIV/depot/DevDiv/releases/whidbey/netfxsp/ndp/clr/src/BCL/System/Globalization/NumberFormatInfo@cs/1/NumberFormatInfo@cs)

```csharp
public String CurrencySymbol {
  get { return currencySymbol; }
  set {
    VerifyWritable();
    if (value == null) {
      throw new ArgumentNullException("CurrencySymbol",
      Environment.GetResourceString("ArgumentNull_String"));
    }
    currencySymbol = value;
  }
}

internal bool isReadOnly=false;

private void VerifyWritable() {
  if (isReadOnly) {
    throw new
InvalidOperationException(Environment.GetResourceString("InvalidOperation_ReadOnly"));
  }
}

public static NumberFormatInfo ReadOnly(NumberFormatInfo nfi) {
  if (nfi == null) {
    throw new ArgumentNullException("nfi");
  }
  if (nfi.IsReadOnly) {
    return (nfi);
  }
  NumberFormatInfo info = (NumberFormatInfo)(nfi.MemberwiseClone());
  info.isReadOnly = true;
  return info;
}
```

Ok so any time the property is set (any of the properties of NumberInfoFormat) the VerifyWritable method is called. And this method
just checks the private field isReadOnly. This makes sense. This property is set up by the ReadOnly static method. So any time you will need a read only instance of NumberFormatInfo you can just call:

```csharp
NumberFormatInfo myReadOnlyInstance = NumberFormatInfo.ReadOnly(formatInfo);
```
