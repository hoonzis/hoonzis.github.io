---
layout: post
title: DotNetOpenAuth and Url rewriting (in Azure)
date: '2011-08-17T06:01:00.000-07:00'
author: Jan Fajfr
tags:
- OAuth
- WCF
modified_time: '2014-06-26T14:51:20.727-07:00'
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-7264593240363821929
blogger_orig_url: http://hoonzis.blogspot.com/2011/08/dotnetopenauth-and-url-rewriting-in.html
---
I have created a simple OAuth provider for my web application using
DotNetOpenAuth just [the way that I described in this last
post.](http:/hoonzis.blogspot.com/2011/08/using-dotnetopenauth-to-create-oauth.html)
On local everything worked just fine. However when I published the
solution to Azure then I obtained errors while processing authorized
request.

Well the error happend because I used the idea of specifing exact
**scope** within each token, which says what URL the consumer
application has right to access. So to each Access Token a String value
representing the scope is added representing the URL which the
application has right to access. When the consumer application demands
data from certain web service, than the Authorization Manager checks the
scope of the access token which was added to the request.
The problem comes when the server which is hosting the OAuth data
provider performs some URL rewriting. In that case the URL which is
being accessed had changed in the HTTP pipeline and the provider has to
take care of that. The URL of each request comming to Azure changes
inside.

If you take a look at the Authorization Manager code from the
DotNetOpenAuth you will see that it checks the scope of the incoming
message.

``` 
public class OAuthAuthorizationManager : ServiceAuthorizationManager
{
  protected override bool CheckAccessCore(OperationContext operationContext)
  {
    //check the access token etc...
    //scopes containes the scopes added to the access token
    if (scopes.Contains(operationContext.IncomingMessageHeaders.Action)) {
      return true;
    }
  }
}
```


And that is actually the problem, while
**operationContext.IncomingMessageHeaders.Action** contains the URL
after rewrite and the consumer application usually specifies the URL
which it wants to access in the form before the rewrite.

What I found as a solution to this issue was to use instead this piece
of code

``` 
Uri requestUri = operationContext.RequestContext.RequestMessage.Properties.Via;
...
...
var action = requestUri.AbsoluteUri.Substring(0, requestUri.AbsoluteUri.IndexOf("?"));
if (scopes.Contains(action))
{
    return true;
}
```
