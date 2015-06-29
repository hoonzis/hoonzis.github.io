---
layout: post
title: SharePoint and InfoPath - Error while processing the form and Schema validation
  error
date: '2010-05-03T06:06:00.000-07:00'
author: Jan Fajfr
tags:
- SharePoint
- InfoPath
modified_time: '2010-08-04T07:44:12.837-07:00'
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-7020566236609855065
blogger_orig_url: http://hoonzis.blogspot.com/2010/05/sharepoint-and-infopath-2-errors.html
---
Lately I came across these 2 following errors:

**Error 1 - There has been an error while processing the form. There is
an unclosed literal string*

If you will come across this issue try one of following solutions(or
combination):

1) You have some secondary data sources in the form which are not used.
If you need them there (maybe they get use only on click of button...)
than just drag and drop this data source to form and hide it.

2) Check the names of secondary data sources for special characters (
'ěšřž...')

3) There is a KB fix which might help:

http://support.microsoft.com/kb/949752

**Error 2 - Schema validation found non-data type errors*

There is a solution from MS for this issue, when you are editing
InfoPath form CODE:

http://blogs.msdn.com/infopath/archive/2006/11/28/the-xsi-nil-attribute.aspx

However in my case helped just setting the **PreserveWhiteSpaces**
property to TRUE on XmlDocument:


``` 
XmlDocument lDoc = new XmlDocument();
...
lDoc.PreserveWhitespace = true;
lDoc.Save(myOutStream);
```
