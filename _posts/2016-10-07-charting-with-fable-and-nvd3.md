---
layout: post
title: Charting with Fable and NVD3
date: '2016-11-02T05:25:00.000-08:00'
author: Jan Fajfr
tags:
- Fable, F#
modified_time: '2016-11-02T05:11:43.965-08:00'
---

If you are F# developer, chances are you have already heard of [Fable](http://fable.io/). Fable transpiles F# code into JavaScript, so you can run F# in the browser. It also generates map files, so that one can debug F# in the browser. The generated JS is very readable so if something goes wrong you can still look to the "compiled" code. It's really a great project and I was amazed on how few modifications were necessary to my code to make it compile into JS. I am working on a [small application to visualize some financial data](http://www.payoffcharts.com) and I have built it with Fable and [NVD3](https://github.com/novus/nvd3) JavaScript charting library and this post describes how to make them work together.

The code is available at [https://github.com/hoonzis/FabledCharting](https://github.com/hoonzis/FabledCharting).

The aim of this tutorial will be just to generate some random data and show it in a scatter chart. But we want to do this with F# in the browser:

![scatterExample](https://raw.githubusercontent.com/hoonzis/hoonzis.github.io/master/images/optionscharts/scatter-example.PNG)

### Installing Fable
Fable is a compiler, so you might expect an executable. In this case it comes bundled as **npm** package, which you might want to install globally (since you might use it over multiple projects). Note that you will need NodeJs, but once you have it you can just type:

```
npm install -g fable-compiler
```

After installing you can compile a F# files into JavaScript by running:

```
fable myApp.fsx
```
If you checkout [this tutorial's repository from github](https://github.com/hoonzis/FabledCharting), you can just run fable without pointing to any files, since I have provided the fable's configuration file.

You future application will be probably composed of multiple files and multiple JavaScript files will be generated, so in order to get started there is a bit of configuration.

### Setup your Fable project
Before you get started with any code, you have to consider few things. Fable will be just a part of your compilation chain. The chain will probably look like this:

```
Install JS dependencies -> F# Code -> Fable -> JS Files -> Bundled JS -> Reference or inject JS into HTML
```

So you will have to decide on what JS files Fable should generate and how those should be bundled into desired output. Let's go over the steps to decide:

#### Single fsx or a project?
Are you going to fit everything into single F# file or you will create a project? Fable allows you to do both and in the simplest situation I can just point fable to a single fsx file, but if you plan to build something bigger, you better create F# project (F# library will work, even though you won't distribute anything as library but compile into JS instead).

#### JavaScript modules
How the resulting JavaScript should be structured and packaged? Two most common ways of defining modules in JavaScript are AMD and CommonJS.

#### JavaScript bundling
Fable will create one JavaScript file per F# file, these files have to be then loaded by the browser. Depending on the module pattern (CommonJS vs AMD) you will have to bundle them together to a single JavaScript file which will be included in the html.

I have chosen the following setup: F# project with multiple files. CommonJS as module system, bundled with Webpack. To go for this configuration, you will have to create the following 3 files in the root of the project folder:

- package.json - configure your npm dependencies and the whole node project
- fableconfig.json - configure fable
- webpack.config.json - configure webpack to wrap fable's output

#### package.json
Will define your JavaScript project (standard Node project), here you can list the necessary npm packages that you depend on. I will be using NVD3 and D3 for charting. But you might also notice fable-core and core-js. These two will assure that the JavaScript generated by Fable will run correctly in the browser.

Notice that webpack and source-map-loader are defined as development dependencies (that is dependencies not used by the application itself, but used to build it). They will be further described bellow.

```json
{
  "private": false,
  "name": "fabled-charting",
  "version": "1.0.0",
  "main": "index.js",
  "author": "Jan Fajfr",
  "license": "MIT",
  "dependencies": {
    "core-js": "^2.4.0",
    "d3": "^3.5.17",
    "fable-core": "0.6.6",
    "nvd3": "^1.8.4",
  },
  "engines": {
    "fable": ">=0.2.11"
  },
  "devDependencies": {
    "source-map-loader": "^0.1.5",
    "webpack": "^1.13.2"
  }
}
```

In order to install all the JavaScript dependencies you have to run **npm install**. You can later also configure Fable to do that for you.

#### fableconfig.json
Fable's configuration. You have to tell fable what are his dependencies, which JavaScript module system should be used. Since I have decided to use webpack for JS files bundling, I can also ask fable to run webpack when the compilation is finished (in post-build).

I am pointing fable to single F# project file and asking him to generate source maps, so that you can debug F# in chrome's console.

Note that things might get more complicated when you have multiple F# projects and you will built them with Fable and reference each other, but I will cover that in future post.

```json
{
  "module":  "commonjs",
  "sourceMaps": true,
  "projFile": "FabledCharting.fsproj",
  "outDir": "compiledjs",
  "scripts": {
    "postbuild": "webpack"
  },
  "targets": {
    "watch": {
      "scripts": {
        "postbuild": "webpack --watch"
      }
    }
  }
}
```

#### webpack.config.json
Webpack's config. We will have to tell fable where the JS files are (generated by fable in the previous step) and where to output the bundled result.

```javascript
module.exports = {
    entry: {
        chartingTest: "./compiledjs/ChartingTest"
    },
    output: {
        filename: "[name].bundle.js",
        path: "./out"
    },
    devtool: "source-map",
    module: {
        preLoaders: [{
            loader: "source-map-loader",
            exclude: /node_modules/,
        }],
    },
    externals: {
        "d3": "d3"
    }
};
```

### Creating the web page
The page itself will be very small - it contains only one div tag which we will use to draw the chart. We also have to reference all JavaScript libraries on which we are depending and finally the JavaScript compiled from our F# code.

```html
<!doctype html>
<html>
<head>
    <meta charset="utf-8">
    <title>Chart test</title>
    <script src="node_modules/d3/d3.min.js" charset="utf-8"></script>
    <script src="node_modules/nvd3/build/nv.d3.js"></script>
    <link href="node_modules/nvd3/build/nv.d3.css" rel="stylesheet">
    <script src="node_modules/core-js/client/core.min.js"></script>
</head>
<body>
    <div>
        <svg id="chart"></svg>
    </div>
    <script src="out/chartingTest.bundle.js"></script>
</body>
</html>
```

You can see that all the JavaScript references got into the node_modules folder. That is where NPM will install them by default.

### Application's entry point
Let's look at the content of "main" file. The file contains a single module, with a single function. This function will be called when the JavaScript is loaded.

```ocaml
namespace FabledCharting

open System

module ChartingTest =

    let random = new Random()

    let randomValues() =
        [|1 .. 10|] |> Array.map (fun i ->
        {
            x = new DateTime(2014, i, 1)
            y = float (random.Next() / 100000)
            size = float (random.Next())
        })

    let drawChart() =
        let series = [|
                {
                    key = "Series 1"
                    values = randomValues()
                };
                {
                    key = "Series 2"
                    values = randomValues()
                }
            |]

        Charting.drawScatter series "#chart"

    drawChart()
```

All we are doing here is preparing the data for the charting. We will generate an array of items for the scatter chart. Each item has three properties (x,y,size). Then we will wrap all the data into two series and pass to the drawing function. Note that nothing is said here about the types of the values, but the standard NVD3 JavaScript code would look very similar. You can look at the [official scatter chart example](https://github.com/nvd3-community/nvd3/blob/gh-pages/examples/scatterChart.html). We will look into the details of **drawScatter** method later.

### Writing NVD3 types
In the previous snippet we have created an array of "Series" with two series of random data. The shape of the series reflects what NVD3 is expecting, but since we are compiling from F# we have to define the types that would resemble to what NVD3 is expecting. That is really not that hard, these 2 records will do:

```ocaml
type DateScatterValue = {
    x: DateTime
    y: float
    size: float
}

type Series<'a> = {
    key: string
    values: 'a array
}
```
Besides the shape of the data, we will have to describe the methods and objects provided by NVD3, but the rest will be much clearer if we look at the function that draws the data first.

### Drawing the charts
Our drawing function takes four parameters. First the data to draw and a selector which will tell us to which element in the HTML page we should draw the chart. We also pass the labels of the axis as parameters.

The content will again resemble a lot to JavaScript NVD3 [official scatter chart example](https://github.com/nvd3-community/nvd3/blob/gh-pages/examples/scatterChart.html).

```ocaml
let drawScatter (data: Series<DateScatterValue> array) (chartSelector:string) xLabel yLabel =
    let colors = D3.Scale.Globals.category10()
    let chart = nv.models.scatterChart().pointRange([|10.0;800.0|]).showLegend(true).showXAxis(true).color(colors.range())
    chart.yAxis.axisLabel(yLabel) |> ignore
    chart.xAxis.axisLabel(xLabel) |> ignore
    let chartElement = D3.Globals.select(selector);
    chartElement.html("") |> ignore
    chartElement.style("height","500px") |> ignore
    chartElement.datum(data).call(chart) |> ignore
```

This function calls two libraries: NVD3 and D3 itself. There is however a small difference. For any NVD3 code that we call, we have to write the F# bindings, so that F# compiler is not confused, and Fable "knows" that it can just output the code itself.

For D3 we can use the [Fable D3 Bindings](https://www.npmjs.com/package/fable-import-d3) which are available as npm package. That basically means that for instance **D3.Scale.Globals.category10** has been defined in the D3 bindings DLL and Fable knows that the correct JavaScript output is **d3.scale.category10()**. These bindings are provided by Fable contributors.

### Writing custom bindings for NVD3
For NVD3, we have to defined the bindings manually. So let's start with the call to **nv.models.scatterChart**.

```ocaml
module nv =
    let models: ChartModels = failwith "JS only"

type ChartModels =
    abstract lineChart: unit -> LineChart
    abstract scatterChart: unit -> ScatterChart
```

So we have defined **nv** module with **models** field, this will make F# compiler happy. And Fable will just output the same code in JavaScript. ChartModels is a type that provides all the charts. Our example is using only **scatterChart** but I have defined also **lineChart** just to show you the whole idea behind:

```ocaml
[<AbstractClass>]
type Chart() =
    abstract xAxis: Axis
    abstract yAxis: Axis
    abstract showLegend: bool -> Chart
    abstract showXAxis: bool -> Chart
    abstract showYAxis: bool -> Chart
    abstract color: string[] -> Chart

[<AbstractClass>]
type LineChart() = inherit Chart()
        with member __.useInteractiveGuideline (value:bool): Chart = failwith "JSOnly"

[<AbstractClass>]
type ScatterChart() = inherit Chart()
    with member __.pointRange(value: double array): ScatterChart = failwith "JSOnly"
```

**ScatterChart** and **LineChart** both inherit from the base **Chart** type. We also have to define the Axis:

```ocaml
type Axis =
    abstract axisLabel: string -> Axis
    abstract tickFormat: System.Func<Object,string> -> Axis
```

Writing these bindings for any library becomes very easy once you understand that your are just defining interfaces and abstract classes - no implementation. You just have to make F# compiler happy and be sure that the underlying JavaScript calls will be available.

Now that's everything there is to it. Just to complete this post I wanted to added two more subjects that I have run into, so continue the read if you wish:

- Issues with custom bindings
- Emitting custom JavaScript

### Custom bindings possible issues - objects as functions
In JavaScript objects are functions and thus can be invoked. Not in F# though and so it can be quite complicated for anyone writing the bindings for existing JavaScript library. For example, I wanted to use D3 Time formatting capabilities to shape the **Axis** ticks into something readable. In JavaScript you would do something like this (very straightforward):

```javascript
chart.xAxis.tickFormat(d3.time.format("%x"));
```

NVD3 has **tickFormat** method available on all Axis, which in JavaScript takes a function transforming anything into string representation of what is shown on the axis. The signature in F# would be something like: **tickFormat(Object -> string)**. Fable comes with D3 bindings, that make the D3 Time Format available on global time module (D3.Time.Globals.format()), so one would expect something like this to work:

```ocaml
chart.xAxis.tickFormat(D3.Time.Globals.format("%x"))
```

That's not really how things works since what you get is a **D3.Time.Format** object not compatible with the **Object->string** function signature. In JavaScript objects are functions, so sometimes (and it is the case of **d3.time.format("%")** you get an object that can be invoked directly as function. D3 uses this pattern a lot. Scales for instance as well are objects an functions in the same time as well.

In F#, one just can't invoke objects as if they would be functions, so it has to be modelled differently. The **D3.Time.Globals.fomrat("%x")** returns **D3.Time.Format** object which has **Invoke** method that takes DateTime. So you might try something like this:

```ocaml
chart.xAxis.tickFormat(fun x -> D3.Time.Globals.format("%x").Invoke(x))
```

Note that this still won't work since **Invoke** method expects DateTime and you are passing in an Object, because the signature of tickFormat is Object -> string. So you will have to cast to DateTime just to make the F# compiler happy, even though you know that the underlying JavaScript would work.

### Emitting custom JavaScript
Fable allows you to interact with JavaScript with few different ways. One way is to define function calls that will emit specific JavaScript code. Here is the official example:

```ocaml
open Fable.Core

[<Emit("$0 + $1")>]
let add (x: int) (y: string): float = jsNative

let result = add 1 "2"
```

This can be very useful sometimes. I have ended up in a situation when I needed create Date objects from the number of ticks. This is quite standard and both, the JavaScript **Date** object as well as .NET **DateTime** have a constructor which takes number of ticks as integer parameter.

So you might want to try the DateTime(int numberOfTicks) constructor which should create a DateTime instance from number of ticks - but this one is not supported by Fable. In fact Fable will try to invoke a different constructor DateTime(int years, int months, int days) and won't the last two parameters.

Here there is no way around than use custom function that will emit the correct JavaScript to construct a new Date which one can than pass safely to the D3 Time Format.

```ocaml
[<Emit("new Date($0)")>]
let fromTicks (ticks: int): DateTime = jsNative

let myDate = DateUtils.fromTicks(1238985)
```

The resulting JavaScript will just look like this:

```javascript
var myDate = new Date(1238985);
```

Now of course the other way around is to do a PR on Fable and fix this issue which after all should not be that hard and I guess that is what I should do instead of continuing this already quite long post.