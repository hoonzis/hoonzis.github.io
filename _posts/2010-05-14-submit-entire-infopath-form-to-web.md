---
layout: post
title: Submit entire InfoPath form to web service
date: '2010-05-14T04:31:00.001-07:00'
author: Jan Fajfr
tags:
- InfoPath
modified_time: '2013-03-25T08:45:39.650-07:00'
thumbnail: http://3.bp.blogspot.com/_fmvjrARTMYo/S_JCNvVoQTI/AAAAAAAAAE4/QH0Mh4JSTmA/s72-c/table.PNG
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-2292374334378652450
blogger_orig_url: http://hoonzis.blogspot.com/2010/05/submit-entire-infopath-form-to-web.html
---
This post explains on a simple example how to submit the entire XML of
the InfoPath form to a Web Service and process it.

During my last assignment, I needed to submit the content of Repeating
Table of InfoPath form to Access Database.
The form had to be published on SharePoint Site - so I could not submit
directly to the database, but I was forced to use the web service
(because web enabled forms do not allow access directly to the
database)

So I build a web service method which allowed me to submit one row of
the Repeating Table to the database and I thought that I would be able
to submit each row in some kind of a loop inside the InfoPath form. Well
it did not work, I was just able to submit the first row all the time
and nothing more.

So I decided to submit "**the entire form**" and than parse it on the
web service side. Here is a short example of how to do this.

### Preparing the InfoPath form
First let's create simple InfoPath form with one repeating table (eg. list of products)

[![](http://3.bp.blogspot.com/_fmvjrARTMYo/S_JCNvVoQTI/AAAAAAAAAE4/QH0Mh4JSTmA/s320/table.PNG)](http://3.bp.blogspot.com/_fmvjrARTMYo/S_JCNvVoQTI/AAAAAAAAAE4/QH0Mh4JSTmA/s1600/table.PNG)

Now what you have to do is rename the fields in the "Data sources" tab.
Later you will use these names of the fields to get the values stored
XML representing the InfoPath form.

[![](http://4.bp.blogspot.com/_fmvjrARTMYo/S_JCoBU3YII/AAAAAAAAAFA/21llrJgBHCs/s320/data_sources.PNG)](http://4.bp.blogspot.com/_fmvjrARTMYo/S_JCoBU3YII/AAAAAAAAAFA/21llrJgBHCs/s1600/data_sources.PNG)

### Creating the Web Service
In the web service you will need a method which takes **XmlDocument** and will parse this XML document representing the InfoPath form and store it's values to the database.


```csharp
[WebMethod]
public void SubmitDocument(XmlDocument doc)
{           
  XmlNamespaceManager nsManager = new XmlNamespaceManager(doc.NameTable);
  nsManager .AddNamespace(formNamespace, formURI);
  nsManager .AddNamespace("dfs",
  "http://schemas.microsoft.com/office/infopath/2003/dataFormSolution");

  XmlNode root = doc.DocumentElement;
  XmlNodeList list = root.SelectNodes("/dfs:IPDocument/my:myFields/my:prodList/my:product", nsManager);

  foreach (XmlNode node in list)
  {
    string name = node.SelectSingleNode("/dfs:IPDocument/my:myFields/my:prodList/my:product/my:name", nsManager).InnerText;
    string price = node.SelectSingleNode("/dfs:IPDocument/my:myFields/my:prodList/my:product/my:price", nsManager).InnerText;
    string amount = node.SelectSingleNode("/dfs:IPDocument/my:myFields/my:prodList/my:product/my:amount", nsManager).InnerText;

    SubmitToDataBase(name,price, amount);
  }
}
```

In this method first we initialize the **XMLNamespaceManager**. To this
manager we will have to add 2 namespace. First one is the namespace of
the data source of the InfoPath document. This one we can find out in
InfoPath client by navigating to **Properties -&gt; Details**.

Now when InfoPath submits the form to the Web Service, it adds another
namespace to the document marked as: **dfs** with the url
**http://schemas.microsoft.com/office/infopath/2003/dataFormSolution"**
and you need to add the namespace to your namespace manager.


```csharp
nsManager .AddNamespace("dfs", "http://schemas.microsoft.com/office/infopath/2003/dataFormSolution");
```

Then to get value of certain field in the **XmlDocument** we need to know the **XPath** which leads directly to the desired **XmlNode**. We can get the XPath by the **Copy XPath** option, which can be found in the context menu of desired field in the "Data Sources" tab in your InfoPath client. For example to get the "amount" XmlNode and later the string representing a number inside this node we can use the following code:

```csharp
XmlNode nAmount = node.SelectSingleNode("/dfs:IPDocument/my:myFields/my:prodList/my:product/my:amount", nsManager);
int amount = Convert.ToInt32(nAmount.InnerText);
```

Now in the following part I will just show a simple example of method which would store some data to the Access database.

### Preparing the Access database connection
To connect to Access database you can use the OLE DB Provider, exactly
the .NET Framework OLE DB provider, in the namespace
**System.Data.OleDB**. First you need to specify the connection string
to your database. Because we are using web service it is good idea to
store it in the **Web.config** file.

Later, already in the code of your Web Service you can prepare yourself
a property which will provide you this connection string (just not to
write too long lines of code).

```csharp
public String ConStr
{
  get { return ConfigurationManager.ConnectionStrings["myDB"].ConnectionString; }
}
```
The following is a simple implementation of a method which stores the
data in the database. You can made this method part of your Web Service
directly or build yourself some data access class.

```csharp
public void SubmitToDataBase(String name, String price, String amount)
{
  OleDbConnection con = new OleDbConnection(ConStr);

  String cmd = "INSERT INTO products(name,price,amount)values(?,?,?)";
  OleDbCommand command = new OleDbCommand(cmd, con);

  OleDbParameter pName = new OleDbParameter();
  pName.Value = name;
  command.Parameters.Add(pName);

  OleDbParameter pPrice = new OleDbParameter();
  pPrice.Value = Convert.ToInt32(price);
  command.Parameters.Add(pName);

  OleDbParameter pAmount = new OleDbParameter();
  pAmount.Value = Convert.ToInt32(amount);
  command.Parameters.Add(pAmount);

  con.Open();
  command.ExecuteNonQuery();
  con.Close();
}
```

There is nothing too interesting here if you are familiar with some
other ADO classes. Just notice that I am using parametrized queries. The
SQL command contains question marks, which are later when the actual
**OleDbCommand** substituted with provided parameters.
That is all about the Web Service now you need to go back and configure
the InfoPath form to connect to the Web Service.

### Connecting the InfoPath form to the Web Service
OK now lets go back to the InfoPath form design. To submit the document to this web service you will have to **add new data source** select **submit data -&gt; to Web Service**. Than localize you web service and find the method that, you just created and than finally in the Data Connection Wizard select submit **Entire form**.

[![](http://4.bp.blogspot.com/_fmvjrARTMYo/S_JI_j9RQpI/AAAAAAAAAFI/rzD2R2B0AU8/s320/new_data_connection.PNG)](http://4.bp.blogspot.com/_fmvjrARTMYo/S_JI_j9RQpI/AAAAAAAAAFI/rzD2R2B0AU8/s1600/new_data_connection.PNG)

Now just to give you a complete idea, here is the XML which is submitted to the web service. However if the document is saved as XML later (eg. in the SharePoint document library), the **dfs** namespace is not presented.

```xml
<dfs:IPDocument xmlns:dfs="http://schemas.microsoft.com/office/infopath/2003/dataFormSolution"><?mso-infoPathSolution solutionVersion="1.0.0.4" productVersion="12.0.0" PIVersion="1.0.0.0" href="file:///C:\Users\fajfr\AppData\Local\Microsoft\InfoPath\Designer2\23fae1f325544a92\manifest.xsf" ?><?mso-application progid="InfoPath.Document" versionProgid="InfoPath.Document.2"?>
<my:myFields xmlns:my="http://schemas.microsoft.com/office/infopath/2003/myXSD/2010-05-18T07:21:28" xml:lang="en-us">
<my:prodList>
<my:product>
<my:name />
<my:price xsi:nil="true" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" />
<my:amout xsi:nil="true" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" />
</my:product>
</my:prodList>
</my:myFields>
</dfs:IPDocument>
```
