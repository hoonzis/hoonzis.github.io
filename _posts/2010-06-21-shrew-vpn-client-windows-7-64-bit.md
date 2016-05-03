---
layout: post
title: Shrew VPN Client Windows 7 - 64 bit
date: '2010-06-21T06:33:00.002-07:00'
author: Jan Fajfr
tags:
- Windows
modified_time: '2012-03-04T10:08:43.870-08:00'
thumbnail: http://4.bp.blogspot.com/_fmvjrARTMYo/TB9vLL82RvI/AAAAAAAAAFo/4N826L85h-U/s72-c/shrew_properties.PNG
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-8055566742777428423
blogger_orig_url: http://hoonzis.blogspot.com/2010/06/shrew-vpn-client-windows-7-64-bit.html
---
Like many other people on the internet I had to use Cisco VPN Client to
connect to our network. First Cisco didn't support 64 bit, now they do
support, but I am unable to install the 64bit client. I get this error
during the installation:

**Installation ended prematurely because of an error.**

Well I am using **Shrew VPN Client** which is freeware and allows to import the Cisco Client **pcf** configuration files.

After the import and first try to connect it ended up just with **"bringing up tunnel ..."**.
I had to make one more manual step: Uncheck and Check again the **Shrew Soft Lightweight Filter** on the connection properties tab. (Configure
the connection which connects you to internet).

[![](http://4.bp.blogspot.com/_fmvjrARTMYo/TB9vLL82RvI/AAAAAAAAAFo/4N826L85h-U/s320/shrew_properties.PNG)](http://4.bp.blogspot.com/_fmvjrARTMYo/TB9vLL82RvI/AAAAAAAAAFo/4N826L85h-U/s1600/shrew_properties.PNG)

After that everything worked ok for me.
