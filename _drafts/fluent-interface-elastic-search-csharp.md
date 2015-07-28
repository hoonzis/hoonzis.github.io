Fluent C# interface for ElasticSearch on top of NEST
====================================================
NEST already provides a Fluent like interface for querying ElastiSearch, but for my taste it stays too close to ElastiSearch json query format. The result is reduced readability of NEST queries and too much technical noise. I have come up with set of extensions methods which just wrap NEST and improve the readability and add a bit of expresiveness (by my ambiased judgement of course). I have only developed filters, queries and aggregations that I am using in my projects.
You can get the code on GitHub or the library directly on NuGet.

Nested Grouping
---------------
One type of aggregations that I use quite often are Sums (or other statistics) nested in groups. Typically you might want get a sum of each group based on some criteria.

```
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
	// do something               
}

```
This is hard to read, specially because one needs to name every aggregations, in order to retrieve it later. Also in order to nest the sum inside the group-by, a call to *Aggregations* method is required.

The same query using *FluentNest* extensios methods looks like this:
```
groupedSum = Statistics
	.SumBy<Car>(s => s.Price)
	.GroupBy(s => s.EngineType);
	
var result = client.Search<Car>(search => search.Aggregations(x => groupedSum);
```
We can as well nest the GroupBy term. Imagine you want to sum prices based on the car type and engine type. Here the query becomes even scarier:
```
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
Now to define the aggregation, we can just add another GroupBy to the existing aggregation in this way.
```
groupedSum = Statistics
	.SumBy<Car>(s => s.Price)
	.GroupBy(s => s.CarType)
	.GroupBy(s => s.EngineType)
```
Other helper methods will allow you to unwrap what you need from the ElastiSearch query result.
```
var carTypes = result.Aggs.GetGroupBy<Car>(x => x.CarType);
foreach (var carType in carTypes)
{
	var engineTypes = carType.GetGroupBy<Car>(x => x.EngineType);
	//do somethign with nested types
}
```
If it is preferable, one can obtain a Dictionary directly from the result, instead of a List<BucketItem>.
```
var engineTypes = result.Aggs.GetDictioanry<Car>(x => x.EngineType);
```
Another overload can give you an enumerable of predefined types if you pass the mapper function.
```
var types = result.Aggs.GetGroupBy<Car, CarType>(x => x.CarType, k => new CarType
{
	Type = k.Key,
	Price = k.GetSum<Car,Decimal>(x=>x.Price).Value
});
```
Notice also that in this example a different overload of GetSum is used. This one takes two generic parameters. Since the *Price* field is decimal, the GetSum method shall return decimal as well. Ideally I would like the type to be inferred from the lambda. However since the compiler cannot infer the Car type passed in, one has to specify the Decimal parameter as well. There is some place for improovement here.

Dynamic nested grouping
-----------------------
In some cases you might need to group dynamically on multiple criteria specified at run-time. For such cases there is an overload of GroupBy which takes the name of the field to used for grouping. This overload can be used to obtain nested grouping on a list of fields:


Conditional statistics
----------------------
There are multiple ways to calculate a conditional sum or conditional count. One can always apply a filter on the whole query and add a sum aggregation:
``` 
var result = client.Search<Car>(search => search
	.Filter("filter", fd=>fd.Term(fd=>fd.EngineType,"Engine1"))
	.Aggregations(x =>x.Sum("sumAgg",f=>f.Field(x=>x.Price))));
``` 
This can be rewriten as a very simple query:
``` 
client.Search<Car>(search => search
	.FilteredOn(x=>x.EngineType == "Engine1")
	.Aggregations(a=>priceSum);
```

The filter is however applied on all the data so all aggregations will be affected. If you want multiple conditional aggregations based on different conditions, you need to define a Terms bucket for each conditional count or sum and then add an inner aggregation. This starts to get really hard to read:

The same can be done very easily:
```
var aggs = Statistics
	.CondSumBy<Car>(x=>x.Price, c=>c.EngineType == "Engine1")
	.AndCondSumBy<Car>(x=>x.Sales, c=>c.CarType == "Car1");
	
var result = client.Search<Car>(search => search.Aggregations(a=>aggs)):
```

Expressions to queries
---------------------
ElasticSearch provides several ways of data filtering. Equality, Ranges, Minimal values and others are supported. The only downside that I see with NEST is that the query is quite complicated compared to standard LINQ query.

Here is a query which tests on a timestamp field, here getting only documents in given range. The date is compared to some fixed dates:
```
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
```
result = client.Search<Car>(s => s.FilteredOn(f => f.Timestamp > startDate && f.Timestamp < endDate));
```

This is achieved of course by the analysis of the lambda expression and it's recursive rewrite as series of NEST expressions. This approach however has limits. Not all standard C# operators are implemented and not all of them can be implemented with NEST capabilities. Right now just a subset of operators was implemented (==, <,<=,>,>=).

Multiple statistics
-------------------
You can defined and obtain multiple statistics in the same time. This has been already shown in the previous snippet of code. The "And" notation can be used to obtain multiple statistics on the same level:
```
var aggs = Statistics
	.SumBy(x=>x.Price)
	.AndCardinalityBy(x=>x.EngineType)
	.AndCondCountBy(x=>x.Name, c=>c.EngineType == "Engine1");
```

Hitograms
---------
Histogram is another useful aggregation supported by ElasticSearch. One option is to simply get documents into buckets using some criteria, other option that I have been using quite a lot is computing some statistics on each bucket. For instance calculating a sum monthly sales, might be done using a histogram with nested aggregation.

Again here is simplified syntax using *FluentNest*:
```
var agg = Statistics
	.SumBy<Car>(x => x.Price)
	.IntoDateHistogram(date => date.Timestamp, DateInterval.Month);

var result = client.Search<Car>(s => s.Aggregations(a =>agg);
var histogram = result.Aggs.GetDateHistogram<Car>(x => x.Timestamp);
```

The result which comes from *GetDateHistogram* function will be a *IList<HistogramItem>*, where *HistogramItem* is defined inside ElasticSearch.

Composing Filters and Aggregations
----------------------------------
Filtering and aggregations are two separate things and can be done in the same time. The filter limits the set of all documents taken into account and then the aggregations are applied. As mentioned before on can define filters inside each aggregation (typically for computing multiple conditional sums). Histogram is just standard aggregation so we can filter the data as well before applying the histogram aggregation.

```
var agg = ...define aggregation here
var result = client.Search<Car>(s => s.FilteredOn(f => f.Timestamp < end && f.Timestamp > start).Aggregations(x =>agg);
var histogram = result.Aggs.GetDateHistogram<Car>(x => x.Timestamp);
```
