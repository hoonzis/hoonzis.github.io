--- layout: post title: Applications of Graph Theory date:
'2012-02-25T08:33:00.000-08:00' author: Jan Fajfr tags: - Graph Theory -
Computer Science modified\_time: '2013-01-25T00:43:46.850-08:00'
thumbnail:
http://lh3.ggpht.com/-E4CVy6sTvgw/T0kMpM8tLwI/AAAAAAAAANY/iM1K7l54JB0/s72-c/image\_thumb1.png?imgmax=800
blogger\_id:
tag:blogger.com,1999:blog-1710034134179566048.post-7667028688376771429
blogger\_orig\_url:
http://hoonzis.blogspot.com/2012/02/applications-of-graph-theory.html
--- Graph Theory is just a beautiful part of mathematics. Not only
Computer Science is heavily based on Graph Theory. There are a lot of
applications of Graph Theory in Operational Research, Combinatorial
Optimization, Bioinformatics.\
For my personal clasification I have separated the tasks, which you can
solve using Graph Theory into two groups:\
\

-   **Obvious applications** – I mean, that the analogy between the
    graph and the problem is quite easy to imagine (maps, cities,
    relations etc.). In this post you can find the following:
    -   Vehicle Routing Problem\
    -   Graph coloring\
    -   Map coloring
-   **Hidden applications** - Tasks which you would never assume can be
    solved using Graph Theory. Than you see one of them, and than you
    think: “Wow, I wonder who came up with that one…”. I will provide
    the following ones in this post:
    -   Image or 3D model reconstruction from projections\
    -   Prove of NP hardness of Integer Linear Programming\
    -   Register allocation\
    -   Approximation of data, data compression\
    -   Task scheduling

Obvious applications
--------------------

Here are some examples of problems for which the creation of the graph
to model the problem is quite easy and obvious.

### Vehicle routing problem and other variants of TSP

There is a whole set of problems, which are just variations of Traveling
Salesman Problem. Vehicle Routing Problem (VRP) can be characterized by
following description:

-   We are provided with a set of trucks, each of a certain capacity\
-   Set of customers, each with certain need of the goods\
-   The central repository, where the goods are stored

The tasks is to root the trucks, to distribute the goods to the clients
and minimalize the distance. [I have written a
blog]({{%20site.baseurl%20}}{%%20post_url%202010-05-02-vehicle-routing-problem%20%})
and a [web
utility](http://hoonzis.github.com/Vehical-Routing-Problem/vrp/) which
is able to solve the problem using two algorithms:

-   Clark & Wright Savings Algorithm\
-   The Sweep Algorithm

[![image](http://lh3.ggpht.com/-E4CVy6sTvgw/T0kMpM8tLwI/AAAAAAAAANY/iM1K7l54JB0/image_thumb1.png?imgmax=800 "image")](http://lh6.ggpht.com/-Cg3z_LSnYQM/T0kMobyf-OI/AAAAAAAAANQ/U5HXLg9lLnQ/s1600-h/image311.png)[![image](http://lh3.ggpht.com/-lsChtOpwoFs/T0kMqoBX9uI/AAAAAAAAANk/6i6Nb6iK1ZY/image_thumb211%25255B1%25255D.png?imgmax=800 "image")](http://lh6.ggpht.com/-S5gbyv-CiZU/T0kMpwf53RI/AAAAAAAAANg/osdC9HbHuN4/s1600-h/image611%25255B1%25255D.png)

You can find more algorithms for [solving VRP
here](http://neo.lcc.uma.es/radi-aeb/WebVRP/).

### Graph coloring problem

Given a graph, we want to decide, whether it is possible to color each
of the vertices in the graph in such way, that none of the vertices
which are sharing and edge have the same color. Many real world problems
can be formulated as Graph coloring problem. Ne first one of the Map
coloring.

### Map coloring

One of the first application is the map coloring problem. It has been
proven, that each map can be colored using 4 colors. This problem can be
converted to graph coloring problem by placing the vertex inside each
country or region in the map. Two vertices are connected if and only if
the two countries have a common border. [More over
here.](http://www.dharwadker.org/pirzada/applications/)

Hidden applications
-------------------

There are tasks and problems for which you would not intuitively search
the solution by applying graph theory. Here are some of them:

### Image reconstruction from X-Rays – Computer tomography.

Tomography is a technique used to reconstruct an image or 3D model from
series of projections, subsequently taken from different angles. When
using technologies such as the x-rays, the image take from an angle
gives for each pixel the total thickness of the scanned object. The
questions is than how to reconstruct the image from several taken images
which are containing only the thicknesses.

As described in great book "[Network Flows – Theory, Algorithms and
Applications](http://www.amazon.fr/Network-Flows-Theory-Algorithms-Applications/dp/013617549X)”,
concrete example of computer tomography is the “Reconstruction of the
Left Ventricle from x-Rays projections”. This problem can be solved
using the application of network flows theory. This method is applicable
only to problems where the scanned object has a uniform structure. As
mentioned in the book this assumes that the well-working Ventricle is
filled uniformly with blood and dye mixture.

The following graphics was taken from the book. It explains the method
on two dimensional image. Using two projections of the project, we
obtain vectors which are containing for each pixel (or other unit) the
probable mass hidden behind this pixel. Now is up to us to find out how
this mass is distributed  - that means where are the ‘1’ in the picture.
The more projections we have, the better results we can obtain.

[![image](http://lh6.ggpht.com/-pYLefkso-f8/T0kMsNAci7I/AAAAAAAAAN4/-iaD-wkGUpM/image_thumb.png?imgmax=800 "image")](http://lh4.ggpht.com/-AROyBYphzoY/T0kMrlIMJzI/AAAAAAAAANw/lJ6q1MUj_2w/s1600-h/image2.png)

The problems is thus simplified to the problem of constructing binary
matrix from the projection sums. This problem is a special case of the
feasible flow problem.

The following image shows similar very simplified task, which I have
taken from the Combinatorial Optimization course offered as part of
[Open Informatics](http://oi.fel.cvut.cz/en/home) program at [CTU
Prague.](http://www.cvut.cz/en?set_language=en)

[![image](http://lh5.ggpht.com/-syZt7yr5Cvk/T0kMtXAarkI/AAAAAAAAAOE/33Mo71hSvHg/image_thumb11.png?imgmax=800 "image")](http://lh5.ggpht.com/-tGlJqXi0kUc/T0kMs7fckOI/AAAAAAAAAN8/vC3Pd6Moa7c/s1600-h/image5.png)

The whole problem can be seen as the question of finding the feasible
flow in a network (G, b, u, l,c). So what does network consist of:

-   Graph G\
-   s – sources – the nodes which provide the fluid into the network –
    the nodes with positive values\
-   t – appliances (or sinks) – the nodes which consume the fluid – the
    nodes with negative values\
-   u – upper bound for the flow of each edge\
-   l – lower bound for the flow of each edge\
-   c – the actual flow in each edge – the one for which we are looking.
    The task is to find the values of c for each edge, in order to
    satisfy all sinks.

Here is the graph G which corresponds to the projections sumR and sumC
from the previous image. Each edge in the graph corresponds to one
pixel, connecting the two projections. The sumR are being sources in
this network and the sumC edges are sinks.

[![image](http://lh3.ggpht.com/-Q2SvY4ogNkA/T0kMvHaJmxI/AAAAAAAAAOY/ZVKLDwraCrA/image_thumb211.png?imgmax=800 "image")](http://lh3.ggpht.com/-3EU145tGhtY/T0kMuK5pFnI/AAAAAAAAAOQ/xjMbcvSpfwk/s1600-h/image8.png)

For each edge the lower bound l(e) = 0, upper bound u(e) = 1 and we are
looking for values of values of c(e), in order to for the flow to be
feasible and also minimal. The edges which are used in the feasible and
minimal flow are pixels which will have ‘1’ value in them.

### Proving NP’s ness of some problems such as Integer Linear Programming

The graph coloring problem has been already mentioned above. We are
trying to color each node of the graph in such a way, that nodes with
same color cannot be connected by an edge.

[Integer Linear Programming
(ILP)](http://en.wikipedia.org/wiki/Linear_programming#Integral_linear_programs)
is [NP-hard](http://en.wikipedia.org/wiki/NP_(complexity)) problem. This
can be proven by the [polynomial
reduction](http://en.wikipedia.org/wiki/Polynomial-time_many-one_reduction)
of Graph coloring problem to the ILP problem. Concretely we can say,
**that for each graph which can be colored using 3 colors, we are able
to construct an ILP problem, which has a solution**. From the
theoretical point of view saying “we are able to construct” means that
there is a polynomial reduction of Graph coloring problem to ILP
problem. Polynomial reduction proves that:

1.  If Graph Coloring is NP-hard problem, than ILP is also NP
    hard problem.

Polynomial reduction has to satisfy two conditions in order to prove the
NP-hardness:

1.  The reduction algorithm – the construction of one problem from
    another has to be performed in polynomial time\
2.  For each instance graph which can be colored with 3 colors an
    instance of ILP can be constructed which has a solution

Here is the reduction algorithm (the algorithm which explains how to
define an ILP problem to given graph):

In the beginning we have a graph colored using 3 colors. We will try to
create an instance of ILP out of this graph. That means we have to
define the variables and the equations which build the ILP problem. We
can do this in 3 steps.

-   Create N variables x~ncolor~ == 1 &lt;=&gt; the node **n** has the
    color **c,** where N is the number of nodes. \
-   For each node in the graph add en equation to the ILP system:
    -   x~nred~ + x~nblue~ + n~ngreen~ = 1
-   for each edge e = {n~i~, n~j~} in the graph add following three
    equations in the system: 
    -   x~nired~ + x~njred~ &lt;= 1\
    -   x~niblue~ + x~njblue~ &lt;= 1\
    -   x~nigreen~ + x~njgreen~ &lt;= 1

Here is an example, we have an simple graph:

[![image](http://lh5.ggpht.com/-FVRzZFGJsQQ/T0kMw6EoX-I/AAAAAAAAAOk/2FQILe_Wd-s/image_thumb111.png?imgmax=800 "image")](http://lh6.ggpht.com/-mb1EyWQx9lU/T0kMwCrUz0I/AAAAAAAAAOg/W8iUsT7JSUQ/s1600-h/image3.png)

Now the first set of equations, which states, that each edge can have at
most one color:

[![image](http://lh3.ggpht.com/-CnFpND88XDI/T0kMyCo10VI/AAAAAAAAAO0/bbsoZt91zgk/image_thumb21.png?imgmax=800 "image")](http://lh6.ggpht.com/-ZSmFCcZLwr4/T0kMxgEG4gI/AAAAAAAAAOs/x_cml-Yg7kY/s1600-h/image6.png)

The following set of equations, which states, that nodes sharing edge
cannot have the same color:

[![image](http://lh6.ggpht.com/-oyUFGKmBIdI/T0kM0tzGe-I/AAAAAAAAAPI/Vkt7YKA0l5s/image_thumb3.png?imgmax=800 "image")](http://lh4.ggpht.com/-bFtZTWbOCoU/T0kM0PhK8mI/AAAAAAAAAPA/tqQY6ta98XM/s1600-h/image9.png)

Now because the ILP problem can be reduced to graph coloring problem, we
know, that this problem has solution, when the graph can be colored with
three colors. Here is the solution:

[![image](http://lh4.ggpht.com/-gnmTUt_v9Ug/T0kM1wN_MPI/AAAAAAAAAPY/1u4wZwZv9aQ/image_thumb4.png?imgmax=800 "image")](http://lh3.ggpht.com/-jQNXbHYlStw/T0kM1VZUavI/AAAAAAAAAPM/2gy37uI8E40/s1600-h/image12.png)

Which corresponds to:

[![image](http://lh6.ggpht.com/-eQlnvll8XN4/T0kM4YkTjrI/AAAAAAAAAPk/os2ufnbdEZc/image_thumb6.png?imgmax=800 "image")](http://lh6.ggpht.com/-uybWvscUPag/T0kM2bprXCI/AAAAAAAAAPg/Z1Tdn9iY_dM/s1600-h/image18.png)

The coloring of the graph is NP hard, so also ILP is NP hard. If you
wonder how to prove that NP graph coloring is NP hard: there is a
[polynomial reduction](http://www.shannarasite.org/kb/kbse60.html) from
one special type of [SAT
problem](http://en.wikipedia.org/wiki/Boolean_satisfiability_problem).

### Register allocation

Register allocation is the process of assigning possibly infinite set of
variables of assembly program to a finite set of registers which are
available in the processor. Not all variables are used at the same time,
so several variables can share a register (if not this mapping would not
be possible). Even this problem is [solved using graph
coloring](http://en.wikipedia.org/wiki/Register_allocation). For each
variable a vertex is created. Vertices are connected if variables “live”
in the program at the same time. The number of colors given to color the
graph is equal to number of registers.

### Approximation of the data – data compression

This technique is used in order to approximate the data which has to be
stored while minimizing the loses of precision.\
\
For example a data which represents taken temperatures during the time
and builds a nice graph. However if this data was taken at high
frequency, there might be too many records. The idea is to minimize the
number of records, while keeping most of the information about the
evolvement of the temperature.

The shortest path algorithm can be used to solve this problem. For
instance the blue line in the following graphics represents the data to
be stored. It is 10 values: the time x and Temperature( x) at the time
x. The green and red line represent possible approximations, when we are
leaving out one or two nodes. Of course there are several nodes which
can be left out and the shortest path algorithm can help us to find
which ones can be left out.

[![image](http://lh5.ggpht.com/-CYor7U0C7N0/T0kM5axi8iI/AAAAAAAAAP0/NXzB3LAuzEg/image_thumb7.png?imgmax=800 "image")](http://lh6.ggpht.com/-NpgxaYrP0Zw/T0kM4kEC1eI/AAAAAAAAAPs/DK6JLhF1TLk/s1600-h/image21.png)\

We can construct a full graph, which will contain 5 nodes, representing
the 5 data points (the times x). Each edge represents the “precision
loose” which we pay, when we take the direct path between the two nodes
of the edge instead of passing the traditional way. The following
picture represents the partial graph – the skipping edges start only in
the first node ( A ). The edge with value x1 corresponds to the red line
in the graph etc. The graph should be also filled with other edges
starting in B and C (the only edge going from D to E is already present
and there are no edges starting in E), but I have left them out for
simplicity.

[![image](http://lh3.ggpht.com/-FzZoSiMVGKQ/T0kM6h9T6cI/AAAAAAAAAQE/TMCwsD85_JU/image_thumb2%25255B1%25255D.png?imgmax=800 "image")](http://lh6.ggpht.com/-9yJ0vICEodQ/T0kM5wHFx2I/AAAAAAAAAP8/ApxNGQZGlhM/s1600-h/image611.png)

 

So without compression  we have the simple path: A,B,C,D,E = 1 + 1 + 1 +
1 = 4

Taking the red edge and the rest of the path: A,C,D,E = 1 + 1 + 1+ x1

Taking the green edge and the rest of the path: A, D, E = 1 + 1 + x2

The values of the edges in the standard path should be the lowest (here
all of them have value 1). On the other hand values of edges which will
make us loose more precision should be the greatest. Then of course we
can introduce some bonus to motivate the algorithm to take less edges
(better compression). All this constraints can be modeled using
heuristics.

One possible heuristics to evaluate the value of the edge is to measure
the distance between the real data and the estimated date. For instance
the value of the second point is 5. If we estimate the value using the
red line (leaving out the second point) the corresponding value on the
red line is 3. The distance between these two values is: 2.

If we use the the green line instead then the distance between the
estimated value f’( x) and the real value f( x) is 1. On the other hand
the green line also estimates the second point 3 point. And we see that
the distance for the second point will be more or less 1.5. So we should
add these distance together. So we get:

x1 = 2

x2 = 2.5

This is just a proposition. We could also multiply it by some
coefficient to obtain some reasonable results.

With the developed and evaluated graph, finding the shortest path in the
full graph from the node A to the node E will give us the best
“size/precision” ratio.

### Task scheduling

In this problem we have a group of workers and group of tasks. Each task
can be processed by each worker. However the workers do not have the
same performance on all tasks – the time for the processing of each task
differs for each worker.

Let’s take a look at very simple example, we have two workers (A,B) and
two tasks (T1,T2). The values in the table represent the processing
time, that the worker needs for the given task.

\
[![image](http://lh3.ggpht.com/-5SUQybRA4M8/T0kM8NGdb-I/AAAAAAAAAQY/oK6Xvaw29t8/image_thumb2.png?imgmax=800 "image")](http://lh4.ggpht.com/-gqDg6yMrVSc/T0kM7fJ1TtI/AAAAAAAAAQQ/Xbl0nPjtbXI/s1600-h/image6%25255B1%25255D.png)

This can be solved as finding the cheapest flow in the following graph.

[![image](http://lh3.ggpht.com/-i-OM51jfzQI/T0kM-K_FqXI/AAAAAAAAAQo/_y22Pj1hx2A/image_thumb1%25255B1%25255D.png?imgmax=800 "image")](http://lh3.ggpht.com/-7eTj-Dy3tFw/T0kM9UUU4jI/AAAAAAAAAQg/TMfe_ao8J4g/s1600-h/image3%25255B1%25255D.png)

Not that each edge has two values: u/c. The ‘u’ represents the capacity
of the edge – it is always one. The ‘c’ represents the cost of the edge.
Finding the cheapest flow in this graph from S to F will give us the
best assignment of workers to tasks.

### Other interesting applications

-   [Development of custom processors for minimization of resources –
    data
    path optimization.](http://www.ics.uci.edu/~jelenat/pubs/jtrajkovic-datapath.pdf)\
-   Scheduling on parallel processors – application of maximal flow
    finding problem.

