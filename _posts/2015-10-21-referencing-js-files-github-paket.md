Paket is a dependency manager for .NET projects, besides referencing NuGet packages, one can reference as well any file from GitHub. One can reference any file from GitHub just by adding single line to **paket.dependencies** files. Such files would however be added as links to the solution and if they should be picked up by IIS or other web server they need to be copied to it's location in the solution.

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
