---
layout: post
title: SharePoint Workflow - AlterTask method problem
date: '2009-12-28T05:25:00.000-08:00'
author: Jan Fajfr
tags:
- SharePoint
- Workflow
modified_time: '2010-02-07T05:11:43.965-08:00'
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-1721662806542422668
blogger_orig_url: http://hoonzis.blogspot.com/2009/12/sharepoint-altertask-method-problem.html
---
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

``` 
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