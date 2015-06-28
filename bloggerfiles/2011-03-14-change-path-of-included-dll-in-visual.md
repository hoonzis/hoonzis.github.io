--- layout: post title: Change path of included DLL in Visual Studio
2010 date: '2011-03-14T14:44:00.000-07:00' author: Jan Fajfr tags: -
Visual Studio - Source Control modified\_time:
'2012-10-27T06:42:43.340-07:00' blogger\_id:
tag:blogger.com,1999:blog-1710034134179566048.post-8712663878438153595
blogger\_orig\_url:
http://hoonzis.blogspot.com/2011/03/change-path-of-included-dll-in-visual.html
--- Visual Studio 2010 remembers the previous locations of the DLL, and
in some cases prefers the "standard" locations of the assemblies.\
In one of our projects we have to add System.Windows.Interaction
assembly which you can find in the .NET tab of the Add Reference dialog
(well only if you had installed Expression studio before).\
But we use Team Foundation Server as Source Control and Continous
Integration solution and while this assembly was missing on the TFS
server, the "after commit" build did not succeed.\
\
So we took out the reference, copied the DLL to the Source Control and
added the reference directly to this DLL. But somehow Visual Studio
remembers the original location of the DLL and always points the PATH
property of the reference to the original position.\
\
The solution is to change the Project file and add manually the PATH to
the DLL file\
\
So instead of:\

``` {.brush:xml}
    
    
    
    
    
    
    
    
    
    
```

\
\
you can change the reference of the interactivity dll to:\
\

``` {.brush:xml}
   ..\Libraries\System.Windows.Interactivity.dll
```
