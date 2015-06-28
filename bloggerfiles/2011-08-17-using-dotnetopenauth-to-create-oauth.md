--- layout: post title: Using DotNetOpenAuth to create OAuth Provider
date: '2011-08-17T03:43:00.000-07:00' author: Jan Fajfr tags: - OAuth -
WCF modified\_time: '2014-06-26T14:49:34.166-07:00' thumbnail:
http://3.bp.blogspot.com/-RavCX64q2tQ/TkubMhKbDnI/AAAAAAAAAMk/aUfz87m9-ks/s72-c/OAuth\_workflow.png
blogger\_id:
tag:blogger.com,1999:blog-1710034134179566048.post-8586859066855713132
blogger\_orig\_url:
http://hoonzis.blogspot.com/2011/08/using-dotnetopenauth-to-create-oauth.html
--- [Download the source code from
GitHub.](https://github.com/hoonzis/OAuthPoc)\
[DotNetOpenAuth](http://www.dotnetopenauth.net/) is an open source
library created and managed by Andrew Arnott which gives you the\
possibility to use OAuth protocol, OpenID and ICard. It si powerful -
and comes with a nice Samples package. Recently I needed to implement
OAuth provider, in other words I wanted to allow third party application
to obtain data from my application after the user authorizes them to do
so.\
\
Before I have implemented that for my application, I have created a
simple Proof of Concept (POC), which I will share with you. Basically it
is just a simplified version of the OAuthProvider project in the Samples
package. That is a greate example, however one fact, that might be
little confusing is that it uses Linq to SQL to store the authentication
Tokens and if you do not want to enter into that you might get lost. My
targeted application uses NHibernate for ORM, but I decided to make the
POC only store data in the memmory to keep it as simple as I could.\
\
At the end I will just lay out how I incorporated DotNetOpenAuth to my
application, where I already had Data Access layer established using
NHibernate.\
\
To understand the rest of this post, you need to understand the basics
of OAuth protocol.\
\
Here is the standard way of communication between Consumer and Provider
using OAuth protocol.\

<div class="separator" style="clear: both; text-align: center;">

[![](http://3.bp.blogspot.com/-RavCX64q2tQ/TkubMhKbDnI/AAAAAAAAAMk/aUfz87m9-ks/s320/OAuth_workflow.png)](http://3.bp.blogspot.com/-RavCX64q2tQ/TkubMhKbDnI/AAAAAAAAAMk/aUfz87m9-ks/s1600/OAuth_workflow.png)

</div>

\
\
\
DotNetOpenAuth provides classes and structures which enable you to
easily create OAuth Consumer or Provider and manipulate Tokens. However
each both Consumer and Provider have to decide on how to handle and
store the Tokens.\
\
The basic scenario is this:\
Provider exposes WCF service which is secured using OAuth protocol.
Consumer can access this services only when he obtains authorization of
the actual user and owner of the resources.\
Here is a digram which shows the structure of OAuth Provider when
implemented using DotNetOpenAuth.\
\

<div class="separator" style="clear: both; text-align: center;">

[![](http://1.bp.blogspot.com/-oS8jJbVEYVA/Tkuad-us3hI/AAAAAAAAAMc/9Spm67ziNUY/s320/oauth_provider_consumer.png)](http://1.bp.blogspot.com/-oS8jJbVEYVA/Tkuad-us3hI/AAAAAAAAAMc/9Spm67ziNUY/s1600/oauth_provider_consumer.png)

</div>

\
There are two entities which perform the communication. First it is
simple Http Handler which takes care of the OAuth "handshake". The
second is the actual WCF service which uses custom Authorization Manager
to perform the authentication. Both of these make use of the Service
Provider (comming from DotNetOpenAuth). Service Provider than uses
implementations of IServiceProviderTokenManager and INonceStore also
defined in DotNetOpenAuth, which take care of the persistance of Nonces
and Tokens. It is up to the programmer to decide how to implement these
interfaces.\
\
OAuth provider needs to store three types of objects: Consumers, Tokens
and Nonces. To keep it simple, I decided to store all of them in memory
in lists inside applications Global file.\
\

``` {.prettyprint}
public class Global : System.Web.HttpApplication
{
  public static List<OAuthConsumer> Consumers { get; set; }
  public static List<OAuthToken> AuthTokens { get; set; }
  public static List<Nonce> Nonces { get; set; }  
}
```

\
Now these list are then used by ServiceProvider, actually by
IServiceProviderTokenManager and INonceStore, which later in turn are
used by ServiceProvider. Lets first take a look at the
IServiceProviderTokenManager interface ([definition is
here](http://docs.dotnetopenauth.net/master/html/AllMembers_T_DotNetOpenAuth_OAuth_ChannelElements_IServiceProviderTokenManager.htm)).
For example the GetRequestToken method would be implemented like this:\

``` {.prettyprint}
public IServiceProviderRequestToken GetRequestToken(string token)
{

    var foundToken = Global.AuthTokens.FirstOrDefault(t => t.Token == token && t.State != TokenAuthorizationState.AccessToken);
    
    if(foundToken==null)
    {
        throw new KeyNotFoundException("Unrecognized token");
    }
    return foundToken;
}
```

\
So it is quite easy. The method actually returns
IServiceProviderRequestToken, methods which work with Nonces or Consumer
also return interfaces defined by DotNetOpenAuth, so in other words all
of your business entities which encapsulate Consumer, Tokens or Nonces
have to implement these interfaces defined by DotNetOpenAuth.\
\
There are two types of tokens: Request Token
(IServiceProviderRequestToken) and Access Token
(IServiceProviderAccessToken). During the OAuth handshake, the request
token is interchanged for the access token. So you can actually create
one class which implements both of these interfaces. In that case
**implement these interfaces explicitely** because there are Properties
which have to be implemented with same name. There are two properties
which are returning String called Token (one comming from Access Token
and other from Request Token interface), here is the way in which they
are implemented:\

``` {.prettyprint}
private String _token;
String IServiceProviderRequestToken.Token
{
    get { return _token; }
}

String IServiceProviderAccessToken.Token 
{
    get { return _token; }
}

public String Token { 
    set { 
        _token = value;  
    }
}
```

When the Token changes from Request to Access token, the actual String
value stays the same. So I have backed up both of these properties by
the same private field and added a property which will allows me to set
this field.

Basically thats it. There is much more code around but actually I just
took most of the code comming from the official set of examples.

Using NHibernate to persists Tokens, Consumer and Nonces
--------------------------------------------------------

In the project where I needed to implement OAuth provider, I was using
NHibernate as my ORM with NFluent(nice framework which allows to write
configuration of NHibernate in C\#). What I like about this combination
is the fact, that there is no XML file and generated properties (such as
with Linq2SQL).\
I always try to keep my database entities as clear as possible, that is
why I did not want my entities to implement the interfaces forced by
DotNetOpenAuth. Instead of that I wrapped my entities by classes which
are using these interfaces and use the database persisted entities as
backup. So just to explain what I mean, here is the persistant class:\

``` {.prettyprint}
public class AuthToken
{
    public virtual int Id { get; set; }
    public virtual AuthConsumer Consumer { get; set; }
    public virtual AuthTokenState State { get; set; }
    public virtual DateTime IssueDate { get; set; }
    public virtual UserIdentity User { get; set; }
    public virtual String TokenSecret { get; set; }
    public virtual String Scope { get; set; }
    public virtual String Token { get; set; }
    public virtual String Version { get; set; }
    public virtual String VerificationCode { get; set; }
    public virtual DateTime? ExpirationDate { get; set; }
    public virtual String[] Roles { get; set; }
    public virtual String Callback { get; set; }
}
```

And here the DotNetOpenAuth compatible wrapper:\

``` {.prettyprint}
public class OAuthToken : IServiceProviderRequestToken, IServiceProviderAccessToken
{
    public OAuthToken(AuthToken token)
    {
        if (token == null)
        {
            throw new ArgumentNullException("Token passed to constructor of OAuthToken cannot be null");
        }
        Token = token;
    }

    public OAuthToken()
    {
        Token = new AuthToken();
    }

    public AuthToken Token {get;set;}

    #region IServiceProviderRequestToken

    Uri IServiceProviderRequestToken.Callback
    {
        get
        {
            return new Uri(Token.Callback);
        }
        set
        {
            if (value != null)
            {
                Token.Callback = value.AbsoluteUri;
            }
        }
    }

    string IServiceProviderRequestToken.ConsumerKey
    {
        get { return Token.Consumer.ConsumerKey; }
    }

    Version IServiceProviderRequestToken.ConsumerVersion
    {
        get
        {
            if (Token == null || Token.Version == null)
            {
                throw new ArgumentNullException("The Token or the Version are null");
            }
            return new Version(Token.Version);
        }
        set
        {
            Token.Version = value.ToString();
        }
    }

    DateTime IServiceProviderRequestToken.CreatedOn
    {
        
        get {
            return Token.IssueDate.ToLocalTime(); }
    }

    string IServiceProviderRequestToken.Token
    {
        get { return Token.Token; }
    }

    string IServiceProviderRequestToken.VerificationCode
    {
        get
        {
            return Token.VerificationCode;
        }
        set
        {
            Token.VerificationCode = value;
        }
    }

    #endregion

    #region IServiceProviderAccessToken

    DateTime? IServiceProviderAccessToken.ExpirationDate
    {
        get { return Token.ExpirationDate; }
    }

    string[] IServiceProviderAccessToken.Roles
    {
        get { return Token.Roles; }
    }

    string IServiceProviderAccessToken.Token
    {
        get { return Token.Token; }
    }

    string IServiceProviderAccessToken.Username
    {
        get {
            if (Token.User == null)
            {
                throw new ArgumentNullException("Token does not have assigned user");
            }
            return Token.User.Identification; 
        }
    }
    #endregion
}
```

In that case the Token Manager has to take care of the conversation with
database as well as with wrapping the recieved entities.\

``` {.prettyprint}
public class DatabaseTokenManager : IServiceProviderTokenManager
{
  private IOAuthServices _oAuthServices;
  
  public IOAuthServices OAuthServices
  {
      get {
          if (_oAuthServices == null)
          {
              _oAuthServices = get your service class which talks to the database....
          }
          return _oAuthServices;
      }
  }
  
  public IServiceProviderRequestToken GetRequestToken(string token)
  {
      var authToken = OAuthServices.GetRequestToken(token);
      if (authToken == null)
      {
          throw new SecurityException("No token found: " + token);
      }
      return new OAuthToken(authToken);
  }
}
```

The rest stays the same and it works fine.\
It took me some time to understand how DotNetOpenAuth on the provider
site works. I hope this post can help someone to jump in fast.\
\
[Get the code from GitHub](https://github.com/hoonzis/OAuthPoc)
