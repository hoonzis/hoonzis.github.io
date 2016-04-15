---
layout: post
title: Programming languages for the age of Cloud
date: '2012-06-27T07:42:00.000-07:00'
author: Jan Fajfr
tags:
- Java
- C#
- Computer Science
modified_time: '2013-04-10T09:58:56.167-07:00'
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-7522731516243956423
blogger_orig_url: http://hoonzis.blogspot.com/2012/06/programming-languages-for-age-of-cloud.html
---
This posts talks about the aspects which are influencing computer languages these days. We are in the age when the sequential execution is over. Even your laptop has a processor with several cores. The cloud
provides us with tons of machines which we can use to run our code on. We are in the age of distribution, parallelization, asynchronous programming and concurrency. As developers we have to deal with the challenges which arise from this new environment. Computer language scientists have worked on the subject since the seventies. Nowadays concepts which have been studied for long time, influence the mainstream languages. This post describes how.

The motivation for this post was this panel discussion at the last's year Lang.NEXT conference, where one of the greatest language architects of these days discuss what the ideal computer language should look like (Anders Hejlsberg - C\#, Martin Odersky - Scala, Gilad Bracha - Newspeak, Java, Dart and Peter Alvaro).

### Web and Cloud programming

"Web and cloud programming" was the title of the mentioned panel
discussion. Cloud programming is quite noncommittal term. What do we
mean by "cloud programming"? Is it programming on the cloud (with the
IDE in the cloud)? or programming applications for the cloud (but that
can be just any web app right)? It turns out this is just a term to
depict the **programming in distributed environment**.

### Programming in distributed environment

Programming in distributed environment is much better term - but again
it might not be completely clear. The majority of todays applications is
not sequential anymore. The code flow of the program is parallel,
asynchronous and the program has to react to external events. The code
and the application itself is distributed. It might be distributed over
several cores, or nodes or it might be just server side - client side
code separation. You can have code running on the backend, some another
bits (maybe in different language) running on the front, some code is
waiting for response from a web service and some other code is waiting
for the response of the user on the client side. **You as a developer
have to handle the synchronization.**

We might actually say that todays web programming is distributed and
asynchronous. As developers, we have to make the switch from the
traditional sequential programming to the distributed and asynchronous
code. The advent of cloud computing is forcing this transition.

Non-sequential, parallel or asynchronous code is hard to write, hard to
debug and even harder to maintain. Write asynchronous code is
challenging, however write asynchronous application in a transparent
maintainable manner might feel impossible. Just think about the global
variables which you have to create to hold the information about
‘current situation’, so that when a response from a web service arrives
you are able to decide and take the right actions. It is this maintenance
of the global state which is particularly difficult in asynchronous
programming.

**What are the tools which will help us with the transition to distruted
asynchronous or parallel coding?**

Here is a list of 3 tools which I think might be helpful:

-   **Conceptual models** - As developers we can follow some conceptual
    model - for instance the actors model in order to organize and
    architecture the program.
-   **Libraries** - To implement one of the models (or design patterns)
    we can use tested and well document code – for instance Akka
-   **Computer languages** - The biggest helper will be on the lowest
    level - the computer language itself.

### Models, Libraries and languages

Libraries are and will be the principal tools to make developers life
easier. There are several libraries available to help with the
asynchronous, event-driven programming for many different languages.
[Akka](http://akka.io/), [Node.JS](http://nodejs.org/) or
[SignalR](https://github.com/SignalR/SignalR) are just examples. But
libraries themselves are build using languages. So the question is: What
can languages bring to help make the life easier for developers in the
age of cloud and distribution?

Modern languages have two characteristics:

-   They are build on powerful type systems: dynamic or static ones.
-   They borrow from different [programming language
    paradigms](http://en.wikipedia.org/wiki/Programming_paradigm).
-   Lately have been strongly influenced by [functional
    programming](http://en.wikipedia.org/wiki/Functional_programming).

### Benefits of functional languages

Functional languages might be one of the helpers in the age of
distributed computing. Some imperative languages are introducing
functional aspects (for instance C\# has been heading that way since
long time), another ones designed from scratch are much more closer to
"pure" functional style (Scala, F\#, Haskell). Let's
first define some terms and possible benefits of functional programming.
From my point of view (and I admit quit simplified point of view) there
are four aspects of functional programming that are useful in everyday
coding.

-   Elimination of the "global state" – the result of method is
    guaranteed no matter what the actual state is.
-   The ability to treat functions as first class citizens.
-   Presence of immutable distributable structures - mainly collections.
-   Lazy evaluation – since there is no global state, we can postpone
    the execution and evaluation of methods till the time the results
    are needed.

### Eliminating the state
It's hard to keep shared state in parallel systems written in imperative
languages. Once we leave the save sequential execution of the language,
we are never sure what are the values in the actual state. Callbacks
might be executed about any time depending for example on network
connection and the values in the main state could have changed a lot
from the time of the "expected execution". Purely functional programming
eliminates the outer "state of the system". The state has to be passed
always locally. If we imagine such a language all the methods defined
would need and additional parameter in the signature to pass the state.

    int computeTheTaxes (List<income> incomes, StateOfTheWorld state);

That is really hard to imagine. So as it has been said in the
discussion: pure functional programming is a lie. However we can keep
this idea and apply it to some programming concerns. For instance the
immutability of the collections might be seen as application of “no
current state" paradigm.

Since the “current state” does not exists, the result of a function
invoked with the same arguments should be always the same. This property
is called “Referential transparency” and enables the lazy evaluation. So
the elimination of the global state  might be seen as the pre-condition
for using other functional language features such as lazy evaluation

### Function as a first class citizen
Another aspect of functional programming is the fact that functions
become first class citizens. That means, that they can be passed as
arguments to other functions (these are called higher order functions).
This is extremely powerful concept and you can do a lot with it.
Functions can be also composed and the functional compositions applied
to values. So instead of applying a several consecutive functions on a
collection of values, we can compose the resulting function and apply it
at once. in C\# the LINQ uses a form of functional composition, which
will be discussed later.

### Lambdas and Closures
Lambdas are anonymous functions - defined on the fly. Closure add the
ability to capture the variables defined around the definition of the
function. C\# has closures and lambdas since the version 3.0, they
should finally arrive to Java in the version 8. Talking about
JavaScript, it just seems that they have always been there. Anytime you
define an anonymous function directly in the code you know you can use
the variables from the current scope in your function. Hence you are
creating a closure.

    var a = 10;
    var input = 5
    request(input, function(x) {
       a = x;
    });

Any time you use the variable from outer scope, in the inner anonymous
function, we say that the **variable is captured**

Closures are also available in Python and since the version 11 they are
even available in C++. Let's stop for a while here, because C++ adds the
ability to distinguish between variables captured by reference and
variables captured by value. The syntax for lambdas in C++ is a bit more
complicated, but allows the developers for each variable to specify how
it should be captured. In this following code the *v1* variable is
captured by value and all the variables are captured as references. So
the value of *v2* will depend on what happened before the lambda
actually executed.

    int v1 = 10;
    inv v2 = 5;
    for_each( v.begin(), v.end(), [=v1,&] (int val)
    {
        cout << val + v2 - v1;
    });

You can see, that even such and old school imperative language like C++
has been influenced and modified to embrace functional programming.

Closures add the ability to use the current state from the moment of the
definition of the anonymous function, as the current state used during
the functional execution.

### Collections in functional programming
In functional programming languages (the pure ones), collections are
immutable. Instead of modification a copy of the collection is returned
on each operation which is performed on the collection. It is up to the
designers of the language to force the compiler to reuse the maximum of
the existing collection in order to lower the memory consumption. This
allows the programmer to write the computation as a series of
transformations overt the collections. Moreover thanks to lambdas and
closures, these transformations may be defined on the fly. Here is a
short example:

    cars.Where(x=>x.Mark == ‘Citroen’).Select(x=>x.MaxSpeed);

This transformation will return an iterator of speeds of all Citroens in
the collection. Here I am using C\#/F\# syntax, however almost the same
code would compile in Scala.

The selector (“Where”) and the mapper (“Select”) both take as argument a
function which takes an item of the collection. In the case of the
selector the function is a predicate which returns “true” or “false” in
case of the mapper, the function just returns a new object. Thanks to
lambdas we can define both of them on the fly.

### Language integrated data queries
Lazy loading also comes from functional languages. Since the result of
the function does not depend on the "state of the world" it does not
matter when we execute any given statement or computation. The designers
of C\# inspired themselves while creating LINQ. LINQ just enables the
translation of the above presented chain of transformations to another
domain specific language. Since the lazy loading is used, each
computation is not performed separately, but rather a form of
“functional composition” is used and the result is computed once it is
needed. If the ‘cars’ collection would an abstraction for relational
database table, the result would be translated into “select maxSpeed
from cars where mark=’Citroen’. Instead of two queries (on for each
function call).

Inside LINQ translates the C\# query (the dotted pipeline of methods)
into an expression tree. The tree is then analyzed (parsed) into domain
specific language (such as SQL). Expression trees are a way to represent
code (computation) as data in C\#. So in order to develop and integrate
the LINQ magic into the language, the language needs to support function
as first class citizen and also has to be able to treat code as data.

Maybe as I wrote it, you are thinking about JavaScript and you are
right. In JavaScript you can pass around functions and you can also pass
around code and later execute it (the eval function). No wonder that
there are several implementations of LINQ for JavaScript.

Similar concept inspired some Scala developers and since Scala posseses
the necessary language features, we might see similar concepts in Scala
also ([ScalaQL](http://jiangxi.cs.uwm.edu/publication/sle09.pdf)).

### Dynamic or Typed languages
What are the benefits of Dynamic or Strongly typed language? Let's look
for the answer in everyday coding: Coding in dynamic language is at
least at the beginning much faster than in a typed language. There is no
need to declare the structure of the object before using it. No need to
declare the type of the simple values nor objects. The type is just
determined on the first assignment.

What works great for small programs might get problematic for larger
ones. The biggest advantage of typed system in the language is the fact,
that it is safer. It won't let you assign apples to oranges. It
eliminates much of the errors such as looking for non-existing method of
type.

The other advantage is the tooling which comes with the language.
Auto-completion (code completion based on the knowledge of the type
system) being example of one such a tool. The editor is able to propose
you the correct types, methods, or properties. The types structure is
used for analysis or later processing. For instance documentation might
be easily generated from the type system just by adding certain
metadata.

Several languages offer compromises between the strongly typed (safe)
and dynamic (flexible) world. In C\# we can use the **dynamic** keyword
to postpone the determination of the object type to runtime. DART offers
optional type system. Optional type systems let us use the tooling,
without polluting our lifes with too much typing exercises. This comes
handy sometimes.


### JavaScript as the omnipresent language
JavaScript is everywhere and lately (with Node.JS and MS investing
heavily into it) drawing more and more attention. The language has some
nice features: it treats the functions as first class citizens, supports
closures, it is dynamic, but one big drawback: It absolutely lacks any
structure. Well it's not typed language, so we cannot expect any
structure in the type system, but it also lacks any modularization.

Objects are defined as functions or 'just on the fly'. And there is
always this giant current state which holds all the variables and
everything, which get's propagated everywhere. I still did not learn how
to write good structured JS programs. And there are to many concepts of
JavaScript which I did not understand completely. As it has been said in
the discussions: you can probably write big programs in JavaScript, but
you cannot maintain them. That's why Google is working on DART. The
future version of ECMAScript will try to solve the problems of
JavaScript by bringing modular systems,classes, static typing. But the
big questions will be of course the adoption by the browsers.

### Summary
-   Libraries will be always the core pieces to enable writing
    distributed software
-   Language should be designed in a way to minimize the state and
    control 'purity' – functional languages are well studied and
    concepts coming from functional languages will become omnipresent in
    everyday programming.
-   Type systems should be there one we need them and should get out of
    our way when we don’t.

The future might be interesting. Lately I have been forced to write a
lot of Java code and interact with some legacy code(&lt;1.5) and besides
the typing exercise it does not provide me any benefits. It just bores
me. Well I am a fun of C\#, because the authors of C\# seem to search
interesting features from other languages or concepts (closures,
expression trees, dynamic typing, or later incorporating the
asynchronous model directly to the language) and introduce them to the
well established static typed, compiled (and for me well known) world.

But whether it is Scala, Python, Dart, JavaScript or C\#/F\# - I thing
we should be trying to adopt modern languages as fast as possible and
that for just one reason: to express more with less code.
