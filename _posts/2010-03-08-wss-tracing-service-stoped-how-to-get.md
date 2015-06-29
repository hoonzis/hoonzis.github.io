---
layout: post
title: WSS Tracing Service stoped - how to get the LOG info?
date: '2010-03-08T02:49:00.001-08:00'
author: Jan Fajfr
tags:
- SharePoint
- IIS
modified_time: '2012-02-14T03:44:21.033-08:00'
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-6133899916746067811
blogger_orig_url: http://hoonzis.blogspot.com/2010/03/wss-tracing-service-stoped-how-to-get.html
---
Sometimes in the SharePoint LOG, you can get the following information
and nothing else:

Service lost trace events.

This happens when the Windows SharePoint Tracing Service is halted or
not working. Usually you can just restart the service, but sometimes it
won't work. The you can try to run TaskManager and end the process
called "wsstracing.exe".

If that is not going to work you can restart the Internet Information
Server or the whole Server.

But sometimes it is not possible - in the production department when you
need the information of some actual error.

In this situation I use great tool called
**[SPTraceView](http://sptraceview.codeplex.com/)**. This tool will
attach itself as online listener to the SharePoint ULS and will capture
the errors and messages for you. You can just run it and there is no
need to restart the server or IIS.
