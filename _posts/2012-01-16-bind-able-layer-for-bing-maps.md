---
layout: post
title: Bind-able layer for Bing Maps
date: '2012-01-16T15:36:00.001-08:00'
author: Jan Fajfr
tags:
- Maps
- Silverlight
- WP7
modified_time: '2014-06-26T14:40:37.365-07:00'
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-8163684139737279068
blogger_orig_url: http://hoonzis.blogspot.com/2012/01/bind-able-layer-for-bing-maps.html
---
I had a special requirement on showing items on Bing Map.

I needed to show a collection of collections of objects - in this case
bike routes. Actually each route was composed of collection of routes
(which could be interconnected at some places).

This cannot be achieved only by **MapItemsControl** - which can render
only one dimensional collection.

So what I really need was to add dynamically a collection of
**MapItemsControl** to the map.

Instead of using **MapItemsControl** I have decided to use the
**MapLayer** class. I have created a new layer, which exposes a
**DependencyProperty** "Routes".

This property can be bound to a two dimensional collection. I use
**List** of Lists, but I guess I should have used **IEnumerable** to
make the usage more general. When the collection changes, the routes are
drawn to the card, by creating **MapPolyline** objects.

Of course a similar class could be created for **Pushpins** - and for
displaying a set of set's of places.

When taken further, actually this approach could even expose a Template
which could be set to specify how each item of the group will be
rendered.

Here is the code:


``` 
public class BikeRoutesLayer : MapLayer
{
    private static Color[] _colors = { Colors.Blue, Colors.Green, Colors.Orange,Colors.Gray };

    public List<List<LocationCollection>> Routes
    {
        get { return (List<List<LocationCollection>>)GetValue(RoutesProperty); }
        set { SetValue(RoutesProperty, value); }
    }
    public static readonly DependencyProperty RoutesProperty =
        DependencyProperty.Register("Routes", typeof(List<List<LocationCollection>>), typeof(BikeRoutesLayer), new PropertyMetadata(new PropertyChangedCallback(RoutesChangedCallBack)));
    
    static int i = 0;
    private static void RoutesChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        var layer = sender as BikeRoutesLayer;

        var list = args.NewValue as List<List<LocationCollection>>;

        if (list != null)
        {
            foreach (var bikeRoute in list)
            {
                foreach (var route in bikeRoute)
                {   
                    MapPolyline line = new MapPolyline();
                    line.Locations = route;
                    line.StrokeThickness = 1;
                    line.Stroke = new SolidColorBrush(_colors[i%_colors.Length]);
                    layer.Children.Add(line);
                }
                i++;
            }
        }
    }
}
```
