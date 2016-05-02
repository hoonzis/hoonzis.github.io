---
layout: post
title: Paket referencing GitHub files
date: '2015-10-21T09:44:00.000-07:00'
author: Jan Fajfr
tags:
- FSharp, Visual Studio, Paket
modified_time: '2015-10-21T01:07:44.866-07:00'
---
Paket is a dependency manager for .NET projects, besides referencing NuGet packages, one can reference as well any file from GitHub. Any file from GitHub just by adding single line to **paket.dependencies** files. Such files would however be added as links to the solution and if they should be picked up by IIS or other web server they need to be copied to it's location in the solution. Here is how to make sure the files will be copied to their location.


#### Referencing a file from Github
Adding a reference to file on github, is just a matter of editing **paket.dependency** file.

*github hoonzis/KoExtensions src/KoExtensions.js*

Then in the **paket references** of the project that should reference the file, one has to specify to which folder in the solution this file should be moved.

*File:KoExtensions.js scripts*
