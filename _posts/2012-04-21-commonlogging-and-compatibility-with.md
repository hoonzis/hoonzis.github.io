---
layout: post
title: Common.Logging and compatibility with other libraries
date: '2012-04-21T07:28:00.000-07:00'
author: Jan Fajfr
tags:
- ".NET"
- ASP.NET
- C#
modified_time: '2014-06-26T14:21:48.886-07:00'
thumbnail: http://lh5.ggpht.com/-j2tpEoC1B1o/T40P8SuuK5I/AAAAAAAAATo/QPH01JPFxQM/s72-c/image_thumb.png?imgmax=800
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-6833152346207990177
blogger_orig_url: http://hoonzis.blogspot.com/2012/04/commonlogging-and-compatibility-with.html
---
It has been the second time since I have run into the issue of
configuring correctly Common.Logging on my project. So what is the
problem? Let's start with the basics:

**Common.Logging** should be a generic interface for logging which can
be used by other frameworks and libraries to perform logging. The final
user (you or me) uses several frameworks in his final application and if
all of these frameworks will use different logging framework it will
turn into configuration nightmare.So our favorite frameworks such as
Spring.NET, Quartz.NET are using Common.Logging. This interface in turn
uses a concrete logging framework to perform the logging (the act of
writing the log lines to somewhere).

Typical scenario can be for instance **Common.Logging** and **Log4Net**
combination. In our application configuration file (*web.config or
app.config)* we have to configure Common.Logging to use the Log4Net and
than we can continue with the Log4Net configuration specifying what
should be logged.

``` 
<common>
<logging>
  <factoryAdapter type="Common.Logging.Log4Net.Log4NetLoggerFactoryAdapter, Common.Logging.Log4Net">
 <arg key="configType" value="INLINE" />
  </factoryAdapter>
</logging>
</common>

<log4net>
<appender name="ConsoleAppender" type="log4net.Appender.ConsoleAppender">
  <layout type="log4net.Layout.PatternLayout">
 <conversionPattern value="%date %-5level %logger - %message%newline"/>
  </layout>
</appender>
</log4net>
```

My general problem is that **Common.Loggin.Log4Net** **facade is looking
for a concrete version of the Log4Net library**. Concretely the version:
'log4net (= 1.2.10)'. That is not a problem if you are not using some
other framework which depends on higher version of Log4Net.
In my case the **le\_log4net** library (the
[logentries](https://logentries.com/doc/appharbor/) library) is using
log4net 2.0. So if you are using NuGet, you might obtain the following
exception while adding the references:



[![image](http://lh5.ggpht.com/-j2tpEoC1B1o/T40P8SuuK5I/AAAAAAAAATo/QPH01JPFxQM/image_thumb.png?imgmax=800 "image")](http://lh4.ggpht.com/-LUsxB0LPQ_o/T40P7mf0eyI/AAAAAAAAATg/-ac99BOYZSA/s1600-h/image2.png)



The similar thing might happen if you just decide to use the latest
Log4Net by default. Then you might get an exception when initializing
Spring.NET context or starting the Quartz.NET scheduler:



*Could not load file or assembly 'log4net, Version=1.2.0.30714,
Culture=neutral, PublicKeyToken=b32731d11ce58905' or one of its
dependencies. The located assembly's manifest definition does not match
the assembly reference. (Exception from HRESULT: 0x80131040)*



### Solution 1: Ignore NuGet, define Runtime Binding



One way to get around this is to define runtime assembly binding. But
this solution forces you to add the reference to log4net manually. NuGet
controls the version and wont let you at references on the fly the way
that you would. So to get over add the latest Common.logging.Log4net
façade and Log4Net version 2 (which you need for some reason). Than you
have to define the assembly  binding in the configuration file.

``` 
<runtime>
<assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
  <dependentAssembly>
 <assemblyIdentity name="Common.Logging" publicKeyToken="af08829b84f0328e"/>
 <bindingRedirect oldVersion="1.2.0.0" newVersion="2.0.0.0"/>
  </dependentAssembly>
</assemblyBinding>
</runtime>
```



### Solution 2: Just use the older version of Log4Net (1.2.10)



If you do not have libraries that are dependent on Log4Net version 2.0.0
than just remember to always use log4net 1.2.10. This is the version
which Common.Logging.Log4Net is looking for. Or just let NuGet manage it
for you. You can add Common.Logging.Log4Net via NuGet and it will
automatically load the correct version of Log4Net.



### Solution 3: Try other logging library for instance NLog



This actually is not a real solution. I have experienced similar issues
while using NLog, concretely try to use the latest NLog library with the
Common.Logging.Nlog façade and you will obtain something similar to:



*{"Could not load file or assembly 'NLog, Version=1.0.0.505,
Culture=neutral, PublicKeyToken=5120e14c03d0593c' or one of its
dependencies. The located assembly's manifest definition does not match
the assembly reference. (Exception from HRESULT: 0x80131040)":"NLog,
Version=1.0.0.505, Culture=neutral, PublicKeyToken=5120e14c03d0593c"}*



The solution here is similar, you will have to define Runtime Binding:

``` 
<runtime>
<assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
  <dependentAssembly>
 <assemblyIdentity name="NLog" publicKeyToken="5120e14c03d0593c" culture="neutral" />
 <bindingRedirect oldVersion="0.0.0.0-2.0.0.0" newVersion="2.0.0.0" />
  </dependentAssembly>
</assemblyBinding>
</runtime>
```



What was interesting here, is that NuGet actually took care of this for
me. I have just added the Common.Logging.NLog façade and I guess NuGet
spotted that I have already NLog 2 and that this Runtime Binding is
necessary. If you look at the [documentation of
bindingRedirect](http://msdn.microsoft.com/en-us/library/eftw1fys(v=vs.71).aspx)
you will see, that we have the right to specify the range of versions in
the oldVersion attribute. Here all the version will be bound to the
2.0.0.0 version.



### Summary



Anyway NLog and Log4Net are both cool logging frameworks, just use the
one you prefer. As I have showed  above it is possible to use them
together with Common.Logging it just takes a few more lines to configure
it correctly.

[CodeProject](http://www.codeproject.com/script/Articles/BlogFeedList.aspx?amid=honga)
