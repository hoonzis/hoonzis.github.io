
1) Can't refernece JS files in TypeScript files (TS lint or someone else refusing)
What I want  - reference JS file directly, so that it will be bunded the bundle.js. Adding custom typings for that file to custom typings location.

This won't work:
/// <reference path="../../../my_fucking_js.js" />

Also tried to place the typings next to the JS file and reference the typings
/// <reference path="../../../my_fucking_js.d.ts" />


2) Mutliple libraries won't work if referenced as module (d3, nvd3, underscore)

import * as d3 from 'd3'

- When you do import in this form, it will fail. because d3 expects "this" to be "window". But on the import stage this is null.
- "this" is null because we are in the strict mode.
- TS compiler is in strict mode by default. But even if I change TS compiler options ("noImplicitUseStrict":true) it won't work.
- This would also force me to place "use strict" everywhere besides all the compoments that lead to my component which is using d3.

3) "this" is null when handling events.
The simpliest way t f

	return (
			<form className="form-inline" onSubmit={this.handleSubmit}>
				<button type="submit" className="btn btn-success pull-right">Update</button>
			</form>
        );
    }

    private handleSubmit = (e: Event) => {
        e.preventDefault();
        // THIS is null here
        this.props.search(query);
    }

The solution to this problem is to either manually bind this or use a lambda which will do it for you

<form className="form-inline" onSubmit={(e) => this.handleSubmit(e) }>

If you go for this, you will get a compile error, because suddenly the parameter passed is not "Event" but "React.FormEvent".

private handleSubmit = (e: Event) => {
	e.preventDefault();
	this.props.search(query);
}


### Issues while searching for ideal react bootstrap components

#### React Date Picker and typings
I think this is a small issue in Typescript type inference. I am using the following [react date-picker](https://www.npmjs.com/package/react-datepicker) commponent. Ambient typings are available for this one.
The type of **onChange** method is simply **any**. However the following won't work:

	<DatePicker selected={this.state.minDate} onChange={this.handleChange} />;

	handleChange = function(e: React.FormEvent) {
        // TODO: do something
    };

It should, handle change is a function with proper signature and it can definitely be cast to any. To fix this you have to force the type of **this.handleChange**, and ofcourse that is said, because you are loosing the type safety on this function.

private handleSinceChanged = (date: any) => {       
	// TODO: fire the change
};

#### Looking for a correct combobox
[react-select](https://github.com/JedWatson/react-select) - very hard to style within a bootstrap form

[react-boostrap](http://react-bootstrap.github.io/components.html) - no real bindable combobox, only dropdown

[react-bootstrap-select](https://github.com/tjwebb/react-bootstrap-select) - no typings

#### Implicit dependecies to node
Authors of some typings assume that you have already installed the typings for basic node framework. For instance **react-bootstrap-table** references the **events** modules internaly.

	import { EventEmitter } from 'events';

And you will need node typings intsalled to make this compile.

λ typings install dt~node --global --save


#### Mutliple typings for same NPM package

{
	"ambientDependencies:" {
		"d3": "registry:dt/d3#0.0.0+20160514171929",
    "node": "registry:dt/node#6.0.0+20160602155235",
    "nvd3": "registry:dt/nvd3#1.8.1+20160317120654",
	},
  "dependencies": {
		"react": "registry:dt/react#0.14.0+20160602151522",
		"react-redux": "registry:dt/react-redux#4.4.0+20160501125835",
    "immutable": "registry:npm/immutable#3.7.6+20160411060006"
  },
  "globalDependencies": {
    "d3": "registry:dt/d3#0.0.0+20160514171929",
    "node": "registry:dt/node#6.0.0+20160602155235",
    "nvd3": "registry:dt/nvd3#1.8.1+20160317120654",
    "react": "registry:dt/react#0.14.0+20160602151522",
    "react-bootstrap": "registry:dt/react-bootstrap#0.0.0+20160526141239",
    "react-bootstrap-table": "registry:dt/react-bootstrap-table#2.3.0+20160602145548",
    "react-datepicker": "registry:dt/react-datepicker#0.27.0+20160505171556",
    "react-dom": "registry:dt/react-dom#0.14.0+20160412154040",
    "react-redux": "registry:dt/react-redux#4.4.0+20160501125835",
    "react-router": "registry:dt/react-router#2.0.0+20160501155536",
    "react-router/history": "registry:dt/react-router/history#2.0.0+20160602154905",
    "redux": "registry:dt/redux#3.3.1+20160326112656",
    "underscore": "registry:dt/underscore#1.7.0+20160505190121"
  }
}


### Web is moving fast
This is just a short remark, but frameworks and tools move quite fast, a this will in-evitably break your project. For instance while I was writing this post, latest update of **typings** tool, changed the syntax for ambient typings.

They are now stored as global typings and the repository **dt** has to be specified explicitily just, like in the command line example above.

#### Build server configuration

[13:17:48][StatsJs] C:\Program Files\nodejs\node.EXE node_modules/gulp/bin/gulp.js
[13:17:50][StatsJs] [13:17:50] Using gulpfile G:\work\62d902966b271a9f\src\RfqHub.Stats\RfqHub.Stats.Site\gulpfile.js
[13:17:50][StatsJs] [13:17:50] Starting 'tslint'...
[13:17:50][StatsJs] [13:17:50] Finished 'tslint' after 5.55 ms
[13:17:50][StatsJs] [13:17:50] Starting 'client-javascript'...
[13:17:50][StatsJs] [13:17:50] Starting 'css'...
[13:17:50][StatsJs] [13:17:50] Starting 'lib'...
[13:17:50][StatsJs] [13:17:50] Starting 'polyfills'...
[13:17:50][StatsJs] [13:17:50] Starting 'copy-js-files'...
[13:17:52][StatsJs] typings/globals/react/index.d.ts(2339,5): Error TS2300: Duplicate identifier 'export='.
[13:17:52][StatsJs] typings/modules/react-redux/index.d.ts(112,1): Error TS2309: An export assignment cannot be used in a module with other exported elements.
[13:17:52][StatsJs] typings/modules/react-router/index.d.ts(179,3): Error TS7010: 'stringifyQuery', which lacks return-type annotation, implicitly has an 'any' return type.
[13:17:52][StatsJs] typings/modules/react-router/index.d.ts(194,3): Error TS7010: 'onError', which lacks return-type annotation, implicitly has an 'any' return type.
[13:17:52][StatsJs] typings/modules/react-router/index.d.ts(199,3): Error TS7010: 'onUpdate', which lacks return-type annotation, implicitly has an 'any' return type.
[13:17:52][StatsJs] typings/modules/react-router/index.d.ts(277,3): Error TS7010: 'onClick', which lacks return-type annotation, implicitly has an 'any' return type.
[13:17:52][StatsJs] typings/modules/react-router/index.d.ts(340,3): Error TS7010: 'push', which lacks return-type annotation, implicitly has an 'any' return type.
[13:17:52][StatsJs] typings/modules/react-router/index.d.ts(347,3): Error TS7010: 'push', which lacks return-type annotation, implicitly has an 'any' return type.
[13:17:52][StatsJs] typings/modules/react-router/index.d.ts(354,3): Error TS7010: 'go', which lacks return-type annotation, implicitly has an 'any' return type.
[13:17:52][StatsJs] typings/modules/react-router/index.d.ts(359,3): Error TS7010: 'goBack', which lacks return-type annotation, implicitly has an 'any' return type.
[13:17:52][StatsJs] typings/modules/react-router/index.d.ts(364,3): Error TS7010: 'goForward', which lacks return-type annotation, implicitly has an 'any' return type.
[13:17:52][StatsJs] typings/modules/react-router/index.d.ts(372,3): Error TS7010: 'createPath', which lacks return-type annotation, implicitly has an 'any' return type.
[13:17:52][StatsJs] typings/modules/react-router/index.d.ts(380,3): Error TS7010: 'createHref', which lacks return-type annotation, implicitly has an 'any' return type.
[13:17:52][StatsJs] typings/modules/react-router/index.d.ts(473,3): Error TS7010: 'getChildRoutes', which lacks return-type annotation, implicitly has an 'any' return type.
[13:17:52][StatsJs] typings/modules/react-router/index.d.ts(506,3): Error TS7010: 'getIndexRoute', which lacks return-type annotation, implicitly has an 'any' return type.
[13:17:52][StatsJs] typings/modules/react-router/index.d.ts(616,3): Error TS7010: 'getComponent', which lacks return-type annotation, implicitly has an 'any' return type.
[13:17:52][StatsJs] typings/modules/react-router/index.d.ts(629,3): Error TS7010: 'getComponent', which lacks return-type annotation, implicitly has an 'any' return type.
[13:17:52][StatsJs] typings/modules/react-router/index.d.ts(647,3): Error TS7010: 'onEnter', which lacks return-type annotation, implicitly has an 'any' return type.
[13:17:52][StatsJs] typings/modules/react-router/index.d.ts(859,18): Error TS7010: 'match', which lacks return-type annotation, implicitly has an 'any' return type.
[13:17:52][StatsJs] typings/modules/react-router/index.d.ts(878,1): Error TS2309: An export assignment cannot be used in a module with other exported elements.
[13:17:52][StatsJs] typings/modules/react/index.d.ts(2330,1): Error TS2300: Duplicate identifier 'export='.
[13:17:52][StatsJs] [13:17:52] Finished 'client-javascript' after 2.12 s
[13:17:52][StatsJs] [13:17:52] Finished 'lib' after 2.12 s
[13:17:52][StatsJs] [13:17:52] Finished 'polyfills' after 2.12 s
[13:17:52][StatsJs] [13:17:52] Finished 'copy-js-files' after 2.43 s
[13:17:52][StatsJs] [13:17:52] Finished 'css' after 2.64 s
[13:17:52][StatsJs] [13:17:52] Starting 'html'...
[13:17:52][StatsJs] [13:17:52] gulp-inject Nothing to inject into index.html.
[13:17:52][StatsJs] [13:17:52] gulp-inject 3 files into index.html.
[13:17:52][StatsJs] [13:17:52] Finished 'html' after 102 ms
[13:17:52][StatsJs] [13:17:52] Starting 'default'...
[13:17:52][StatsJs] [13:17:52] Finished 'default' after 34 Î¼s
