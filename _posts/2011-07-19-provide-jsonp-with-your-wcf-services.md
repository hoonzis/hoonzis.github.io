---
layout: post
title: Provide JSONP with your WCF services (using .NET 3.5)
date: '2011-07-19T07:45:00.001-07:00'
author: Jan Fajfr
tags:
- WCF
modified_time: '2014-06-26T14:52:34.291-07:00'
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-2193220764597818623
blogger_orig_url: http://hoonzis.blogspot.com/2011/07/provide-jsonp-with-your-wcf-services.html
---
I wrote this post mainly to correct one "bug", or let's say complete the
MS example which shows how to configure your WCF services to provide
data in JSONP format.

This example works except the case when you are returning a raw JSON,
that is you are not returning object which is serialized in to JSON, but
rather returning a Stream which represents this JSON.

The exception which you might obtain will be:

Encountered invalid root element name 'Binary'. 'root' is the only
allowed root element name.


### About JSONP


JSON with Padding is a transport format, which uses the ability of
SCRIPT tag to execute scripts from different domains to overcome the
cross-domain access issue. Generally the returned JSON is wrapped by
JavaScript function which can be executed cross-domain.

So before we start - JSONP support is already added to .NET 4 so the
services can be configured to use JSONP only by adding the
CrossDomainScriptAccessEnabled attribute.


### When the problem occurs

However I am stuck with NET 3.5 - so I needed to provide JSONP manually.
Actually that is not that hard because MS provides this functionality in
the WCF-WF example package ([Downloadable
here](http://msdn.microsoft.com/en-us/library/cc716898(v=vs.90).aspx)).

**The problem is, that this example is not complete. To be more
specific: It works only when the service returns .NET objects which are
serialized to JSON by WCF. However in some cases you might be serving
the JSON which is already prepared. In this case your service returns a
**Stream**. And in this case the example provided by MS will not
work.*

To understand the problem, we have to take a look at what exactly does
the example of MS code. Well to start you can simply look at [this
blog.](http://jasonkelly.net/2009/05/using-jquery-jsonp-for-cross-domain-ajax-with-wcf-services/)

So basically to enable JSONP you just need to add JSONPBehavior
attribute to your service. In fact this behavior uses
JSONPEncoderFactory class, which defines an encoder (JSONPEncoder) which
converts the messages to JSONP. The encoding takes place in the override
WriteMessage method. Let's take a look at the method provided in the MS
example.

``` 
public override ArraySegment&ltbyte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
{
    MemoryStream stream = new MemoryStream();
    StreamWriter sw = new StreamWriter(stream);

    string methodName = null;
    if (message.Properties.ContainsKey(JSONPMessageProperty.Name))
        methodName = ((JSONPMessageProperty)(message.Properties[JSONPMessageProperty.Name])).MethodName;

    if (methodName != null)
    {
        sw.Write(methodName + "( ");
        sw.Flush();
    }
    XmlWriter writer = JsonReaderWriterFactory.CreateJsonWriter(stream);
    message.WriteMessage(writer);
    writer.Flush();
    if (methodName != null)
    {
        sw.Write(" );");
        sw.Flush();
    }

    byte[] messageBytes = stream.GetBuffer();
    int messageLength = (int)stream.Position;
    int totalLength = messageLength + messageOffset;
    byte[] totalBytes = bufferManager.TakeBuffer(totalLength);
    Array.Copy(messageBytes, 0, totalBytes, messageOffset, messageLength);

    ArraySegment&ltbyte> byteArray = new ArraySegment&ltbyte>(totalBytes, messageOffset, messageLength);
    writer.Close();
    return byteArray;
}
```


So what is happening here: the **Message** object contains the object
which returns your method. The WriteMessage method will take this object
and write it to a Stream which is passed to it in argument. In the
method the passed stream is a **JsonWriter**. The problem is that
JsonWriter expects the structure of the message to be object represented
by XML, which it will convert to JSON.

Now you can see that before we are actually writing the content of the
message, we write "methodName(" and after ");". Generally this is the
wrapping by JavaScript function. The result will be something like
"methodName({JSONOBject});".

The resulted Stream is than just converted to byte array.

This works, but the problem is that when you are returning raw JSON, in
other words, that your method returns **Stream**, than you cannot use
JsonWriter, because the Message.WriteMessage will push to the writer XML
of different structure, than it expects.

To be specific the XML will have a form of
&lt;binary&gt;asdqwetasfd&lt;/Binary&gt; and JsonWriter will not be able
to create reasonable Json object.


### Solution

The solution to the problem is following:

-   Check the format of the message (if it Json or Raw Stream)
-   If it is a Raw Stream, than just convert the Stream to array of
    bytes
-   If it is Json, than use the same procedure as before

``` 
public override ArraySegment&ltbyte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
{
    WebContentFormat messageFormat = this.GetMessageContentFormat(message);

    MemoryStream stream = new MemoryStream();
    StreamWriter sw = new StreamWriter(stream);

    string methodName = null;
    if (message.Properties.ContainsKey(JSONPMessageProperty.Name))
        methodName = ((JSONPMessageProperty)(message.Properties[JSONPMessageProperty.Name])).MethodName;

    if (methodName != null)
    {
        sw.Write(methodName + "( ");
        sw.Flush();
    }

    XmlWriter writer = null;
    if (messageFormat == WebContentFormat.Json)
    {
        writer = JsonReaderWriterFactory.CreateJsonWriter(stream);
        message.WriteMessage(writer);
        writer.Flush();
        //writer.Close();
    }
    else if (messageFormat == WebContentFormat.Raw)
    {
        String messageBody = ReadRawBody(ref message);
        sw.Write(messageBody);
        sw.Flush();
    }

    if (methodName != null)
    {
        sw.Write(" );");
        sw.Flush();
    }

    byte[] messageBytes = stream.GetBuffer();
    int messageLength = (int)stream.Position;
    int totalLength = messageLength + messageOffset;
    byte[] totalBytes = bufferManager.TakeBuffer(totalLength);
    Array.Copy(messageBytes, 0, totalBytes, messageOffset, messageLength);

    ArraySegment&ltbyte> byteArray = new ArraySegment&ltbyte>(totalBytes, messageOffset, messageLength);
    stream.Close();
    
    return byteArray;
} 
```


You can see that I am using two additional methods:
**GetMessageContentFormat** and **ReadRawBody**. I did not came up with
these methods, instead I have borrowed them from the [blog of Carlos
Figueira](http://blogs.msdn.com/b/carlosfigueira/archive/2011/04/19/wcf-extensibility-message-inspectors.aspx)
In his blog, he describes how to use these methods when Inspecting
messages. That is not the same scenario, but actually Inspecting
outgoing methods or creating own MessageEncoder are just two ways to
achieve the same thing.
I will add the definitions of the methods here, but the above mentioned
blog post is a great source of information regarding customization of
WCF Service, definitely worth checking.


``` 
private WebContentFormat GetMessageContentFormat(Message message)
            {
                WebContentFormat format = WebContentFormat.Default;
                if (message.Properties.ContainsKey(WebBodyFormatMessageProperty.Name))
                {
                    WebBodyFormatMessageProperty bodyFormat;
                    bodyFormat = (WebBodyFormatMessageProperty)message.Properties[WebBodyFormatMessageProperty.Name];
                    format = bodyFormat.Format;
                }

                return format;
            }

private String ReadRawBody(ref Message message)
            {
                
                XmlDictionaryReader bodyReader = message.GetReaderAtBodyContents();
                
                bodyReader.ReadStartElement("Binary");
                byte[] bodyBytes = bodyReader.ReadContentAsBase64();
                
                string messageBody = Encoding.UTF8.GetString(bodyBytes);

                // Now to recreate the message
                MemoryStream ms = new MemoryStream();
                XmlDictionaryWriter writer = XmlDictionaryWriter.CreateBinaryWriter(ms);
                writer.WriteStartElement("Binary");
                writer.WriteBase64(bodyBytes, 0, bodyBytes.Length);
                writer.WriteEndElement();
                writer.Flush();
                ms.Position = 0;
                XmlDictionaryReader reader = XmlDictionaryReader.CreateBinaryReader(ms, XmlDictionaryReaderQuotas.Max);
                Message newMessage = Message.CreateMessage(reader, int.MaxValue, message.Version);
                newMessage.Properties.CopyProperties(message.Properties);
                message = newMessage;
                //return bodyBytes;
                return messageBody;
            }
```
