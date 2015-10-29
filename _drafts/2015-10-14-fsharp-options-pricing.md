Pricer is a small library for pricing options and generating payoff charts.

Payoff charts show you the profit of an option based on the movement of the underlying security itself. You can create a payoff charts of any derivative (options, futures, convertible bonds). Multiple options traded in the same time form strategies which can be used to obtain different type of protection or leverage against the movements of the underlying stock.

There are generally two methods to price options:
- Black Scholes formula
- Binomial pricing

Black Scholes is a closed formula for pricing European options (options that can be excercised only on the maturity). Binomial pricing is a sort of Monte Carlo method that can be applied to pricing diverse types of financial derivatives. Both of these methods place the same assumptions on underlying share and it's movements.

There are quite a lot of examples of the Black & Scholes implementation around the web. Taking into account the fact that it is a closed mathematical solution, besides writing the formula in programming language, there is not that much to talk about. So this blog post will be about Binomial Pricing. However, for both methods it is quite important to understand how the underlying stock movements are modelized.

Share price and it's movements
------------------------------
In order to determine the price of the option, one has to guess how the shares will move in the future. That is not possible be we can assume two facts:

- the stock price follows a long term direction (in the long term it keeps climbing or it declines)
- the stock oscilates randomly in the short term

These two assumptions are contained in the following equation, which describes the stock movement as generalized [Wiener process](https://en.wikipedia.org/wiki/Wiener_process) (type of stochastic process).

```
dS = alpha*S*dt + sigma*S*dW
```
This describes the change in the share price (dS) in time (dt), in the function of the current share price (S), the drift  (alpha), which is the long term direction of the stock, the volatility (sigma) and (dW) random value - the random swing in the share price.

This by it self would not be enough to build any pricing model. A second assumption is based on the share prices changes and specially about it's log returns.

Log returns are used because they are additive. We can say that the logarithm of weekly return is the same as the some of the daily returns of the given week:

```
log(Stweek/S_lastweek) = log (S_monday/S_lastweek) + log (S_tuesday/S_monday) + ... + log(S_friday/S_thursday)
```
The second assumption is: the logarithms of daily returns of share prices are independent - there is no correlation between the results the stock in consecutive days. Of course this is far from reality. The daily returns and the log returns as well can be correlated and that would happen specially during the crises periods where bing swings would happend in consecutive days on the markets. But it is crucial to make this assumption.

As a consequence it can be shown, that if we model the share as process of log returns, the daily log returns of the share are normally distributed. This is the key to Black Scholes and Binomial Pricing as well, because both of these method are based on the fact that the log returns are normally distributed.

Binomial pricing
----------------
The basic idea behind binomial pricing is the following. We assume that the share can move every day. With this knowledge we will build a tree (binomial) which will contain all paths that the share price could undertake from now until certain time (if we pricing the option, until maturity). 

Imperative way:
//we need to construct the binomial pricing model, using the CRR (Cox, Ross and Rubinstein)
//the original model is composed of 3 parameters p,u,d.
//u - up probability, d - down probability. p is the technical probability
//here we have PUp and PDown, for further simplifacation of the calculations

```FSharp
let deltaT = option.TimeToExpiry/float steps
let up = exp(stock.Volatility*sqrt deltaT)
let down = 1.0/up
let R = exp (stock.Rate*deltaT)
let p_up = (R-down)/(up-down)
let p_down = 1.0 - p_up

let pricing = {
    Periods = steps
    Up = up
    Down = down
    PUp = p_up
    PDown = p_down
    Rate = R
    Option = option
    Ref = stock.CurrentPrice
}

let prices = Array.zeroCreate pricing.Periods
let optionValue =
    match pricing.Option.Kind with
            | Call -> fun i -> max 0.0 (prices.[i] - pricing.Option.Strike)
            | Put -> fun i -> max 0.0 (pricing.Option.Strike - prices.[i])

prices.[0] <- pricing.Ref*(pricing.Down**(float pricing.Periods))
let oValues = Array.zeroCreate pricing.Periods
oValues.[0]<- optionValue 0
for i in 1 ..pricing.Periods-1 do
    prices.[i] <- prices.[i-1]*pricing.Up*pricing.Up
    oValues.[i]<- optionValue i

let counter = pricing.Periods-2
for step = counter downto 0 do
    for j in 0 .. step do
        oValues.[j] <- (pricing.PUp*oValues.[j+1]+pricing.PDown*oValues.[j])*(1.0/pricing.Rate)
oValues.[0]
```

Functional way
--------------
The basic step is here:

```FSharp
let step (derPrice:float list) (pricing:BinomialPricing) =
    derPrice  |> Seq.pairwise
              |> Seq.fold (fun derPrice' (down,up)->
                let price' = (pricing.PUp*up+pricing.PDown*down)*(1.0/pricing.Rate)
                derPrice' @ [price'])[]

let rec reducePrices (derPrice:float list) (pricing:BinomialPricing) =
    match derPrice with
            | [single] -> single
            | prices -> reducePrices (step prices pricing) pricing
```

The **step** method takes a list of prices, which are the prices of the option in the depth n of the pricing tree. If the tree has 4 steps, then in the last time step (at the maturity), this list will have 4 members. Step method takes this list and moves it one time step up in the tree. So the output of this method should have 3 elements.

We can profit of F# **pairwise** method to iterate over tuples of consecutive elements.

Rewriting all in functional way:
```FSharp
let binomialPricingFunc (pricing:BinomialPricing) =
    let optionVal stock =
        match pricing.Option.Kind with
                | Call -> max 0.0 (stock - pricing.Option.Strike)
                | Put -> max 0.0 (pricing.Option.Strike - stock)

    let prices = generateEndNodePrices pricing.Ref pricing.Up pricing.Periods optionVal

    let step (prices:(float*float) list) =
        prices
            |> Seq.pairwise
            |> Seq.fold (fun prices' ((sDown,dDown),(sUp,dUp)) ->
                let derValue = (pricing.PUp*dUp+pricing.PDown*dDown)*(1.0/pricing.Rate)
                let stockValue = sUp*pricing.Down
                let der' = if pricing.Option.Style = American then
                                let prematureExValue = optionVal stockValue
                                max derValue prematureExValue
                            else derValue
                prices' @ [stockValue,der'])[]

    let rec reducePrices prices =
        match prices with
                | [(stock,der)] -> der
                | prs -> reducePrices (step prs)

    reducePrices prices
```
