---
layout: post
title: Screen scraping in C# using WebClient
date: '2013-05-09T01:08:00.000-07:00'
author: Jan Fajfr
tags:
- C#
modified_time: '2014-06-26T10:37:24.329-07:00'
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-6719751879680739053
blogger_orig_url: http://hoonzis.blogspot.com/2013/05/screen-scraping-in-c-using-webclient.html
---
This post is intended to give you some useful tips to perform screen
scraping in C\#. Well first let's put it clear. In the ideal world we
should not be forced to do screen scraping. Every solid web site,
application or service should propose a decent API to provide the data
to other applications. If the application holds resources of it's users,
than it should propose OAuth protected API and thus allow the users to
use their data through another application. But we are not yet in this
situation.

Observing the communication
---------------------------

In order to know what kind of HTTP request you have to issue, you have
to observe what the browser is doing when you browse the web page. There
is not a better tool for the job than [Fiddler](http://fiddler2.com/).
One of the features provided which you might find really useful is that
it can **automatically decrypt HTTPS traffic.**

Getting the data
----------------

Once you determine which web requests you should replay you need the
infrastructure necessary to execute the requests. .NET provides the
**WebClient** class. Note that WebClient is a facade for using creating
and handling HttpWebRequest and HttpWebResponse objects. Feel free to
use these classes directly if you want, but by default the compiler will
not like their usage since they are marked as obsolote.

Parsing the data
----------------

If you are just need to screen scrape a simple site which is invoked by
HTTP GET request, than you do not need any special information. You can
just fire **WebClient**, obtain the string and than parse the result.
When parsing the result, you have to keep in mind, that HTML is not a
regular language. Therefor you cannot always use Regular Expressions to
parse it. However you can usually get around with it. A common task is
to match some information in some concrete tag, here are two examples:

Matching any text inside a div with some special styles:

``` 

```

``` 
var addressTerm = new Regex("");
```

Matching two decimal values inside a div separated by BR tag:

``` 

```

``` 
var dataTerm = new Regex("");
```

Posting values
--------------

When submiting a form to a web application, the browser usually performs
a http POST request and encodes the values to the posting URL. In order
to create such a request, you have to set the content type of the
request to **application/x-www-form-urlencoded**. Then you can use the
**UploadData** of the WebClient.

``` 
using(var client = new WebClient()){
 var contentType = "application/x-www-form-urlencoded";
 client.Headers.Add("Content-Type", contentType);
 
 var values = new NameAndValueCollection();
 values.Add("name", name);
 values.Add("pass", pass);
 var response = client.UploadValues(url, "POST", values);
}
```

Handling the authentification
-----------------------------

In some cases you have to pass the authentication before you get to the
information that you need. Most of the web sites use cookie based
authentication. Once the user is authenticated the server generates an
authentication cookie which than is automatically added to any
susccesive request by the web browser. By default **WebClient** does not
accept store cookies. The infrastructure to handle cookies is
implemented on the level of HttpWebRequest. I have found a very useful
example of "cookie aware" WebClient which keeps all the cookies that it
has recieved so far and adds them to any newer request on the following
StackOverflow link:

<http://stackoverflow.com/questions/1777221/using-cookiecontainer-with-webclient-class>

``` 
public class WebClientEx : WebClient
{
    public WebClientEx(CookieContainer container)
    {
        this.container = container;
    }

    private readonly CookieContainer container = new CookieContainer();

    protected override WebRequest GetWebRequest(Uri address)
    {
        WebRequest r = base.GetWebRequest(address);
        var request = r as HttpWebRequest;
        if (request != null)
        {
            request.CookieContainer = container;
        }
        return r;
    }

    protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
    {
        WebResponse response = base.GetWebResponse(request, result);
        ReadCookies(response);
        return response;
    }

    protected override WebResponse GetWebResponse(WebRequest request)
    {
        WebResponse response = base.GetWebResponse(request);
        ReadCookies(response);
        return response;
    }

    private void ReadCookies(WebResponse r)
    {
        var response = r as HttpWebResponse;
        if (response != null)
        {
            CookieCollection cookies = response.Cookies;
            container.Add(cookies);
        }
    }
}
```

### Diggest authentication

Some web site may employ "digest" authentication, which based on
hashing, adds a little more security againts "man-in-the-middle attacks.
In that case you will see, that a login request is not just composed of
a simple POST request with the "login" and "password" values. Instead a
combination of random value (which the server knows) and the password is
composed, hashed together and sent to the server.

``` 
digestPassword = hash(hash(login+password)+nonce);
```

Nonce - in the previous definition is the "Number Used Only Once", which
is generated by the server and which the server keeps in a pool in order
to keep track of already used values. Here are two simple methods to
create a digestPassword:

``` 
public static String DigestResponse(String idClient, String password, String nonce)
{
 var cp = idClient + password;
 var hashedCP = CalculateSHA1(cp, Encoding.UTF8);
 var cnp = hashedCP + nonce;
 return CalculateSHA1(cnp, Encoding.UTF8);
}

public static string CalculateSHA1(string text, Encoding enc)
{
 byte[] buffer = enc.GetBytes(text);
 var cryptoTransformSHA1 = new SHA1CryptoServiceProvider();
 return BitConverter.ToString(cryptoTransformSHA1.ComputeHash(buffer)).Replace("-", "").ToLower();
}
```

Ofcourse when using the digest authentication, the server has to provide
the value of the "Nonce" to the client. The value is usually a part of
the login page and the authentication and the hashing is one in
JavaScript

State-full JSF applications
---------------------------

Most of the web applications that we see today are composed of stateless
services. There are some really good reasons for that, however it is
still posible that you might have to analyze a stateful application. In
this situation the order of the http web requests matters. JSF is one of
such web technologies which favor stateful applications. In my case I
needed to obtain a CSV file which was generated using the data
previously shown to the user in a HTML table. The way this was done, was
that the ID of the table element was passed to the CSV generation
request. So these two requests were interconnected. More than that, the
ID value was generated by JSF and I think that it was dependent on the
number of previously generated HTML elements. Typically the generated ID
values are prefixed by "j\_id" and if I wanted to hardcode this value, I
had to compose always exactly the same set of HTTP requests.

``` 
values.Add("source", "j_id630");
```

Make them think you are a serious browser
-----------------------------------------

Some web page check for the browser accessing the page, you can easily
make them think you are Mozilla Firefox:

``` 
var mozilaAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Win64; x64; Trident/5.0)";
client.Headers.Add("User-Agent", mozilaAgent);
```

Summary
-------

If there is any other way to obtain the data, than it is probably better
way. If you cannnot avoid it, I hope this gave you couple hints.
