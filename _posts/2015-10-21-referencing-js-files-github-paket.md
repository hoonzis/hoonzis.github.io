---
layout: post
title: Referencing JS files from GitHub with Paket
date: '2015-10-21T09:44:00.000-07:00'
author: Jan Fajfr
tags:
- FSharp, Visual Studio, Paket
modified_time: '2015-10-21T01:07:44.866-07:00'
---

Paket is a dependency manager for .NET projects, besides referencing NuGet packages, one can reference as well any file from GitHub. Any file from GitHub just by adding single line to **paket.dependencies** files. Such files would however be added as links to the solution and if they should be picked up by IIS or other web server they need to be copied to it's location in the solution. Here is how to make sure the files will be copied to their location.

#### Referencing a file from Github:

```
github hoonzis/KoExtensions src/KoExtensions.js
```

Then in the **paket references** of the project that should reference the file:

```
File:KoExtensions.js scripts
```

Running **paket update** will update the project file and add the mentioned file as linked reference:

```xml
<Content Include="..\paket-files\hoonzis\KoExtensions\src\KoExtensions.js">
  <Paket>True</Paket>
  <Link>Scripts/KoExtensions.js</Link>
</Content>
```

#### Task to hard copy files
The problem is that if this is simple Content file just as JS file or CSS it won't be picked up by IIS or other hosting web server, because the web server would just read the folder structure, but the file is not there. The solution is proposed [over here](http://mattperdeck.com/post/Copying-linked-content-files-at-each-build-using-MSBuild.aspx)

Adding the following build target will copy the linked files to the location where there are linked to.

```xml
<Target Name="CopyLinkedContentFiles" BeforeTargets="Build">
 <Copy SourceFiles="%(Content.Identity)"
       DestinationFiles="%(Content.Link)"
       SkipUnchangedFiles='true'
       OverwriteReadOnlyFiles='true'
       Condition="'%(Content.Link)' != ''" />
</Target>
```
