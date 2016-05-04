---
layout: post
title: Emulation of Azure needs registered ASP.NET 4
date: '2011-07-22T09:49:00.000-07:00'
author: Jan Fajfr
tags:
- IIS
- Azure
modified_time: '2011-07-22T09:49:10.765-07:00'
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-8060563881874695079
blogger_orig_url: http://hoonzis.blogspot.com/2011/07/emulation-of-azure-needs-registered.html
---
While debugging Azure Web Role I have obtained error while Visual Studio
debugger was attaching to the emulator.

Here is a good blog which describes hot to diagnose this error:

<http://dunnry.com/blog/2011/07/14/HowToDiagnoseWindowsAzureErrorAttachingDebuggerErrors.aspx>

At the core the issue was:

**Handler "PageHandlerFactory-Integrated" has a bad module
"ManagedPipelineHandler" in its module list**

I quickly found out what was the issue. Later to perform some testing I
have uninstalled ASP.NET 4 extensions from my IIS and kept just the 3.5
versions (actually 2.0, because 3.5 were just framework extensions). So
a quick solution **aspnet\_regiis.exe -i** in 4 version folder will fix
it.
