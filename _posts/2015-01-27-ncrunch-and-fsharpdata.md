---
layout: post
title: FSharp.Data and Unit Testing
date: '2015-01-27T04:53:00.001-08:00'
author: Jan Fajfr
tags:
- FSharp
modified_time: '2015-04-22T02:42:06.483-07:00'
thumbnail: http://2.bp.blogspot.com/-bhTeegAseTs/VMeJ7PPap_I/AAAAAAAAEMM/Dab5HZms5rM/s72-c/reference_fsharpdata.PNG
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-7292279599877960101
blogger_orig_url: http://hoonzis.blogspot.com/2015/01/ncrunch-and-fsharpdata.html
---
I have recently run into two separate issues while testing some F\# data providers based code. Both issues were linked to the FSharp Data assembly. I am using ReSharper's NUnit runner and sometimes NCrunch. One of the problem was linked to the availability of *FSharp.Data.DesignTime.dll* on the compile time and other to NUnit not correctly handling Portable Library Class projects. I have tested and had the same issues with FSharp.Data 2.1.0 and 2.2.0.

#### NCrunch issues
For the first issue (missing *FSharp.Data.DesignTime.dll*) the fix depends on whether you are using Paket or Nuget to reference the package *FSharp.Data* package.

NCrunch won't compile your solution when *FSharp.Data* is referenced. This component internally references *FSharp.Data.DesignTime* which has to be available for the compilation - and NCrunch does not have those libs available, because the DLL is not referenced the standard way, but must be provided by Visual Studio.

The current solution is to reference FSharp.Data.DesignTime manually. If you are using Nuget, than the dll can be found in the packages folder as shown bellow:

![reference_fsharpdata](https://raw.githubusercontent.com/hoonzis/hoonzis.github.io/master/images/reference_fsharpdata.PNG)

If you are using *Paket* then you can locate in the project file, the following reference to *FSsharp.Data*.

```xml
<Choose>
  <When Condition="$(TargetFrameworkIdentifier) == '.NETFramework' And ($(TargetFrameworkVersion) == 'v4.5' Or $(TargetFrameworkVersion) == 'v4.5.1' Or $(TargetFrameworkVersion) == 'v4.5.2' Or $(TargetFrameworkVersion) == 'v4.5.3' Or $(TargetFrameworkVersion) == 'v4.6')">
    <ItemGroup>
      <Reference Include="FSharp.Data">
        <HintPath>..\packages\FSharp.Data\lib\net40\FSharp.Data.dll</HintPath>
        <Private>True</Private>
        <Paket>True</Paket>
      </Reference>
    </ItemGroup>
  </When>
</Choose>
```

And you can place a similar reference for the **DesigneTime** dll just bellow:

```xml
<Choose>
    <When Condition="$(TargetFrameworkIdentifier) == '.NETFramework' And ($(TargetFrameworkVersion) == 'v4.5' Or $(TargetFrameworkVersion) == 'v4.5.1' Or $(TargetFrameworkVersion) == 'v4.5.2' Or $(TargetFrameworkVersion) == 'v4.5.3' Or $(TargetFrameworkVersion) == 'v4.6')">
      <ItemGroup>
        <Reference Include="FSharp.Data.DesignTime">
          <HintPath>..\packages\FSharp.Data\lib\net40\FSharp.Data.DesignTime.dll</HintPath>
          <Private>True</Private>
          <Paket>True</Paket>
        </Reference>
      </ItemGroup>
    </When>
  </Choose>
```

Notice that you can also remove the **Paket** tag. Removing this tag will assure that Paket will ignore this reference and won't remove it while running **paket install**.

#### NUnit runner issues
NUnit did load the test correctly, but the test failed with **SystemMissingMethodException**

Luckily the solution [can be found on StackOverflow](http://stackoverflow.com/questions/22608519/fsharp-data-system-missingmethodexception-when-calling-freebase-provider-from) and was caused by the tested library being Portable Class Library instead of standard F# library.
