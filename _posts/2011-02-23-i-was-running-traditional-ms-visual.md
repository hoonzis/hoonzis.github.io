---
layout: post
title: NumberFormatInfo ReadOnly
date: '2011-02-23T14:18:00.000-08:00'
author: Jan Fajfr
tags:
- ".NET"
- C#
modified_time: '2014-06-27T02:55:19.237-07:00'
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-7285824319203390560
blogger_orig_url: http://hoonzis.blogspot.com/2011/02/i-was-running-traditional-ms-visual.html
---
I was running traditional MS Visual Studio 2008 Unit Tests and the test
of the following method was always failing. On the first look I did not
why but from the fact that when I placed the breakpoint on the return
line it never got hit, I figured out that there must have been exception
before.

As this was running in the Unit Test, the exception was not thrown but
just shown in the Error Message column in the Test Results table.



``` 
public String GetBalance(String globalBalance){
  NumberFormatInfo formatInfo = NumberFormatInfo.InvariantInfo;
  formatInfo.CurrencySymbol = "$";
  return globalBalance.ToString("C", formatInfo);
}
```

I obtained the following Error Message:

``` 
System.InvalidOperationException: Instance is read-only..
```

Ok now it was clear. I was trying to change the InvariantFormat which
surely was not allowed. The right way of doing things would be:

Since I was looking at the Stack Trace, I decided just to look a bit
deeper:

``` 
at System.Globalization.NumberFormatInfo.VerifyWritable()
   at System.Globalization.NumberFormatInfo.set_CurrencySymbol(String value)
   at 
```

Lets take a look at the source code of the NumberFormatInfo class and
search for the CurrencySymbol property, for example [at this
site&gt;](http://reflector.webtropy.com/default.aspx/Net/Net/3@5@50727@3053/DEVDIV/depot/DevDiv/releases/whidbey/netfxsp/ndp/clr/src/BCL/System/Globalization/NumberFormatInfo@cs/1/NumberFormatInfo@cs)

``` 
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

Ok so any time the property is set (any of the properties of
NumberInfoFormat) the VerifyWritable method is called. And this method
just checks the private field isReadOnly. This makes sence. This
property is set up by the ReadOnly static method. So any time you will
need a read only instance of NumberFormatInfo you can just call:

``` 
NumberFormatInfo myReadOnlyInstance = NumberFormatInfo.ReadOnly(formatInfo);
```
