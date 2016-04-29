---
layout: post
title: Silverlight Event Calendar
date: '2011-06-26T12:45:00.001-07:00'
author: Jan Fajfr
tags:
- Silverlight
modified_time: '2014-06-26T15:01:20.744-07:00'
thumbnail: http://2.bp.blogspot.com/-7bC4PEIU4bg/TgBYmhQJ5CI/AAAAAAAAAL0/fLewf14ICZQ/s72-c/EventCalendar.PNG
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-1362817029301735257
blogger_orig_url: http://hoonzis.blogspot.com/2011/06/silverlight-event-calendar.html
---
For one of my latest project I needed a quite simple Event Calendar component for Silverlight. I did not want to use any third party libraries and I wanted this component to stay simple. The code is available on [GitHub](https://github.com/hoonzis/SilverlightEventCalendar)

I had the following constraints and requirements on the component:

- It has to be bind-able
- It should accept any IEnumerable collection
- I should be able just specify which property of objects in the collection holds the DateTime value, which will be used to place the objects in the calendar
- It should expose a template to be able to change the view of the event
- It should expose events such as "Calendar Event Clicked"
- It should expose a SelectedItem property

Here is the resulting component - it does not look great, but you can easily style it as you want.

![eventcalendar](https://raw.githubusercontent.com/hoonzis/hoonzis.github.io/master/images/EventCalendar/EventCalendar.PNG)

The component is based on Calendar component. Calendar is not really flexible component but there are some workarounds to make it work the way, that you like. First the calendar is placed inside a UserControl.

```xml
<usercontrol x:class="EventCalendarLibrary.EventCalendar">
    <grid background="White" x:name="LayoutRoot">
        <controls:calendar x:name="InnerCalendar">
    </controls:calendar></grid>
</usercontrol>
```


**Calendar** component is composed of **CalendarDayButtons**. **CalendarDayButton** resides in the **System.Windows.Controls.Primitives** namespace. The problem is that the Calendar does not hold a collection of these buttons so we are not able to dynamically add components to these buttons. However the style of the each button in the calendar can be set by setting the **CalendarDayButtonStyle** property. We can use this style to override the control template and this way set our proper handler for Loaded and Click events. The handler for Loaded event will simply allow us to add the loaded Button to a collection which we will maintain inside our components and which later allows us to add the "events" to the calendar.


```xml
<grid.resources>
<style targettype="controlsPrimitives:CalendarDayButton" x:key="CalendarDayButtonStyle">
            <setter Property="Template">
                <Setter.Value>
                    <controltemplate TargetType="controlsPrimitives:CalendarDayButton">
                        <border BorderBrush="#FF598788" BorderThickness="1,1,1,1" CornerRadius="2,2,2,2">
                            <stackpanel HorizontalAlignment="Stretch" VerticalAlignment="Stretch" MinHeight="30" MinWidth="10">
                                <controlsPrimitives:CalendarDayButton
                                    Loaded="CalendarDayButton_Loaded"
                                    Background="{TemplateBinding Background}"
                                    BorderBrush="{TemplateBinding BorderBrush}"

                                    Content="{TemplateBinding Content}"
                                    BorderThickness="{TemplateBinding BorderThickness}"

                                    x:Name="CalendarDayButton" Click="CalendarDayButton_Click"/>
                            </StackPanel>        
                        </Border>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>

</style>
</grid.resources>
<controls:calendar background="White" calendardaybuttonstyle="{StaticResource CalendarDayButtonStyle}" x:name="InnerCalendar">
</controls:calendar>
```

We are changing the ControlTemplate of CalendayDayButton for a new one which consists of a Border and a StackPanel containing a new CalendarDayButton. This is important, because now we now that each "day" in the Calendar will be represented by this StackPanel to which we can
add additional components. As promised we override the Loaded event. Let's see the code-behind:


```csharp
private void CalendarDayButton_Loaded(object sender, RoutedEventArgs e)
{
 var button = sender as CalendarDayButton;
 calendarButtons.Add(button);

 //Resizing the buttons is the only way to change the dimensions of the calendar
 button.Width = this.ActualWidth / 9;
 button.Height = this.ActualHeight / 8;

 if (calendarButtons.Count == 42)
 {
  FillCalendar();
 }
}
```

We are simple take the button, store it in our inner collection (called calendarButtons) for further manipulations and then we perform some resizing. The only way to force Calendar to Resize itself to the values which you specify in "Width" and "Height" properties is actually to change the dimensions of the inner buttons.

And last we check if all button had been loaded and if yes then we call "FillCallendar" method - yes this will be the method which will fill in
the events to the calendar.

Before we go there we need to define the Dependency Properties which will allow us to bind the desired values (collection of items, DateTime property name event style and SelectedEvent property).

```csharp
public static readonly DependencyProperty SelectedEventProperty = DependencyProperty.Register("SelectedEvent", typeof(Object), typeof(EventCalendar), null);
public Object SelectedEvent
{
 get { return (Object)GetValue(SelectedEventProperty); }
 set { SetValue(SelectedEventProperty, value); }
}

public static readonly DependencyProperty CalendarEventButtonStyleProperty = DependencyProperty.Register("CalendarEventButtonStyle", typeof(Style), typeof(EventCalendar), null);
public Style CalendarEventButtonStyle
{
 get { return (Style)GetValue(CalendarEventButtonStyleProperty); }
 set { SetValue(CalendarEventButtonStyleProperty, value); }
}

public static readonly DependencyProperty DatePropertyNameProperty = DependencyProperty.Register("DatePropertyName", typeof(String), typeof(EventCalendar), null);
public String DatePropertyName
{
 get { return (String)GetValue(DatePropertyNameProperty); }
 set { SetValue(DatePropertyNameProperty, value); }
}

public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(EventCalendar),
 new PropertyMetadata(ItemsSourcePropertyChanged));

public IEnumerable ItemsSource
{
 get { return (IEnumerable)GetValue(ItemsSourceProperty); }
 set { SetValue(ItemsSourceProperty, value); }
}
```


You can see that there is a handler attached to the change of *ItemsSourceProperty*. This handler is called whenever this property
changes. This is a important part, we take the Items, determine which property contains the DateTime value and we will group these Items by this property and store it in internal dictionary of type Dictionary<DateTime,<List<Object>>.

```csharp
public static void ItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
{
    var owner = d as EventCalendar;
    //if the property was set to null we have to clear all the events from calendar
    if (e.NewValue == null)
    {
        owner.ClearCallendar();
        return;
    }

    IEnumerable rawItems = (IEnumerable)e.NewValue;
    PropertyInfo property = null;

    //to determine the if the Count>0
    var enumerator = rawItems.GetEnumerator();
    if(!enumerator.MoveNext()){
        owner.ClearCallendar();
        return;
    }

    Object o = enumerator.Current;
    Type type = o.GetType();

    //get the type of the properties inside of the IEnumerable
    property = type.GetProperty(owner.DatePropertyName);

    if (property != null)
    {
        IEnumerable<Object> items = Enumerable.Cast<Object>((IEnumerable)e.NewValue);
        //group the items and store in a dictionary
        if (items != null)
        {
            var parDate = items
                        .GroupBy(x => GetDateValue(x, property))
                        .ToDictionary(x => x.Key, x => x.ToList());
            owner.ItemsSourceDictionary = parDate;
            owner.FillCalendar();
        }
    }
}

//Returns the DateTime value of a property specified by its information
public static DateTime GetDateValue (Object x, PropertyInfo property)
{
    return ((DateTime)property.GetValue(x,null)).Date;
}
```

It is a bit complicated - and that comes probably from my poor knowledge and experience of working with raw IEnumerable. Basically I need to get the type of the items inside of the IEnumerable and then using this Type I can obtain the value of the DateTime property and group the values and
store in an inner dictionary.


You can see that there is a simple helper functions which just takes PropertyInfo and Object and returns the Date value of that property. I
prefer to get when using "Data" property I am sure that I will have exact "day" without hours and minutes and than I can group this data by
this "day".

Now that we have the grouped events, we have to place them in the
calendar. To create this function I have used to example shown on [this
blog](http://blogs.msdn.com/b/aurelien/archive/2008/11/03/comment-ajouter-du-contenu-un-calendar-silverlight.aspx).

```csharp
private void FillCalendar(DateTime firstDate)
{
    if (ItemsSourceDictionary!=null && ItemsSourceDictionary.Count >0)
    {                
        DateTime currentDay;

        int weekDay = (int)firstDate.DayOfWeek;
        if (weekDay == 0) weekDay = 7;
        if (weekDay == 1) weekDay = 8;

        for (int counter = 0; counter < calendarButtons.Count;counter++)
        {
            var button = calendarButtons[counter];
            var panel = button.Parent as StackPanel;


            int nbControls = panel.Children.Count;
            for (int i = nbControls - 1; i > 0; i--)
            {
                panel.Children.RemoveAt(i);
            }

            currentDay = firstDate.AddDays(counter).AddDays(-weekDay);

            if (ItemsSourceDictionary.ContainsKey(currentDay))
            {
                var events = ItemsSourceDictionary[currentDay];
                foreach (Object calendarEvent in events)
                {
                    Button btn = new Button();
                    btn.DataContext = calendarEvent;
                    btn.Style = CalendarEventButtonStyle;
                    panel.Children.Add(btn);
                    btn.Click += new RoutedEventHandler(EventButton_Click);
                }
            }
        }
    }
}
```

This function accepts a DateTime parameter which is the first date of the month which is being shown in the Calendar. When the first of the month is Monday, than it will be shown as a first in the second row. When it is Tuesday, it will be shown as the second in the second row. For other cases, it will be shown in the first row. Thus we can easily subtract the integer values specifying which the day in the week (eg. 3 for Thursday) and we will obtain the date which is shown in the first cell.

The day which is being shown in the calendar is exposed by **Calendar.DisplayDate** Property and we can easily access that to obtain the month which is being shown (and thus the first day of the month).

So we just iterate over all the buttons, determine the date for each and knowing that the buttons are wrapped by a StackPanel we can add to this panel the events. Each event is represented by a Button and the style which is exposed as DependencyProperty is applied.

#### Exposed events
This component exposes two events, one for the moment when the user
clicks on existing "Event" in the calendar and second one for the click
on the button of the day.

```csharp
public event EventHandler<calendareventargs> EventClick;
public event EventHandler DayClick;
```

When the user clicks on existing event in the calendar, we pass the
clicked "Event" wrapped by "CalendarEventArgs" class.

```csharp
void EventButton_Click(object sender, RoutedEventArgs e)
{
    object eventClicked = (sender as Button).DataContext as object;

    //set the selected event
    SelectedEvent = eventClicked;

    //just pass the click event to the hosting envirenment of the component
    if (EventClick != null)
    {
        EventClick(sender, new CalendarEventArgs(eventClicked));
    }
}
```

When the user clicks on the button of the day, we passed the Date of
this day wrapped up by CalendarEventArgs.

```csharp
private void CalendarDayButton_Click(object sender, RoutedEventArgs e)
{
    CalendarDayButton button = sender as CalendarDayButton;
    DateTime date = GetDate(GetFirstCalendarDate(),button);

    if(date!=DateTime.MinValue && DayClick!=null)
    {
        DayClick(sender,new CalendarEventArgs(date));
    }
}
```

We can obtain the Date for the button by method similar to the one
described above.

#### Summary
There is no more to that, as I said the component stays super simple, just one class, you can style the Events which are placed to the Calendar and you have to handle the other actions (like eg. adding an "Event" when clicking on the "day" button) by yourself. [Download the source from GitHub](https://github.com/hoonzis/SilverlightEventCalendar)
