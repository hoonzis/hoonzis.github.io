---
layout: post
title: Graph theory in Latex
date: '2012-03-08T11:59:00.001-08:00'
author: Jan Fajfr
tags:
- Graph Theory
- Computer Science
modified_time: '2014-06-26T14:37:34.086-07:00'
thumbnail: http://lh6.ggpht.com/-MOBHZdjyJyc/T1kPhAIMGxI/AAAAAAAAAQ4/rU5Ls5O9iVg/s72-c/image_thumb.png?imgmax=800
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-2137478854784529909
blogger_orig_url: http://hoonzis.blogspot.com/2012/03/graph-theory-in-latex.html
---
For one of my [previous
posts]({{site.baseurl}}{post_url2012-02-25-applications-of-graph-theory}\),
I needed some images of graphs. Initially I have taught, that I will
just draw them in Inkscape or some other tool, but after a while I have
decided to do something more clever – which might maybe serve me in the
future – draw the graphs in Latex. After a quick search I have found
this blog:

<http://graphtheoryinlatex.wordpress.com/> and older posts from the same
author: <http://graphtheoryinlatex.blogspot.com/>

This blog is about drawing graphs in TeX. So what do you need:

TikZ – a graphic system for Tex

[Tkz-graph](http://www.ctan.org/pkg/tkz-graph) – style with basic graph
drawing macros.

[Tkz-berge](http://www.ctan.org/pkg/tkz-berge) – style with more complex
drawing – such as predefined graphs (full graphs, bipartite graphs,
grids etc.)

Some TeX editor. I am using [Inlage](http://www.inlage.com/home). Which
is not expensive and takes care for downloading all the necessary
packages.

So here are the graphs and variants which I have used in the previous
post:

#### Graph with 4 edges in a circle:

``` 
\begin{tikzpicture}
\GraphInit[vstyle=Welsh]
\SetGraphUnit{2}
\Vertices{circle}{A,B,C,D}
\Edges(A,B,C,D,A,C)
\SetVertexNoLabel
\end{tikzpicture}
```

[![image](http://lh6.ggpht.com/-MOBHZdjyJyc/T1kPhAIMGxI/AAAAAAAAAQ4/rU5Ls5O9iVg/image_thumb.png?imgmax=800 "image")](http://lh3.ggpht.com/-JudznZ7FN1I/T1kPgaSMkSI/AAAAAAAAAQw/n_q6ktKAaSM/s1600-h/image%25255B2%25255D.png)

Coloring some of the nodes:

    \begin{tikzpicture}
    \GraphInit[vstyle=Classic]
    \SetGraphUnit{2}
    \Vertices{circle}{A,B,C,D}
    \Edges(A,B,C,D,A,C)
    \SetVertexNoLabel
    \AddVertexColor{red}{B,D}
    \AddVertexColor{green}{A}
    \AddVertexColor{blue}{C}
    \end{tikzpicture}  

![](http://lh4.ggpht.com/-gnmTUt_v9Ug/T0kM1wN_MPI/AAAAAAAAAPY/1u4wZwZv9aQ/image_thumb4.png?imgmax=800)

 

### Graph with labeled edges

\\begin{tikzpicture}
\\GraphInit\[vstyle=Welsh\]
\\SetGraphUnit{2}
\\Vertices{circle}{A,B,C,D,E}
\\SetUpEdge\[style={-&gt;}\]
\\Edges\[label=1\](A,B)
\\Edges\[label=1\](B,C)
\\Edges\[label=1\](C,D)
\\Edges\[label=\$1\$\](D,E)
\\Edges\[label=x1\](A,C)
\\Edges\[label=x2\](A,D)
\\Edges\[label=x3\](A,E)
\\SetVertexNoLabel
\\end{tikzpicture}

![](http://lh3.ggpht.com/-FzZoSiMVGKQ/T0kM6h9T6cI/AAAAAAAAAQE/TMCwsD85_JU/image_thumb2%25255B1%25255D.png?imgmax=800)

### Graph with positioned vertices.

Using the EA, NOEA and similar macros (EA = East of first vertex define
the second vertex, NOEA = Northeast of…)

``` 
\begin{tikzpicture}
\SetGraphUnit{2}
\GraphInit[vstyle=Normal]
\Vertex{S}
\NOEA(S){A} \SOEA(S){B}
\EA(A){T1} \EA(B){T2}
\SOEA(T1){F}
\Edges[label=1](S,A)
\Edges[label=1](S,B)
\Edges[label=4](A,T1)
\Edges[label=2](B,T2)
\Edges[label=1](T1,F)
\Edges[label=1](T2,F)
\Edges[style={pos=.25},label=3](A,T2)
\Edges[style={pos=.25},label=2](B,T1)
%Could use this for bending%
\tikzset{EdgeStyle/.append style = {bend left}}
\end{tikzpicture}
```

![](http://lh3.ggpht.com/-i-OM51jfzQI/T0kM-K_FqXI/AAAAAAAAAQo/_y22Pj1hx2A/image_thumb1%25255B1%25255D.png?imgmax=800)

### Bipartite graph using the berge styles:

\\begin{tikzpicture}
\\grCompleteBipartite\[RA=4,RB=3,RS=6\]{2}{2}
\\end{tikzpicture}

[![image](http://lh5.ggpht.com/-L4SZlSkKJcw/T1kPi5ikd3I/AAAAAAAAARI/Ui1pvCeppaQ/image_thumb%25255B1%25255D.png?imgmax=800 "image")](http://lh5.ggpht.com/-SnR7kWavbMU/T1kPiEkntEI/AAAAAAAAARA/Xk6ARO93xE0/s1600-h/image%25255B5%25255D.png)

For now I am content that I have found this blog and actually I have to
say, that drawing graphs in TeX is much easier than I have expected.
