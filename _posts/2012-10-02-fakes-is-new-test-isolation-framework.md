---
layout: post
title: Introduction to Fakes and migration from Moles
date: '2012-10-02T14:04:00.000-07:00'
author: Jan Fajfr
tags:
- ".NET"
- Testing
- C#
modified_time: '2014-06-26T14:16:44.410-07:00'
thumbnail: http://lh5.ggpht.com/-9V7HQSZhYHw/UGi3opGKBaI/AAAAAAAAAcw/U3SZYHnvWVM/s72-c/image_thumb%25255B1%25255D.png?imgmax=800
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-319112185055504358
blogger_orig_url: http://hoonzis.blogspot.com/2012/09/fakes-is-new-test-isolation-framework.html
---

[Fakes](http://msdn.microsoft.com/en-us/library/hh549175(v=vs.110)) is a new test isolation framework from Microsoft. It is inspired by and resembles to [Moles](http://research.microsoft.com/en-us/projects/moles/) a framework which I have described in one of my previous blog posts.
In this post I will briefly describe Fakes and than show the steps which have to be taken when migrating from Moles. You will find that the migration itself is not that complicated. Besides some changes in the structure of the project only few changes are needed in the code.

Code example related to this post are available at [GitHub](https://github.com/hoonzis/PexMolesAndFakes). Fakes framework contains two constructs which can be used to isolate code:

* Stubs – should be used to implement interfaces and stub the behavior of public methods.
* Shims – allow mocking the behavior of ANY method, including static and private methods inside .NET assemblies.

Stubs are generated classes. For each public method in the original interface a delegate is create which holds the action that should be executed while invoking the original method. In the case of Shims, such delegate is generated for all methods, even the private and static ones.

When using Stubs, you provide mocked implementation of any interface to the class or method which you are testing. This is done transparently before compilation and no changes are made to the code after the compilation. On the other hand Shims use the technique called IL weaving (injection of MSIL assembly at runtime). This way the code which should be executed is replaced at runtime by the code provided in the delegate.

The framework has caused some interesting discussions across the web. The [pluralsight ](http://blog.pluralsight.com/2012/04/23/vs11-fakes-framework/) blog has found some negative points Rich Czyzewski describes how [noninvasive tests](http://www.richonsoftware.com/post/2012/05/02/Noninvasive-Unit-Testing-in-ASPNET-MVC-A-Microsoft-Fakes-Deep-Dive.aspx") can be created while using Fakes. And finally <a href="http://codeobsession.blogspot.fr/2012/05/microsoft-fakes.html%20Fake">David Adsit nicely summarizes the benefits</a> and the possible usage of the Fakes. From what has been said on the net, here is a simple summary of negative and positive points.

**Pros**
* Lightweight framework, since all the power of this framework is based only on generated code and delegates
* Allows stubbing of any method (including private and static methods)
* No complicated syntax. Just set the expected behavior to the delegate and you are ready to go
* Great tool when working with legacy code
**Cons**
* The test are based od generated code. That is to say, a phase of code generation is necessary to create the “stubs”
* No mocking is built-in into the framework. There is no built-in way to test whether a method has been called. This however can be achieved by manually adding specific code inside the stubbed method

Migrating from Moles to Fakes
=============================
First a small warning, <a href="http://connect.microsoft.com/VisualStudio/feedback/details/751873/building-unit-test-project-with-fakes-fails-with-error-exit-code-9009">A bug was apparently introduced by the Code Contracts team, which causes a crash when building a solution which uses Fakes</a>. You will need to install the<a href="http://msdn.microsoft.com/en-us/devlabs/dd491992.aspx"> latest version of Code Contracts</a>. (If you do not know or use Code Contracts, you should not be impacted).

If you have already used Moles before, you might be wondering, how much code changes will the migration need. To give you a simple idea, I have migrated the code from [my previous post](http://www.hoonzis.com/pex-moles-testing-business-layer") in order to use Fakes. Few steps have to be taken:

* Change the structure of the project and generate new stubs
* Rewrite the unit tests to use newly generated classes

To prepare the solution we have to remove all the references to Moles as well as the **.moles** files which were previously used during the code generation by the Moles framework. Next step is the generations of new stubs using Fakes framework. This is as simple as it has been before. Open the references window and right click on the DLL for which you want to generate the stubs. Than you should be able to select **Add Fakes Assembly** from the menu.
Following images show the difference between the old and new project structure (also note that I was using VS 2010 with Moles and I am now using VS 2012 with Fakes). After this is done, one needs to take care of the code changes.

[![moles](http://lh5.ggpht.com/-9V7HQSZhYHw/UGi3opGKBaI/AAAAAAAAAcw/U3SZYHnvWVM/image_thumb%25255B1%25255D.png?imgmax=800)](http://lh4.ggpht.com/-oc7YGzDMH0k/UF9-wzQZ-ZI/AAAAAAAAAcY/4hPypdEZcXY/image_thumb%25255B1%25255D.png?imgmax=800)
[![fakes](http://lh4.ggpht.com/-oc7YGzDMH0k/UF9-wzQZ-ZI/AAAAAAAAAcY/4hPypdEZcXY/image_thumb%25255B1%25255D.png?imgmax=800)](http://lh5.ggpht.com/-taClN6maVXc/UF9-wGsJH-I/AAAAAAAAAcQ/kYIq69RkRfU/s1600-h/image%25255B5%25255D.png)

### Rewriting code using Shims

Here is a classical example of testing a method which depends on **DataTime.Now** value. The first snippet is isolated using **Moles** and the second contains the same test using **Fakes:**

```cs
[TestMethod]
[HostType("Moles")]
public void GetMessage()
{
  MDateTime.NowGet = () =>
  {
    return new DateTime(1, 1, 1);
  };
  string result = Utils.GetMessage();
  Assert.AreEqual(result, "Happy New Year!");
}
```


```cs
[TestMethod]
public void GetMessage()
{
 using (ShimsContext.Create())
 {
   System.Fakes.ShimDateTime.NowGet = () =>
   {
    return new DateTime(1, 1, 1);
   };
   string result = Utils.GetMessage();
   Assert.AreEqual(result, "Happy New Year!");
 }
}
```

### The main differences

* Methods using Shims, do not need the **HostType** annotation previously needed by Moles.
* On the other hand a **ShimsContext** has to be created and later disposed when the stubbing is not needed any more. The using directive provides a nice way to dispose the context right after its usage and marks the code block in which the system has “stubbed” behavior.
* Only small changes are needed due to different names generated classes.



### Rewriting code which is using only Stubs
Here the situation is even easier. Besides the changes in the naming of
the generated classes, no additional changes are needed to migrate the
solution. The following snippet test “**MakeTransfer**” method, which
takes two accounts as parameters.

The service class containing the method needs Operations and Accounts
repositories to be specified in the constructor. The behavior of these
repositories is stubbed. This is might be typical business layer code of
any CRUD application. First let’s see the example using Moles.

```cs
[TestMethod]
public void TestMakeTransfer()
{
 var operationsList = new List<Operation>();

 SIOperationRepository opRepository = new SIOperationRepository();
 opRepository.CreateOperationOperation = (x) =>
 {
  operationsList.Add(x);
 };

 SIAccountRepository acRepository = new SIAccountRepository();
 acRepository.UpdateAccountAccount = (x) =>
 {
  var acc1 = _accounts.SingleOrDefault(y => y.Id == x.Id);
  acc1.Operations = x.Operations;
 };

 AccountService service = new AccountService(acRepository, opRepository);
 service.MakeTransfer(_accounts[1], _accounts[0], 200);
 Assert.AreEqual(_accounts[1].Balance, 200);
 Assert.AreEqual(_accounts[0].Balance, 100);
 Assert.AreEqual(operationsList.Count, 2);
 Assert.AreEqual(_accounts[1].Operations.Count, 2);
 Assert.AreEqual(_accounts[0].Operations.Count, 3);
}
```


Note the way the repository methods are stubbed. Due to the fact that
the stubs affect the globally defined variables (the list of operations,
and the list of accounts) we can make assertions on these variables.
This way we can achieve “mocking” and be sure that the
**CreateOperation** method and the **UpdateAccount** method of Operation
and Account repository have been executed. The **operationsList**
variable in this example acts like a repository and we can easily Assert
to see if the values have been changed in this list.

Let’s see the same example using Fakes:

```cs
[TestMethod]
public void TestMakeTransfer()
{
 var operationsList = new List<Operation>();

 StubIOperationRepository opRepository = new StubIOperationRepository();
 opRepository.CreateOperationOperation = (x) =>
 {
  operationsList.Add(x);
 };

 StubIAccountRepository acRepository = new StubIAccountRepository();
 acRepository.UpdateAccountAccount = (x) =>
 {
  var acc1 = _accounts.SingleOrDefault(y => y.Id == x.Id);
  acc1.Operations = x.Operations;
 };

 AccountService service = new AccountService(acRepository, opRepository);
 service.MakeTransfer(_accounts[1], _accounts[0], 200);
 //the asserts here....
}
```


You can see, that the code is almost identical. The only difference is
in the prefix given to the stubs (**SIAccountRepository** becomes
**StubIAccountRepository**). I am almost wondering if MS could not just
keep the old names, than we would just need to change the using
directive…


### Fakes & Pex
One of the advantages of Moles compared to other isolation frameworks,
was the fact that it was supported by Pex. When Pex explores the code,
it enters deep into any isolation framework which is used. Since Moles
is based purely on delegates, Pex is able to dive into the delegates and
generated tests according the content inside the delegates. When using
another isolation framework, Pex will try to enter the isolation
framework itself, and thus will not be able to generate valid tests.

So now, when Fakes are here as replacement of Moles, the question is
whether we will be able to use Pex with Fakes? Right now it is not
possible. Pex add-on for Visual Studio 11 does not (yet) exists and I
have no idea whether it will ever exists.

I guess Pex & Moles were not that adopted by the community. On the other
hand both were good tools and found their users. Personally I would be
glad if MS continued the investment into Pex and automated unit testing
though I will not necessarily use it everyday in my professional
projects. On the other hand I would always consider it as an option when
starting new project.
