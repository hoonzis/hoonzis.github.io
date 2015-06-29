---
layout: post
title: Mock Java Calendar - JMockit vs Mockito
date: '2012-04-03T05:12:00.000-07:00'
author: Jan Fajfr
tags:
- Java
- Testing
modified_time: '2014-06-26T14:25:09.134-07:00'
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-5873479968341259306
blogger_orig_url: http://hoonzis.blogspot.com/2012/04/mock-java-calendar-jmockit-vs-mockito.html
---
To get the current time or day in Java, one should be using the Calendar
class in the following way:


``` 
Calendar c = Calendar.getInstance();
int day = c.get(Calendar.DAY_OF_WEEK);
```

Now I imagine this code can be hidden somewhere inside a business method
and the behaviour of that method would be dependent on the current day.
Typical example can be the method which returns the schedule of the
cinema on the current day.

``` 
public class ScheduleService{
  public Schedule getTodaySchedule(){
    Calendar c = Calendar.getInstance();
    int day = c.get(Calendar.DAY_OF_WEEK);

    //get it from DB or wherever you want
    Schedule s = lookupAccordingToDay(day);
  }
}
```

In order to test this method you have to mock the Calendar. You will
have to verify, that for monday the service will return the schedule for
monday. However since the test will be automatically called every day,
not only monday, you will obtain whole different schedules and the
assert will fail. There are several solutions to this. I have 3 in my
mind.

Solution 1: create a separate service
-------------------------------------

First solution has nothing to do with mocking. The way to go here is to
isolate the Calendar into a separate service (let's call it
CurrentDayService). Than you can manually create a mock for this
service. You will also have to change the body of your ScheduleService
to use this CurrentDayService.

``` 
public interface ICurrentDayService {
   int getCurrentDay();
}

public class CurrentDayService {
   public int getCurrentDay(){
      Calendar c = Calendar.getInstance();
      return c.get(Calendar.DAY_OF_WEEK);
   }
}

public class CurrentDayServiceMock {
   private int dayToReturn;
   public CurrentDayServiceMock(int dayToReturn){
     this.dayToReturn = dayToReturn;
   }
   public int getCurrentDay(){
      return dayToReturn;
   }
}

public class ScheduleService {
  //@Autowire or inject this service
  private CurrentDayService dayService;
  
  public Schedule getTodaySchedule(){
    int day = dayService.getCurrentDay();
    //get it from DB or wherever you want
    Schedule s = lookupAccordingToDay(day);
  }
}
```

Now in the unit test your schedule service, can use the mock instead of
the real implementation. If you are using Dependency Injection than you
can define a different context for unit tests. If not, you will have to
do it manually.

Solution 2: use Mockito
-----------------------

Mockito allows you to mock the real **Calendar** class. That means that
you no longer need to wrap the Calendar by some **CurrentDayService**
class just to be able to mock the behavior. However you will still have
to add a mechanism to pass the mocked Calendar to your service. That is
not that complicated. Have a look at the following definition o the
ScheduleService and the unit test which comes with it.

``` 
public class ScheduleService{
  private Calendar calendar;
  public ScheduleService(){
    calendar = Calendar.getInstance();
  }
  public Schedule getTodaySchedule(){
    int day = calendar.get(Calendar.DAY_OF_WEEK);
    Schedule s = lookupAccordingToDay(day);
  }

  public setCalendar(Calendar c){
    calendar = c;
  }
}

@Test
public void testGetTodaySchedule() {
 Calendar c = Mockito.mock(Calendar.class);
 Mockito.when(c.get(Calendar.DAY_OF_WEEK)).thenReturn(2);

 ScheduleService sService = new SomeStrangeService();
 //there has to be a way to set the current calendar
 sService.setCalendar(c);
 Schedule schedule = sService.getTodaySchedule();
 //Assert your schedule values
}
```

To sum it up: if the **setCalendar** method is not called, than the
Calendar is instantiated in the constructor. So in production, it will
return the current day. In your unit test, you can easily mock it, to
specify different behavior. Tha drawback: if someone accidentaly calls
the **setCalendar** method in the production, you will get into truble.

Solution 3: use JMockit, mock all the calendars in you JVM
----------------------------------------------------------

JMockit is strong framework which as some other mocking frameworks is
using the **Java Instrumentation API**. The code that you want to
execute in your mocks is injected as bytecode at runtime. This enables
JMockit to, for instance mock all the instances of Calendar class in
your JVM. Here is how you can achieve this:

``` 
@MockClass(realClass = Calendar.class)
public static class CallendarMock {

 private int hour;
 private int day;
 private int minute;

 public CallendarMock(int day, int hour, int minute) {
  this.hour = hour;
  this.day = day;
  this.minute = minute;
 }

 @Mock
 public int get(int id) {
  if (id == Calendar.HOUR_OF_DAY) {
   return hour;
  }
  if (id == Calendar.DAY_OF_WEEK) {
   return day;
  }

  if (id == Calendar.MINUTE) {
   return minute;
  }
  return -1;
 }
}
```


The previous code snippet is the infrastructure which I can use to mock
the Calendar's **get** method. A utility class CalendarMock has to be
created, which specifies the methods which are mocked. The **realClass**
attribute in the MockClass annotation specifies which class is mocked by
the defined class. So now the unit test is simplified. There is not need
to specify the Calendar which should be used by the ScheduleService.

``` 
@Test
public void testGetTodaySchedule() {
 Mockit.setUpMocks(new CallendarMock(Calendar.MONDAY, 12, 20));
 ScheduleService sService = new SomeStrangeService();
 Schedule schedule = sService.getTodaySchedule();
 //Assert your schedule values
}
@After
public void destroyMock() {
    Mockit.tearDownMocks();
}
```

At the end, you have to remember to switch-off the mocking of the
Calendar. If not the Calendar will be mocked in all the tests executed
after this one. Hence the call to the **tearDownMocks()** method.

Summary
-------

With Mockito you can mock the real Calendar. However you have to pass
the instance of the mocked callendar to the class, which actually uses
it. With JMockit you are able to tell to the JVM: "from now all my mocks
behave like this...". For me this simplifies the situation, while I am
not forced to create a setter for a Calendar to be passed to my service
class. But it would take much more time and effort to compare the two
frameworks. It might be that Mockito handles some situations better than
JMockit.
[CodeProject](http://www.codeproject.com/script/Articles/BlogFeedList.aspx?amid=honga)
