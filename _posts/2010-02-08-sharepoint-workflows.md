---
layout: post
title: Sharepoint, workflows, infopath and headaches
date: '2010-02-08T04:47:00.001-08:00'
author: Jan Fajfr
tags:
- Workflow
modified_time: '2012-02-14T03:44:39.402-08:00'
---
My first partial job was in a company that was heavily rallying on SharePoint. My task was to write custom workflows using the **Workflow Foundation** and ASP.NET. I was a very bad programmer than and there was absolutely no development culture in the place that I worked. I have started to blog at the same time about my daily fights with SharePoint. I have merged the old posts into single one and I leave them up here for two reasons. If any pour programmer works with these technologies, he might find a fix here and less suffering. And I also like to look back at what I did as beginner and how I have evolved.

### Owner activity of correlation token
In approval workflow it is quite common to give the approver possibility to ask for additional information. When I develop sequential workflow I am modeling it the way this picture shows.

[![](http://4.bp.blogspot.com/_fmvjrARTMYo/S3AIFS2Yv7I/AAAAAAAAADQ/PqRQFHy9hCo/s320/WF.png)](http://4.bp.blogspot.com/_fmvjrARTMYo/S3AIFS2Yv7I/AAAAAAAAADQ/PqRQFHy9hCo/s1600-h/WF.png)

In other words first task "managementApproveTask" is creted and when the
manager ask for additional information then "addManagementInfoTask" is
created. When the person adds the information the big WHILE returns and
creates new task for the manager to approve the document - this gives
the manager the possibility to ask for additional information as many
times as he wants and ask different people and so on...

Well then the question is which activity set as the owner of
CorrelationToken of the CreateTask activity and also of OnTaskChanged
activity - because they are always the same.

If you set the "ManagementApproveActivity" or any other activity PARENT
TO THE WHILE loop as the owner of the CorrelationToken, you will get the
following error:

*System.InvalidOperationException: Correlation value on declaration
**"TOKEN NAME"** is already initialized.
at System.Workflow.Runtime.CorrelationToken.Initialize(Activity
activity, ICollection\`1 propertyValues)
at
System.Workflow.Activities.CorrelationService.InvalidateCorrelationToken(Activity
activity, Type interfaceType, String methodName, Object\[\]
messageArgs)...

The problem is as the exception says that the Correlation Token is
already initialized. To resolve this you have to set as Owner activity
of the CorrelationToken **any activity inside the WHILE loop**. Then any
time the while loop is entered the activities inside are recreated, that
means that there will be also new CorrelationToken created for each new
task.

### Alter task method not firing

During the development of a simple sequential workflow in SharePoint I came across "OnTaskChanged not firing" issue. I had this problem already couple times, so I tried to check the usual causes of this problem:

Usually in sequential workflow you have a CreateTask or CreateTaskWithContentType activity, which is followed by OnTaskChanged activity in the loop.

Correlation token, TaskId, TaskProperties of these two activities have to match - so your
task can be "wired" together with OnTaskChanged activity.

The last time I was having this issue was that one of my assemblies in GAC was not up to date. In that time I found the following in the logs:

```
System.TypeLoadException: Could not load type
'BaseClassesLibrary.Library.MailLibrary' from assembly
'BaseClassesLibrary, Version=1.0.0.0, Culture=neutral,
PublicKeyToken=cfc77ab1c27103f4'. at
RequestWorkflowASP.ITApproveActivity.ITApproveTask\_MethodInvoking(Object
sender, ExternalDataEventArgs e) at
System.Workflow.ComponentModel.Activity.RaiseGenericEvent\[T\](DependencyProperty
dependencyEvent, Object sender, T e) at
System.Workflow.Activities.HandleExternalEventActivity.RaiseEvent(Object\[\]
args) at
System.Workflow.Activities.HandleExternalEventActivity.System.Workflow.ComponentModel.IActivityEventListener.OnEvent(Object
sender, QueueEventArgs e) at
System.Workflow.ComponentModel.ActivityExecutorDelegateInfo\`1.ActivityExec...
```

So I was searching for something similar(missing assemblies), but after hours of digging I found the solution. The task is displayed as ASP page. In the code-behind of this page I change the SharePoint task item (because task is a simple **SPListItem**) by calling
the SPWorkflowTask.AlterTask(...) method and this method takes a *Hashtable* as parameter.

```csharp
//Create the hashtable which will be populated to ExtendedProperties of task AfterProperties
var taskHash = new Hashtable();

//Fill the values of the HashTable
taskHash["hwDescription"] = txbHWDescription.Text;
taskHash["swDescription"] = txbSWDescription.Text;
taskHash["částka"] = txbPrice.Text;

//Call the AlterTask method to populate AfterProperties
SPWorkflowTask.AlterTask(_spListItem, taskHash, true);
```

The problem was of course in the line: **taskHash["částka"]** where I have used
some Czech alphabet characters to specify the property. SharePoint couldn't alter the task correctly, so my OnTaskChanged event didn't fire.

I try to avoid using the Czech alphabet in my code and this was just
"temporary" solution, which somehow stayed there. If I would keep some
coding best practices and never left temporary solutions, I could have
saved myself couple hours of time :).

Strange is the fact, that I didn't find any exceptions in the SharePoint log. Just the OnTaskChanged was not firing.

#### Disposing Sharepoint in wrong moment

Using **SPControl.GetContextWeb(this.Context)** and disposing the context can cause exceptions. In one of my WebParts I was using this little piece of code to get the current user from SharePoint.

```csharp
String name, email;

//get the current SPWeb
//THE PROBLEM: You should not dispose after calling GetContextWeb Method
using (SPWeb lWeb = SPControl.GetContextWeb(this.Context))
{
  var lUser = lWeb.CurrentUser;
  name = lUser.Name;
  email = lUser.Email;
}
```

After executing this I've got the following exception:

```
(Exception from HRESULT: 0x80020009 (DISP\_E\_EXCEPTION))
Description: An unhandled exception occurred during the execution of the
current web request. Please review the stack trace for more information
about the error and where it originated in the code.
Exception Details:

Microsoft.SharePoint.WebPartPages.WebPartPageUserException: Došlo k
výjimce. (Exception from HRESULT: 0x80020009 (DISP\_E\_EXCEPTION))
Source Error:
An unhandled exception was generated during the execution of the current
web request. Information regarding the origin and location of the
exception can be identified using the exception stack trace below.
```

I have googled a bit and found the solution:

```csharp
//get the current user
SPUser lUser = SPContext.Current.Web.CurrentUser;
//work with it as you need
name = lUser.Name;
email = lUser.Email;
```

In the [MSDN Best Practices](http://msdn.microsoft.com/en-us/library/aa973248.aspx) you
can find, that when using the **SPControl.GetContextWeb()** or **SPControl.GetContextSite()** method you should not dispose the created context until done completely.


### Enabling WebPage ContentType
If you want to use ASP.NET pages with your workflow in SharePoint you need to follow these steps:

- Create the web page
- Create ContentType containing your WebPage
- Add the ContentType to the ContentTypes gallery in SharePoint site
- Allow ContentTypes on the SharePoint task list, where the task of your workflow will be stored
- Add the ContentType to the list where your workflow tasks will be stored
- Finally you can use CreteTaskWithContentType activity and specify the GUID of the ContentType which you have created
- For more details of how to develop SharePoint workflows with ASP.NET pages refer to [this blog](http://rshelton.com/archive/2007/10/05/free-workshop-sharepoint-document-workflow-for-developers---part-1.aspx)

If you do not add the ContentType to the gallery or if you forgot to add it to the SharePoint list, you can receive the following exception:

```
System.Reflection.TargetInvocationException: Exception has been thrown by the target of an invocation. ---> System.NullReferenceException: Object reference not set to an instance of an object. at Microsoft.SharePoint.Workflow.SPWinOETaskService.CreateTaskWithContentTypeInternal(Guid taskId, SPWorkflowTaskProperties properties, Boolean useDefaultContentType, SPContentTypeId ctid, HybridDictionary specialPermissions) at Microsoft.SharePoint.Workflow.SPWinOETaskService.CreateTaskWithContentType(Guid taskId, SPWorkflowTaskProperties properties, String taskContentTypeId, HybridDictionary specialPermissions)
```

The best way to add the ContentType to the ContentTypes library is to create Feature containing all your ContentTypes and install and activate the feature on SharePoint. On details pleas follow the part 4 of the above mention blog how-to.

#### Programaticaly allow and add ContentTypes on SharePoint list

Here is a method which I am using to programatically allow and add ContentTypes on SharePoint task list, which accepts String representing the GUID of the ContentType and SPList representing the task list.I've found the code in this method again in part 4 of [this great series](http://rshelton.com/archive/2007/10/05/free-workshop-sharepoint-document-workflow-for-developers)

```csharp
public static void AllowContentType(SPList lTaskList, String lTaskContentType)
{
  //make sure that the content types are enabled on the task list
  if (lTaskList.ContentTypesEnabled != true)
  {
	//if not allow them. This can be done also in the SharePoint GUI
	lTaskList.ContentTypesEnabled = true;
  }

  // convert given String representing the GUID to SPContentTypeId
  SPContentTypeId myContentTypeID = new SPContentTypeId(lTaskContentType);

  // Find the content type in SharePoint site content types library
  SPContentType myContentType = lTaskList.ParentWeb.Site.RootWeb.ContentTypes[myContentTypeID];

  //if the content type was found
  if (myContentType != null)
  {

	// Check if the content type was already added to the SharePoint task list
	bool contenTypeExists = false;
	foreach (SPContentType contentType in lTaskList.ContentTypes)
	{
	  if (contentType.Name == myContentType.Name)
	  {
		// This content type was already added
		contenTypeExists = true;
		break;
	  }
	}

	// If I didn't find this content type in the list I will have to add it
	if (contenTypeExists != true)
	{
	  lTaskList.ContentTypes.Add(myContentType);
	}
  }
  else
  {
	throw new Exception("Content type is not register with the web site");
  }
}
```

### WSS Tracing service and *SPTraceView*
Sometimes in the SharePoint LOG, you can get the following information and nothing else:

*Service lost trace events.*

This happens when the Windows SharePoint Tracing Service is halted or
not working. Usually you can just restart the service, but sometimes it
won't work. The you can try to run TaskManager and end the process
called "wsstracing.exe".

If that is not going to work you can restart the Internet Information
Server or the whole Server.

But sometimes it is not possible - in the production when you need the information of some actual error.

In this situation I use great tool called **[SPTraceView](http://sptraceview.codeplex.com/)**. This tool will
attach itself as online listener to the SharePoint ULS and will capture
the errors and messages for you. You can just run it and there is no
need to restart the server or IIS.

### InfoPath & SharePoint - PathTooLongException
When opening a InfoPath form in SharePoint, you may get into he ULS log
the following exception:

```
Unhandled exception when rendering form System.IO.PathTooLongException:
The specified file or folder name is too long. The URL path for all
files and folders must be 260 characters or less (and no more than 128
characters for any single file or folder name in the URL). Please type a
shorter file or folder name.
```

Some general info about the exception can be found
[here](http://support.microsoft.com/kb/894630/en-us).

It is caused by the **MSO-INFOPATHSOLUTION** directive in the head of
the xml definition of the form. The attribute **HREF** of this directive
can not exceed the size of 260 characters.

Our InfoPath forms where designed as InfoPath 2003 forms, and the
**HREF** attribute was really long:


```
<mso-infoPathSolution solutionVersion="1.0.0.414" productVersion="12.0.0" PIVersion="1.0.0.0" href="http://intranet/teamwebs/informatics/EvidencepoadavkWF/Forms/template.xsn?SaveLocation=http://intranet/teamwebs/informatics/EvidencepoadavkWF/&amp;Source=http://intranet/teamwebs/informatics/Evidence%2520poadavk%2520WF/Forms/Nedokonen%2520poadavky.aspx&amp;OpenIn=Browser&amp;NoRedirect=true&amp;XsnLocation=http://intranet/teamwebs/informatics/EvidencepoadavkWF/Forms/template.xsn" name="urn:schemas-microsoft-com:office:infopath:Evidence-ponadavk--WF:-myXSD-2008-10-03T13-14-12" ?>
```

but if you strip it down, you can see that a simple link to the file is enough.

```
href="http://intranet/teamwebs/informatics/EvidencepoadavkWF/Forms/template.xsn"
```

The forms designed in InfoPath 2007 do not generate the href attribute
that long. Also note if you just want the forms to open in the client
application(InfoPath), you can set this on the advanced settings of the
Document Library, in which the forms are contained. But even there, if
there would be link to the file outside of this library, or if you will
access the direct link, SharePoint will always try to open it in Web
Browser.

This forum post had led me to the solution:
http://www.k2underground.com/forums/p/8709/26302.aspx\#26302

### SharePoint && InfoPath - Error while processing the form and schema validation
Lately I came across these 2 following errors:

**Error 1 - There has been an error while processing the form. There is
an unclosed literal string**

If you will come across this issue try one of following solutions(or
combination):

1) You have some secondary data sources in the form which are not used.
If you need them there (maybe they get use only on click of button...)
than just drag and drop this data source to form and hide it.

2) Check the names of secondary data sources for special characters

3) There is a KB fix which might help:

http://support.microsoft.com/kb/949752

**Error 2 - Schema validation found non-data type errors**

There is a solution from MS for this issue, when you are editing
InfoPath form CODE:

http://blogs.msdn.com/infopath/archive/2006/11/28/the-xsi-nil-attribute.aspx

However in my case helped just setting the **PreserveWhiteSpaces**
property to TRUE on XmlDocument:

```csharp
XmlDocument lDoc = new XmlDocument();
...
lDoc.PreserveWhitespace = true;
lDoc.Save(myOutStream);
```
