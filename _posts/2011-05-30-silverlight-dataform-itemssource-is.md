---
layout: post
title: Silverlight DataForm ItemsSource is null when DataForm is not visible
date: '2011-05-30T06:34:00.001-07:00'
author: Jan Fajfr
tags:
- Silverlight
modified_time: '2014-06-26T15:03:45.722-07:00'
---
This is just a quick post about Silverlight's DataForm. When DataForm is not visible, then its ItemsSource collection is null.
This can make sense, but it may also cause problems in certain cases when one would expect the collection to be filled.

Imagine the following: The DataForm is in Expander. On a click of a button you want to expand the Expander and add new item to the DataForm:

```csharp
MyExpander.IsExpanded = true;
//this call passes
MyDataForm.AddNewItem();

//but here CurrentItem will be null
var vm = MyDataForm.CurrentItem as MyViewModel;

//and here you will get NullPointerException
vm.MyProperty = "value";
```

The problem is that when you set **IsExpanded** to true, the UI does not get updated right by the way, so you will have to force it by calling **this.UpdateLayout()**. After this the **ItemsSource** collection will be binded to whatever enumeration you have set in the XAML and the next call of AddNewItem() will set the **CurrentItem** property of DataForm to a non-null value.
