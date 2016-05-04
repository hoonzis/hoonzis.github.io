---
layout: post
title: TFS and crashing QTagent32
date: '2011-03-31T01:26:00.000-07:00'
author: Jan Fajfr
tags:
- Testing
- Visual Studio
- Source Control
modified_time: '2012-10-27T06:42:00.033-07:00'
---
I made a bug in my code which resulted in never-ending recursion which was produced only while UnitTesting. This might result into QTAgent32 crash locally. However if you have previously committed such code to TFS you might have to restart **VSPerfMon** process on TFS server.

When there is recursion in unit test than the **QTAgent32 crashes down**,  this is described [here](http://connect.microsoft.com/VisualStudio/feedback/details/465633/qtagent32-exe-crashes-when-running-a-unit-test-that-calls-itself).

But if you commit to TFS than and this commits launches the UnitTests
than these will never finish (so if you do not check than you might just have builds running for couple days and new builds in the queue behind).

However when you cancel the build and run a new one, this one will fail,
saying:

**'data.coverage' because it is being used by another process.**

So there is some process which has locked the "data.coverage". To solve this issue log on the TFS and kill the process VSPerfMon.exe which runs under NT AUTHORITY\\NETWORK SERVICE (that should be the account which is running your builds & unit tests on TFS).
