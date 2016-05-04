---
layout: post
title: RSA implementation using GMP library
date: '2010-10-15T08:32:00.001-07:00'
author: Jan Fajfr
tags:
- Security
- Computer Science
modified_time: '2012-02-14T02:14:02.117-08:00'
---
Right now I am studying in Paris in one of the engineering schools here and one of my last assignments of cryptography was to implement the RSA in C. To achieve this and to allow manipulation of big integers I have used GMP library which is an open source library for arithmetic.

One part of the assignment was also an implementation of [**Miller - Rabin primarity test**](http://en.wikipedia.org/wiki/Miller%E2%80%93Rabin_primality_test) and implementation of [**Right - to - left binary method**](http://en.wikipedia.org/wiki/Modular_exponentiation) to perform Modular exponentiation. These two algorithms are already implemented in GMP so if you just want to implement RSA you can use my source code and modify it so it will use the functions from GMP instead of my implementations.

You can download the [source code here.](http://www.super6.cz/downloads/blog/crypto/tp4.zip)

If you speak French you can also download my [poor-french-written report](http://www.super6.cz/downloads/blog/crypto/report.pdf).
