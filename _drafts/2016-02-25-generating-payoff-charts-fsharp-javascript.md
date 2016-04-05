---
layout: post
title: Options charts in F\# and JavaScript
date: '2015-12-7T05:25:00.000-08:00'
author: Jan Fajfr
tags:
- Maps
modified_time: '2015-12-07T05:11:43.965-08:00'
---
I have recently been playing with options, their pricing and pay-off charts generation. I have created a small library call [Pricer](https://github.com/hoonzis/Pricer). This library can do few things:

- Calculate options prices
- Generate data for pay-off charts
- Analyze stock data from Quandl and calculate volatility and floating averages

In order to demonstrate what the library can do, I have created a small web application [Payoffcharts.com](http://www.payoffcharts.com/). Here is the list of the visualizations that it does:

Payoff chart of any strategy
![payoffcharts](https://raw.githubusercontent.com/hoonzis/hoonzis.github.io/master/images/optionscharts/payoffcharts_viz.PNG)

Using bubble chart to compare option prices depending on strike and expiry
![pricebubble](https://raw.githubusercontent.com/hoonzis/hoonzis.github.io/master/images/optionscharts/price_bubble_chart.PNG)

Using line chart to compare option prices with same strike and different expiries
![putexpiry](https://raw.githubusercontent.com/hoonzis/hoonzis.github.io/master/images/optionscharts/put_expiry.PNG)

Comparing the American and European option price with different expiry
![americaneuropean](https://raw.githubusercontent.com/hoonzis/hoonzis.github.io/master/images/optionscharts/american_vs_european.PNG)

## Payoff charts
A bit of theory is necessary here, just to describe the domain - options and strategies. Options are financial contracts that give you the right to buy some asset in the future for agreed price.

### Call and Put Options
- Call is the right to **buy** the stock for agreed price
- Put is the right to **sell** the stock for agreed price
- Option parameters:
	- Underlying (stock or commodity)
	- Strike
	- Expiry
	- Type (Put/Call)
	- Style (American/European)
	- Premium (the price)

Payoff chart, shows us how much we earn when the stock moves up or down
- Example: Call with strike 45, Premium: 2.5
![call](https://raw.githubusercontent.com/hoonzis/hoonzis.github.io/master/images/optionscharts/call.png)

### Strategies

- Traders can buy or sell multiple options in the same time
- Multiple options bought at the same time make up strategies
- Example: Call Spread
	- Buying a call
	- Selling a call with higher strike

![callspread](https://raw.githubusercontent.com/hoonzis/hoonzis.github.io/master/images/optionscharts/callspread.png)

Another example of a strategy: Covered Call

- Strategy built of a cash leg and an option leg
- I bought the stock at 100 and I want to sell at 120
- Let's profit a bit and sell a call option with strike 120

![callspread](https://raw.githubusercontent.com/hoonzis/hoonzis.github.io/master/images/optionscharts/coveredcall.png)

In order to generate the charts, we will need to defined the domain model. All this is part of the *Pricer* library and I have simplified it here a bit.
```fsharp
type OptionKind =
    | Call
    | Put

type OptionStyle =
    | American
    | European

type OptionLeg = {
        Direction : float
        Strike : float
        Expiry : DateTime
        Kind : OptionKind
        Style: OptionStyle
        PurchaseDate: DateTime
    }
    member this.TimeToExpiry = this.Expiry - this.PurchaseDate

type CashLeg = {
    Direction: float
    Price:float
}

type LegInfo =
    | Cash of CashLeg
    | Option of OptionLeg

type Pricing = {
    Premium: float
}

type Leg = {
    Definition:LegInfo
    Pricing:Pricing option
}

type StockInfo = {
    Rate:float
    Volatility: float
    CurrentPrice: float
}

type Strategy = {
    Stock : StockInfo
    Name : String
    Legs: Leg list
}
```
Now that we have the model, let's actually use it to create an example of a strategy. We will use the Covered Call as our example.
```fsharp
let buyingCash = {
	Definition = Cash {
		Price = 100.0
		Direction = 1.0
	}
	Pricing = None
}

let sellingCall = {
    Definition = Option {
        Direction = -1.0
        Strike = 120.0
        Expiry = new DateTime(2017,1,1)
        Kind = Call
        Style = European
        PurchaseDate = DateTime.Now
    }
    Pricing = None
}

let coveredCall = {
    Name = "Covered Call"
    Legs = [
            buyingCash
            sellingCall
        ]
    Stock =
        {
            CurrentPrice = 100.0
            Volatility = 0.20
            Rate = 0.01
        }
}
```
Now let's see how we can generate the payoff chart from the *coveredCall* variable.

- Let's forget the pricing of options
- How can we generate the payoff chart of any strategy?
- Chart is a composition of lines connecting X,Y coordinates
- The Y coordinate reflects the value of the option, so how do we calculate it?
- How much I can earn if I exercise the option now - that is when the underlying share is at certain given level?
```
let optionValue option stockPrice =
	match option.Kind with
			| Call -> max 0.0 (stockPrice - option.Strike)
			| Put -> max 0.0 (option.Strike - stockPrice)
```

![callvalue](https://raw.githubusercontent.com/hoonzis/hoonzis.github.io/master/images/optionscharts/callvalue.png)

- Up to this point we actually didn't take into account the price of the options, we have assumed, that we already had it.
- Next question is: What is the payoff of an option?
- When calculating the payoff we take into account the price of the option

```
let legPayoff leg pricing stockPrice =
	match leg with
		| Cash cashLeg -> stockPrice - cashLeg.Price
		| Option optionLeg -> optionValue optionLeg stockPrice - pricing.Premium

```
![callvalue](https://raw.githubusercontent.com/hoonzis/hoonzis.github.io/master/images/optionscharts/callvalue.png)

### Sampling the data

- In order to chart the data, we will need to generate X,Y pairs
- What are the interesting points on X axis?
- Basically the chart can change only on the strikes

```
let getInterestingPoints strategy =
	let strikes = strategy.Legs |> List.map (fun leg ->
			match leg.Definition with
				| Cash cashLeg -> cashLeg.Price
				| Option optionLeg -> optionLeg.Strike
		)
		let min = 0.5*(strikes |> Seq.min)
		let max = 1.5*(strikes |> Seq.max)
		seq {
			yield min
			yield! strikes
			yield max
		}
```
If we have all the interesting X points, we can now compute the option lines (one line per each leg). The following function will return a list of functions. Each function will be a payoff calculator which takes X will and returns Y payoff value of the option.
```fsharp
let payOffs = strategy.Legs |> Seq.map (fun leg ->
    let legPricing =
		  match leg.Pricing with
                | Some pricing -> pricing
                | None -> getLegPricing strategy.Stock leg

    let pricedLeg = { leg with Pricing = Some legPricing }
    legPayoff pricedLeg.Definition legPricing
)
```
Now let's get the actual values for all the interesting points on the X axis:
```fsharp
let interestingPoints = getInterestingPoints strategy
let legLinesData =
	payOffs |> Seq.map (fun payoff ->
		[for stockPrice in interestingPoints -> stockPrice, payOff stockPrice]
	)
```
So far we have calculated the values for all the legs, but not yet the actual strategy payoff line. The strategy payoff is just a sum of all the line payoffs.
```
let strategyLine = [for stockPrice in interestingPoints do yield stockPrice,
	payOffs |> Seq.sumBy (fun payOff -> payOff stockPrice)
]
```

## Serving the data by WebAPI F# controller
```fsharp
//given complete strategy  (stock and legs, returns the payoff chart data)
member x.Put([<FromBody>] strategy:Strategy) : IHttpActionResult =
    let strategyData, legsData = Options.getStrategyData strategy

    let buildLines (data:(Leg*(float*float) list) seq)=
        data |> Seq.map (fun (leg,linedata) ->
            {
                Linename = leg.Definition.Name
                Values = linedata
            })

    let payoff = {
        Legs = legsData |> Seq.map (fun (leg,_) -> leg)
        LegPayoffs = buildLines legsData
        StrategyPayoff =
        {    
            Linename = "Strategy"
            Values = strategyData
        }
    }
    x.Ok(payoff) :> _
```
## JavaScript front end
I have built the front end with KnockoutJS, mainly because I know it quite well. The view models reflect quite a lot the F\# domain presented above:
Leg definition is quite simple, I am ski
```javascript
function LegViewModel(dto,parent) {
    var self = this;

    self.parent = parent;
    self.expiry = ko.observable(new Date()).extend({ required: true });
    self.strike = ko.observable().extend({ required: true });
    self.kind = ko.observable().extend({ required: true });
    self.style = ko.observable("European");
    self.premium = ko.observable();
    self.delta = ko.observable();
}
```

Strategy is a composition of legs with some additional

```javascript
function StrategyViewModel(legs, stock,name,parent){
    var self = this;
    self.legs = ko.observableArray([]);
    self.stock = ko.observable();
    self.payoff = ko.observable();
    self.isBusy = ko.observable(false);
    self.message = ko.observable();
    self.name = ko.observable();
    self.parent = parent;
}
```

## Payoff charts html view
```html
<table class="table">
   <thead>
   <tr>
	   <th>Direction</th>
	   <th>Kind</th>
	   <th>Strike</th>
	   <th>Expiry</th>
	   <th>Price</th>
	   <th>Delta</th>
	   <th><button type="submit" class="btn btn-primary btn-xs" data-bind="enable:!isBusy(), click: addLeg">Add Leg</button></th>
   </tr>
   </thead>
   <tbody data-bind="foreach:legs">
   <tr>
	   <td><select class="form-control" data-bind="options: directions,value:direction" id="directions"></select></td>
	   <td><select class="form-control" data-bind="options: legTypes,value:kind"></select></td>
	   <td><input type="text" class="form-control form-control-inline" placeholder="Strike" data-bind="value: strike" id="strike"></td>
	   <td><input type="text" class="form-control form-control-inline" placeholder="Expiry" data-bind="visible:isOption,datepicker: expiry" id="expiry"></td>
	   <td><span data-bind="formattedValue:premium, rounding:2"></span></td>
	   <td><span data-bind="formattedValue:delta, rounding:2"></span></td>
	   <td>
		   <button type="button" class="btn btn-danger btn-xs" aria-label="Left Align" data-bind="click:remove">
			   <span class="glyphicon glyphicon-trash" aria-hidden="true"></span>
		   </button>
	   </td>
   </tr>
   </tbody>
</table>

<div id="payoffchart" data-bind="linechart: payoff,chartOptions:chartOptions">
</div>
```
