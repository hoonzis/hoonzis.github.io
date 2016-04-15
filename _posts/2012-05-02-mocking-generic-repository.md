---
layout: post
title: Mocking the generic repository
date: '2012-05-02T13:40:00.001-07:00'
author: Jan Fajfr
tags:
- ".NET"
- Testing
- NHibernate
- C#
modified_time: '2014-06-26T14:20:01.298-07:00'
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-6294818593906360547
blogger_orig_url: http://hoonzis.blogspot.com/2012/05/mocking-generic-repository.html
---
This post describe one way to mock the generic repository when you can't use any mocking framework. It assumes that you are familiar with the **Service &lt;-&gt; Repository &lt;-&gt;
Database** architecture. Another prerequisite is the knowledge of the [repository pattern]() and
[it's generic variant](http://www.tugberkugurlu.com/archive/generic-repository-pattern-entity-framework-asp-net-mvc-and-unit-testing-triangle).

In the majority of my projects I am using the following generic repository class.

```cs
public interface IRepository
{
 T Load<T>(object id);
 T Get<T>(object id);
 IEnumerable<T> Find<T>(Expression<Func<T, bool>> matchingCriteria);
 IEnumerable<T> GetAll<T>();
 void Save<T>(T obj);
 void Update<T>(T obj);
 void Delete<T>(T obj);
 void Flush();
 int CountAll<T>();
 void Evict<T>(T obj);
 void Refresh<T>(T obj);
 void Clear();
 void SaveOrUpdate<T>(T obj);
}
```

Based on this technique, some people decide to implement concrete classes of this interface (**CarRepository : IRepository**), whereas others decide to keep using the generic implementation. That depends on the ORM that you are using. [With
EF](http://elegantcode.com/2009/12/15/entity-framework-ef4-generic-repository-and-unit-of-work-prototype/)
and
[NHibernate](http://stackoverflow.com/questions/2587965/using-a-generic-repository-pattern-with-fluent-nhibernate)
you can easily implement the generic variant of the repository (check the links).

I am also using the generic variant (mostly with NHibernate). Now the
question is: **How to mock this generic repository**? It can be a bit
tricky to mock. When you have one class for each repository which works
for one concrete type you can mock the repository quite easily. For
example StudentRepository which handles entities of type Student might
be backed up a list of students.

While when working with generic repository, it might be a bit harder.
Here is how I have solved the problem:

```cs
public class MockedRepository :IRepository
{
 public MockedRepository()
 {
  cities = DeserializeList<City>("CityDto");
  stations = DeserializeList<Station>("StationDto");
  tips = DeserializeList<InformationTip>("InformationTipDto");
  countries = DeserializeList<Country>("CountryDto");

  dataDictionary = new Dictionary<Type, object>();
  dataDictionary.Add(typeof(City), cities);
  dataDictionary.Add(typeof(Station), stations);
  dataDictionary.Add(typeof(InformationTip), tips);
  dataDictionary.Add(typeof(Country), countries);
  }   

 public T Get<T>(object id)
 {
  Type type = typeof(T);
  var data = dataDictionary[type];
  IEnumerable<T> list = (IEnumerable<T>)data;
  var idProperty = type.GetProperty("Id");
  return list.FirstOrDefault(x=>(int)idProperty.GetValue(x,null) == (int)id);
 }

 public IEnumerable<T> Find<T>(Expression<Func<T, bool>> matchingCriteria)
 {
  Type type = typeof(T);
  var data = dataDictionary[type];
  IEnumerable<T> list = (IEnumerable<T>)data;
  var matchFunction = matchingCriteria.Compile();
  return list.Where(matchFunction);
 }

 public IEnumerable<T> GetAll<T>()
 {
  Type type = typeof(T);
  return (IEnumerable<T>)dataDictionary[type];
 }

 public void Save<T>(T obj)
 {
  Type type = typeof(T);
  List<T> data = (List<T>)dataDictionary[type];
  data.Add(obj);
 }
}
```

The main building block of this mocked repository is the dictionary
which contains for each type in the repository the enumerable collection
of objects. Each method in the mocked repository can use this dictionary
to determine which is the collection addressed by the call (by using the
generic type T.).

```cs
Type type = typeof(T);
var data = dataDictionary[type];
IEnumerable<T> list = (IEnumerable<T>)data;
```

Now what to do next, depends on each method. I have shown here only the
methods which I needed to mock, but the other ones should not be harded
to mock. The most interesting is the **Find** method, which takes as the
parameter the matching criteria. In order to pass this criteria to the
**Where** method on the collection, this criteria (represented by an
Expression) has to be compiled into a predicate Func (in other words
function which takes an object of type T and returns boolean value.

The **Get** also has some hidden complexity. In this implementation I
assume, that there is a **Id** property defined on the object of type T.
I am using reflection to obtain the value of that property and the whole
thing happens inside the a LINQ statement.

This repository might be useful, but it is definitely not the only way
to isolate your database. So the question is - **Should this be the
method to isolate my Unit or Integration tests?** Let's take a look at
other possible options:


-   **Use mocking framework ([there is quite a choice
    here](http://stackoverflow.com/questions/37359/what-c-sharp-mocking-framework-to-use))*
     This essentially means that in each of your tests you define the
    behavior of the repository class. This requires you to write a mock
    for each repository method that is called inside the service method.
    So it means more code to write. On the other hand you control the
    behavior needed for the particular tested method. While using
    mocking framework you have also the option to verify that methods
    have been called.
-   **Use the repository implementation and point it to in-memory
    database** (SQL Lite). That is a good option in the case when:
    -   You are able to populate the database with the data.
    -   You are sure of your repository implementation
-   **Use the generic repository mock presented here.** That is not a
    bad option if you have some way to populate the collections which
    serve as in-memory database. I have used de-serialization from JSON.
    Another option could be to use a framework such as
    [AutoPoco](http://autopoco.codeplex.com/)to generate the data. You
    can also create one repository which can be used for the whole test
    suite (or application presentation).

### Summary

As said before this might be a variant to consider. I am using it for
Proof of Concepts and portable versions of database based applications.
On the other hand for unit test you might consider either mocking
framework or in-memory database. There is no clear winner in this
comparison.
