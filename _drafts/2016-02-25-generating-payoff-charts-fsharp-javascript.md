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

- Calculate options practices
- Generate data for pay-off charts
- Analyze stock data from Quandl and calculate volatility of stock

In order to demonstrate what the library can do, I have created a small web application [Payoffcharts.com](http://www.payoffcharts.com/). Here is the list of the visualizations that it does:

Payoff chart of any strategy
![payoffcharts](https://raw.githubusercontent.com/hoonzis/hoonzis.github.io/master/images/optionscharts/payoffcharts_viz.PNG)

Using bubble chart to compare option prices depending on strike and expiry
![pricebubble](https://raw.githubusercontent.com/hoonzis/hoonzis.github.io/master/images/optionscharts/price_bubble_chart.PNG)

Using line chart to compare option prices with same strike and different expiries
![putexpiry](https://raw.githubusercontent.com/hoonzis/hoonzis.github.io/master/images/optionscharts/put_expiry.PNG)

Comparing the American and European option price with different expiry
![americaneuropean](https://raw.githubusercontent.com/hoonzis/hoonzis.github.io/master/images/optionscharts/american_vs_european.PNG)

## Using Quandl to get stock data

## Estimating volatility

## JavaScript front end
