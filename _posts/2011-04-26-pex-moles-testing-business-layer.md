---
layout: post
title: Pex & Moles - Testing business layer
date: '2011-04-26T09:35:00.000-07:00'
author: Jan Fajfr
tags:
- Testing
- C#
modified_time: '2014-06-26T15:18:06.836-07:00'
thumbnail: http://4.bp.blogspot.com/-jGyZ_e-2ax0/Ta4P7sdJPeI/AAAAAAAAAKU/fbPU0gC_neQ/s72-c/pex2.PNG
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-4233076503427730415
blogger_orig_url: http://hoonzis.blogspot.com/2011/04/pex-moles-testing-business-layer.html
---
The question is fairly simple: Should I use Pex to generate unit tests
for my business layer?

Code examples related to this post are available at [this GitHub repository.](https://github.com/hoonzis/PexMolesAndFakes)

In this post I would like to cover two parts:

-   **Pex and Moles basics** - just a quick overview, because this is
    covered by other blogs and by official documentation.
-   **Using Pex to test business layer** - I have been strungling to
    find a pattern to use Pex to generate unit tests for business layers
    of my applications. The problem is that there are quite a lot of
    samples which explain the basic and advanced aspects of Pex, but
    there is not that many examples which would show you have to use Pex
    in real life (putting aside the ambiguous definition of what real
    life is:).



### Pex and Moles basics
Pex is a testing tool which helps you generate unit tests. Moles is a
framework which enables you to isolate parts which are tested from other
application layers.

### Pex basics
Pex is a tool which can help you generate inputs for your unit tests. To
use Pex you have to be writing **Parametrized Unit Tests**. Parametrized
Unit Tests are simple tests which accept parameters and Pex could help
you generate these parameters.

Lets take a look at a first example, here is a simple method which you
would like to test:

```csharp
public static string SomeDumbMethod(int i, int j)
{
 if (i > j )
 {
  if (j == 12)
   return "output1";
  else
   return "output2";
 }
 else
 {
  return "output3";
 }
}
```

To test this method, you should write at least 3 unit test - in order to
cover all the branches of the method, thus cover all the possible
outputs (that is not a generic rule). But instead of that we will write
a unit test which accepts the possible inputs as parameters.

```csharp
[PexClass(typeof(Utils))]
[TestClass]
public partial class UtilsTest
{
    [PexMethod]
    public string SomeDumpMethod(int i, int j)
    {
         string result = Utils.SomeDumbMethod(i, j);
         return result;
    }
}
```

I have decorated the method with **PexMethod** and the class with
**PexClass** attribute this way Pex knows that this class is used to
generate unit tests. So now to ask Pex to generate the inputs, click
right on the body of the method and select **Run Pex Explorations**. Pex
will generate 3 unit tests, which you can review in the Pex Window.

[![](http://4.bp.blogspot.com/-jGyZ_e-2ax0/Ta4P7sdJPeI/AAAAAAAAAKU/fbPU0gC_neQ/s320/pex2.PNG)](http://4.bp.blogspot.com/-jGyZ_e-2ax0/Ta4P7sdJPeI/AAAAAAAAAKU/fbPU0gC_neQ/s1600/pex2.PNG)

### How does Pex work
Pex is using static analysis of your code, to determine which inputs
will achieve the maximal coverage of exposed method. Pex does not
randomly pick values to use as inputs, instead of that Pex is using an
algebraic solver (MS Research Z3 project)to determine what values of
parameters will suite the conditions leading to enter a not-yet explored
branch of code.
The main force of Pex is above all the ability to generate parameters
which would allow to cover all the branches of tested method.

### Moles basics
Moles is a stubbing framework. It allows you to isolate the parts of the
code which you want to test from other layers. Several other stubbing or
mocking frameworks (RhinoMock, NMock) are out there for free or not, so
the question is which is the advantage of Moles?
There are basically two reasons why use Moles:

-   **Moles works great with Pex**. Because Pex explores the execution
    tree of your code, so it also tries to enter inside all the mocking
    frameworks which you might use. This can be problematic, since Pex
    will generate inputs which will cause exceptions inside the
    mocking frameworks. By contrast Moles generates simple stubs of
    classes containing delegates for each method, which are completely
    customizable and transparent.
-   **Moles allows to stub static classes**, including the ones of .NET
    framework which are usually problematic to mock(typically DateTime,
    File, etc)

As it says on the official web: "Moles allows you to replace any .NET
method by delegate". So before writing your unit test, you can ask Moles
to generate the needed stubs for any assembly (yours or other) and than
use these moles in your tests.
Instead of complicated descriptions, here is a simple method, which
checks the actual date and outputs a string based on the date:

```csharp
public static String GetMessage()
{
 if (DateTime.Now.DayOfYear == 1)
 {
  return "Happy New Year!";
 }
 return "Just a normal day!";
}
```

Now to test this method, we need to be able to set the output of the
static DateTime.Now property. Moles will help us to achieve this. You
can see that in the following testing method I use **MDateTime** which
is a mole for **DateTime** class, which allows me to set the delegate
NowGet, which gets called when asked for DateTime.Now. To be able to use
**MDateTime** you have to add the moles assemblies by right clicking the
References in your project.

[![](http://4.bp.blogspot.com/-odL1eEO2Q5g/Ta4OspA13PI/AAAAAAAAAKE/6JE3k4LgcrM/s320/add_moles.png)](http://4.bp.blogspot.com/-odL1eEO2Q5g/Ta4OspA13PI/AAAAAAAAAKE/6JE3k4LgcrM/s1600/add_moles.png)

After that you can write your method as follows:

```csharp
[PexMethod]
public string GetMessage(bool newyear)
{
 MDateTime.NowGet = () =>;
  {
   if (newyear)
   {
    return new DateTime(1,1,1);
   }
   return new DateTime(2,2,2);
  };

 string result = Utils.GetMessage();
 return result;
}
```

Note that here I am using Pex play around a bit. I want to test both
branches of my method. The only possibility which Pex has to influence
the executed brunch is by generating parameters. So I add a bool
parameter to the test method, which I will ask Pex to generate. Here is
the result which I get:

[![](http://2.bp.blogspot.com/-dw4FichE0ZY/Ta4PjRJg1QI/AAAAAAAAAKM/T2iwY7oLdM0/s320/pex1.PNG)](http://2.bp.blogspot.com/-dw4FichE0ZY/Ta4PjRJg1QI/AAAAAAAAAKM/T2iwY7oLdM0/s1600/pex1.PNG)

This was a particular case, but the approach should be always the same.
When stubs are needed for certain assembly you can always generate them
by right-clicking the reference and selecting **Add moles assembly**.
Than you can use these stubs as any other classes in your test methods.

### Use Pex to test business layer
So you are probably thinking that all that is nice, but it does not
really serve in real projects? That is what I am sometimes thinking
also, so here I would like to present an attempt to use Pex to test
business layer of a typical Bank application. This application uses
**Repository** pattern. Simply service classes which provide the
business methods (like MakeTransfer etc.) use repositories to access the
database (or any other data source).

In this example I introduce an **AccountService** class, which depends
on two repositories: **AccountRepository** and **OperationRepository**.
Here are the definitions of the repositories:

```csharp
public interface IOperationRepository
{
 void CreateOperation(Operation o);
}

public interface IAccountRepository
{
 void CreateAccount(Account account);
 Account GetAccount(int id);
 void UpdateAccount(Account account);
}
```

The actual implementations of these repositories are not important,
since I want to test just the **AccountServices** class which is
dependend on these two repositories. To test just AccountServices class
I will mock these repositories (but about that later).
Here is AccountServices class:

```csharp
public class AccountService
{
 private IAccountRepository _accountRepository;
 private IOperationRepository _operationRepository;

 public AccountService(IAccountRepository accountRepository, IOperationRepository operationRepository)
 {
  _accountRepository = accountRepository;
  _operationRepository = operationRepository;
 }
    public void MakeTransfer(){ ... }
    public IList<operation> GetOperationsForAccount() {...}
    public decimal ComputeInterest(Account account, double rate) { ... }
}
```

AccountServices will have three methods to test:

-   MakeTransfer
-   ComputeIntereset
-   GetOperationsForAccount

Now to test these methods we have to stub or mock OperationRepository
and AccountRepository.
Let's start with **MakeTransfer** method.

```csharp
public void MakeTransfer(Account creditAccount, Account debitAccount, decimal amount)
{
 if (creditAccount == null)
 {
  throw new AccountServiceException("creditAccount null");
 }

 if (debitAccount == null)
 {
  throw new AccountServiceException("debitAccount null");
 }

 if (debitAccount.Balance < amount && debitAccount.AutorizeOverdraft == false)
 {
  throw new AccountServiceException("not enough money");
 }

 Operation creditOperation = new Operation() { Amount = amount, Direction = Direction.Credit};
 Operation debitOperation = new Operation() { Amount = amount, Direction = Direction.Debit };

 creditAccount.Operations.Add(creditOperation);
 debitAccount.Operations.Add(debitOperation);

 creditAccount.Balance += amount;
 debitAccount.Balance -= amount;


 _operationRepository.CreateOperation(creditOperation);
 _operationRepository.CreateOperation(debitOperation);

 _accountRepository.UpdateAccount(creditAccount);
 _accountRepository.UpdateAccount(debitAccount);
}
```

This method calls the CreateOperation method of OperationRepository and
UpdateAccount method of AccountRepository. Neither of these two methods
returns any value, so in your unit test you do not have to define exact
behavior of these methods, so you can provide a simple stub generated by
Moles to the constructor of AccountServices class.
In the following example SIAccountRepository and SIOperationRepository
are stubs generated by Moles.

```csharp
[PexMethod, PexAllowedException("SimpleBank", "SimpleBank.AccountServiceException")]
public void MakeTransfer(Account creditAccount,Account debitAccount,decimal amount)
{
 SIAccountRepository accountRepository = new SIAccountRepository();
 SIOperationRepository operationRepository = new SIOperationRepository();
 AccountService service = new AccountService(accountRepository, operationRepository);
 service.MakeTransfer(creditAccount, debitAccount, amount);
}
```

Let's take a look at Pex's output after running the Pex Test.

[![](http://2.bp.blogspot.com/-ysVhq1WdxaE/TbSKnhs1vjI/AAAAAAAAAKs/N8FQDGoRNWA/s320/make_transfer.PNG)](http://2.bp.blogspot.com/-ysVhq1WdxaE/TbSKnhs1vjI/AAAAAAAAAKs/N8FQDGoRNWA/s1600/make_transfer.PNG)

That is not bad, so Pex generated for me 6 unit tests, which normally I
would have to write and also discovered Overflow exception which I did
not cover in my code. What might be missing is the possibility to verify
if the Update/Create method of each of the repositories was called. In
other words we are limited by the fact that Moles can generate only
stubs, which are not able to verify that method was executed as Mocks
would be. If we wish to check whether the methods were called, we have
to implement this on our own.
Now let's take a look at **GetCustomersForAdvisor**.

```csharp
public List<operation> GetOperationsForAccount(int accountID)
{
 Account account = _accountRepository.GetAccount(accountID);
 if (account == null)
 {
  return null;
 }

 if (account.Operations == null)
 {
  return null;
 }

 return account.Operations.ToList();
}
```

This method calls the GetAccount(int id) method of AccountRepository,
than it performs some null value checks and returns the result. So in
order to test this method we will have to provide the behavior of the
GetAccount method. In the following snippet of code I use
SIAccountRepository stub generated by Moles and I specify the value
which should be return after callin GetAccount(int x) method.

```csharp
[PexMethod]
public List<Operation> GetOperationsForAccount(int accountID)
{
 List<Operation> operations1 = new List();
 operations1.Add(new Operation { Amount = 100, Direction = Domain.Direction.Credit });
 operations1.Add(new Operation { Amount = 200, Direction = Domain.Direction.Debit });


 List<Account> accounts = new List<Account>();
 accounts.Add(new Account { Balance = 300, Operations = operations1, AutorizeOverdraft = true, Id = 1 });
 accounts.Add(new Account { Balance = 0, Operations = null, AutorizeOverdraft = false, Id = 2 });

 SIAccountRepository accountRepository = new SIAccountRepository();
 accountRepository.GetAccountInt32 = (x) =>
 {
  return accounts.SingleOrDefault(a => a.Id == x);
 };

 SIOperationRepository operationRepository = new SIOperationRepository();
 AccountService service = new AccountService(accountRepository, operationRepository);

 List result = service.GetOperationsForAccount(accountID);
 return result;
}
```

At the beginning of the testing method I define a list of accounts, with
two accounts, one having several operations and other with no
operations. Than I set the delegate of GetAccount method of the
SIAccountRepository stub to search in the list by the account id. Now
let's run Pex and see the result.

[![](http://3.bp.blogspot.com/-tDeBoQSzJPY/TbZ_QSSd_AI/AAAAAAAAAK0/TaQvwe5-4i0/s320/getoperations.PNG)](http://3.bp.blogspot.com/-tDeBoQSzJPY/TbZ_QSSd_AI/AAAAAAAAAK0/TaQvwe5-4i0/s1600/getoperations.PNG)


So Pex basically tried the two ID's of the accounts in the predefined
list and also checked the null account. There is still a drawback and
that is the fact, that I have to define my own list of accounts to stub
the account repository, on the other hand I do it only once and also the
way the stub is of the GetAccount method is defined is quite
straight-forward; I only tell Pex to search in the list, and I do not
have to specify exactly which ID will provide me with which account.
The last method is **ComputeInterest**, which should compute the monthly
interest computed on annual basis (note that this is here just for
demonstration).

```csharp
public decimal ComputeInterest(Account account, double annualRate, int months)
{
 if (account == null)
 {
  throw new AccountServiceException("Account is null");
 }

 double yearInterest = Math.Round((double)account.Balance * annualRate);
 double monthInterest = yearInterest / 12;

 return (decimal)(monthInterest * months);

}
```

This method takes the balance of the account, computes the annual
interest and gives a value for one month(yes it is completely non-real
life method). Now lets take a look at the test for this method.

```csharp
[PexMethod, PexAllowedException(typeof(AccountServiceException))]
public decimal ComputeInterest(Account account,double annualRate,int months)
{
 PexAssume.Implies(account != null, () => account.Balance = 1000);
 PexAssume.IsTrue(annualRate != 0);
 PexAssume.IsTrue(months != 0);

 SIAccountRepository accountRepository = new SIAccountRepository();
 SIOperationRepository operationRepository = new SIOperationRepository();

 AccountService service = new AccountService(accountRepository, operationRepository);

 decimal result = service.ComputeInterest(account, annualRate, months);

 return result;
}
```

Here we use **PexAssume** to shape the inputs of the unit tests.
PexAssume is a static class which provides several methods to elaborate
the inputs. The most useful methods are **IsTrue(cond)** which shapes
the inputs in that form that the condition will always be true, and
**Implies(cond, fact)** which allows conditional clarification of
inputs.

Pex tries always the simpliest inputs, so right after trying a null
account, it will try an account with 0 balance. If we want Pex to
provide an account with different balance, than we have to use
**PexAssume.Implies** method. If we would use just
**PexAssume.IsTrue(account.Balance==1000)**, than we would obtain null
pointer exception in the test for which Pex generates null account. Now
let's take a look at the result:

[![](http://3.bp.blogspot.com/-GlOFpTMUTx0/TbaAqgxA22I/AAAAAAAAAK8/JNQxS5lFMGY/s320/interest.PNG)](http://3.bp.blogspot.com/-GlOFpTMUTx0/TbaAqgxA22I/AAAAAAAAAK8/JNQxS5lFMGY/s1600/interest.PNG)

So here Pex generates only two cases - but that is exactly sufficient to
cover all the code blocks. What is interesting is that we do not obtain
the case for OverflowException here, maybe because the multiplications
result in double values and the later conversion to decimal does not
throw OverflowException.

### Summary
Pex is a great tool when it comes to code coverage. It will exercise all
the paths in your code to look for errors or exceptions.
However sometimes you will have to generate the data for your test by
hand and provide them to Pex.
Moles is a great tool to provide stubs for static methods (and specially
static framework's methods) which normally are hard to test. It also
cooperates well with Pex, because it is completely transparent. For each
of you abstract classes or interfaces a stub is generated with delegates
that you can redefine to fit your needs. If you would try to use other
mock/stub framework, Pex will try to enter the scenes behind the
framework, which might result in unexpected exceptions.
However Moles lacks the "mocking" functionality. You can substitute any
method with a delegate, but there is no build-in function which would
tell you if the delegate was invoked. On the other hand this
functionality can be easily developed.
The provided description is my personal experience, I am still not sure
if I should use Pex in my personal projects and I am definitely not sure
if I am using it the right way. From my point of view Pex is great for
projects containing complex method with several branches. Quite a lot of
time the code, that I have to write is quite straight-forward and
because Pex generates the simplest values often it will finish by a
single null value passed as a test parameter.
This post does cover only small fraction of Pex capabilities and there
is a lot more to learn, to start with you can check PexFactories which
allow customize the generation of test inputs, the capabilities of
PexAssert or cooperation of Pex and CodeContracts.

PS: If someone has another approach or some additional advices on how to
use Pex it would be great to share them, I have wrote this post
partially because I would like to get some feedback on the subject.
