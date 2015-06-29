---
layout: post
title: KnockoutJS and Google Maps binding
date: '2012-03-17T02:59:00.008-07:00'
author: Jan Fajfr
tags:
- MVVM
- JavaScript
modified_time: '2014-06-26T14:29:07.926-07:00'
thumbnail: http://lh4.ggpht.com/-HXElwVXPlV4/T2RgnYw2SeI/AAAAAAAAATY/RHL4H9fBXqw/s72-c/image_thumb.png?imgmax=800
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-1973359973792824666
blogger_orig_url: http://hoonzis.blogspot.com/2012/03/knockoutjs-and-google-maps-binding.html
---
This post describes the integration between Google Maps and KnockoutJS.
Concretely you can learn how to make the maps marker part of the View
and automatically change it's position any time when the ViewModel
behind changes. The ViewModel obviously has to contain the latitude and
longitude positions of the point that you wish to visualize on the map.

Previously I have worked a bit with Silverlight/WPF which in general
leaves one mark on a person: the preference for declarative definition
of the UI leveraging the rich possibilities of data binding provided by
the previously mentioned platforms. In this moment I have a small
free-time project where I am visualizing a collection of points on a
map. This post describes how to make the marker automatically change
it's position after the model values behind changes. Just like in this
picture bellow, where the position changes when user changes the values
of latitude and longitude in the input boxes.

[![image](http://lh4.ggpht.com/-HXElwVXPlV4/T2RgnYw2SeI/AAAAAAAAATY/RHL4H9fBXqw/image_thumb.png?imgmax=800 "image")](http://lh6.ggpht.com/-KtZ1v4DOEl4/T2RgmGXhokI/AAAAAAAAATQ/rOq7ve_0OH8/s1600-h/image%25255B2%25255D.png)

Since I like Model-View-ViewModel pattern I was looking for a framework
to use this pattern in JS, obviously [KnockoutJS](http://knockoutjs.com)
saved me. The application that I am working on, has to visualize several
markers on Google Maps. As far as I know there is no way to define the
markers declaratively. You have to use JS:

``` 
marker = new google.maps.Marker({
  map:map,
  draggable:true,
  animation: google.maps.Animation.DROP,
  position: parliament
});
google.maps.event.addListener(marker, 'click', toggleBounce);
```

So let's say you have a ViewModel which holds a collection of
interesting points, that will be visualized on the map. You have to
iterate over this collection to show all of them on the map. One
possible way around would be to use the **subscribe** method of KO. You
could subscribe for example to the latitude of the point (assuming that
the latitude would be an observable) and on any change perform the JS
code. There is a better way.

### Defining custom binding for Google Maps.

The way to go here is to [define a custom
binding](http://knockoutjs.com/documentation/custom-bindings.html),
which will take care of the update of the point on the map, any time,
that one of the observable properties (in basic scenario: latitude and
longitude) would change.

``` 
ko.bindingHandlers.map = {
init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
  var position = new google.maps.LatLng(allBindingsAccessor().latitude(), allBindingsAccessor().longitude());
  var marker = new google.maps.Marker({
    map: allBindingsAccessor().map,
    position: position,
    icon: 'Icons/star.png',
    title: name
  });
  viewModel._mapMarker = marker;
},
update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
  var latlng = new google.maps.LatLng(allBindingsAccessor().latitude(), allBindingsAccessor().longitude());
  viewModel._mapMarker.setPosition(latlng);
 }
}
```

``` 
;
```

So let's describe what is going on here. We have defined a **map**
binding. This binding is used on a div element. Actually the type of the
element is not important. There are also **latitude** and **longitude**
bindings, which are not defined. That is because the **map** binding
takes care of everything. The binding has two functions: **init** and
**update**, first one called only once, the second one called every time
the observable value changes.

The **allBindingsAccessor** parameter contains a collection of all
sibling bindings passed to the element in the **data-bind** attribute.
the **valueAccessor**, holds just the concrete binding (in this case the
map value, because we are in definition of the map binding). So from the
**allBindingsAccessor** we can easily obtain the values that we need:

``` 
allBindingsAccessor().latitude()
allBindingsAccessor().longitude()
allBindingsAccessor().map()
```

Notice that the **map** is passed to the binding in parameter (that is
concretely the **google.maps.Map** object, not the DOM element. Once we
have these values, there is nothing easier than to add the marker to the
map.

And there is one important thing to do at the end – **save the marker,
somewhere so we can update it’s position later**. Here again KO comes
with rescue, because we can use the **viewModel** parameter passed to
the binding and we can attach the marker to the ViewModel. Here I
suppose that there is no existing variable with name *_mapMarker** in
the viewModel and JS can happily add the variable to the ViewModel.

``` 
viewModel._mapMarker = marker; 
```

The update method has an easy job, because the marker has been stored,
and we only need to update it's position.

``` 
viewModel._mapMarker.setPosition(latlng);
```

### Almost full example

[Just check it here on JsFiddle](http://jsfiddle.net/Wt3B8/23/).

### Possible improvements

One thing that I do not like about this, is the fact, that you have to
pass the map as an argument to the binding and the div element has to be
outside of the map. Coming from Silverlight/WPF you would like to do
something like this:

    
    
    

That is actually the beauty of declarative UI definition. You can save a
lot of code only by composing the elements in the correct order. However
this is not possible – at least I was not able to get it to work. I was
close however:

``` 
init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
var map = element.parentNode;
var googleMap = new google.maps.Map(.map.);
//add the pointer
var contentToAddToMarker = element;
}
```

Again thanks to KO, here the element variable represents the DOM element
to which the binding is attached. If the div element is inside the map,
than we can get the parent element (which is the div for the map) and we
are able to create a new map on this element. The problem which I had is
the once, the new map was created, the div elements nested inside the
map disappeared. Even if that would work, some mechanism would have to
be introduced, in order to create the map only the first time (in case
there are more markers to show on the map) and store it somewhere
(probably as global JS variable).

On the other hand, thanks to the element you can get all the div which
should be for example given as the description to the marker.

Summary: **KnouckoutJS** **is great**. It lets me get rid of the
bordelic JS code.

[CodeProject](http://www.codeproject.com/script/Articles/BlogFeedList.aspx?amid=honga)
