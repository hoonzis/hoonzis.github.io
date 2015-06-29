---
layout: post
title: Consuming WCF Services with Java Client
date: '2011-07-08T03:07:00.000-07:00'
author: Jan Fajfr
tags:
- Java
- WCF
modified_time: '2014-06-27T03:16:50.639-07:00'
thumbnail: http://4.bp.blogspot.com/-Yli21ev2fgI/TgdPxAh6YFI/AAAAAAAAAMI/4Na-cg7zLIU/s72-c/fidler_cookie.PNG
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-291252293214641586
blogger_orig_url: http://hoonzis.blogspot.com/2011/07/consuming-wcf-services-with-java-client.html
---
Here is the state of my latest project: I have a Silverlight application
which talks to traditional WCF services in backend. The services have so
far been configured automatically - so let's say Visual Studio took care
of the web.config. Newest requirement to my application was to allow
Java clients consume these services.

The prerequisites for this post are some basic knowledge of WCF
(bindings, services, endpoints) and some knowledge of Java (I am using
Axis to generate the clients...for the first time).

To make it a bit more complicated: I was using FormsAuthentication on
the backend side, since these services are hosted by IIS 7.

Here I want to describe how to configure WCF services to be consumed by
JAVA clients.
The second part which describes how to keep using Forms Authentication
is described in [my other
post]({{site.baseurl}}{post_url2011-07-08-aspnet-forms-authentication-and-java}\)

To expose the services for JAVA client, we have two options:

-   Expose the services using SOAP protocol
-   Expose the services using REST approach

Both of these are possible with WCF. This ability to take existing
services and expose them using different protocols and transfer formats
is what makes WCF so powerful and useful.

Here I will describe in details how to expose the services using SOAP
protocol and in the end I will give a brief description of what to do to
expose these services using REST approach.


### Changing WCF configuration

The first step is to start changing WCF configuration which is presented
in "web.config" file (at least in the case of service hosted in IIS).

If you let Visual Studio configure your service, you will see that it
creates for each services it's own binding - event though the services
can share binding configuration.

Also - if you consume service by Silverlight client, than VS chooses to
define **binnaryMessageEncoding** as a transport format. Because both -
the backend and the client are .NET applications, WCF can be configured
to transfer the objects over the wires in binary format (because both
the client and the server know how to serialize/deserialize) the data in
this format. To consume the service by a Java application you will need
to use a traditional **basicHttpBinding** - that is simple binding which
uses standard WSDL specification in hand with XML serialization.

So first step is to locate your binding and service definition and
change the binding to **basicHttpBinding**.


``` 
&ltbinding name="BinaryOverHTTPBinding">
    &ltbinaryMessageEncoding />
    &lthttpTransport />
</binding>
    
&ltservice name="Octo.Bank.Web.WCFServices.WCFUserService" behaviorConfiguration="NeutralBehavior">
    &ltendpoint address="" binding="BinaryOverHTTPBinding" contract="MyProject.WCFUserService"/>
    &ltendpoint address="mex" binding="mexHttpBinding" contract="IMetadataExchange"/>
</service>
```

Replace the binding configuration in the **endpoint** definition.

``` 
&ltendpoint address="" binding="basicHttpBinding" contract="MyProject.WCFUserService"/>
```

Just to completed the image here, the **service** is configured to user
"NeutralBehavior".

``` 
&ltbehavior name="NeutralBehavior">
  &ltserviceMetadata httpGetEnabled="true"/>
  &ltserviceDebug includeExceptionDetailInFaults="false"/>
</behavior>
```

What is important is, that the **httpGetEnabled** set to true in the
combination with the **mex** endpoint will ensure that the WSDL
definition of this service will be exposed (the url of the WSDL
definition will be simply http://server/myService?wsdl).

Now that is the bare minimum to be able to connect to this
WCFUserService with Java client.

### Defining the namespaces and ports

While WCF Client, or Silverlight Client do not have a problem to
generate a stub client for the defined service, when you will try to
generate the client in java, you will obtain an exception saying that
one of the port bindings was not properly defined. The cause is that you
need to define different namespace and name in your **ServiceContract**
and **ServiceBehavior**. These are two attributes which can be placed on
top of your service class.

``` 
[ServiceContract(Namespace = "octo.users.service",Name="UserService")]
[ServiceBehavior(Namespace = "octo.users.port", Name = "UserPort")]
[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
public class WCFUserService { }
```

This completely changes the resulting **WSDL** file, which is describing
the service. This is really important because if you do not make these
changes, you will not be able to generate the client with Axis
framework.


### Creating the Java client

I am using Eclipse in combination with Axis framework to talk to my
services. But first let's put out the 2 options that we have to access
Web Services.

-   **Creating dynamically the client**
-   **Using Axis to generate the client for us**

### Accessing Web Service using Axis created client

Before we start, we need to generated the client, either you can use the
build in tool in Eclipse ("New -&gt; Other -&gt; Web Service Client") or
you can use the commander line "WSDLtoJava" utility. In both cases you
have to enter just the URL of the WSDL.

When the client is ready, you can see that there is quite a lot of
code(10kLines) generated for you.

``` 
MyServiceLocator locator = new MyServiceLocator();
AuthService client = locator.getBasicHttpBinding_AuthService();
String cookie = client.LoginCookie("login","password");
```

I am calling the method defined before which gives me the authentication
cookie. Remember that this "Authentication Service" stays open, so
anybody can call the methods. Now when we have the cookie, we can use it
to make calls to other already protected services.

``` 
MyServiceLocator locator = new MyServiceLocator();
WCFUserService client = locator.getBasicHttpBinding_WCFUserService();
((Stub)client)._setProperty(Call.SESSION_MAINTAIN_PROPERTY,new Boolean(true));
((Stub)client)._setProperty(HTTPConstants.HEADER_COOKIE, ".ASPXAUTH=" + 
cookie);
Object data = client.GetSecuredData(myParam);
```

The generated client does not allow you to add cookies, but you can
convert the client to **org.appache.axis.client.Stub** which allows you
to call *_setProperty** method a static HttpConstatns class provides
the names of the headers which you can set.

**Now notice the "ASPXAUTH=" that is the prefix(or in other words the
name) of the cookie and it has to be there**. It took me a while to find
out in what exact form should I send the cookie, finally
[Fiddler](http://www.fiddler2.com/fiddler2/) came as help - I used the
Silverlight client to see what exactly he is sending and I just did the
same.



[![](http://4.bp.blogspot.com/-Yli21ev2fgI/TgdPxAh6YFI/AAAAAAAAAMI/4Na-cg7zLIU/s320/fidler_cookie.PNG)](http://4.bp.blogspot.com/-Yli21ev2fgI/TgdPxAh6YFI/AAAAAAAAAMI/4Na-cg7zLIU/s1600/fidler_cookie.PNG)



### Creating the client dynamically

The **java.rmi** namespace provides classes allowing the creation of web
service client on the fly (without code generation). This has some
advantages, specially that you can create a **javax.rmi.xml.Service**
class permitting you to assign special handlers, which are executed
during the "reception" and "sending" of SOAP messages. These handlers
can allow you to alter the content of the message and thus provide
possibility to do some additional tuning, or security checks.

When working with WCF or CXF framework, you have probably heard of
Interceptors, which are equivalent to "Handlers".

Personally I thought, that I will be able to create my own handler **to
recuperate the authentication cookie** send the standard way. But I did
not manage to get the cookie from the SOAP message. I will provide here
a conception of my solution - maybe someone will be able to finalize and
obtain the cookie from the response of the authentication service.

``` 
try {
  QName serviceName = new QName("http://mynamespace","AuthService");
  URL wsdlLocation = new URL("http://localhost:49830/WCFServices/WCFUserService.svc?wsdl");
  // Service
  ServiceFactory factory = ServiceFactory.newInstance();
  Service service =  factory.createService(wsdlLocation,serviceName);
  
  //Add the handler to the handler chain
  HandlerRegistry hr = service.getHandlerRegistry();
  HandlerInfo hi = new HandlerInfo();
  hi.setHandlerClass(SimpleHandler.class);
  handlerChain.add(hi);
  
  QName  portName = new QName("http://localhost:49830/WCFServices/WCFUserService.svc?wsdl", "BasicHttpBinding_AuthService");
  List handlerChain = hr.getHandlerChain(portName);
  
  QName operationName = new QName("http://localhost:49830/WCFServices/WCFUserService.svc?wsdl", "Login");
  Call call = service.createCall(portName,operationName);
  
  //call the operation
  Object resp = call.invoke(new java.lang.Object[] {"login","pass"});
}
```

To be able to call the web service dynamically, you will need to specify
the name of the service, the port and the operations. You can find these
easily in the WSDL definition file.
Here follows the definition of the **SimpleHandler** class which is
added to the HTTP handler chain

``` 
public class SimpleHandler extends GenericHandler {
 
  HandlerInfo hi;
 
  public void init(HandlerInfo info) {
    hi = info;
  }

  public QName[] getHeaders() {
    return hi.getHeaders();
  }

  public boolean handleResponse(MessageContext context) {
    try {
     
     //Iterate over all properties - did not find the cookie there :(
     Iterator properties = context.getPropertyNames();
        while(properties.hasNext()){
         Object property = properties.next();
         System.out.println(property.toString());
        }
        
      //examine the response header - did not find the cookie there either :( 
      if(context.containsProperty("response")){
       Object response = context.getProperty("response");
       HttpResponse httpResponse = (HttpResponse)response;
       
       Header[] headers = httpResponse.getAllHeaders();
       for(Header header:headers){
        System.out.println(header.toString());
       }
      }
     
     //here is how to get the SOAP headers - they do not serve - we need pure HTTP response
      // get the soap header
      SOAPMessageContext smc = (SOAPMessageContext) context;
      SOAPMessage message = smc.getMessage();
      
    } catch (Exception e) {
      throw new JAXRPCException(e);
    }
    return true;
  }
  public boolean handleRequest(MessageContext context) { 
    return true;
  }
}
```

### Securing services by SSL

In my other post I have described how to secure the web services by SSL,
you can find the information which describes how to configure the JAVA
client to connect to these secured services.

### REST approach

To expose the service as RESTfull we will have to define another
endpoint for the service.

``` 
&ltservice behaviorConfiguration="NeutralBehavior" name="MyServic">
  &ltendpoint address="json" binding="webHttpBinding"  behaviorConfiguration="jsonBehavior" contract="Octo.Bank.Web.WCFServices.WCFAccountService" name="JsonEndpoint"/>
  &ltendpoint address="soap" binding="basicHttpBinding" .../>
  &ltendpoint address="mex" .../>
</service>
```

Notice that this endpoint uses **webHttpBinding** and a special behavior
called **jsonBehavior**. This behavior as it's name says just defines
JSON as the transport format.

``` 
&ltendpointBehaviors>
  &ltbehavior name="jsonBehavior">
    &ltwebHttp defaultOutgoingResponseFormat="Json"/>
  </behavior>
</endpointBehaviors>
```

This is enough for the configuraiton. Now just some minor changes to the
Service itself.
At the end I showed guidlines for exposing WCF services using the REST
approach.

``` 
public class MyService {  
  [OperationContract]
  [WebGet(UriTemplate="/accounts?id={id}", BodyStyle=WebMessageBodyStyle.Wrapped)]
  public IList&ltAccountDto> GetAccountsByCustomer(int id)
  {
    return AccountService.GetCustomerAccounts(id);
  }
}
```

It is the **WebGet** attribute which exposes the service for HTTP GET
request. The UriTemplate defines which URL will invoke the service.
Notice that the parameter of the service is extracted from the URL
itself.
If we would have a method which posts data, it would be decorated with
\[WebInvoke\] attribute.
This is just a slight intro, you can find more information on internet,
here I wanted just to provide some basic information to make this post
complete enough.


### Summary

I have shown how to change the configuration to publish WCF Service
using SOAP protocol and consume this services with JAVA client. At the
end I just showed how to expose the service using REST approach.
