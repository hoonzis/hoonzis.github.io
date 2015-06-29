---
layout: post
title: System.IO.PathTooLongException while opening InfoPath forms in SharePoint
date: '2010-03-11T05:42:00.000-08:00'
author: Jan Fajfr
tags:
- SharePoint
- InfoPath
modified_time: '2010-03-11T07:35:23.091-08:00'
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-5483396861207742640
blogger_orig_url: http://hoonzis.blogspot.com/2010/03/systemiopathtoolongexception-while.html
---
When opening a InfoPath form in SharePoint, you may get int he ULS log
the following exception:

Unhandled exception when rendering form System.IO.PathTooLongException:
The specified file or folder name is too long. The URL path for all
files and folders must be 260 characters or less (and no more than 128
characters for any single file or folder name in the URL). Please type a
shorter file or folder name.

Some general info about the exception can be found
[here](http://support.microsoft.com/kb/894630/en-us).

It is caused by the **MSO-INFOPATHSOLUTION** directive in the head of
the xml definition of the form. The attribute **HREF** of this directive
can not exceed the size of 260 characters.

Our InfoPath forms where designed as InfoPath 2003 forms, and the
**HREF** attribute was really long:


``` 
<mso-infoPathSolution solutionVersion="1.0.0.414" productVersion="12.0.0" PIVersion="1.0.0.0" href="http://intranet/teamwebs/informatics/EvidencepoadavkWF/Forms/template.xsn?SaveLocation=http://intranet/teamwebs/informatics/EvidencepoadavkWF/&amp;Source=http://intranet/teamwebs/informatics/Evidence%2520poadavk%2520WF/Forms/Nedokonen%2520poadavky.aspx&amp;OpenIn=Browser&amp;NoRedirect=true&amp;XsnLocation=http://intranet/teamwebs/informatics/EvidencepoadavkWF/Forms/template.xsn" name="urn:schemas-microsoft-com:office:infopath:Evidence-ponadavk--WF:-myXSD-2008-10-03T13-14-12" ?>
```



but if you strip it down, you can see that a simple link to the file is
enough.


``` 
href="http://intranet/teamwebs/informatics/EvidencepoadavkWF/Forms/template.xsn"
```



The forms designed in InfoPath 2007 do not generate the href attribute
that long. Also note if you just want the forms to open in the client
application(InfoPath), you can set this on the advanced settings of the
Document Library, in which the forms are contained. But even there, if
there would be link to the file outside of this library, or if you will
access the direct link, SharePoint will always try to open it in Web
Browser.

This forum post had led me to the solution:
http://www.k2underground.com/forums/p/8709/26302.aspx\#26302
