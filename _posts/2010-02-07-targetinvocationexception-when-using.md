---
layout: post
title: CreateTaskWithContentType activity
date: '2010-02-07T05:17:00.000-08:00'
author: Jan Fajfr
tags:
- SharePoint
- ASP.NET
- Workflow
modified_time: '2010-02-07T06:12:51.198-08:00'
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-9208815226182117591
blogger_orig_url: http://hoonzis.blogspot.com/2010/02/targetinvocationexception-when-using.html
---

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

###Programatically allow and add ContentTypes on SharePoint list

Here is a method which I am using to programatically allow and add ContentTypes on SharePoint task list, which accepts String representing the GUID of the ContentType and SPList representing the task list.I've found the code in this method again in part 4 of [this great series](http://rshelton.com/archive/2007/10/05/free-workshop-sharepoint-document-workflow-for-developers)

```
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