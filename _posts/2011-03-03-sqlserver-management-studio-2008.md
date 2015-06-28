--- layout: post title: SqlServer Management Studio 2008 standalone
date: '2011-03-03T06:36:00.000-08:00' author: Jan Fajfr tags: - SQL
Server modified\_time: '2011-03-03T06:38:10.541-08:00' blogger\_id:
tag:blogger.com,1999:blog-1710034134179566048.post-932161318188364212
blogger\_orig\_url:
http://hoonzis.blogspot.com/2011/03/sqlserver-management-studio-2008.html
--- It is quite difficult and confusing process to install a standalone
SQL Server Management Studio 2008.\
After you [download the setup
file](http://www.microsoft.com/downloads/en/details.aspx?FamilyID=08e52ac2-1d62-45f6-9a4a-4b76a8564a2b&DisplayLang=en),
you will see, that it is a 170mb installation package and when you run
the installation you will be provided with a full SQL Server Express
Intaller.\
\
So how to go on:\
\
-&gt; SQLServer Installation Center\
-&gt; Select: **New SQL Server stand-alone installation or add features
to an existing installation**\
-&gt; Setup Support Rules - **OK**\
!!! Add features to an existing instanace of SQL Server 2008 - this
would be WRONG choice !!!\
-&gt; Select: **Perfom a new installation of SQL Server 2008**\
-&gt; Select in the Shared Features: **Management Tools - Basic**\
\
So even if you have already SQL Server Express intalled you can not add
the Management Studio to the features. You have to pretend that you are
installing a new copy of SQL Server!\
\
Topic is covered on [this
blog](http://www.asql.biz/Articoli/SQLX08/Art3_1.aspx#SSMSE)
