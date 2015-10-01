---
layout: post
title: Reflection & Order of discovered properties
date: '2012-09-02T12:24:00.002-07:00'
author: Jan Fajfr
tags:
- ".NET"
- C#
modified_time: '2014-06-26T14:20:55.050-07:00'
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-5448790318473932249
blogger_orig_url: http://hoonzis.blogspot.com/2012/09/reflection-order-of-discovered.html
---
In .NET Reflection provides several methods to obtain information about any type from the type system. One of these methods is **GetProperties** method which retrieves a list of all the properties of a given type. This method returns an array of **PropertyInfo** objects, but the order of these properties is not guarantied to be the same.

``` 
PropertyInfo[] propListInfo = type.GetProperties();
```

In most cases you don't care, but the order of the properties does not have to be the same if you run this method several times. This is well described in [the documentation of this
method.](http://msdn.microsoft.com/en-us/library/kyaxdd3x.aspx) Microsoft also states, that your code should not be depending on the order of the properties obtained.

I had a very nice example of a bug resulting from the misuse of this
method. A ObjectComparer class, which is dedicated to comparison of two
objects of same type by recursively comparing it's properties, which I
have inherited as legacy code on my current Silverlight project.

I have noticed, that the results of the comparison are not the same
every time I run the comparison. Concretely the first time the
comparison was run on two same objects it always told me, that the
objects are not equal. Take a look at the problematic code, which I have
simplified a bit for this post:

``` 
private static bool CompareObjects(object initialObj, object currentObj, IList<String> filter)
{
 string returnMessage = string.Empty;

 Type type = initialObj.GetType();
 Type type2 = currentObj.GetType();

 PropertyInfo[] propListInfo = type.GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance).Where(x => !filter.Contains(x.Name)).ToArray();
 PropertyInfo[] propListInfo1 = type2.GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance).Where(x => !filter.Contains(x.Name)).ToArray();

 //if class type is native i.e. string, int, boolean, etc.
 if (type.IsSealed == true && type.IsGenericType == false)
 {
  if (!initialObj.Equals(currentObj))
  {
   return false;
  }
 }
 else //class type is object
 {
  //loop through each property of object
  for (int count = 0; count < propListInfo.Length; count++){
   var result = CompareValues(propListInfo[count].GetValue(initialObj),propListInfo2[count].GetValue(currentObj));
   if(result == false) {
    return result;
   }
  }
 }
 return true;
}
```

So in order to correct this code, you will have to **order both arrays
by MetadataToken**, which is a unique identifier of each property.

``` 
propListInfo = propListInfo.OrderBy(x=>x.MetadataToken).ToArray();
propListInfo1 = propListInfo1.OrderBy(x=>x.MetadataToken).ToArray();
```

There is some more information about how reflection works [in this blog
post.](http://blogs.msdn.com/b/haibo_luo/archive/2006/07/09/661091.aspx)The
issue is that the Reflection engine holds a "cache" for each type, in
which it stocks the already "discovered" properties. The problem is
that, this cache is cleared during garbage collection. When we ask for
the properties, than they are served from the cache in the order in
which they have been discovered.

However in my case, this information does not help. The issue occures
only the first time that I ask the ObjectComparator to compare the
objects and there is no reason that there should be any garbage
collection between the first and second run...well no idea here. Sorting
by MetadataToken has fixed the issue for me.
