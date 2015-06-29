---
layout: post
title: JavaScript asynchronously uploading files
date: '2012-02-18T07:21:00.002-08:00'
author: Jan Fajfr
tags:
- JavaScript
- HTML
modified_time: '2014-06-26T14:39:36.304-07:00'
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-7822574463715932634
blogger_orig_url: http://hoonzis.blogspot.com/2012/02/javascript-asynchronously-uploading.html
---
At the beginning I have thought, that it has to be easy; just make a
POST to the server using jQuery and the only question is how to get the
data. Well I have found out that it is not that easy and googling around
I have found there are quite a lot of pre-build components and plugins,
which makes it quite difficult to decide for one of those.

Why it is not possible to use simple JavaScript POST?
-----------------------------------------------------

Because of the security restrictions. The browser is not allowed to post
the file content asynchronously. This is however about to change thanks
to HTML 5.


Workarounds
-----------

-   HTML 5 - has a support for file uploading. Use the [File
    API](http://dev.w3.org/2006/webapi/FileAPI/). Follow [this
    how to.]() This does not work in current versions of IE (7,8,9).
-   Create a hidden iFrame on the page and redirect the return of the
    post to this iFrame
-   Use Flash, Silverlight, or Java applet
-   Use some component, or jQuery plugin, which usually makes use of the
    preceding ones (usually the iFrame hack)



jQuery plugins
--------------

There are quite few of those:

-   [Plupload](http://www.plupload.com/example_queuewidget.php)
-   [JqUploader](http://valums.com/ajax-upload/)
-   [Uploadify](http://www.uploadify.com/demos/)
-   [jQuery File Upload](http://blueimp.github.com/jQuery-File-Upload/)
-   [jQuery Form Plugin](http://malsup.com/jquery/form/#file-upload)


I have tested [jQuery File
Upload.](http://blueimp.github.com/jQuery-File-Upload/) Which is cool,
comes with nice GUI but at the time of writing this, I have found it
little hard to customize. Actually I have struggled to use a simple
form, which would upload just one file, instead of the predefined GUI
with it's behavior.

The second one that I have tested is [jQuery Form
Plugin](http://malsup.com/jquery/form/#file-upload) which contrary to
the previous one, is simple to use in a one file upload scenario.
However it does not provide the nice UI, ready for multiple files upload
etc...


Using jQuery Form Plugin in ASP.NET
-----------------------------------

### Client side

On the client side you need jQuery and the Plugin js file. Then with one
jQuery call you can set up the form, to use the plugin.

``` 
<form id="uploadForm" action="Upload.ashx" method="POST" enctype="multipart/form-data">
  <input type="hidden" name="MAX_FILE_SIZE" value="100000" />
  File:
  <input type="file" name="file" />
  <input type="submit" value="Submit" />
</form>
```

``` 
$('#uploadForm').ajaxForm({
    beforeSubmit: function (a, f, o) {
        o.dataType = 'html'
    },
    success: function (data) {
        alert('upload OK:' + data);
    }
});
```

The **dataType** property which is set to **'html**, specifies to Form
Plugin what kind of response should it expect. To check the other
options see the documentation.

You can see, that the form action is set to "Upload.ashx". This is the
server side script, being a Http Handler (in case of ASP.NET
application). It could also probably be a WCF service - but let's keep
it simple when we can.

### Server side

On the server side you have to define a Http Handler which will take
care of the upload functionality.

``` 
public class Upload : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        System.Web.HttpPostedFile file = context.Request.Files[0];
        String filePath = "Uploads" + "\\" + RandomString(10) + "." + extension;
        string linkFile = System.Web.HttpContext.Current.Server.MapPath("~") + filePath;

        file.SaveAs(linkFile);
        context.Response.StatusCode = 200;
        context.Response.Write(filePath);
    }
}
```

And that's it. The handler will save the file and send back the address
of the file.
