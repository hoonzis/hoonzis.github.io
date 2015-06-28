--- layout: post title: Which Linux install in Virtual PC on Windows 7
date: '2010-03-06T04:22:00.001-08:00' author: Jan Fajfr tags: -
Virtualization modified\_time: '2012-02-14T02:16:51.746-08:00'
blogger\_id:
tag:blogger.com,1999:blog-1710034134179566048.post-1718246012559864587
blogger\_orig\_url:
http://hoonzis.blogspot.com/2010/03/which-linux-install-in-virtual-pc-on.html
--- Because I needed Linux for some school projects regarding opensource
development I have decided to install it as Virtual Machine to Virtual
PC.\
\
First advice - Linux guests are officially not supported for MS Windows
Virtual PC, if you really don't need it don't do it.\
\
Second advice - Virtual PC supports hosting 64bit operating systems (but
as Linux is not supported only Windows) - but in some cases I found that
I was able to run only 32 bit version (OpenSuse, Fedora).\
\
What I have tried so far\
------------------------\
\
**Ubuntu 9.10** - installation ok, but ater "Segmentation fault". After
increasing memory amount from 800Mb to 1024Mb the os started, but the
performance was REALLY slow. [Here is a possible workaround which I
didn't
tested.](http://jagbarcelo.blogspot.com/2009/11/segmentation-fault-ubuntu-virtual-pc.html)\
\
**Fedora 12**\

-   version i686. I was not able to boot the disk. The screen just
    blanked at went of.
-   version i386. The installation started but after selecting "Install
    or upgrade" the virtual machine went off.
-   version x86\_64. this version told me, that I can not use 64bit os
    on 32bit platform (I'am on 64 bits)

\
**Debian 5.04** - Version i386 net install - after starting the
installation the virtual pc went off.\
\
**OpenSuse**\

-   version x86\_64 - this version told me, that I can not use 64bit os
    on 32bit platform (I'am on 64 bits)
-   Version i586 installed fine, but after the "1st configuration" did
    not finish and system did not boot up correctly.

\
**Mandriva 2010** - version i586 - the installation did not start
correctly\
\
**Slackware 13** - installation ok, it works now, but so far in the
textmode.\
\
UPDATE: ok now it works fine with GNOME.\
\
4 basic steps:\
\
1) Install [GNOME version for Slackware.](http://gnomeslackbuild.org/)\
2) Run *xorgsetup* to configure the XSERVER\
3) Run *xwmconfig* to configure which X manager do you want to use -
select GNOME.\
4) Run *xinit* to start X Server\
\
To Install KDE you just need to add the KDEBASE packages (eg. from GNOME
start the package management tool).\
\
If you want to use bigger resolution than 800x600(which is default),
than you will have to modify your xorg.conf file, details can be found
here:\
<http://demiliani.com/blog/archive/2004/03/18/1808.aspx>\
\
\
**Summary**\
OK - I was able to get Ubuntu tu run - but realllly slow. Slackware is
so far working without a problem.
