---
layout: post
title: Wrong SystemRoot variable causes 0xC0150004
date: '2012-03-04T10:05:00.000-08:00'
author: Jan Fajfr
tags:
- Windows
modified_time: '2012-03-04T10:05:31.222-08:00'
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-3498587474896128605
blogger_orig_url: http://hoonzis.blogspot.com/2012/03/wrong-systemroot-variable-causes.html
---
Last week I had experienced this quite "funny" situation on WinXP
machine.

When I have run some post-build scripts using Jenkings & Maven I have
obtained something like:

CMD is not known, not an executable file etc...

I took a look at it and I taught to myself - the "SystemRoot" variable
is not set! I have to correct this and it will work fine. Actually this
is not true, but apparently it Jenkins does some cleaning in the
variables, so it might happend, that the scripts runned by Jenkins will
not work as you have expected. Anyway that is not the point of my story.
So what I did to "correct" the error was to add the following variable
to my USER environement variables:

**SystemRoot = SystemRoot\\System32*

I was too quick and too naive. First the SystemRoot had been already
defined in the System variables and second the correct path is just
C:\\Windows. So I have used this recursion to define the varible - in
wrong path.

When I have saved my master work, I could not execute almost any new
program. I have always obtained the following error:

**0xC0150004*

So after a fast google I have found out, that this error appears when
the SystemRoot variable is not correctly set. Cool so how to fix the
mistake? You cannot launch any program or system utility at all and you
need to correct the environement variable.

Well after a wail I have found this solution:

-   Login as another user
-   Start the CMD as the user with the wrong environement variable
-   Correct the variable using: SET SystemRoot=C:\\Windows
-   Now you think it is ok, but after a restart you will find out, that
    the new settings is never taken into account. So do not bother to
    restart and try the following:
-   Run REGEDIT from the CMD which is opened using the account and where
    ther correct SystemRoot was set. Notice it will run....and before it
    was failing because regedit is dependent on that SystemRoot
    variable.
-   Find the user variables searching here:
    HKEY\_CURRENT\_USER\\Environment
-   Now correct the SystemRoot variable and voila after restart you can
    log in again with the "infected account"




The moral of the story:
-----------------------


Lot of applications (notepad, regedit, explorer, task manager) would not
start if the SystemRoot is not set correctly.

WinXP will let you set the variable to such a nonsence as:
**SystemRoot = SystemRoot\\System32**
