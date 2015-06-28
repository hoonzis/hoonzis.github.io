--- layout: post title: CORBA - Java Naming Service, manipulating
contexts date: '2010-11-05T06:06:00.000-07:00' author: Jan Fajfr tags: -
Java - Corba modified\_time: '2014-06-27T05:13:53.741-07:00' thumbnail:
http://3.bp.blogspot.com/\_fmvjrARTMYo/TNQAvdcv8fI/AAAAAAAAAIA/j51yXl\_dLBg/s72-c/nsadmin.JPG
blogger\_id:
tag:blogger.com,1999:blog-1710034134179566048.post-6874415143809519334
blogger\_orig\_url:
http://hoonzis.blogspot.com/2010/11/corba-java-naming-service-manipulating.html
---

This is a short post describing how to implement simple tool to manage
CORBA Naming Service.

### Corba

The aim of this paragraph is not to describe CORBA, but just to explain
little bit how it works, so that later I can explain what is and how
works Naming Service - which is part of the CORBA architecture.

So CORBA stands for Common Object Request Broker Architecture. It is a
standard which enables programmers to write distributed applications, we
can simplify it a lot and say it enables us to write apps working over
the network.

Corba is just implementation of standard Server - Client architecture.
In Corba's world we create objects on the Server side - and these
objects can be used by the clients, which are running on another
computers on the network.

The client has to know the structure of the object (the methods and the
attributes). The objects which are created on server side and used by
the clients are before described by the IDL - interface definition
language. The very big advantage of Corba is that if we describe the
object by IDL we can later generate Java code as well as C++ code.\
\
When the server creates the object it will put its reference to ORB -
Object Request Broker. This is the container that takes care of
serialization of object over the network. This is the abstraction layer
which permits us to run distributed apps. When the Server submits the
object to ORB, it will obtain IOR - string identification of the object
reference in ORB.\
\
When the client wants to access the object it will need the IOR. If the
server and client meet before they can somehow exchange this info - if
not we need to transfer the IOR a different way. Here we will use Naming
Service.\
\

### Naming Service

Naming Service is a tree structure which allows to store object
references of objects which are being managed by the Object Request
Broker. Basically that means that the client can now access the object
not only by its IOR - but also by a string like name of the object.\
\
Naming Service is a tree structure. There are generally 2 types of nodes
- contexts and object references. When a node is a context it can
contain several children - this can be again of type context or object.
When the client wants an object reference, he needs to now the exact
place of the object reference in the tree structure and its name.\
There is some valuable information about CORBA Naming Service on [this
site.](http://download.oracle.com/javase/1.4.2/docs/guide/idl/jidlNaming.html#startingnameserver)\
\

### NSAdmin

The tool which I called NSAdmin allows creating or deleting references
to objects as well as creating and destroying contexts. The tool can
visualise the tree structure using the Java JTree component. Here is how
it looks like:\

<div class="separator" style="clear: both; text-align: center;">

[![](http://3.bp.blogspot.com/_fmvjrARTMYo/TNQAvdcv8fI/AAAAAAAAAIA/j51yXl_dLBg/s320/nsadmin.JPG)](http://3.bp.blogspot.com/_fmvjrARTMYo/TNQAvdcv8fI/AAAAAAAAAIA/j51yXl_dLBg/s1600/nsadmin.JPG)

</div>

### How it works

I wrote NSAdmin in Java and I have used SWING to create the GUI. The
program is composed of two classes NSAdminFrame (the GUI) and NSAdmin a
static class which actually contains the implementations of the methods.
To test the tool you can use the Java implementation of ORB which is a
generic part of JDK. To start ORB run:\
**orbd.exe -ORBInitialPort 1234**\

### The initialisation

In order to work with the Naming Service we have to initialize the
Object Request Broker. From the ORB we can obtain the reference to root
naming context. This context is always called "NameService". The method
for initialization looks like this, if you have started the ORB on your
local machine, than the parametrs that you will submit to this method
will be: 1234,localhost. Note that the variable **rootContext** and
**orb** are defined on top of the static class.\

``` {.prettyprint}
public static void init(String port, String initialHost) throws Exception{ 
  String[] args = new String[4]; 
  args[0] = "-ORBInitialPort"; 
  args[1] = port; 
  args[2] = "-ORBInitialHost"; 
  args[3] = initialHost; 
 
  orb = ORB.init(args,null); 
  rootContext = NamingContextExtHelper.narrow(orb.resolve_initial_references("NameService")); 
} 
```

### Exploring the Naming Service tree

To list the content of the naming context we have a method which
recursively calls itself. This method will create a tree structure of
classes which I called Entry.\
\
Entry is a simple class which has a name and can have multiple childern
in its ArrayList entries.\

``` {.prettyprint}
public class Entry {
 public ArrayList entries;
 
 public String name;
 
 public Entry(String n){
  name = n;
  entries = new ArrayList();
 }
}
```

The structure of Entry classes will be later used to visualise the
content in JTree. OK now the method **explore** which recursivelly
traverses the naming context.

``` {.prettyprint}
public static Entry explore(String contextName,NamingContext context,int treeDepth) throws InvalidName, NotFound, CannotProceed, org.omg.CosNaming.NamingContextPackage.InvalidName{
  
  BindingListHolder blh = new BindingListHolder();
  BindingIteratorHolder bih = new BindingIteratorHolder();
  
  context.list(0,blh, bih);
  
  Entry entry = new Entry(contextName);
  
     System.out.println(contextName); 
  //binding iterator
  BindingIterator bit = bih.value;
  
  boolean remains = true;
  while(remains){
   BindingHolder biholder = new BindingHolder();
   remains = bit.next_one(biholder);
   Binding binding = biholder.value;
   
   NameComponent[] name = binding.binding_name;
   
   
   if(binding.binding_type == BindingType.nobject){
    if(name.length == 1){
     System.out.println("ID: " + name[0].id + " KIND: " + name[0].kind + " Depth: " + treeDepth);
     entry.entries.add(new Entry(name[0].id));
    }
   }else if(binding.binding_type == BindingType.ncontext){
    NamingContext tmpContext =  NamingContextHelper.narrow(context.resolve(binding.binding_name));
    NameComponent component = name[0];
    entry.entries.add(explore(component.id,tmpContext,treeDepth+1));
   }
  }
  return entry;
 }
```

The methdo takes **NamingContext** as parametr and will return a
structure of Entry classes which will corespond to the naming service
tree. There are couple thinks to notice:\
The method **list(int,BindingListHolder, BindingIteratorHolder)** has
two ways of usage. The first posibility is to specify how many binding
we want to load. If we specify the value, than the bindings will be
loaded into the array **Binding\[\]** which will be returned in the
BindingListHolder class.

The second possibility is to call **list(0,blh,bih)**. This way the
BindingListHolder will contain no results and we will obtain an iterator
which we can use to iterate through the array. That is the posibility
which I chosed.

You can see that I iterate through all the bindings and for each binding
I determine the type. If it is a context - than I will recursively
explore the context, if the type is object(reference is binded) than I
will just add new Entry which will represent this reference.\

### Destroying and Creating a subcontext

Creating the context is quite straight forward.\

``` {.prettyprint}
public static void createContext(String name) throws org.omg.CosNaming.NamingContextPackage.InvalidName, NotFound, CannotProceed, AlreadyBound{
  
  NameComponent[] contextName = rootContext.to_name(name);
  NamingContext newContext = rootContext.new_context();
  rootContext.bind_context(contextName,newContext);
 }
```

Destroying context can be a bit tricker because in order to destroy a
context, the context has to be empty. So I implemented a recursive
method, which will destroy all the children of context and than the
context can be destroyed.\
\

``` {.prettyprint}
public static void deleteContext(String name) throws org.omg.CosNaming.NamingContextPackage.InvalidName, NotFound, CannotProceed, NotEmpty{
  NameComponent[] contextName = rootContext.to_name(name);
  NamingContext context = NamingContextHelper.narrow(rootContext.resolve(contextName));
  
  BindingListHolder blh = new BindingListHolder();
  BindingIteratorHolder bih = new BindingIteratorHolder();
  context.list(Integer.MAX_VALUE, blh, bih);
  
  for(Binding binding:blh.value){
   NameComponent[] componentName = binding.binding_name;
   if(binding.binding_type == BindingType.ncontext){
    NSAdmin.deleteContext(name + "/" + componentName[0].id);
   }
   else{
    NSAdmin.deleteObjectReference(name + "/" + componentName[0].id);
   }
  }
  
  context.destroy();
  rootContext.unbind(contextName);
 }
```

### Creating object reference

This last method is a method to manually add object reference to a
context. Let say that an object was created on the server and we have
the IOR - the Interoperable Object Reference - which is a identificator
of the object. Well than we can bind the reference of the object to the
tree and a client application can use just the name to obtain the
reference.\
\

``` {.prettyprint}
public static void createObjectReference(String ior,String context,String objectName) 
 throws org.omg.CosNaming.NamingContextPackage.InvalidName, NotFound, CannotProceed, AlreadyBound{
  org.omg.CORBA.Object reference=orb.string_to_object(ior);
  NameComponent[] componentName = rootContext.to_name(context + objectName);
  rootContext.bind(componentName,reference);
 }
```

To obtain the reference of the object we just call the method
**string\_to\_object** of Object Request Broker. The actual binding is
than performed as for any other object we want to bind in the tree.\

### Summary

This is just a light overview of the functions which are the core of the
program. Of course there is a bit of glue code to put it together and
let it work with the GUI. I thinks you can easily understand the rest
from the code itself.\
[CodeProject](http://www.codeproject.com/script/Articles/BlogFeedList.aspx?amid=honga)
