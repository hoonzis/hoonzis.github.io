--- layout: post title: 'Office 2007 Crash - wwlib.dll - error code:
c0000005' date: '2010-04-19T01:55:00.001-07:00' author: Jan Fajfr tags:
- Office modified\_time: '2012-02-14T03:43:16.720-08:00' blogger\_id:
tag:blogger.com,1999:blog-1710034134179566048.post-1495426470267099924
blogger\_orig\_url:
http://hoonzis.blogspot.com/2010/04/office-2007-crash-wwlibdll-error-code.html
--- Recently I had to resolve this issue:\
\
Office 2007 Crash\
wwlib.dll\
code error: c0000005\
OS: Windows 7\
\
It started with Word and then with Outlook. I found the solution here:\
\
<http://www.edbott.com/weblog/?p=1771>\
KB Article:
<http://support.microsoft.com/default.aspx?scid=kb;en-us;940791>\
\
If you are lazy to read, the solution is removing this registry entry:\
**HKEY\_CURRENT\_USER\\Software\\Microsoft\\Office\\12.0\\Word\\Data**
