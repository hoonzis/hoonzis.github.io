---
layout: post
title: FSharp.Data and Unit Testing issues
date: '2015-01-27T04:53:00.001-08:00'
author: Jan Fajfr
tags:
- FSharp
modified_time: '2015-04-22T02:42:06.483-07:00'
thumbnail: http://2.bp.blogspot.com/-bhTeegAseTs/VMeJ7PPap_I/AAAAAAAAEMM/Dab5HZms5rM/s72-c/reference_fsharpdata.PNG
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-7292279599877960101
blogger_orig_url: http://hoonzis.blogspot.com/2015/01/ncrunch-and-fsharpdata.html
---
I have recently run into two separate issues while testing some F\# data
providers based code. I am using ReSharper's NUnit runner and sometimes
NCrunch.

NCrunch issues
--------------

NCrunch won't compile your solution when FSharp.Data is referenced. This
component internally references FSharp.Data.DesignTime which has to be
avaiable for the compilation - and NCrunch does not have those libs
available, because the DLL is not referenced the standard way, but must
be provided by Visual Studio.

The current solution is to reference FSharp.Data.DesignTime manually. If
you are using nuget, than the DLL can be found in the pacakages foder as
shown bellow:



[![](http://2.bp.blogspot.com/-bhTeegAseTs/VMeJ7PPap_I/AAAAAAAAEMM/Dab5HZms5rM/s640/reference_fsharpdata.PNG)](http://2.bp.blogspot.com/-bhTeegAseTs/VMeJ7PPap_I/AAAAAAAAEMM/Dab5HZms5rM/s1600/reference_fsharpdata.PNG)



Note that I have tested and had the same issue with FSharp.Data 2.1.0
and 2.2.0.

NUnit runner issues
-------------------

NUnit did load the test correctly, but the test failed with
**SystemMissingMethodException**

Luckily the solution [can be found on
StackOverflow](http://stackoverflow.com/questions/22608519/fsharp-data-system-missingmethodexception-when-calling-freebase-provider-from)
and was caused by the tested library being Portable Class Library
instead of standard F\# library.
