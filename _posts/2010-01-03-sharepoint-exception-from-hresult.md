---
layout: post
title: 'SharePoint - Exception from HRESULT: 0x80020009 (DISP_E_EXCEPTION)'
date: '2010-01-03T01:07:00.000-08:00'
author: Jan Fajfr
tags:
- SharePoint
modified_time: '2010-02-07T05:09:41.151-08:00'
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-6963231788914646923
blogger_orig_url: http://hoonzis.blogspot.com/2010/01/sharepoint-exception-from-hresult.html
---
In one of my WebParts I was using this little piece of code to get the
current user from SharePoint:

``` 
//values I wanted to work with
String name, email;

//get the currnet SPWeb
//THE PROBLEM: You should not dispose after calling GetContextWeb Method
using (SPWeb lWeb = SPControl.GetContextWeb(this.Context))
{
  SPUser lUser = lWeb.CurrentUser;
  name = lUser.Name;
  email = lUser.Email;
}
```


after executing this I've got the folowing exception:

(Exception from HRESULT: 0x80020009 (DISP\_E\_EXCEPTION))
Description: An unhandled exception occurred during the execution of the
current web request. Please review the stack trace for more information
about the error and where it originated in the code.
Exception Details:
Microsoft.SharePoint.WebPartPages.WebPartPageUserException: Došlo k
výjimce. (Exception from HRESULT: 0x80020009 (DISP\_E\_EXCEPTION))
Source Error:
An unhandled exception was generated during the execution of the current
web request. Information regarding the origin and location of the
exception can be identified using the exception stack trace below.

I have googled a bit and found the solution:

``` 
//get the currnet user
SPUser lUser = SPContext.Current.Web.CurrentUser;
//work with it as you need
name = lUser.Name;
email = lUser.Email;
```


In the [MSDN Best
Practices](http://msdn.microsoft.com/en-us/library/aa973248.aspx) you
can find, that when using the **SPControl.GetContextWeb()** or
**SPControl.GetContextSite()** method you should not dispose the created
objects.
