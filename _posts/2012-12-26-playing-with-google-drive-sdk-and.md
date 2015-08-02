---
layout: post
title: Playing with Google Drive SDK and JavaScript
date: '2012-12-26T15:17:00.001-08:00'
author: Jan Fajfr
tags:
- JavaScript
modified_time: '2014-06-26T14:12:08.280-07:00'
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-2514104088306056042
blogger_orig_url: http://hoonzis.blogspot.com/2012/12/playing-with-google-drive-sdk-and.html
---
I am just starting to use the Google Drive SDK for one of my personal
projects. The front-end of the application is written entirely in
JavaScript. I am in the process of integrating Google Drive and it took
me some time to get throw the API reference and get it to work, here are
some useful code snippets. I have started with the [JavaScript
Quickstart available at the google sdk
page](https://developers.google.com/drive/quickstart-js) and I have
added couple useful methods:

The Google Drive API is a standard RESTful API. You can access the
functionalities only by issuing HTTP requests, so you do not need any
special SDK. However the requests have to be signed. OAuth protocol is
used to secure the communication. Google provides a SDK for many
languages, JavaScript being one of them. Using this SDK facilitates the
creation of HTTP requests. The API provides a good compromise between
the simplicity and flexibility.

**The OAuth handshake**

Every request has to be signed using OAuth token. The application has to
first perform the OAuth handshake to obtain the token. JavaScript SDK
provides **gapi.auth.authorize** function which can be used. This
function takes the necessary  parameters (OAuth client ID and the scope)
and also the callback which will be executed when the handshake is over.

```javascript 
function checkAuth() {
 gapi.auth.authorize(
  {'client_id': CLIENT_ID, 'scope': SCOPES, 'immediate': true},
  handleAuthResult);
}

function handleAuthResult(authResult) {
 if (authResult && !authResult.error) {
  //Auth OK
 }
}
```

Once the client is authenticated, the SDK stores the token internally
and adds it to any new request, created before the web page is closed.

**Composing the Google Drive requests**

Any simple request can be created with **gapi.client.request** function.
The SDK will create a HTTP request using the supplied information. The
method takes in JavaScript object. I have found that I am using mostly 4
fields in this object:

-   path – the url of the request
-   method – http method of the request (get/post/put/delete)
-   params – any information passed here will be added to the request as
    URL parameter.
-   headers – any information passed here will be added to the header of
    the HTTP request.
-   body – the body of the request. Usually posted JSON.

**Getting first 10 items from the drive**

```javascript
function getItems() {
 var request = gapi.client.request({
  'path': 'drive/v2/files',
  'method': 'GET',
  'params': {
   'maxResults': '10'
  }
 });
 request.execute(listItems);
}

function listItems(resp) {
 var result = resp.items;
 var i = 0;
 for (i = 0; i < result.length; i++) {
  console.log(result[i].title);
 }
}
```

**Creating a folder*

```javascript
function createFolder(folderName) {
 var request = gapi.client.request({
  'path': 'drive/v2/files',
  'method': 'POST',
  'body': {
   'title': folderName,
   'mimeType': 'application/vnd.google-apps.folder'
  }
 });
 request.execute(printout);
}

function printout(result) {
 console.log(result);
}
```

In this request, nothing is passed as parameter in the URL. Instead of
that JSON object containing two fields (title and mimeType) is passed in
the body of the request.

**Searching for folders**

```javascript
function getAllFolders(folderName) {
 var request = gapi.client.request({
  'path': 'drive/v2/files',
  'method': 'GET',
  'params': {
   'maxResults': '20',
   'q':"mimeType = 'application/vnd.google-apps.folder' and title contains '" + folderName + "'"
  }
 });

 request.execute(listItems);
}
```

You can get more information about searching the google drive
[here](https://developers.google.com/drive/search-parameters).
