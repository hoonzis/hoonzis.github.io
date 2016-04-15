---
layout: post
title: C# Fluent Interface for ElasticSearch
date: '2015-07-29T09:44:00.000-07:00'
author: Jan Fajfr
tags:
- cs, ElasticSearch
modified_time: '2015-07-29T01:07:44.866-07:00'
---

NEST already provides a Fluent like interface for querying ElasticSearch, but to my taste this query language stays too close to ElasticSearch JSON query format. The result is reduced readability of NEST queries and too much technical noise. I have come up with set of extensions methods which just wrap NEST and improve the readability and add a bit of expressiveness (by my biased judgement of course).

For now I have developed filters, queries and aggregations that I am using in our latest project, this code will go into production within a month, so we shall get into stabilized library soon. This post should describe the motivation behind this small library.

*UPDATE:* We have slightly changed and simplified the syntax. Please refer to the [wiki](https://github.com/hoonzis/fluentnest/wiki/FluentNest-wiki) for examples and more details.

You can get the code from [GitHub](https://github.com/hoonzis/fluentnest) or the binaries directly from [NuGet](https://www.nuget.org/packages/FluentNest/).

### Nested Grouping
One type of aggregations that I use quite often are **Sums** (or other statistics) nested in groups. Typically you might want get a sum of each group based on different field of the indexed document. The following code should just get the sum of the price of cars, based on the the type of the car.

```cs
var result = client.Search<Car>(s => s
  .Aggregations(fstAgg => fstAgg
    .Terms("firstLevel", f => f
      .Field(z => z.CarType)
        .Aggregations(sums => sums
          .Sum("priceSum", son => son
          .Field(f4 => f4.Price)
        )
      )
    )
  )
);

var carTypes = result.Aggs.Terms("firstLevel");
foreach (var carType in carTypes.Items)
{
  var priceSum = (decimal)carType.Sum("priceSum").Value;
}

```

This is hard to read, specially because one needs to name every aggregation in order to retrieve it later. Also in order to nest the sum inside the group-by, inner call to **Aggregations** method is required.

The same query using **FluentNest** extensions methods looks like this:

```cs
groupedSum = Statistics
  .SumBy<Car>(s => s.Price)
  .GroupBy(s => s.EngineType);

var result = client.Search<Car>(search => search.Aggregations(x => groupedSum);
```

We can as well nest the *GroupBy* term into another group. Imagine you want to sum prices based on the car type and engine type. Here the query becomes scarier:

```cs
var result = client.Search<Car>(s => s
  .Aggregations(fstAgg => fstAgg
    .Terms("firstLevel", f => f
      .Field(z => z.CarType)
      .Aggregations(sndLevel => sndLevel
        .Terms("secondLevel", f2 => f2.Field(f3 => f3.EngineType)
          .Aggregations(sums => sums
            .Sum("priceSum", son => son
            .Field(f4 => f4.Price))
          )
        )
      )
    )
  )
);
```

Now to define the aggregation, we can just add another *GroupBy* to the existing aggregation in this way:

```cs
groupedSum = Statistics
  .SumBy<Car>(s => s.Price)
  .GroupBy(s => s.CarType)
  .GroupBy(s => s.EngineType)
```

Helper methods are available in the library, which will allow you to unwrap what you need from the ElasticSearch query result.

```cs
var carTypes = result.Aggs.GetGroupBy<Car>(x => x.CarType);
foreach (var carType in carTypes)
{
  var engineTypes = carType.GetGroupBy<Car>(x => x.EngineType);
}
```

If you are getting multiple statistical values, you might consider using typed aggregations container, so that you do not have to specify the type of the indexed entity. In the following example, the container holds stats based on type **Car**.

```cs
var container = result.Aggs.AsContainer<Car>();
var priceSum = container.GetSumBy(x => x.Price);
var allTypes = container.GetCardinalityBy(x => x.EngineType);

```
If it is preferable, one can obtain a **Dictionary** directly from the result, instead of a **List<BucketItem>**.

```cs
var engineTypes = result.Aggs.GetDictioanry<Car>(x => x.EngineType);
```

Another overload can give you an enumerable of predefined types if you pass the mapper function.

```cs
var types = result.Aggs.GetGroupBy<Car, CarType>(x => x.CarType, k => new CarType
{
  Type = k.Key,
  Price = k.GetSum(x=>x.Price).Value
});
```
Since the *Price* field is decimal, the *GetSum* method shall return decimal as well.

### Dynamic nested grouping
In some cases you might need to group dynamically on multiple criteria specified at run-time. For such cases there is an overload of GroupBy which takes the name of the field to used for grouping. This overload can be used to obtain nested grouping on a list of fields:

```cs
var agg = Statistics
  .SumBy<Car>(s => s.Price)
  .GroupBy(new List<string> {"engineType", "carType"});

var result = client.Search<Car>(search => search.Aggregations(x => agg));
```

### Multiple statistics
You can defined and obtain multiple statistics in the same time. This has been already shown in the previous snippet of code. The "And" notation can be used to obtain multiple statistics on the same level:

```cs
var aggs = Statistics
  .SumBy(x=>x.Price)
  .AndCardinalityBy(x=>x.EngineType)
  .AndCondCountBy(x=>x.Name, c=>c.EngineType == "Engine1");
```

### Conditional statistics
There are multiple ways to calculate a conditional sum or conditional count. One can always apply a filter on the whole query and add a sum aggregation:

```cs
var result = client.Search<Car>(search => search
  .Filter("filter", fd=>fd.Term(fd=>fd.EngineType,"Engine1"))
  .Aggregations(x =>x.Sum("sumAgg",f=>f.Field(x=>x.Price))));
```

This can be rewritten as a very simple query using FluentNest:

```cs
client.Search<Car>(search => search
  .FilteredOn(x=>x.EngineType == "Engine1")
  .Aggregations(a=>priceSum);
```

However there is one big problem. The filter is applied on all the data so all aggregations will be affected.

If you want multiple conditional aggregations based on different conditions, you need to define a **Terms** bucket for each conditional count or sum and then add an inner aggregation. Now imagine getting to different conditional sum, for instance summing according to Car type and engine type.

```cs
var result = client.Search<Car>(search => search
  .Aggregations(agg => agg
    .Filter("filterOne", f => f.Filter(innerFilter => innerFilter.Term(fd => fd.EngineType, EngineType.Diesel))
    .Aggregations(innerAgg => innerAgg.Sum("sumAgg", innerField =>
      innerField.Field(field => field.Price)))
    )
    .Filter("filterTwo", f => f.Filter(innerFilter => innerFilter.Term(fd => fd.CarType, "Type1"))
    .Aggregations(innerAgg => innerAgg.Sum("sumAgg", innerField =>
      innerField.Field(field => field.Price)))
    )
  )
);

var sumAgg = result.Aggs.Filter("filterOne");
Check.That(sumAgg).IsNotNull();
var sumValue = sumAgg.Sum("sumAgg");
Check.That(sumValue.Value).Equals(50d);
```

This starts to get really hard to read. There is no global filter here, the filter is applied only on the single sum. The same can be done very easily:

```cs
var aggs = Statistics
  .CondSumBy<Car>(x=>x.Price, c=>c.EngineType == "Engine1")
  .AndCondSumBy<Car>(x=>x.Sales, c=>c.CarType == "Car1");

var result = client.Search<Car>(search => search.Aggregations(a=>aggs)):
var sum = result.Aggs.GetCondSum<Car>(x=>x.Price, y=>y.EngineType == "Engine1");
```

The original query could get much longer, if the condition for the query would be more complex. Here we have used a simple **Terms** filter, but this could be a composed filter as well. This is described further in next paragraph.

### Expressions to queries compilation
One of the motivations for FluentNest was also to simplify this issue - filter should be quite easy to write. In the example above the filter was specified with the following lambda:

```cs
c=>c.EngineType == "Engine1"
```

The lambda got translated into **Terms** query:

```cs
f => f.Filter(innerFilter => innerFilter.Term(fd => fd.CarType, "Type1"))
```

The aim here would be to make any lambda translate into a valid filter. That is of course not possible, but we can get quite close and cover the most used queries.

ElasticSearch provides several ways of data filtering. Equality, Ranges, Minimal values and others are supported. The only downside that I see with NEST is that the query is quite complicated compared to standard LINQ query.

Here is a query which tests on a timestamp field, here getting only documents in given range. The date is compared to some fixed dates:

```cs
var startDate = new DateTime(2010, 1, 1);
var endDate = new DateTime(2010, 5, 1);

var result = client.Search<Car>(s => s.Query(
    q=>q.Filtered(fil=>fil.Filter(
      x => x.And(
        left=>left.Range(f=>f.OnField(fd=>fd.Timestamp).Greater(startDate)),
        right=>right.Range(f=>f.OnField(fd=>fd.Timestamp).Lower(endDate))
      )
    )
  )
));
```

With FluentNest you can write this query simply as:

```cs
result = client.Search<Car>(s => s.FilteredOn(f => f.Timestamp > startDate && f.Timestamp < endDate));
```

This is achieved of course by the analysis of the lambda expression and it's recursive rewrite as series of NEST expressions. This approach however has limits. Not all standard C# operators are implemented and not all of them can be implemented with NEST capabilities. Right now just a subset of operators was implemented (==, <,<=,>,>=).

Some more examples:

```cs
var result = client.Search<Car>(s => s.FilteredOn(f => f.Timestamp > startDate && f.Timestamp < endDate));
var result = client.Search<Car>(s => s.FilteredOn(f=> f.Ranking.HasValue || f.IsAllowed);
var result = client.Search<Car>(s => s.FilteredOn(f=> f.Ranking!=null || f.IsAllowed == true);
```

Hitograms
---------
Histogram is another useful aggregation supported by ElasticSearch. One typical usage is to simply get documents into buckets using some criteria, other usage that I have been using quite a lot is computing some statistics on each bucket. For instance calculating a sum monthly sales, might be done using a histogram with nested aggregation.

```cs
var result = client.Search<Car>(s => s.Aggregations(a => a.DateHistogram("by_month",
    d => d.Field(x => x.Timestamp)
        .Interval(DateInterval.Month)
        .Aggregations(
          aggs => aggs.Sum("priceSum", dField => dField.Field(field => field.Price))))));

var histogram = result.Aggs.DateHistogram("by_month");
Check.That(histogram.Items).HasSize(10);
var firstMonth = histogram.Items[0];
var priceSum = firstMonth.Sum("priceSum");
Check.That(priceSum.Value.Value).Equals(10d);
```

Again here is simplified syntax using *FluentNest*:

```cs
var agg = Statistics
  .SumBy<Car>(x => x.Price)
  .IntoDateHistogram(date => date.Timestamp, DateInterval.Month);

var result = client.Search<Car>(s => s.Aggregations(a =>agg);
var histogram = result.Aggs.GetDateHistogram<Car>(x => x.Timestamp);
```

The result which comes from *GetDateHistogram* function will be a *IList<HistogramItem>*, where *HistogramItem* is defined inside ElasticSearch.

### Composing Filters and Aggregations
Filtering and aggregations are two separate things and can be done in the same time. The filter limits the set of all documents taken into account and then the aggregations are applied. As mentioned before on can define filters inside each aggregation (typically for computing multiple conditional sums). Histogram is just standard aggregation so we can filter the data as well before applying the histogram aggregation.

```cs
var agg = ...define aggregation here
var result = client.Search<Car>(s => s.FilteredOn(f => f.Timestamp < end && f.Timestamp > start).Aggregations(x =>agg);
var histogram = result.Aggs.GetDateHistogram<Car>(x => x.Timestamp);
```

### Common pattern for statistical queries
While looking at queries in the project that I am working on, quite clear pattern emerged for me, the I am now using in all queries:

```cs
var aggs = Statistics.SumBy<Car>(x => x.Price)
  .AndCondCountBy(x => x.CarId, x => x.New == true)
  .IntoDateHistogram(d => d.Timestamp, DateInterval.Month);

var filter = ...

var sc = new SearchDescriptor<Car>()
  .FilteredOn(filter)
  .Aggregations(agg => aggs);

var json = Encoding.UTF8.GetString(elasticClient.Serializer.Serialize(sc));
log.Debug($"ES Query: {json}");

var result = client.Search<Car>(sc);
```

All queries filter the results and perform few aggregations on them. Creating the **SearchDescriptor** before actually running the query also allows us to log the whole ElasticSearch query in the JSON format. The descriptor can be serialized using the NEST provided serializer.

### Why did we do this?
We have been moving the statistics module of .NET application from SQL Server, we are probably going to use ElasticSearch, since the [ES aggregations](https://www.elastic.co/guide/en/elasticsearch/reference/current/search-aggregations.html) are very flexible and fit our needs.

At the end our queries are quite complex. I don't want to enter into the details of the domain, but the following query gets some aggregated values grouped by a company name and than different values as global aggregations.

```cs
var aggDescription = Statistics
  .SumBy<Car>(x => x.PriceEuro)
  .AndSumBy(x => x.TotalCompetitors)
  .AndCondCountBy(x => x.TradeID, x => x.Won == true)
  .AndCardinalityBy(x => x.TradeID)
  .GroupBy(x => x.Company)
  .AndSumBy(x => x.PriceEuro)
  .AndCardinalityBy(x => x.TradeID)  
```

You can imagine that such query would be very complicated using only the NEST client and we were forced to write an abstraction layer. The question is now whether such abstraction library would be useful as well for someone else?
