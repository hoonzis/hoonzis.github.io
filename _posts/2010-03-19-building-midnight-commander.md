---
layout: post
title: Building Midnight-Commander
date: '2010-03-19T17:25:00.000-07:00'
author: Jan Fajfr
tags:
- Linux
modified_time: '2010-08-31T13:59:00.446-07:00'
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-6234917587551473974
blogger_orig_url: http://hoonzis.blogspot.com/2010/03/errors-during-configure.html
---
I was trying to compile Midnight-Commander from source and get it to run. I downloaded the source files using Git and than went to the project folder and run the **./configure** script. Because it was on my
newly installed OpenSuse machine I run into following errors and solved
them by installing required components:

```
maint/autopoint: line 421: /usr/share/gettext/archive.tar.gz: No such
file or directory
tar: This does not look like a tar archive
tar: Exiting with failure status due to previous errors
cvs checkout: cannot find module \`archive' - ignored
find: \`archive': No such file or directory
find: \`archive': No such file or directory
find: \`archive': No such file or directory
autopoint: \* infrastructure files for version 0.14.3 not found;
this is autopoint from GNU gettext-tools 0.17
autopoint: \* Stop.
```

**install gettext-tools**

./autogen.sh: line 55: aclocal: command not found

**install autogen & automake**

./autogen.sh: line 48: libtoolize: command not found

**install libtool**

checking for GLIB... no
configure: error: glib-2.0 not found or version too old (must be &gt;=
2.8)

**install glib2-devel**

checking for slang/slang.h... no
configure: error: Slang header not found

**Install slang and slang-devel**

After that I was able to run MAKE and the run the compiled project.
