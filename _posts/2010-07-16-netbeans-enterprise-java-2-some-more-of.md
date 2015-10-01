---
layout: post
title: J2EE - NetBeans, JSF, Persistence API - 2nd Part
date: '2010-07-16T06:01:00.001-07:00'
author: Jan Fajfr
tags:
- Java
- J2EE
modified_time: '2014-06-27T05:28:06.643-07:00'
thumbnail: http://4.bp.blogspot.com/_fmvjrARTMYo/TD9UvVLJnfI/AAAAAAAAAGw/aXkxFg01ZAI/s72-c/class_diagram.PNG
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-3792458602013198883
blogger_orig_url: http://hoonzis.blogspot.com/2010/07/netbeans-enterprise-java-2-some-more-of.html
---
This is part two from old series about J2EE applications using Java Beans.

- [Part 1](http://www.hoonzis.com/enterprise-java-netbeans/)
- [Part 2](http://www.hoonzis.com/netbeans-enterprise-java-2-some-more-of/)
- [Part 3](http://www.hoonzis.com/j2ee-netbeans-jsf-persistence-api-part/)

In the first part I described how to develop a simple page which shows
all the "companies" stored in the company table. Now I would like to
describe how to add some additional functions - create new company, edit
company's details. 

### Class diagram and actual state

[![](http://4.bp.blogspot.com/_fmvjrARTMYo/TD9UvVLJnfI/AAAAAAAAAGw/aXkxFg01ZAI/s320/class_diagram.PNG)](http://4.bp.blogspot.com/_fmvjrARTMYo/TD9UvVLJnfI/AAAAAAAAAGw/aXkxFg01ZAI/s1600/class_diagram.PNG)
And here the page with some with table containing 2 companies.
[![](http://4.bp.blogspot.com/_fmvjrARTMYo/TEBlaHdpOwI/AAAAAAAAAHA/QnJTpbwvmgs/s320/sample_data.PNG)](http://4.bp.blogspot.com/_fmvjrARTMYo/TEBlaHdpOwI/AAAAAAAAAHA/QnJTpbwvmgs/s1600/sample_data.PNG)

### What we will add


Now I will add a button to each row of the table, which will direct the
page to a site where the user could edit the company details and a
second button which will allow creating a new company. Before creating
web pages again I will start with defining methods which we will need in
Session Beans (the lowest layer) and additional methods and fields in
Backing Beans (the middle layer).

### New methods in Session Beans

In the Session Bean I will add a method called "saveCompany".

``` 
public void saveCompany(Company company) {
  company = em.merge(company);
  em.persist(company);
}
```

This method will take an instance of "Company" object and will perform a
"merge" of this object with object in Persistence Context. If there is
already object distinguished by the same ID in the Persistence Context,
than the Entity Manager will update the state of the object in
Persistence Context. If there is no such a object than a new one will be
created (or lets say the new Company will be added to the Persistence
Context. Than by calling the "persist" method of Entity Manager the
Company will be stored in the database.

### Changes in Backing Bean


Now the Backing Bean will need some more changes. I will add a field of
type "Company" which will represent a Company which is actually showed
or edited. Than instead of using a basic collection of type List I will
use a class called DataModel, which is a part of JSF Framework and
allows me to get the row selected by the user in the GUI.

``` 
public DataModel companiesModel;
private Company company;
public DataModel getCompaniesModel(){
  companiesModel = new ListDataModel(getAllCompanies());
  return companiesModel;
}
```


Now when I have the model I am able to get Company object representing
the actually selected row. This is show in method "editCompany".

``` {.prettyprint><br ./>public .String .editCompany(){<br ./> .company .= .(Company)companiesModel.getRowData();<br ./> .return edit-company";<br=""}
```

}


You see that the method returns String, concretely "edit-company". That
string represents a name of a navigation rule which will be used by JSF
Framework. Basically it means that the user will be redirected to
another page which is specified in the faces-config.xml file. That I
will show later. First let me put the last 2 new methods in Backing
Bean, one for saving and one for creating a new company. Again the
methods return String values for navigation purposes, it will all be
clear soon :).

``` 
public String saveCompany(){
  ssl.saveCompany(company);
  return "show-companies";
}

public String newCompany(){
  company = new Company();
  return "edit-company";
}
```

### New JSF Page to show Company's details

Lets create a new JSF JSP page (On the Web Module -&gt; New -&gt; JSF
JSP) and call it "company.jsp". This page will be used to specify the
company details. The code is quite simple:

``` 
<h1><h:outputText value="Company Details:"/></h1>
<h:panelGrid columns="2">
<h:outputText value="Name"/>
<h:inputText value="#{sales.company.name}"/>
<h:outputText value="Description"/>
<h:inputText value="#{sales.company.description}"/>
</h:panelGrid>
<h:commandButton id="btnSave" action="#{sales.saveCompany}" value="Save"/>
</h:form>
```


You see that it uses the h:form tag. This tag is basically later
rendered as HTML form tag. This TAG has to be present if your page
contains buttons or other components that perform some action. You see
that the action of a button is binded to the "saveCompany" method of our
Backing Bean and the value of the input fields are binded to the
"company" members again in the Backing Bean.

### Changes to page containing table of companies

Now lets do some changes to the "companies.jsp" page - the page
containing the table of all companies. I will add a "Edit" button to
each row (by defining a new column) and in the bottom of the page I will
add a "New" button. Both of these buttons will use method previously
defined in the Backing Bean.

``` 
<h:form>
<h1><h:outputText value="Companies List"/></h1>
<h:dataTable value="#{sales.companiesModel}" var="item">
<h:column>
<f:facet name="header"><h:outputText value="Name"/></f:facet>
<h:outputText value="#{item.name}"/>
</h:column>
<h:column>
<f:facet name="header"><h:outputText value="Description"/></f:facet>
<h:outputText value="#{item.description}"/>
</h:column>
<h:column>
<h:commandButton id="btnEdit" action="#{sales.editCompany}" value="Edit"/>
</h:column>
</h:dataTable>
<h:commandButton id="btnNew" action="#{sales.newCompany}" value="New"/>
</h:form>
```


Notice, that because there are buttons performing actions I had to
surround the whole table by **h:form** tag.

### Changes to faces-config.xml


OK - now we have the pages, last step missing is to explain the
navigation rules. As I said before these rules are managed by JSF
Framework, and are "called" by returning string expression of our
Backing Beans methods (editCompany, saveCompany, newCompany). Open the
file "faces-config.xml" and in the upper tab select "Page Flow". You
will see a diagram of your pages (there should be 3 of them:
companies.jsp, company.jsp, index.jsp). Edit the "Page Flow diagram so
it will resemble the following one.
[![](http://1.bp.blogspot.com/_fmvjrARTMYo/TEFosQFIslI/AAAAAAAAAHQ/oeZsBQQFlB4/s320/page_layout_2.PNG)](http://1.bp.blogspot.com/_fmvjrARTMYo/TEFosQFIslI/AAAAAAAAAHQ/oeZsBQQFlB4/s1600/page_layout_2.PNG)
You can change the view to XML and see the XML declaration of these
rules (the Page Flow diagram is there just to visualize the navigation
rules.

``` 
<navigation-rule>
<from-view-id>/companies.jsp</from-view-id>
<navigation-case>
<from-outcome>edit-company</from-outcome>
<to-view-id>/company.jsp</to-view-id>
</navigation-case>
</navigation-rule>
<navigation-rule>
<from-view-id>/company.jsp</from-view-id>
<navigation-case>
<from-outcome>show-companies</from-outcome>
<to-view-id>/companies.jsp</to-view-id>
</navigation-case>
</navigation-rule>
```

Now you should be ready to try out the project. In the "index.jsp" page I added redirect from index page to "companies.jsp" - so you do not have to type the address and it just gets opened after running the project from NetBeans.

```
```

- [Part 1](http://www.hoonzis.com/enterprise-java-netbeans/)
- [Part 2](http://www.hoonzis.com/netbeans-enterprise-java-2-some-more-of/)
- [Part 3](http://www.hoonzis.com/j2ee-netbeans-jsf-persistence-api-part/)
