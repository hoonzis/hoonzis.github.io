---
layout: post
title: Installing Ruby & Rails & RVM on Windows 7
date: '2011-11-16T14:37:00.001-08:00'
author: Jan Fajfr
tags:
- Linux
- Rails on Ruby
modified_time: '2014-06-26T14:42:16.868-07:00'
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-7700633139896977904
blogger_orig_url: http://hoonzis.blogspot.com/2011/11/installing-ruby-rails-rvm-on-windows-7.html
---
I write this post, just in case that someone will have the same issues
as I had when installing Ruby & Rails (on Windows) and maybe it will
save someones some hours.

One limitation I had: I would use lots of versions of ruby, so I needed:
RVM. (Ruby Version Manager) which is not available for windows.

My config: Win 7 64 bit

In this case, you have two options:

-   Use cygwin
-   Install on Virtual Machine



### Use Cygwin - did not work for me!

-   Install RVM.
    I started with this option. To install RVM, there is some [great
    help
    here](http://www.tiplite.com/how-to-install-rvm-on-windows-using-cygwin/)
-   Install RubyGem. First [download
    RubyGem](http://rubygems.org/pages/download), than run **ruby
    setup.rb**
-   Install rails by typing: **gem install rails**. And I just got the
    following errors:

``` 
Building native extensions.  This could take a while...
      0 [main] ruby 1192 C:\cygwin\bin\ruby.exe: *** fatal error - unable to rem
ap \\?\C:\cygwin\lib\ruby\1.8\i386-cygwin\etc.so to same address as parent: 0x1B
0000 != 0x210000
      Stack trace:
Frame     Function  Args
023F9BB8  6102796B  (023F9BB8, 00000000, 00000000, 00000000)
023F9EA8  6102796B  (6117EC60, 00008000, 00000000, 61180977)
023FAED8  61004F1B  (611A7FAC, 61243684, 001A0000, 00210000)
End of stack trace
      1 [main] ruby 3856 fork: child 1188 - died waiting for dll loading, errno
11
      0 [main] collect2 3220 fork: child -1 - died waiting for longjmp before in
itialization, retry 10, exit code 0xC0000135, errno 11
ERROR:  Error installing rails:
        ERROR: Failed to build gem native extension.

        /usr/bin/ruby.exe extconf.rb
checking for re.h... yes
checking for ruby/st.h... no
creating Makefile

make
gcc -I. -I/usr/lib/ruby/1.8/i386-cygwin -I/usr/lib/ruby/1.8/i386-cygwin -I. -DHA
VE_RE_H    -g -O3   -Wall  -c parser.c
gcc -shared -s -o parser.so parser.o -L. -L/usr/lib -L.  -Wl,--enable-auto-image
-base,--enable-auto-import,--export-all   -lruby  -ldl -lcrypt
collect2: fork: Resource temporarily unavailable
      0 [main] collect2 3220 fork: child -1 - died waiting for longjmp before in
itialization, retry 10, exit code 0xC0000135, errno 11
make: *** [parser.so] Error 1
```

**And I never got over this issue. And I tried quite long enough. So if
you are running the same config as I. Be aware you might end up like
this...*

### Installing on Ubuntu 11 in VMWare

This should be just a piece of cake I thought.

-   Ubuntu comes with Ruby already installed.
-   Install RVM: **sudo apt-get install ruby-rvm**
-   Install RubyGems: **sudo apt-get install rubygems**
-   Install Rails**sudo gem install rails**
-   And Bundle: **sudo gem install bundle**

I needed some special libraries for our application, which had some
prerequisites which I did not have:
**To install "nokogiri" I had to do:*

    sudo apt-get install libxslt-dev
    sudo gem install nokogiri


**To install "rmagick":*

    sudo apt-get install libmagicwand-dev
    sudo gem install rmagick


Now I did run "bundle" on the application, which actually did finish.
But with some warnings, and the application did not run. So I started
with cleaning up the warnings:

**First warning:*


    Invalid gemspec in [/var/lib/gems/1.8/specifications/capybara-1.1.1.gemspec]: invalid date format in specification: "2011-09-04 00:00:00.000000000Z"
    Invalid gemspec in [/var/lib/gems/1.8/specifications/polyamorous-0.5.0.gemspec]: invalid date format in specification: "2011-09-03 00:00:00.000000000Z"

[Apparently quite
common](http://stackoverflow.com/questions/5771758/invalid-gemspec-because-of-the-date-format-in-specification)
and apparently for everyone there is different solution. For me what
worked was:
*
**

    sudo gem install rubygems-update
    sudo update_rubygems 



**Another strange warning which I was getting:*

    ERROR:  While executing gem ... (Gem::DocumentError)
        ERROR: RDoc documentation generator not installed: no such file to load -- json


Solved by:
*
**

    gem install rdoc-data
    rdoc-data --install


After that I had to reinstall "bundle". And gem actually reinstalled all
dependencies. But after that, it run!!

**UPDATE:*
Also I needed to access to my application from our network. VMWare has
two options for setting up network: NAT and Bridged. Bridged interface
did not work with Ubuntu 11 (don't know why).
So when you are in NAT mode and you need to access to your VM, you will
need to configure port forwarding.
[Here is good way to set it
up.](http://asunix.tufts.edu/howto/vmware/portforwardingWin7)

I get that Ruby is not Windows friendly, but on Ubuntu it was not much
better, until I solved those strange warnings, nothing worked correctly
- and that was clean install, I mean a clean machine! With ruby
pre-installed. I was just adding RubyGem and Rails...it took me a half a
day...
