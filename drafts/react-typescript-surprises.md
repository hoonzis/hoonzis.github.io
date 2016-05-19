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
The simpliest wyat t f

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
