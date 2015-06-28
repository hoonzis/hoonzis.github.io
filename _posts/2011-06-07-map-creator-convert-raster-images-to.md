--- layout: post title: Map Creator - convert raster images to maps data
(in Silverlight) date: '2011-06-07T15:53:00.000-07:00' author: Jan Fajfr
tags: - Maps - Silverlight modified\_time:
'2014-06-26T15:02:39.937-07:00' thumbnail:
http://4.bp.blogspot.com/-OWat90XDXOI/Te6rdb5WY-I/AAAAAAAAALQ/ArDO4OjD7Gw/s72-c/map\_creator\_view.PNG
blogger\_id:
tag:blogger.com,1999:blog-1710034134179566048.post-8296620690953124543
blogger\_orig\_url:
http://hoonzis.blogspot.com/2011/06/map-creator-convert-raster-images-to.html
--- This posts talks about a tool which can help you convert lines in
raster maps to a set of locations which later can be visualized using
Silverlight Bing Maps (or some other mapping framework).\
\
I call the application **Silverlight Map Creator** :) and it is
disponible at:\
<http://mapcreator.codeplex.com/>.\
\
\
This application can help you a bit when you have an existing map in a
bitmap picture format (png, jpeg) and you would like to use the route
lines from this map in your application.\
\
Then you have two options:\
\

-   Use MapCruncher. MapCruncher is tool from MS which allows you to
    create your own Tiles source from existing map. OK, if you never
    heard of Tiles:\
    \
    When using a map component (Google Maps, Bing Maps or any other) the
    entire map is composed of several tiles, each tile, when you zoom in
    is again composed of "smaller tiles" (they are of the same size, but
    have higher precision).\
    So MapCruncher lets you create your own map, composed of you own
    tiles based on the raster image. Later you can use this new map and
    "put it over" the standard Bing/Google/Open map - thus showing the
    additional information.\
    \
    This however has one disadvantage - the map is quite static - it is
    just a bunch of pixel on top of your the classic map - you are for
    example not able to get the total length of the route on the map.
-   You need to obtain the geo-data which specify the route lines - in
    other words coordinations of the route points. To obtain this data
    from predefined image you would need to first set the correspondence
    between the image and the map and than analyze the image to get all
    the points of your map.\
    I decided to create a tool which would help me with this task - this
    post is a brief description of the tool.

\
Here is a screen shot of the Map Creator tool which will help you
accomplish it:\
\

<div class="separator" style="clear: both; text-align: center;">

[![](http://4.bp.blogspot.com/-OWat90XDXOI/Te6rdb5WY-I/AAAAAAAAALQ/ArDO4OjD7Gw/s320/map_creator_view.PNG)](http://4.bp.blogspot.com/-OWat90XDXOI/Te6rdb5WY-I/AAAAAAAAALQ/ArDO4OjD7Gw/s1600/map_creator_view.PNG)

</div>

\
\
If you are wondering in the screenshot I am converting a map of a
ski-race (www.jiz50.cz) to a set of points. In the left part you can see
the map (jpg image) and in the right part the resulting route.\
\

### Converting raster image to map

The task of converting raster image to map data is composed of the
following parts:\

-   Load the image (by clicking the browse button...)
-   Set correspondence points between the image and the map
-   Pick up the color which defines the route or path in the raster
    image
-   Set some parameters for the analysis of the map
-   Press Start and hope to get some results
-   Perform some changes to the route
-   Add the route which you have obtained to the "result" set\
    -&gt; result set defines the data which is used to generate the
    XML.\
    -&gt; also this is the data which is saved any time, that you press
    save
-   Generate XML data for your maps

To accomplish all of that the application has a simple menu:\

<div class="separator" style="clear: both; text-align: center;">

[![](http://4.bp.blogspot.com/-O3lmZzFGSBw/Te6rvXPC68I/AAAAAAAAALY/sNLJ49jLVVs/s320/map_creator_menu.PNG)](http://4.bp.blogspot.com/-O3lmZzFGSBw/Te6rvXPC68I/AAAAAAAAALY/sNLJ49jLVVs/s1600/map_creator_menu.PNG)

</div>

\
\
\
Here are some details to the parts which are not straightforward:\
\

### Setting correspondences

\
**Technical background**\
Generally to set correspondences between two coordination systems you
need to determine if there is a transformation which could transform the
coordinates of one point from the resource to the resulting coordination
systems.\
\
Map Creator is not really sophisticated tool so it supports only the
case when there is a Affine Transformation between the two coordination
systems.\
\
Affine transformation preserves colinearity, that means that points
which lie on a line in one coordination system will also lie on the map
in the second one. Basically it means that Affine transformation can be
composed of any linear transformation (scaling, rotation) and
translation, for example skewing is not allowed.\
\
The relation between the two coordination systems can be specified using
the following equation:\
\
sx = c00\*rx + c01\*ry + c02\
sy = c10\*rx + c11\*ry + c12\
\
(rx, ry) - coordinates in resource coordination system (so lets say
pixels in the image)\
(sx,sy) - coordinates in the resulting system (so lets say longitude and
latitude)\
\
So we need 6 parameters. For each point we have 2 equations, so we need
3 points to have 6 equations for 6 parameters. In matrix notation we can
write it like this.\
\
\[c00 c01 c02\] \[x1 x2 x3\] \[u1 u2 u3\]\
\[c10 c11 c12\] \[y1 y2 y3\] = \[v1 v2 v3\]\
\[ 1 1 1\]\
\
**In Map Creator**\
Just select the "Correspondences" radio button. Then every time you
click "Right" on the image, a new point is added to a list, when you
select the point and click right on the map a correspondence will be
set.\
\

Select colors of the route
--------------------------

Just select the "Color selection" radio button. Than when you click
right button the mouse in the picture the color will be selected (and
added to the list).\
\

### Set the parameters

There are 4 parameters which somehow change the why the resulting route
is going to look like:\
**1)Search Range** - basically it says what is the minimal distance in
pixels of points of the same route. Setting this parameter to bigger
value will cause connections between routes which are normally not
connected. To low parameter will increase the density of point in the
route (which is not desirable either).\
**2) Color toleration** - determines the color of the pixel which will
still be marked as in the route. Each color is composed of 3 parts (RGB)
with values in range 0-255. This parameter sets the tolerance for each
part (RGB).\
**Min Points Per Route** - Each bike route is composed of several routes
(or lets say lines). This is caused by side routes, which have to be
represented separately. This parameter sets the minimal points for each
route. If there is a route with less points that this parameter sets, it
will not be added to the result.\
**Distance to connect**After the analysis, some routes which should be
connected are will not be. Typical example are the side routes. There
will always be a little space between the main route and the side
routes. This parameter sets what is the maximal distance between to
routes which should be connected.\
\

### Performing changes to the resulting route

There are two possible changes that you can do:\
1) Remove the point by clicking the right button\
2) Change the position of the point by dragging it\
\

### Adding the route the the results

Generally a map is composed of several routes. The basic idea behind
this tool is that once you have finished working on a route, you can add
it to the result (pressing the button on the list of colors). When the
route is in the results it will not be affected by running the analysis
again.\
\

### Saving your work

By pressing "Save" button you can save your work. Saved will be the list
of correspondences and the routes in the "result" set. This why the next
time you can continue working on existing map.\
\

### Generating XML

The main idea is to use the data which you have generated in your
application. The "Generate XML" button simply serializes the "result"
set to XML.\
As said before: Map is a collection of routes. Route is a collection of
lines. Line is a collection of locations.\
OK, in C\# or Java or whichever language it is something like this:\
\

``` {.prettyprint}
List<List<List<Location>>> result
```

\
When you serialize this object to XML (here just using the standard C\#
serializer you will obtain XML with following structure:\

``` {.prettyprint}
<?xml version="1.0" encoding="utf-16"?>
<arrayofarrayofarrayoflocation xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <arrayofarrayoflocation>
    <arrayoflocation>
      <location>
        <latitude>50.834165728659457</Latitude>
        <longitude>15.292032040283674</Longitude>
        <altitude>0</Altitude>
        <altitudereference>Ground</AltitudeReference>
      </Location>
      <location>
        <latitude>50.83278001263735</Latitude>
        <longitude>15.293082116316537</Longitude>
        <altitude>0</Altitude>
        <altitudereference>Ground</AltitudeReference>
      </Location>
</ArrayOfLocation>
</ArrayOfArrayOfLocation>
</ArrayOfArrayOfArrayOfLocation>
```

\
OK, I agree - it is too verbose and not optimized and for most ugly, but
I did not have time to implement my own format.\
\

### Future

So that's it. I am not sure that this tool will be useful to anyone, if
you think that you might use it, if you have a suggestion or a bug, just
leave me a note here...(ok not for the bugs, there is too many of them
anyway).
