---
layout: post
title: Events visualization with EvenDrops and KnockoutJS
date: '2014-11-02T15:40:00.000-08:00'
author: Jan Fajfr
tags:
- MVVM
- JavaScript
- Data
modified_time: '2014-11-10T08:44:58.407-08:00'
thumbnail: http://1.bp.blogspot.com/-Uq06dPrA0nk/VFa78Ko291I/AAAAAAAAEAE/AAyc-A9AhCY/s72-c/eventDrops.PNG
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-8504375191575189411
blogger_orig_url: http://hoonzis.blogspot.com/2014/11/events-visualization-with-evendrops-and.html
---
I have recently needed to visualize a set of events which occurred within a certain interval. Each event would have couple parameters and there would be multiple event lines. Let's say that you want to visualize the occurrences of car sales in couple countries. For each sale you would also want to visualize the price and the mark of the sold car. Before writing everything from scratch, I have found [EventDrops](https://github.com/marmelab/EventDrops) project which responded to the majority of my requirements. It had just one flaw and that is that there is no way to chart another characteristics for each event.

EventDrops KnockoutJS integration
=================================

I have decided to add such possibility and since I am using KnockoutJS and binding to create all of my charts I have also decided to add EventDrops to my [KoExtensions](https://github.com/hoonzis/KoExtensions) project - in order to make it's usage simpler. The resulting chart looks like this:

[![](http://1.bp.blogspot.com/-Uq06dPrA0nk/VFa78Ko291I/AAAAAAAAEAE/AAyc-A9AhCY/s320/eventDrops.PNG)](http://1.bp.blogspot.com/-Uq06dPrA0nk/VFa78Ko291I/AAAAAAAAEAE/AAyc-A9AhCY/s1600/eventDrops.PNG)

This [example](https://github.com/hoonzis/KoExtensions/blob/master/testpages/EventDrops.html) is available on GitHub as part of KoExtensions.

What I have added to the original event drops are the following
possibilities:

-   The chart now accepts generic collection instead of just a
    collection of dates. The developer in turn has to specify a function
    to get the date for each item
-   The size of the event is dynamic
-   The color of the event is dynamic
-   Better possibility to provide a however action
-   The size of the event can use logarithmic or linear scale
-   Everything is available as KnockoutJS binding

The html is now really straightforward:

```html 
<div data-bind="eventDrops: carSales, chartOptions: carSalesOptions"></div>
```

The javascript behind this page contains a bit more to generate the
data:

```javascript 
require(['knockout-3.2.0.debug', 'KoExtensions/koextbindings', 'KoExtensions/Charts/linechart', 'KoExtensions/Charts/piechart', 'KoExtensions/Charts/barchart'], function(ko) {
 function createRundomSales(country) {
  var event = {};
  var marks = ['Audi', 'BMW', 'Peugot', 'Skoda'];

  event.name = country;
  event.dates = [];
  
  var endTime = Date.now();
  var oneMonth = 30 * 24 * 60 * 60 * 1000;
  var startTime = endTime - oneMonth;

  var max = Math.floor(Math.random() * 80);
  for (var j = 0; j < max; j++) {
   var time = Math.floor((Math.random() * oneMonth)) + startTime;
   event.dates.push({
    timestamp: new Date(time),
    carMark: marks[Math.floor(Math.random() * 100) % 4],
    price: Math.random() * 100000
   });
  }

  return event;
 }


 function createSales() {
  var sales = [];
  var countries = ['France', 'Germany', 'Czech Republic', 'Spain'];
  countries.forEach(function(country) {
   var countrySales = createRundomSales(country);
   sales.push(countrySales);
  });
  return sales;
 }

 function TestViewModels() {
  var self = this;

  self.carSales = ko.observableArray([]);
  self.carSales(createSales());

  self.carSalesOptions = {
   eventColor: function (d) { return d.carMark; },
   eventSize: function (d) { return d.price; },
   eventDate: function (d) { return d.timestamp; },
   start: new Date(2014, 8, 1)
  };
 }

 var vm = new TestViewModels();

 ko.applyBindings(vm);
});
```

In this example the **createSales** and **createRandomSales** methods are just use to get testing data. Once the testing data is generated it is stored to the **carSales** observable collection. Any time this collection is changed the chart would be updated.

The sales collection looks a bit like this:

[![](http://3.bp.blogspot.com/-TdC5QSwU6gk/VFa_ve8kWXI/AAAAAAAAEAQ/2w9F3cFbACE/s320/eventDropsstruc.PNG)](http://3.bp.blogspot.com/-TdC5QSwU6gk/VFa_ve8kWXI/AAAAAAAAEAQ/2w9F3cFbACE/s1600/eventDropsstruc.PNG)

The **carSalesOptions** object contains the charting options. These tell to the event drops chart the necessary information to specify how big and which color should be used for the given event.