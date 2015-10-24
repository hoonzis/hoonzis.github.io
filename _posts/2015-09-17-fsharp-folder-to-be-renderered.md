---
layout: post
title: F# folder rendered multiple times
date: '2015-09-17T09:44:00.000-07:00'
author: Jan Fajfr
tags:
- FSharp, Visual Studio
modified_time: '2015-09-17T01:07:44.866-07:00'
---

I was recently testing the [ASP.MVC 5 F# template](https://visualstudiogallery.msdn.microsoft.com/39ae8dec-d11a-4ac9-974e-be0fdadec71b), which works fine but almost always breaks down as soon as you add new item to the solution.
F# has hard time with folders. The problem has been described [here] (http://stackoverflow.com/questions/22845020/visual-studio-f-project-cant-have-two-folders-in-a-file-tree-with-the-same-na) . And as soon as you add new file to the F# project you will run to the same problem and the next time, you won't be able to load the project file with:

```
The project '...' could not be opened because opening it would cause a folder to be rendered multiple times in the solution explorer.
```

The suggestion proposed solution si to forgot the nested folders and just rename the files, but there is a better way around. Here is the problem:

```xml
<Content Include="Views\Web.config" />
<Content Include="Views\Home\Index.cshtml" />
<Content Include="Views\Shared\_Layout.cshtml" />
<Content Include="Views\Home\PayoffCharts.cshtml" />
```

In real F# can handle nested folders, but all files in the same folder have to be consecutive in the items group. This will fix the problem:
```xml
<Content Include="Views\Web.config" />
<Content Include="Views\Home\Index.cshtml" />
<Content Include="Views\Home\PayoffCharts.cshtml" />
<Content Include="Views\Shared\_Layout.cshtml" />
```

Note that the same applies for any nested level. The following would not work, because thoug all items in *Shared* folter are aligned, the items in the *Another* subfolder are not.

```
<Content Include="Views\Home\PayoffCharts.cshtml" />
<Content Include="Views\Shared\Another\_Layout.cshtml" />
<Content Include="Views\Shared\Test2\_Layout.cshtml" />
<Content Include="Views\Shared\Another\_Layout.cshtml" />
```
	