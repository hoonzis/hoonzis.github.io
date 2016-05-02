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
I have recently run into two separate issues while testing some F\# data providers based code. Both issues were linked to the FSharp Data assembly. I am using ReSharper's NUnit runner and sometimes NCrunch. One of the problem was linked to the availability of *FSharp.Data.DesignTime.dll* on the compile time and other to NUnit not correctly handling Portable Library Class projects. I have tested and had the same issues with FSharp.Data 2.1.0 and 2.2.0.

#### NCrunch issues
blah blah blah
