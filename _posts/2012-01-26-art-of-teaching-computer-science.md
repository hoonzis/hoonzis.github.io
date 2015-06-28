--- layout: post title: Art of teaching computer science date:
'2012-01-26T02:26:00.000-08:00' author: Jan Fajfr tags: - Computer
Science modified\_time: '2012-02-11T01:19:49.666-08:00' blogger\_id:
tag:blogger.com,1999:blog-1710034134179566048.post-3176407602173488223
blogger\_orig\_url:
http://hoonzis.blogspot.com/2012/01/art-of-teaching-computer-science.html
--- I was studying for my final masters exams at [CTU
Prague](http://www.fel.cvut.cz/en/), that means revising more or less
all my previous studies. Thanks to some job experiences, that I had
before, I am more or less able to see if what I have learned will be
useful for me in the future and also what was missing during my
studies.\
\
From this consideration I concluded this post, which describes what the
school should give to computer science students. I have visited [ENSTA
ParisTech](http://www.ensta-paristech.fr/) as well as [UPV
Valencia](http://www.upv.es/index-en.html) during my studies and I will
mix here my experiences from all these universities, so it's not only
about the [Faculty of Electronics at CTU
Prague](http://www.fel.cvut.cz/)(my alma mater).\
\
I have structured the ideas into groups:\

-   Importance of math
-   Understand the full stack
-   Don't bet too much on general engineering
-   The importance of being polyglot
-   Start at the lower level
-   Adoption of new technologies in courses
-   Teaching project management
-   The importance of the world around

\

### The importance of math

Math is important. We don't like to study it, because it hurts - you
have to think and it takes in general lot of time (that depends on your
talent). What are the areas important for future CS engineer?\
\
Well I think: Algebra, Analysis, Probabilistic, Statistic, Graph
theory.\
The studies of these subjects should be done during the first years - so
the theory can be build up on these grounds.\
\
There is a small difference between French and Czech approach. In
France, when going to engineering school, you have to pass two years of
"Preparatory classes". These classes are common to all future engineers
and contain lot of general sciences, specially math. This gives to the
French students better starting position for the later courses. I took
Cryptography course back at ENSTA, it was completely based on
Information theory and concepts from algebra such as Groups and Rings,
which we have only scratched at my Algebra course at CTU and I had some
hard time passing this course.\
\
I was able to pass the course only because of the fact, that there was a
practical part of implementation of RSA algorithm in C, which most of
the French guys failed to finish (about that later).\
\

### Understand the full stack

This to me means to have complete understanding of how computer works.
Basically if you are a software engineer you should know how your
programs are compiled and interpreted by the hardware. You should know
the structure of the processor, memory, arithmetic unit. You should
understand that arithmetic unit can be constructed from NANDs etc...\
\
What is important is the notion of abstraction. We do not have to be
experts on all levels, but we should know what each lower layer contains
and understand how it works.\
\

### Don't bet too much on general engineering

I had it the first year at CTU. And as I mentioned in France, they even
have two years of general engineering. (To be honest in France it is
more "general science"). It is important to learn general engineering
approach to solve a problem - but it has to stay in intentions.\
\
At CTU I remember having first year courses from Circuit theory. Even
the mathematical analysis course was done the way, that the math could
be later used for describing circuits by differential equations and
solving them. This did not serve me later at all. As I said I believe
knowing the entire computer stack is important - but not to this
extend.\
\
In France I have experienced even more off this "too much of general
engineering" effect. I know some guys who after two years of higher
studies for Computer Science degree did not understand well Object
oriented approach. That is wrong - learning the OO programming takes
time and practice specially we have to start early. And if you spend two
years of your CS studies on general sciences and maybe you add a little
bit of Algorithmic, it's already too late.\
\
When I come back to my Cryptography course, which I have passed in
France, the implementation of RSA in pure C was no peace of a cake,
specially when you have to treat the large numbers using libraries such
as (http://gmplib.org/). But I have managed it thanks to the quantity of
the code which I have written before.\
\
If I remember the French guys, with better understanding of the
mathematical principals behind, did not succeed - you need to get
programming in your blood to be able to solve complicated tasks...and it
was no quite yet in their blood.\
\

### The importance of being polyglot

You cannot really understand benefit's of one language, when you cannot
compare it to other languages. From my point of view students should
meet at least 4 type of languages during their studies:\
\

-   Imperative languages: C
-   Object oriented languages: C++, Java, C\# (yes I know that they are
    not pure)
-   Dynamic languages (completely interpreted): Python, Ruby or even
    only JavaScript
-   Functional languages: Scala, F\#
-   And some assembly language

\
OO programming has to be mastered - the other ones have to be at least
touched!\
\
When young CS students gets out of school, he might be asked to develop
a small web application which performs CRUD operations on some data in
database. If the only languages that he met, during the studies are Java
and C, than he will probably take Java and start defining the different
layers, take some web framework and start wiring all the pieces
together.\
\
If he would hear about Ruby before, he would probably be able to code
the same application in half the time, thanks to the scaffolding and the
dynamic nature of the language.\
\

### Start at the lower level

For some reasons, Java is taught as primary language at the
universities. That is wrong. It is much harder to grasps the pointer
arithmetic, when you start C/C++ later. I think that the partial reason
for that is laziness. When you know that most of the stuff can be
managed without C++, why bother? (Yes I am talking from personal
experience).\
\
Maybe start with C and teach OO programming with Java/C\# or C++?\
\

### Adoption of new technologies in courses

This is a big problem. At ENSTA as well at CTU, we took too much time
talking about SOA, Web Service, WSDL, UDDI. These concepts which were IN
but let's say 5 years ago. So what is the reason of learning the
structure of SOAP protocol - well, there are some messages, operations -
yes sure. But technologies come and change. Schools have to react FAST
to new coming technologies and also to the technologies leaving the
spot-light. It is not useful now to know how UDDI works, when it is
never used in the real world.\
\
The technologies have to be carefully selected - and if they are out -
they can be simple withdrawn from the syllabus and replaced with new
ones. I had a course of J2ee where JSF and the JavaBeans approached had
been thought as the way to do Java web apps. I think that this came from
the fact that at some time around 2009 when jsf was released it was
considered to be the way to do J2ee/web application development.\
\
Later I realized, that it is not the only way, actually it is not even
"the way" to do that ([check
this](http://prezi.com/dr3on1qcajzw/www-world-wide-wait-devoxx-edition/)).
So there are other frameworks and maybe all of them should be presented
to the student equally - not betting only on one of them.\
\
Since couple years, there is quite a buzz around big-data treatment. Lot
of news is also coming from NoSQL DBs. It would be great to have a
course about HBase, Cassandra, MongoDB, Map-Reduce and the way to treat
big data in general...but to offer that, schools would have to be fast.\
\
Maybe following the [Gartner
predictions](http://www.gartner.com/technology/research/predicts/) or
[Forrester](http://www.forrester.com/rb/research/) would help them find
the right topic (of course these predictions have to be filtered by
someone with a bit of distance. There is still too much marketing in
stuff coming from gartner/forrester.).\
\

### Teaching project management

This is topic of it's own. I remember that at Valencia and later in
Paris I was forced to do some estimations using the COCOMO metrics,
which no one really proved to me that it works. I had to see too much
overcrowded slides describing the traditional waterfall approach. I had
also short course on Agile methods - but completely useless again while
being TOO THEORETICAL.\
Project management when taught has always the tendency to become too
verbose - too much slides and talking and no practice!\
\
But project management is important. And there is just one way to teach
it - through projects. Why not have projects which last the whole year
and in which the students have to participate?\
\
There are thinks such as Continuous Integration and Configuration
Management, which are of great importance to the success of the project.
And these thinks are not really taught at schools. Maybe they are
presented on the slides.\
\
I would like to see a course, during which the first week or two would
be dedicated to the building the Continuous Integration platform, using
let's say Jenkins, with automatic build, unit testing, acceptance
testing (FitNesse, GreenPepper, Selenium or what-ever), notifications
etc. The rest of the course can be dedicated to the development of the
project on THAT platform and everyone will see the benefits.\
\

### The importance of the world around

IT students, and some geeks in general have the tendency to ignore the
world around. We use computer to about anything: study, learn,
communicate with social networks, watch movies.\
\
But there are some social skills which you can not pick up at the
computer desk.\
\
School should find a way to make students spend more time together.\
At ENSTA the school has an area with pools, baby-foot and small bar. The
students are in charge. There are regular parties and the students
sometimes spend the whole day at school. You go to your classes, spend
time at the library and you just switch to the bar later. You can meet
all the peers from other study fields...\
\
At last and not least. The profs, should motivate the students to leave
for one year and study abroad...the option which is not that popular at
IT faculties.\
\

### Summary

That's about all that on in my mind. Maybe someone will pick from this
lists some points which might be useful while creating new computer
science program or syllabus.
