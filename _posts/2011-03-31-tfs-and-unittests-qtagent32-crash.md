--- layout: post title: TFS and UnitTests - QTagent32 crash &
"data.coverage' because it is being used by another process" date:
'2011-03-31T01:26:00.000-07:00' author: Jan Fajfr tags: - Testing -
Visual Studio - Source Control modified\_time:
'2012-10-27T06:42:00.033-07:00' blogger\_id:
tag:blogger.com,1999:blog-1710034134179566048.post-1550920881194426278
blogger\_orig\_url:
http://hoonzis.blogspot.com/2011/03/tfs-and-unittests-qtagent32-crash.html
--- I made a bug in my code which resulted in never-ending recursion
which was produced only while UnitTesting.\
\
When there is recursion in unit test than the **QTAgent32 crashes down**
(this is described
[here](http://connect.microsoft.com/VisualStudio/feedback/details/465633/qtagent32-exe-crashes-when-running-a-unit-test-that-calls-itself)).\
\
But if you commit to TFS than and this commits launches the UnitTests
than these will never finish (so if you do not check than you might just
have builds running for couple days and new builds in the queue
behind).\
\
However when you cancel the build and run a new one, this one will fail,
saying:\
\
**'data.coverage' because it is being used by another process.**\
\
So there is some process which has locked the "data.coverage".\
To solve this issue log on the TFS and kill the process VSPerfMon.exe
which runs under NT AUTHORITY\\NETWORK SERVICE (that should be the
account which is running your builds & unit tests on TFS).
