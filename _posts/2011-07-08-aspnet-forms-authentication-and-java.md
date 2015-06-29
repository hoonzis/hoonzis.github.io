---
layout: post
title: ASP.NET Forms Authentication and Java client
date: '2011-07-08T02:36:00.000-07:00'
author: Jan Fajfr
tags:
- Security
- Java
- WCF
modified_time: '2014-06-26T14:59:05.450-07:00'
thumbnail: http://4.bp.blogspot.com/-Yli21ev2fgI/TgdPxAh6YFI/AAAAAAAAAMI/4Na-cg7zLIU/s72-c/fidler_cookie.PNG
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-5837908066316981280
blogger_orig_url: http://hoonzis.blogspot.com/2011/07/aspnet-forms-authentication-and-java.html
---
This post describes my later situation. I have a Silverlight application
which talks to traditional WCF services in backend. The services have so
far been configured automatically - so let's say Visual Studio took care
of the web.config. Newest requirement to my application was to allow
Java clients consume these services.

The prerequisites for this post are some basic knowledge of WCF
(bindings, services, endpoints) and some knowledge of Java (I am using
Axis to generate the clients...for the first time).

To make it a bit more complicated: I was using FormsAuthentication on
the backend side, since these services are hosted by IIS 7.

Here I want to show what to do use Forms Authentication from Java
application, mobile client or any other non-browser client.

The second part which describes how to enable WCF services to be
consumed by JAVA client is described in [my other
post.]({{site.baseurl}}{post_url2011-07-08-consuming-wcf-services-with-java-client}\)

### IIS 7 buid-in Authentication Service

I was using the build-in authentication service in order to authenticate
the client, which is just a basic service, which offers methods such as
Login, Logout etc.
This service can be enabled on IIS server using the following
configuration:

``` 
&ltsystem.web.extensions>
  &ltscripting>
    &ltwebServices>
      &ltauthenticationService enabled="true" requireSSL="false"/>
    </webServices>
  </scripting>
</system.web.extensions>
```

And we also need to expose this service:

``` 
&ltservice behaviorConfiguration="NeutralBehavior" name="System.Web.ApplicationServices.AuthenticationService">
    &ltendpoint address="" binding="basicHttpBinding" contract="System.Web.ApplicationServices.AuthenticationService" />
    &ltendpoint address="mex" binding="mexHttpBinding" contract="IMetadataExchange" />
</service>
```

Now that service works great from Silverlight client but I was not able
to generate Java client for this service - I tried with different
versions of Axis and settings - but it did not work for me.

So for the non-Silverlight client I needed to write my own
Authentication service. That is actually prety easy using the
**FormsAuthentication** static class.

``` 
[OperationContract]
public Login(String login, String password)
{
    //your way to auth the user againts DB or whatever
    var user = UserService.AuthenticateUser(login, password);
    
    if (user != null)
    {
        FormsAuthentication.SetAuthCookie(login, true);
    }
    return null;
}
```

After you check if the user is connected, you can just call the
**SetAuthCookie** method. This method adds authentication token to the
response which will go to server. Then the browser adds this token to
any request which he will send to the server.
And here comes the problem: how to use this with non-browser based
application?
Let me continue.


### Services secured using PrincipalPermission

I use FormsAuthentication, because it allows me to secure all service
just by adding the **PrincipalPermission** attribute over each Service
method. So my WCFUserService can look like this:

``` 
public class WCFTagService
{
  public WCFTagService()
  {
      Thread.CurrentPrincipal = HttpContext.Current.User;
  }
  
  [OperationContract]
  [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
  public Object GetSecuredData(int param)
  {
      return MyDB.GetData();
  }
}
```

In the constructor the **CurrentPrincipal** is set to the current user
of the ASP.NET application (again we are hosting this service in IIS),
than the \[PrincipalPermission\] attribute will be check even before the
method is executed if the user is logged in.
And how is the **HttpContext.Current.User determined**?
Well simply by checking the authentication token which the browser adds
to the request. IIS will automatically check this token and populated
the **User** static class with the correct identity.


### Adding one more authentication service for Java Clients

This is definitely not correct but it is the only way I was able to get
it to work. Basically when I call

``` 
FormsAuthentication.SetAuthCookie(login, true);
```

the cookie is added to the response and I will have to get it on the
client (Java) side. Actually I was not able to achieve that - I will
describe the approach I took lower, but I just did not get the cookie
from the response. So I decided to build one more service which will
just return the authentication token (or cookie if you will).

``` 
[OperationContract]
public String LoginCookie(String login, String password)
{
  var user = UserService.AuthenticateUser(login, password);
  if (user != null)
  {
      var cookie = FormsAuthentication.GetAuthCookie(login, true);
      return cookie.Value;
  }
  return null;
}
```

Ok that's it, we are done. We can almost switch to JAVA.


### Accessing Authentication Service using the Axis generated client

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

That is quite simple, I am calling the method which I have defined
before which gives me the authentication cookie. Remember that this
"Authentication Service" stays open, so anybody can call the methods.
Now when we have the cookie, we can use it to make calls to other
already protected services.

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



What is little bit said is the fact, that we have to create a special
method to be called by the Java client which returns the authentication
token directly and no as a cookie.
I was thinking - it could not be that hard, generate a client and get
the cookie. This way I could have only one authentication method used by
browser-based clients and Java clients. But I just did not managed to do
that.

I will show an attempt which I did - but did not succeed.


### Creating the client dynamically

The **java.rmi** namespace provides classes which will the creation of
web service client on the fly (without generation). This has some
advantages, specially that you can create a **javax.rmi.xml.Service**
class which allows assignment of special handlers, which are executed
during the "reception" and "sending" of SOAP messages, these handlers
can allow you to alter the content of the message and thus provide
possibility to do some additional tuning.

Personally I thought, that I will be able to create my own handler **to
recuperate the authentication cookie** send the standard way. But I did
not manage to get the cookie from the SOAP message. Well that is
actually normal, because the Cookie is not part of the SOAP message but
instead part of the HTTP message (which wraps the SOAP message). But
that is the problem I was not able to locate the cookie in the HTTP
Response message, anyone knows how to do that?

I will provide here a conception of my solution - maybe someone will be
able to finalize and obtain the cookie from the response of the
authentication service.

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
the names of the service, the port and the operations, you can find
these easily in the WSDL definition.
Here follows the definition of the SimpleHandler which is added to the
handler chain

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




### Alternative approach using WCF Inspectors

When looking into this problem, I found one alternative approach that
you can use when dealing with Security and WCF Service.
The solution is basic:

-   Give up on FormsAuthentication
-   Define your own authentication tickets or just pass the login/pass
    combination on each request in the HTTP Header
-   Define a Message InInspector on the Server which would read the
    message upon its reception and check the availability of the
    authentication token or the credentials in the message header

When following this approach what might come handy is an easy way to
generate and later control the Authentication ticket.
FormsAuthentication can actually help you with this. Here is what
happens when you call the **FormsAuthentication.GetAuthCookie**.

``` 
FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, login, DateTime.Now, DateTime.Now.AddMinutes(30), false, login);
string encryptedTicket = FormsAuthentication.Encrypt(ticket);
HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
```

So you can create an Inspector class, which will do the reverse of this
process:

``` 
public class TestInspector : IDispatchMessageInspector
{
    public TestInspector()  { }
    
    public object AfterReceiveRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel, System.ServiceModel.InstanceContext instanceContext)
    {
        var httpRequest = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
        var cookie = httpRequest.Headers[HttpRequestHeader.Authorization];
        if(cookie == null)
        {
          throw new SecurityException("Not authenticated!");
        }
        var ticket = FormsAuthentication.Decrypt(cookie);
        if(ticket.IsExpired)
        {
          throw new SecurityException("Ticket expired");
        }
    }

    public void BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
    {
        
    }
}
```

### Securing the servicing using SSL

When we pass the authentication token over the wire, we want to be sure,
that no-one can intercept this token and act in name of the user against
the services. To prevent this situation we can use SSL to secure the
whole communication between client and server.

The WCF configuration which is needed is quite simple, we just have to
alter the standard **basicHttpBinding** by adding the **security**
mode.

``` 
&ltbasicHttpBinding>
  &ltbinding name="SecurityByTransport">
    &ltsecurity mode="Transport">
      &lttransport clientCredentialType="None"/>
    </security>
  </binding>
</basicHttpBinding>
```


Than comes the infrastructure work:


-   Be sure to publish the service on your local IIS server (you cannot
    use the build-in Visual Studio Server
-   On the IIS server create a new certificate - for test purposes
    auto-signed
-   Configure a new binding to application that you have deployed using
    the certificate, that you have created

This should be enough. Now we need to go back to the Java client - if we
can regenerated the client using Axis. When you run the client for the
first time, you will get the following exception:
*java client unable to find valid certification path to requested
target
That is because JVM maintains its list of trusted server. If he sees
that the certificate is signed by Certification Authority, he will add
it to its "keystore". Because for testing you usually use Self-Signed
certificate, JVM will not add it do the keystore, it has to be done
manually.

So: go back to the IIS 7 configuration and in the list of the
certificates, select the certificate and on the "Details" tab page
choose: "Copy to File".



[![](http://1.bp.blogspot.com/-ouZAfq6dmq4/TgiUWA_eqdI/AAAAAAAAAMQ/Af-8ZEURtdY/s320/Capture1.PNG)](http://1.bp.blogspot.com/-ouZAfq6dmq4/TgiUWA_eqdI/AAAAAAAAAMQ/Af-8ZEURtdY/s1600/Capture1.PNG)



You can leave the predefined option and just save the ".CER" wherever
you want to.

Now to finish you have to run the following command in the
JAVA-HOME\\BIN directory:

    keytool.exe -import -alias localhost -file C:\myCert.cer -keystore "c:\Program Files\Java\jre6\lib\security\cacerts"

-   **localhost** - stands for the web server which holds the
    certificate (your local IIS).
-   **cacerts directory** - is the store of trusted certificates.
-   The password is "changeit".



### Summary

I tried to connect to secured WCF services hosted on IIS server with
Java client. During the process I found some issues, but at the end I
was able to connect securely to the services. The main steps are:

-   Don't use IIS build-in Authentication Service
-   Provide a service which will return the Authnentication Cookie to
    the Java client
-   Pass this cookie along with any request which is sent to secured
    services

In the end, I have showed how to enable SSL on the WCF service and how
to consume the service with Java client.
And at last I presented an approach which should be taken to replace
FormsAuthentication with your own authentication scheme using WCF
Message Inspectors.
