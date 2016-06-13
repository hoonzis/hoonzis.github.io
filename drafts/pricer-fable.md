#### Only arrays are observed
It seems that currently only arrays are observed by Vue.js correctly as collections. Typically for each strategy has a list of legs, that I expose as a member.

type StrategyViewModel(strat) =
       let mutable strategy: Strategy = strat
       member __.legs = strategy.Legs |> List.map (fun l -> LegViewModel(l))

This works the first time just for rendering, but as soon as you modify the original **strategy** mutable field and add or remove leg, nothing happens. In order to force Vue.js to observe the list, cast it to array:

member __.legs = strategy.Legs |> List.map (fun l -> LegViewModel(l)) |> Array.ofList

#### Broken types
In my Vue.js example, I have created a getter and setter for the strike field.


with get() =
    match leg.Definition with  
           | Option option -> option.Strike
           | _ -> 0.0
and set(strike) =
  let newLegDefinition = match leg.Definition with  
                                  | Option option -> { option with Strike = strike}
                                  | _ -> failwith "we should not be able to modify strike while not working on option"
  let newLeg = {
      Definition = Option newLegDefinition
      Pricing = leg.Pricing
  }
  leg <- newLeg


If you don't specify the type of **strike** the compiler will infer it to float, based on it's usage, however since this is bounded input field, one will actually get string on run-time.

#### Not supported
I have stumbled across the following two

Cannot find replacement for sign
Cannot find replacement for System.DateTime.toString
Cannot find replacement for System.Int32.parse (L66,20-L66,42) (C:\dev\Pricer\Pricer.Fabled\App.fs)

One can actually use **int** function instead of Int32.parse
