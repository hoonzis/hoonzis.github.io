--- layout: post title: Choosing technologies for .NET project date:
'2012-02-04T10:44:00.000-08:00' author: Jan Fajfr tags: - Security -
".NET" - Testing - Architecture modified\_time:
'2012-02-14T12:15:36.700-08:00' blogger\_id:
tag:blogger.com,1999:blog-1710034134179566048.post-747965679601342524
blogger\_orig\_url:
http://hoonzis.blogspot.com/2012/02/choosing-technologies-for-net-project.html
--- Our latest research and development project was an online banking
application. While choosing the building pieces of this application, we
tried to pick the State-Of-Art frameworks and technologies. This is not
an easy task, while there are always several alternatives for each
component. I have decided to created this post which sums up the
technologies available for different parts of application and I will try
regularly update it, to keep up with the changes.\
\
Here is the structure of this blog, according to which the technologies
are grouped.\
\

-   DataAccess - ORM, data generation
-   Platform - Dependency Injection, Aspect Oriented Programming
-   Integration - SOAP/REST, messaging, distributed objects...
-   Testing - Unit testing and Mocking, Parametrized testing, Functional
    testing
-   Presentation layer
-   Security
-   Logging

\
\

### Typical application

\
Our application was a classical 3-tier application with database,
business and presentation layers.\
Data stored in SQL Server 2008. Data access layer implemented using
Repository pattern and using ORM. Dependency Injection and Aspect
Oriented Programming used to put together the application pieces.
Services exposed using WCF, and two types of client applications: mobile
and web.\
\
So the technologies presented here, are the ones mostly used in this
scenarios, however as said before, I would like to update the post to
give more information any time I cross another technology, and that
might while working on different architectures.\
\

### Data Access

The most important part of the Data Access layer is the framework used
for Object Relational Mapping (ORM). There are currently two major ORM
frameworks in .NET: **NHibernate** and **Entity Framework.** Both
provide similar ORM functionalities (Code only approach, Lazy loading ,
use of POCOs as persistence classes).\
\
**Entity Framework** 4.0 has brought a lot of improvement to its
previous version (named EF 1.0) which did not provide above mentioned
functionalities and its comparable to
[NHibernate](http://nhforge.org/Default.aspx). Crucial for ORM framework
in .NET environment is the integration of LINQ (Language Integrated
Query). [Entity
Framework](http://msdn.microsoft.com/en-us/library/bb399572.aspx) was
the first to offer this functionality but the implementation in
NHibernate followed shortly after.\
\
**NHibernate** has still several advantages among these it’s better
ability to process batch treatment and also the fact that as an open
source product it can be customized. On the other hand Entity Framework
provides better tools integrated into Visual Studio.\
One last thing which can justify the choice of NHibernate is the
possibility of using FluentNHibernate.\
\
**[FluentNHibernate](http://fluentnhibernate.org/)**\
NHibernate uses its XML based HBM format to define the mappings between
entities and POCOs. While the separation of code and configuration in
XML can be seen as nice approach it gets complicated once the XML
configuration files are larger and once we are introducing changes into
the POCOs. The XML is not checked upon the compilation, so potential
errors can be detected at run-time only and are generally hard to
localize.\
NFluent allows us to define the mappings in strongly-typed C\#, which
practically eliminates these issues. If there is an error in
configuration, it will be most likely discovered during the compilation.
Currently Fluent allows provides almost full compatibility with HBM
files, which means that what can be defined in HBM can be also defined
in Fluent.\
\
**Data Generation**\
**[AutoPoco](http://autopoco.codeplex.com/)** is a simple framework
which allows generation of POCOs (Plain Old CLR Objects) with meaningful
values. When building enterprise application we often need generate
initial data for the database. This can of course be done using SQL
scripts or in imperative language which we are using, but consists of
lots of repetitive code and for loops in order to create sufficient
amount of data. AutoPoco provides easy way to generate the starting
data. It also provides several build-in sources for common properties
which are stored in databases such as phone numbers, birth dates, name
and credit card numbers.\
\

### Platform

There are two design patterns (or approaches) which are very often
present among the several layers of enterprise applications: Dependency
Injection and Aspect Oriented Programming.\
\
Dependency Injection is used to assemble complex system from existing
blocks. There are several Dependency Injection containers available for
.NET framework: [Spring.NET](http://www.springframework.net/),
[CastleWinsdor](http://www.castleproject.org/container/),
[StructureMap](http://structuremap.net/structuremap/),
[AutoFac](http://code.google.com/p/autofac/),
[Ninject](http://ninject.org/), [Unity](http://unity.codeplex.com/) (by
Microsoft), [LinFu](http://code.google.com/p/linfu/).\
\
Aspect Oriented Programming allows developers to separate cross-cutting
concerns from the applications blocks. This is usually done by injecting
code into object's existing methods.\
There are several ways to implement AOP, two of these being most common:
Proxy based AOP and IL Weaving based AOP.\
\
Proxy based AOP is easily achieved by wrapping targeted object by a
proxy class. Than it is easy to intercept the calls to the target object
by the proxy class and call the code, which should be injected. It just
happens so, that the Dependency Injection containers use proxy classes
and therefor most of them offer also AOP. (Spring.NET, CastleWinsdor).\
\
IL Weawing is an expression for injection of IL code after compile time
before the generation of byte-code.\
\
There are two frameworks which provide AOP through IL Weaving:
[PostSharp](http://www.sharpcrafters.com/) and
[LinFu](http://code.google.com/p/linfu/). PostSharp has a commercial
licence, however at the time of writing this post(July 2011), there is
also 45 days free trial. LinFu is an opensource project under LGPL
licence which covers both IoC and AOP.\
\
I have used to choose Spring.NET because of it’s maturity, the fact that
it is well documented, works great with NHibernate and allows both AOP
as well as Dependency Injection. One of the disadvantages of Spring.NET
is the XML configuration which as always can become too large to
maintain. Other frameworks use C\# as the language to configure the AOP
or Dependency Injection (PostSharp makes use of attributes and
frameworks such as Ninject or StructureMap use strongly typed classes to
configure the dependency injection container).\
\
I have however decided to use Ninject on my last project, which seems to
have a bit of momentum right now, and I will post here later pros/cons.\
\
**Code Verification ([Code
Contracts](http://msdn.microsoft.com/en-us/devlabs/dd491992))**\
Design by contract is software design approach, which implies that
developers define clear interfaces for each software component,
specifying its exact behavior. The interfaces are defined by contracts
and extend the possibilities of code verification and validation.\
The term was first used by [Bertrand Meyer](http://se.ethz.ch/~meyer/),
who made it part of his [Eiffel programming
language](http://en.wikipedia.org/wiki/Eiffel_(programming_language)).\
\
Code Contracts is a language agnostic framework which enables the
Design-by-Contract approach by allowing the programmer to define three
types of conditions for each method:\
Pre-condition - states in what forms the arguments of the method should
be.\
Post-condition - states what forms the outputs of the method will have.\
Invariants - conditions which will always be true during the execution
of the method.\
\
These conditions can be later verified by two types of checks:\
Static checking - is being done at the compilation type. At this time
the compiler does not know what will be the values passed as arguments
to the methods, but from the execution tree can determine which method
calls might potentially be evoked with non-compliant parameters.\
Runtime checking - the code contracts are compiled as conditions
directly into .NET byte-code. This allows the program to avoid writing
conditions manually inside the method bodies.\
\
Note that Code Contracts are not language feature. They are composed of
class library and the checking tools which are available as plugins for
Visual Studio.\
\

### Integration

Distributed applications need a way of communication between the
components. Remote Procedure Call(RPC) was the first technology used in
distributed systems back in 70's. The choice here surely depends on the
architecture of the application (client-server, publish-subscribe, ESB,
and more...)\
\
**WCF**\
Flexible platform which provides abstraction of transport layer
configuration (security, transport format, message patterns).\
WCF options and choices:\
Transportation protocol: WCF can user HTTP, TCP, MSMQ\
Transportation format: XML, JSON, or Binnary\
\
One service can expose several Endpoints (URIs). Each Endpoint can be
configured to use different Binding. Binding can have different
transportation protocol and format options. The same services can be
thus exposed using different protocols and formats. In our application
we can use this advantage and expose different endpoints for different
clients.\
\

### Testing

Several types of tests can be used to confirm the correct behavior of
the application: Unit Tests, Integration tests, smoke tests, functional
tests (or acceptance tests).\
\
**Unit Testing**\
**Mocking frameworks**\
When it comes to isolating the unit tests there are several Mocking
frameworks available: [NMock](http://nmock.org/),
[EasyMock](http://www.easymock.org/),
[Moq](http://code.google.com/p/moq/),
[JustMock](http://www.telerik.com/products/mocking.aspx) (commercial),
[TypeMock](http://www.typemock.com/) (comercial),
[RhinoMocks](http://hibernatingrhinos.com/open-source/rhino-mocks),
[NSubstitute](http://nsubstitute.github.com/),
[FakeItEasy](http://code.google.com/p/fakeiteasy/) and
[Moles](http://research.microsoft.com/en-us/projects/moles/).\
\
In our application we have decided for RhinoMocks and Moles. Moles are
used in connection with Pex - test generation framework, which will be
described later.\
Most of the Mocking frameworks provide more or less the same
functionalities thus the decision is quite complicated. RhinoMocks has
the following characteristics:\

-   Free and Open Source
-   Easy to use
-   Active community
-   Compatible with Silverlight (existing port to Silverlight)

Possible disadvantage: three types of syntax, which might be confusing
for beginners\
Actual version 3.6, version 4 which should break backwards compatibility
is [in
development](http://ayende.com/blog/4169/planning-for-rhino-mocks-4-0),
but if I have not missed something, there are so far no releases.\
\
\
**Pex & Moles - Parametrized Unit Testing**\
[Pex & Moles](http://research.microsoft.com/en-us/projects/pex/) are
used in order to build Unit Tests for the back-end part. Pex is a tool
which helps generate inputs for unit tests while Moles enables the
isolation of tested code. In order for Pex to generate the inputs the
the test cases have to be parametrized.\
\
Instead of writing concrete test cases, the test method is just a
wrapper which takes the same arguments as the tested method, performs
necessary set-up and then passes the arguments to the tested method. Pex
analyses the execution tree of tested method and suggests the parameters
which should be passed to the method and builds concrete test cases.\
\
The aim of Pex is to obtain maximal code coverage. In order to achieve
that, it uses algebraic solver ([Microsoft’s
Z3](http://research.microsoft.com/en-us/um/redmond/projects/z3/)) to
determine the values of variables used in the method which will lead to
execution of each branch. Than it varies the parameters to obtain these
values.\
\
Moles is a stubbing framework. It allows you to isolate the parts of the
code which you want to test from other layers. There are basically two
reasons why use Moles:\
Moles works great with Pex. Because Pex explores the execution tree of
your code, so it also tries to enter inside all the mocking frameworks
which you might use. This can be problematic, since Pex will generate
inputs which will cause exceptions inside the mocking frameworks. By
contrast Moles generates simple stubs of classes containing delegates
for each method, which are completely customizable and transparent.\
Moles allows to stub static classes, including the ones of .NET
framework which are usually problematic to mock(typically DateTime,
File, etc)\
\
As it says on the official web: "Moles allows you to replace any .NET
method by delegate". So before writing your unit test, you can ask Moles
to generate the needed stubs for any assembly (yours or other) and than
use these “moles” in your tests.\
\

### Presentation Layer

The presentation layer is quite large topic with several choices:
ASP.NET, ASP.MVC + JavaScript, pure HTML5 + JavaScript, some JS
frameworks (jQuery, KnockOutJS, Silverlight - and all of these
technologies can be combined.\
\
**Silverlight**\
Here is a list of characteristics which can be seen as advantages:\

-   Intend ed to develop Rich Internet Applications.
-   Supports separation of the view and the logging using the
    MVVM pattern.
-   Possibility to use declarative language (XAML) to design user
    interface and imperative language tode ne the application logic.
-   Data visualization support u sing open source Silverlight Toolkit
    (charts, line series)
-   Re-usability of code on .NET compliant platform.
-   Possibility to access audio and video devices on client side.
-   Plug-in based technology. Requires the plug-in to be run inside
    the browser. The plug-in is not available for all possible
    combinations of platform and browser. This lowers the availability
    of the developed application and brings also higher requirements
    on hardware.
-   Standard web features are missing such as navigation.
-   Limited testability. Silverlight can not be tested with traditional
    functional testing frameworks such as Selenium. On the other hand,
    when the MVVM pattern is implied, the ViewModels can be tested as
    simple classes, using traditional Unit Testing technologies.

\
**HTML + JavaScript**\

-   No plug-in needed, HTML 5 is supported on the majority of the
    current browsers.
-   Naturally comes with web standard features: navigation, bookmarking.
-   Developers has to handle the "all browsers compatibility" issue.
-   Compared to C\\\# JavaScript is dynamic language, not compiled
    before the execution. This may be seen as advantage
    and disadvantage.

\
Knouckout.JS seems to me as a great possibility to use the MVVM pattern
with JavaScript, I will be checking it and writing about it later.\
\

### Logging

Logging is an essential part of each application. Following frameworks
are available in .NET:\

-   \
    [Log4Net]() - easy configurable framework.
-   Logging in MS Enterprise library
-   [NLog](http://nlog-project.org/) - version 2.0 released 7/2011
    including logging framework for Windows Phone 7 and Silverlight -
    seems very nice, but I have never tried.
-   The Objects Guy Logging Framework - lightweight logging framework
-   .NET build-in tracing - alternative approach of using
    System.Diagnostics namespace which enables output of standard Trace
    and Debug Write method to XML file.

Good recapitulation for logging is available at [this stackoverflow
thread.](http://stackoverflow.com/questions/4775194/when-should-i-use-tracing-vs-logger-net-enterprise-library-log4net-or-ukadc-di)\
\

### Security

There is usually a need to handle the user authentication in enterprise
applications. When using ASP.NET I have found out that there are the
standard [Forms Authentication](http://support.microsoft.com/kb/301240)
usually satisfies my needs. To handle OpenID authentication
[DotNetOpenAuth](http://www.dotnetopenauth.net/) is an excellent
choice.\
\
**Forms Authentication**\
Forms Authentication scheme works by issuing a token to user the first
time that he authenticates. User can be authenticated against database
or any other information source.\
\
This token in the form of cookie is added to the response which follows
the authentication request. This way the cookie is added to the next
request by the same client. Forms Authentication than takes care of
revoking the cookie (after demanded time) as well as of checking the
cookie in each requests.\
\
Forms Authentication works automatically with browser based clients,
when used from different clients, some additional work on the client has
to be done in order to add the authentication cookie to each request.\
\
**DotNetOpenAuth**\
\
I have previously used this library for two task: integrating OpenID
authentication and [creating OAuth
provider.](http://honga.super6.cz/2011/08/using-dotnetopenauth-to-create-oauth.html)\
\
Integration of OpenID works hand in hand with Forms Authentication.
DotNetOpenAuth library provides a means to authenticate user against any
Open ID provider. Once the user is authenticated the authentication
cookie can be generated using Forms Authentication.\
\

### Conclusion

When new application is being developed, there are several decisions,
that have to be taken regarding the framework and technologies which
might be used. This article does not give direct answers to these
question, but rather lists all the possible frameworks which should be
taken into account.\
\
New frameworks are being delivered by Microsoft and by Open Source
community and it is hard to see which technologies will hold on which
will be forgotten. I hope this overview can help to make the right
decision. Any suggestions are welcomed.\
[CodeProject](http://www.codeproject.com/script/Articles/BlogFeedList.aspx?amid=honga)
