--- layout: post title: Owner activity of CorrelationToken date:
'2010-02-08T04:47:00.001-08:00' author: Jan Fajfr tags: - Workflow
modified\_time: '2012-02-14T03:44:39.402-08:00' thumbnail:
http://4.bp.blogspot.com/\_fmvjrARTMYo/S3AIFS2Yv7I/AAAAAAAAADQ/PqRQFHy9hCo/s72-c/WF.png
blogger\_id:
tag:blogger.com,1999:blog-1710034134179566048.post-6537596422169992500
blogger\_orig\_url:
http://hoonzis.blogspot.com/2010/02/owner-activity-of-correlationtoken.html
--- In approval workflow it is quite common to give the approver
possibility to ask for additional information. When I develop sequential
workflow I am modeling it the way this picture shows.\
\
[![](http://4.bp.blogspot.com/_fmvjrARTMYo/S3AIFS2Yv7I/AAAAAAAAADQ/PqRQFHy9hCo/s320/WF.png)](http://4.bp.blogspot.com/_fmvjrARTMYo/S3AIFS2Yv7I/AAAAAAAAADQ/PqRQFHy9hCo/s1600-h/WF.png)\
\
In other words first task "managementApproveTask" is creted and when the
manager ask for additional information then "addManagementInfoTask" is
created. Whene the person adds the information the big WHILE returns and
creates new task for the manager to approve the document - this gives
the manager the possibility to ask for additional information as many
times as he wants and ask different people and so on...\
\
Well then the question is which activity set as the owner of
CorrelationToken of the CreateTask activity and also of OnTaskChanged
activity - because they are always the same.\
\
If you set the "ManagementApproveActivity" or any other activity PARENT
TO THE WHILE loop as the owner of the CorrelationToken, you will get the
following error:\
\
*System.InvalidOperationException: Correlation value on declaration
**"TOKEN NAME"** is already initialized.\
at System.Workflow.Runtime.CorrelationToken.Initialize(Activity
activity, ICollection\`1 propertyValues)\
at
System.Workflow.Activities.CorrelationService.InvalidateCorrelationToken(Activity
activity, Type interfaceType, String methodName, Object\[\]
messageArgs)...*\
\
The problem is as the exception says that the Correlation Token is
already initialized. To resolve this you have to set as Owner activity
of the CorrelationToken **any activity inside the WHILE loop**. Then any
time the while loop is entered the activities inside are recreated, that
means that there will be also new CorrelationToken created for each new
task.
