---
layout: post
title: Catalina - Life cycle exception
date: '2011-10-12T16:14:00.000-07:00'
author: Jan Fajfr
tags:
- Java
- J2EE
modified_time: '2014-06-26T14:46:34.429-07:00'
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-3145146715838380579
blogger_orig_url: http://hoonzis.blogspot.com/2011/10/catalina-life-cycle-exception.html
---
Last project: Tomcat and Java Web applicaiton.
Suddenly after just after adding some dependencies using Maven, I was
not able to start the server. The exception did not give me much
detaisl:

```
Grave: ContainerBase.addChild: start:
org.apache.catalina.LifecycleException: Failed to start component [StandardEngine[Catalina].StandardHost[localhost].StandardContext[/Bank]]
at org.apache.catalina.util.LifecycleBase.start(LifecycleBase.java:152)

Caused by: org.apache.tomcat.util.bcel.classfile.ClassFormatException: Invalid byte tag in constant pool: 60
at org.apache.tomcat.util.bcel.classfile.Constant.readConstant(Constant.java:131)
at org.apache.tomcat.util.bcel.classfile.ConstantPool.(ConstantPool.java:60)

oct. 07, 2011 2:26:19 PM org.apache.catalina.startup.HostConfig deployDirectory

Grave: Erreur lors du déploiement du répertoire Bank de l'application web
java.lang.IllegalStateException: ContainerBase.addChild: start: org.apache.catalina.LifecycleException: Failed to start component [StandardEngine[Catalina].StandardHost[localhost].StandardContext[/Bank]]
at org.apache.catalina.core.ContainerBase.addChildInternal(ContainerBase.java:816)
```


So after checking the state of web.xml (no changes there) I started to
take a look at te dependencies which I have added. Well actually I took
a look at the JARs which were being donwloaded automatically by Maven.
I found out that when I added a dependency to Jaxen library, than
several JARs war downloaded.

I started deleting one by one and redeploying, just to find out, that it
was ICU4J.JAR which was causing the problem. Well I was sure that I did
not need it, so I solved the problem by declaring Maven exclusion.

```xml
<dependency>
<groupid>jaxen</groupId>
  <artifactid>jaxen</artifactId>
  <version>1.1.1</version>
  <exclusions>
   <exclusion>
 <groupid>com.ibm.icu</groupId>
    <artifactid>icu4j</artifactId>
   </exclusion>
  </exclusions>
  <scope>runtime</scope>
</dependency>
```

Couriously enough I needed Jaxen in order to be able to use the
Azure4Java tools and access to Azure table storage.


Still I do not understand the real cause of this problem, so if anyone
finds similar issue, I hope this helps.
