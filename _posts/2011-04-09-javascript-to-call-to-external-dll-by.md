---
layout: post
title: JavaScript to call to external DLL by NPAPI plugin
date: '2011-04-09T06:29:00.000-07:00'
author: Jan Fajfr
tags:
- JavaScript
- C++
modified_time: '2014-06-27T02:29:38.086-07:00'
thumbnail: http://2.bp.blogspot.com/-GSf6I51qfpI/TaBW9FN3NGI/AAAAAAAAAJs/rTZrt0eBDU8/s72-c/export_symbols.PNG
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-4506190779342821698
blogger_orig_url: http://hoonzis.blogspot.com/2011/04/javascript-to-call-to-external-dll-by.html
---
This is a somewhat special scenario: you want to use JavaScript to call
a function exposed by DLL. The DLL is old Native C++. Here in this
simple Proof of Concept I call a DLL which simple writes to a file on
disk.

I have achieved this by using NPAPI plugin. (I did not do it for fun, but
it was one of my assignments last week...)

Netscape Plugin API is a plugin architecture which allows you to write
components (plugins) which can be embeded to web pages. Plugins are
developed in native code and are deployed via registering the resulting
DLL file to Windows Registry. (using regsvr32 on Windows).

### Creating the native dll library to be called
In this part I will just define a simple DLL library with one function
allowing you to write to file. Normally you have already some legacy dll
which you want to call, this one will serve just for a test. There is a
[MSDN page concerning this topic.](http://msdn.microsoft.com/en-us/library/ms235636(v=vs.80).aspx)


In Visual Studio create new Win32 Console Application. You will be presented by a Application Settings dialog where you can
select DLL library and Export symbols.

[![](http://2.bp.blogspot.com/-GSf6I51qfpI/TaBW9FN3NGI/AAAAAAAAAJs/rTZrt0eBDU8/s320/export_symbols.PNG)](http://2.bp.blogspot.com/-GSf6I51qfpI/TaBW9FN3NGI/AAAAAAAAAJs/rTZrt0eBDU8/s1600/export_symbols.PNG)


A project structure is created for you, where a header file and
corresponding cpp file named after you project will be prepared for you
to define and implement functions which you want to expose. I have
called my project "NativeLib". Here is the header file:

```cpp
class NATIVELIB_API CNativeLib {
  public:
   CNativeLib(void);
};

extern NATIVELIB_API int nNativeLib;

NATIVELIB_API int fnNativeLib(void);

NATIVELIB_API int fnPrintToFile(void);
```

And here is the code file:

```cpp
NATIVELIB_API int fnNativeLib(void)
{
 return 42;
}

NATIVELIB_API int fnPrintToFile(void)
{
 ofstream myfile;
 myfile.open ("C:\example.txt");
 if(myfile.is_open()){
  myfile << "Writing this to a file.\n";
                return 1;
 }else{
            return 0;
        }
 myfile.close();
}

// This is the constructor of a class that has been exported.
// see NativeLib.h for the class definition
CNativeLib::CNativeLib()
{
 return;
}
```

You can see that there is already one function predefined which returns
the ultimate answer. I have just added my function to write a simple
text to a file. When you compile the solution, you will have a dll file
and also a lib file.

### Creating the plugin structure
The architecture of the plugin is quite complicated so to help out there
is a tool called FireBreath which helps you with creation of the simple
plugin.

Firebreath is written in C++ and uses Python to run PREP scripts so you
will need Python to execute it. You can download Firebreath from its
[official page](http://www.firebreath.org) or from its GIT repository.
On the official page you can find a great video works you through the
process of creation of the plugin structure.

Firebreath also uses CMAKE to create a build which will fit your
platform needs. CMAKE is a crossplatform build system which from one
configuration written in a CMakeList.txt file will build Make file or
Visual Studio Solution structure which later can be build on your
system.

Follow the [video on the Firebreath page](http://www.firebreath.org/display/documentation/Windows+Video+Tutorial)
to create the plugin structure and the VS solution. During the process
of creation you will be asked for the name of the plugin, description,
if you want your plugin to have GUI and some additional information. If
you open created Visual Studio solution you will see that it consists of
several projects, but you will be concerned only with the with the same
name as you gave to your plugin.

[![](http://1.bp.blogspot.com/-6qA7j0NH4dI/TaAwbCIuArI/AAAAAAAAAJc/bx4AUeTCfxo/s320/project_structure.png)](http://1.bp.blogspot.com/-6qA7j0NH4dI/TaAwbCIuArI/AAAAAAAAAJc/bx4AUeTCfxo/s1600/project_structure.png)

If you opened the details of the project you will see that it contains a
header file with "API" suffix and corresponding cpp file.

[![](http://4.bp.blogspot.com/-hHro78SMPqo/TaAw6R4wzZI/AAAAAAAAAJk/PFCbvdRpgg8/s320/project_structure2.png)](http://4.bp.blogspot.com/-hHro78SMPqo/TaAw6R4wzZI/AAAAAAAAAJk/PFCbvdRpgg8/s1600/project_structure2.png)

Now to add your function to the API file you will add it respectively to
header and to cpp file:

```cpp
class MyAPI : public FB::JSAPIAuto
{
public:
    MyAPI(const OctoTestPtr& plugin, const FB::BrowserHostPtr& host);
    virtual ~MyAPI();

    MyPtr getPlugin();

   //call to native library to print to file
   int printToFile();
};
```

```cpp
//add the header file
#include "NativeLib.h"

//add the function implementation
int MyAPI::printToFile()
{
 return fnPrintToFile();
}
```

There is one last step and that is to register this method so it will be
exposed by the plugin to JavaScript calls. This is done in the
constructor of the plugin:

```cpp
MyAPI::MyAPI(const MyPtr& plugin, const FB::BrowserHostPtr& host) : m_plugin(plugin), m_host(host)
{
  registerMethod("printToFile", make_method(this,&MyAPI::printToFile));
}
```

Now you will have to add the header file to your solutions header files
and than the LIB for the linker to be able to find your native library
functions. To do so go to **Project properties -> Linker -> Input -> Additional Dependencies** and add your lib file here. One last
will be to copy the NativeLib.dll to the location of you plugins
generated DLL.

Now you can compile the solution (first time it will take maybe couple
minutes, next time it will be faster since you are only changing one
project). In the "BIN\\Debug" folder you will find the DLL of your
plugin. Now run regsvr32 myplugin.dll and the plugin should be
registered.

Now you can locate FBControl.html (generated for you by Firebreath)
which already contains your plugin so you can test it directly (you
should find it in "projects\\MyPluging\\gen" folder). Now that file
already contains a JavaScript function to get your plugin.

```javascript
function plugin0()
{
   return document.getElementById('plugin0');
}
plugin = plugin0;
```

So in the body of the html just add a button to call the plugins
function by JavaScript and show the result of the function:

```html
<input type="button" value="Print to file" name="button2" onClick="alert(plugin().printToFile());">
```
This is quite special scenario so I am not sure if this post will serve
anyone...but never know.
