---
layout: post
title: ElasticSearch TopHits with FluentNest
date: '2016-04-12T05:11:43.965-08:00'
author: Jan Fajfr
tags:
- ElasticSearch
modified_time: '2016-04-12T05:11:43.965-08:00'
---

When providing statistics or charts to the users, one usually groups up the values into aggregated collection - let's say sum per month, or average per category. In certain cases you might want to provide few details about the most significant elements in each bucket. For instance what were the biggest trades contributing to the total for given month. This can be easily done with ElasticSearch, but the queries get quite complicated. This post shows an easy way using [FluentNest](https://github.com/hoonzis/fluentnest) library.

Let's say you are building statistics of sold records per music style and want to see who were the most selling artists per each style. When using ElasticSearch, you would use the to perform such "aggregation with detail" scenario, you should use  [TopHits](https://www.elastic.co/guide/en/elasticsearch/reference/current/search-aggregations-metrics-top-hits-aggregation.html), combined with [Terms](https://www.elastic.co/guide/en/elasticsearch/reference/current/search-aggregations-bucket-terms-aggregation.html) aggregation. The above described scenario could be queried like this:

```json
{
  "aggs": {
    "sales_per_style": {
      "terms": {
          "field": "style",
          "size": 10
      },
      "aggs": {
      	"sum_sold_records": {
          "sum": {
            "field": "soldRecords"
          }
        },
        "top_artists": {
          "top_hits": {
              "sort": [
                  {
                      "soldRecords": {
                          "order": "desc"
                      }
                  }
              ],
              "_source": {
                  "include": [
                      "artistName"
                  ]
              },
              "size" : 10
          }
        }
      }
    }
  }
}
```

We are using C\# and we have developed a wrapper around *NEST* C\# library, which facilitates statistical queries. Imagine that we have stored a documents in ElasticSearch, one per each artist, something along these lines:

```csharp
public class Artist {
    public int Id {get;set;}
	public string ArtistName {get;set;}
	public int SoldRecords {get;set;}
	public string Style {get;set;}
}
```

Getting the sum of sold records per style, including the top 10 artists per each style can be achieved just in the following way:

```csharp
var result = client.Search<Artist>(sc => sc.Aggregations(agg => agg
    .SumBy(x => x.SoldRecords)
    .SortedTopHits(10, x => x.SoldRecords, SortType.Descending)
    .GroupBy(x => x.Style))
);
```

That is pretty cool if you compare that to the JSON above. What about getting the values from the result? Well it's a group by query, so we should be able to get a list of styles and for each style the top artists.

```csharp
var styles = result.Aggs.GetGroupBy<Car>(x => x.Style);
foreach (var styleBucket in styles)
{
    var topArtists = styleBucket.GetSortedTopHits<Artist>(x => x.SoldRecords, SortType.Descending);
    Check.That(topArtists).HasSize(10);
}
```
**FluentNest** provides extensions method such as *GetSortedTopHits* which you can use on the result. You might find it bit awkward to specify the query details again to get the result. But currently this is the only "typed" way to retrieve the values from ElasticSearch result, which is not typed and all aggregations and values are defined as string values.

In some cases one does not care about the sorting of the, in such case a simpler method is available which takes just the number of documents to get.

```csharp
var result = client.Search<Artist>(sc => sc.Aggregations(agg => agg
    .SumBy(x => x.SoldRecords)
    .TopHits(10)
    .GroupBy(x => x.Style))
);
```

### Fetching only few properties per hit
In some cases you don't need the whole documents, but just the important parts of the document. Usually these will be the names, ids or other unique fields. In order to save some bandwidth, you can tell ElasticSearch to retrieve only the important parts.

Using ElasticSearch query language, this can be achieved by appending *_source* details to the aggregation.

```
"top_artists": {
  "top_hits": {
	"sort": [
		{
			"soldRecords": {
				"order": "desc"
			}
		}
	],
	"_source": {
		"include": [
			"artistName"
		]
	},
"size" : 10
}
```

In the C\# version, you can just append all the fields to fetch right into the query. The following query will fetch the artist name and style. Other fields will keep their default value.

```csharp
var result = client.Search<Artist>(sc => sc.Aggregations(agg => agg
    .SumBy(x => x.SoldRecords)
    .SortedTopHits(10, x => x.SoldRecords, SortType.Descending, incl => incl.ArtistName, incl => incl.Style)
    .GroupBy(x => x.Style))
);
```
