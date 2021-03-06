---
layout: post
title: Bing Maps - Bindable TilesLayer
date: '2011-05-17T02:53:00.001-07:00'
author: Jan Fajfr
tags:
- Maps
- Silverlight
modified_time: '2014-06-26T15:05:20.899-07:00'
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-2919375794157951507
blogger_orig_url: http://hoonzis.blogspot.com/2011/05/bing-maps-bindable-tileslayer.html
---

Specifying secondary tiles source allows you to cover parts of the map with you another layer. This layer might contain some additional information, such as new routes or points or any other geolocated information. In Silverlight you can use secondary tiles source by using the **TilesLayer** and **LocationRectTileSource** elements.

With Bing Maps and Silverlight you would basically do it like this:

```xml
<map:Map CredentialsProvider="..." Mode="Road" LogoVisibility="Collapsed"
                x:Name="map">
<map:TilesLayer x:Name="layer">
</map:TilesLayer>
</map:Map>
```
And in codebehind:

```csharp
  layer.TileSources.Clear();
  LocationRectTileSource source = new LocationRectTileSource();
  source.UriFormat = viewModel.TilesURL;
  source.BoundingRectangle = new LocationRect(viewModel.leftCorner, viewModel.rightCorner);
  source.ZoomRange = new Range<double>(10, 18);
  layer.TileSources.Add(source);
```

That works just fine, nevertheless it would be nice to do all of this in xml markup, in other words use data binding to populate UriFormat and Bounding Rectangle properties.

However some properties in the Bing Maps framework are not defined as DependencyProperties but as classical CLR properties. This is the case of UriFormat of LocationRectTileSource.

When you try to bind whatever string to this property, than you will obtain XML parsing error during run-time.

The similar situation happens for example for Stroke property of MapPolyline, which is probably more common issue. Here is a [fine solution for that one](http://geekswithblogs.net/bdiaz/archive/2010/02/27/bing-maps-data-binding-issues---cant-bind-to-stroke.aspx)

But back to the LocationRectTileSource. When there is property which is
not DependencyProperty and you would still like to bind data to this
property you can always write some kind of wrapper class.

Because I have always only one TilesLayer with one
LocationRectTileSource, I have decided to inherit my wrapping component
directly from TilesLayer.


```csharp
namespace CustomControls {
public class CustomTilesLayer : MapTileLayer
{
    public static readonly DependencyProperty TilesURLProperty = DependencyProperty.Register("TilesURL", typeof(String),
            typeof(CustomTilesLayer), new PropertyMetadata(new PropertyChangedCallback(TilesURLPropertyChanged)));


    public String TilesURL
    {
        get { return (String)GetValue(TilesURLProperty); }
        set { SetValue(TilesURLProperty, value); }
    }

    private static void TilesURLPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        CustomTilesLayer layer = sender as CustomTilesLayer;
        layer.TileSources.Clear();
LocationRectTileSource source = new LocationRectTileSource();
        source.UriFormat = a.NewValue;
        source.BoundingRectangle = new LocationRect(leftCorner,rightCorner);
        source.ZoomRange = new Range(10, 18);
        layer.TileSources.Add(source);
    }
}
}
```


Now you can use this component in your map the following way:

```xml
<usercontrol x:Class="YourClass"
xmlns:controls="clr-namespace:CustomControls">
<map:Map CredentialsProvider="..." Mode="Road" LogoVisibility="Collapsed"
                x:Name="map">
<controls:CustomTilesLayer TilesURL="{Binding TilesURL}" Opacity="0.8"/>
</map:Map>
</UserControl>
```
