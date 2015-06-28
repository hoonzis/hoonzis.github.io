--- layout: post title: NHibernate NFluent and custom HiLo generator
date: '2012-01-09T23:44:00.000-08:00' author: Jan Fajfr tags: - ".NET" -
NHibernate - Azure - C\# modified\_time: '2014-06-26T14:41:27.390-07:00'
blogger\_id:
tag:blogger.com,1999:blog-1710034134179566048.post-4289302571095177087
blogger\_orig\_url:
http://hoonzis.blogspot.com/2012/01/nhibernate-nfluent-and-custom-hilo.html
--- Azure SQL is not completely compatible with SQL Server. All the
limitations are described [over
here.](http://msdn.microsoft.com/en-us/library/windowsazure/ee336245.aspx)
One of the limitations is that every table in Azure SQL needs CLUSTERED
INDEX.\
\
If you are using NHibernate & NFluent, than any identity mapping will
create clustered index if it can.\
\
If you want to use HiLo generator to get the ID's, than you need to
configure special table for the generator. To use the generator you can
let NHibernate to create the table.\

``` {.brush:csharp}
Id(x => x.Id).GeneratedBy.HiLo("1000");
```

However this way it will create only one table with one ID. In a typical
scenario you will want to use one table and store all the actual ID's in
a particular row or column for each of the entities in the database.\

``` {.brush:csharp}
Id(x => x.Id).GeneratedBy.HiLo("1000","hiloTable","myentity");
```

This supposes that you have a table called "hiloTable" which contains
"myentity" column.\
\
However you would have to write the script for the table creation, so
you are loosing the possibility to run NHibernate and generate your
database.\
\
The solution which solves this two issues is to create own generator and
base it on HiLo generator.\
Here is the mapping for using own generator\

``` {.prettyprint}
Custom%lt;UniversalHiloGenerator%gt;(
x => x.AddParam("table", "NH_HiLo")
.AddParam("column", "NextHi")
.AddParam("maxLo", "10000")
.AddParam("where", "TableKey='BalancePoint'"));
```

\
When overriding the NHibernate.Id.TableHiLoGenerator we have the option
to override the script which is used for the creation of the table
containing the IDs. This can be achieved by overriding the
SqlCreateStrings method which returns an array of Strings, which are
executed as SQL scripts against the database.\
\

``` {.prettyprint}
public class UniversalHiloGenerator : NHibernate.Id.TableHiLoGenerator
{
public override string[] SqlCreateStrings(NHibernate.Dialect.Dialect dialect)
{
List commands = new List();
var dialectName = dialect.ToString();

if(dialectName != "NHibernate.Dialect.SQLiteDialect")
commands.Add("IF OBJECT_ID('dbo.NH_HiLo', 'U') IS NOT NULL \n DROP TABLE dbo.NH_HiLo; \nGO");

commands.Add("CREATE TABLE NH_HiLo (TableKey varchar(50), NextHi int)");

if (dialectName != "NHibernate.Dialect.SQLiteDialect")
commands.Add("CREATE CLUSTERED INDEX NH_HiLoIndex ON NH_HiLo (TableKey)");

string[] tables = {"Operation","Account"};

var returnArray = commands.Concat(GetInserts(tables)).ToArray();
return returnArray;
}

private IEnumerable GetInserts(string[] tables)
{
foreach (var table in tables)
{
yield return String.Format("insert into NH_HiLo values ('{0}',1)", table);
}
}
}
```

\
This code is quite simple. The sql scripts create the table for storing
the ID's for all the entities in the database. In this particular case,
in each row of the HiLo table there are two columns, one specifying the
name of the table for which the ID is stored and in the second column is
the ID.\
\
The code also checks the dialect of the database. This way it can create
an CLUSTERED index on the table (which will run fine for SQL server and
Azure SQL and is REQUIRED for AZURE) and will skip the creation of the
index SQL Lite, where clustered indexes do not exists.\
\
In the example above two table entities are envisaged: Operations and
Accounts in separate tables.\
\
This way several issues are solved:\

-   The schema of the database can be created automatically by
    NHibernate
-   The HiLo table is created for each entity. To add an entity you can
    simply just add the name of the entity into the list of tables.
-   Clustered index is created on the entity in the case that the script
    is not run against SQL lite.

