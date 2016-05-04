---
layout: post
title: Eclipse Indigo + Maven 3 + Tomcat debugging
date: '2011-10-16T02:48:00.000-07:00'
author: Jan Fajfr
tags:
- Java
- J2EE
- Maven
modified_time: '2014-06-26T14:46:02.414-07:00'
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-2268583853707437924
blogger_orig_url: http://hoonzis.blogspot.com/2011/10/eclipse-indigo-maven-3-tomcat-debugging.html
---
I had a little hard time to see how to debug a Web project with Maven
structure using Eclipse and build-in Tomcat debugging.

The problem is that Maven project has different structure thant Elicpses
Dynamic Web Project, so finally eclipse does not know hot to package a
WAR file and deploy it to the server.

Finally I found [this blog](http://horrikhalid.wordpress.com/2011/01/24/you-want-to-debug-your-maven-project-with-embedded-tomcat-in-eclipse-now-its-easy/),
which explains the issue. This blog describes the situation when using
Maven 2 and Eclipse Helios.

I use Maven 3 and Eclipse Indigo. When using Indigo, not all the steps
described in the blog are needed (no changement of .project file
needed). Basically you have to perform two steps.

1) Facets - change project nature. Go to the properties of the project
and **Project Facets** and choose **Dynamic Web Model.** This step will
change the structure of your project - add the **WebContent** folder.

2) Change the **org.eclipse.wst.common.component** file. This file is
added when you have changed the project structure.


```xml
<?xml version="1.0" encoding="UTF-8"?>
<project-modules id="moduleCoreId" project-version="1.5.0">
<wb-module deploy-name="ProjectName">
<wb-resource deploy-path="/" source-path="/WebContent" tag="defaultRootSource"/>
<wb-resource deploy-path="/WEB-INF/classes" source-path="/src/main/java"/>
<wb-resource deploy-path="/" source-path="/src/main/webapp"/>
<wb-resource deploy-path="/WEB-INF/lib" source-path="/target/ProjecName/WEB-INF/lib"/>
<property name="context-root" value="ProjectName"/>
<property name="java-output-path" value="/ProjectName/target/classes"/>

</wb-module>
</project-modules>
```


What we need is to copy the everything we need into the WebContent
directory. There are parts which you can copy directly from your project
(jsp files, web.xml) and than parts which you have to copy from the
Maven build output (libs, classes).

If you are lost, take a look at the [mentioned blog.](http://horrikhalid.wordpress.com/2011/01/24/you-want-to-debug-your-maven-project-with-embedded-tomcat-in-eclipse-now-its-easy/)
