---
layout: post
title: J2EE - NetBeans, JSF, Persistence API - Part 3
date: '2010-07-29T01:26:00.001-07:00'
author: Jan Fajfr
tags:
- Java
- JSF
- J2EE
modified_time: '2014-06-27T05:26:32.970-07:00'
---
This is part 3 of old series about J2EE, Java Beans and JSF.

- [Part 1](http://www.hoonzis.com/enterprise-java-netbeans/)
- [Part 2](http://www.hoonzis.com/netbeans-enterprise-java-2-some-more-of/)
- [Part 3](http://www.hoonzis.com/j2ee-netbeans-jsf-persistence-api-part/)

The aim of this part is to extend the detail page of a company by adding a list of products of the company and a drop down list box, which will allow a user to add a new product to the list of the products of the company. Here is who it should look like in the end.

[![](http://4.bp.blogspot.com/_fmvjrARTMYo/TFcPE-ZjuZI/AAAAAAAAAHY/13-nbFLL-B4/s320/result.PNG)](http://4.bp.blogspot.com/_fmvjrARTMYo/TFcPE-ZjuZI/AAAAAAAAAHY/13-nbFLL-B4/s1600/result.PNG)

Generally adding the list of the products of the company to the page will be a piece of cake. You can again use the **dataTable** component provided by JSF. What will be a bit tricky is adding a drop down list. We will use the **selectOnListBox** component populated by **List**. Because this component allows to show **String** value and as I seed we will put in **Product** class. We will have to use a **Converter** component which will translate the **Product** to a single **String** and vice versa.

### Changes to the Session Bean
Again here I will start with the changes in the lower level. We will
need to add a method which will provide all the products in the database
as a List object. Later we will populate a drop down list with these
products.

```java
public List<product> getAllProducts() {
 List<product> products = em.createNamedQuery("Product.getAllProducts").getResultList();
return products;
}
```

This method is calling a named query which you need to place above the
"Product" class declaration.

```java
@Entity
@NamedQuery(name="Product.getAllProducts",query="SELECT p FROM Product p")
public class Product implements Serializable {

}
```

Then we will need a method which will find a product for a given ID. We
can easily use u method of the Entity Manager which allows to find o
object by a given ID (the ID field is decorated as a identity in the
Entity Beans declaration. This method will be used by the **Converter**
class.

```java
public Product getProductWithId(String id) {
try {
  return em.find(Product.class, Integer.parseInt(id));
} catch (java.lang.NumberFormatException e) {
  e.printStackTrace();
}
return null;
}
```

### The converter class
As it had been said already, we will implement class which allows us
convert object of type **Product** to a **String**. This class has to
implement **Converter** interface, provided by the JSF framework. This
interface has two methods which have to be overriden **getAsObject** and
**getAsString**. Both of these methods do have 3 arguments.

The **FacesContext** specifies the context of the current request.
FacesContext is an object which takes care of all the phases needed to
render a response to the request. When an user demands an page, the
users request is first processed by the **Faces Servlet**. If the call
is a postback of previously viewed page, Faces Servlet will use the ID
of the view and will look up the FacesContext of the page. If it is a
initial view of the page, new FacesContext will be created. FacesContext
connects the Component Tree of your page with the Backing Bean
containing the logic. To get more information about the JSF life cycle
and the FacesContext [**visit this
series.**](http://www.ibm.com/developerworks/library/j-jsf2/). Now the
second argument is the UI Component to which the Converter is connected
and the 3rd argument is either String value or Object - depends which
way are you currently converting. Now - you just have to implement these
method, you will probably never call them in your code. Here is my
**ProductConverter** class which I decided to put into package
**sales.util**.

```java
public class ProductConverter implements Converter{
    public Object getAsObject(FacesContext context, UIComponent component, String value) {
    SalesSessionLocal ssb = lookupSessionBean();
    Product p = ssb.getProductWithId(value);
    return p;
  }

  public String getAsString(FacesContext context, UIComponent component, Object value) {
    String id = String.valueOf(((Product)value).getId());
    return id;
  }

  private SalesSessionLocal lookupSessionBean(){
    try{
      Context c = new InitialContext();
      SalesSessionLocal ssb = (SalesSessionLocal) c.lookup("java:comp/env/SalesSessionBean");
      return ssb;
    }catch(NamingException ex){
      return null;
    }
    }
}
```

The conversion of the object to the String is quite easy - you just need to decide what describes the object. Here you can see that I just return the products ID. The conversion of an String to object is the processing of looking up the product of given ID in our Entity Manager. We had prepared a method for this lookup in the Sesion Bean, which we are basically calling by the following line.

```java
Product p = ssb.getProductWithId(value);
```

Now the question is how to obtain the Session Bean. Normally in the
Backing Bean we are using the EJB injection by the "@EJB" decoration.
This means that we just write one single line and we obtain the Session
Bean (no initialization, the framework takes care of this).

```java
@EJB
SalesSessionLocal ssl;
```

However in the Converter you are not able to use this approach. The
injection is allowed just in Managed Beans (another name for Backing
Beans). The solution here is the usage of JNDI Lookup. JNDI - in general
is a standard JAVA interface of locating users, computers but also
objects and services. We need to lookup an object, concretely Session
Bean, which is a part of the same J2EE application. The details of this
lookup can be [**found
here**](http://www.oracle.com/technology/sample_code/tech/java/oc4j/htdocs/Jndi/JNDIlookup.html).
Generally when the application server is started it creates for each
application and **InitialContext**, which is an object that contains the
Name - Object bindings. Using this InitialContext we can obtain the
desired object if we know the name the object. We give the name to the
object (Sesstion Bean) when we register it in the **web.xml**
configuration file. Here is the registration of EJB in web.xml.

```xml
<ejb-local-ref>
<ejb-ref-name>SalesSessionBean</ejb-ref-name>
<ejb-ref-type>Session</ejb-ref-type>
<local>sales.SalesSessionLocal</local>
<ejb-link>SalesManagement-ejb.jar#SalesSessionBean</ejb-link>
</ejb-local-ref>
```

And Here is how we can use the JNDI Lookup to get the Session Bean in  our GlassFish InitialContext.

```java
Context c = new InitialContext();
SalesSessionLocal ssb = (SalesSessionLocal) c.lookup("java:comp/env/SalesSessionBean");
```

### Changing Faces Configuration
So that's it, the Converter class is ready to be used. Now we have to
register it in the Faces Configuration; in the **faces-config.xml** file
add the following:

```xml
<converter>
<description>Converts Product Entity to String</description>
<converter-id>ProductConverter</converter-id>
<converter-class>sales.util.ProductConverter</converter-class>
</converter>
```

### Changes to the Managed Bean (another name for Backing Bean)
Now we will have to add some methods and fields to the Backing Bean to
assure the functionality. First we will need to provide a field (and off
course setters and getters) which will store the actual selected
"Product" in the drop down list.

```java
private Product selectedProduct;
  public Product getSelectedProduct() {
  return selectedProduct;
}

public void setSelectedProduct(Product selectedProduct) {
this.selectedProduct = selectedProduct;
}
```


Now the next method called "addProduct" will just add the
"selectedProduct" to the list of products of the current company.

```
public void addProduct(){
if(this.company.getProducts() == null){
this.company.setProducts(new ArrayList());
}
this.company.getProducts().add(selectedProduct);
ssl.saveCompany(company);
}
```


The latest method will return a collection of products in as a list of
**SelectItem**, which will represent the collection of products in the
drop down list. This method benefits from the previously created method
for getting all products in the Session Bean.

```
public List getAllProductsSelectList() {
List items = new ArrayList();
for (Product p : ssl.getAllProducts()) {
items.add(new SelectItem(p, p.getId() + " " + p.getName()));
}
return items;
}
```



### Using the converter in the web page


Now you should be ready to use to converter in your web page. Open the
page with the details of the company ("company.jsp" in my case), and add
the following table of products of one company.

```
<h:datatable value="#{sales.company.products}" var="item">
<h:column>
<f:facet name="header"><h:outputtext value="Name"></h:outputtext></f:facet>
<h:outputtext value="#{item.name}">
</h:outputtext></h:column>
<h:column>
<f:facet name="header"><h:outputtext value="Description"></h:outputtext></f:facet>
<h:outputtext value="#{item.description}">
</h:outputtext></h:column>
</h:datatable>
```

That is basically the same as done before in part 1, just notices that
the table is binded to the list of the products of the "Company" object
in Backing Bean. Now the following peace of code actually adds the drop
down list box, which allows user to select one product and add it to the
company.

```xml
<h:outputtext value="Add Product:"></h:outputtext></h3>
<h:selectonelistbox size="1" value="#{sales.selectedProduct}">
<f:selectitems value="#{sales.allProductsSelectList}">
<f:converter converterid="ProductConverter">
</f:converter></f:selectitems></h:selectonelistbox>
<h:commandbutton action="#{sales.addProduct}" id="btnAdd" value="Add">
</h:commandbutton>
```

You can see that the **selectOneListbox** component is binded to the
"selectedProduct" field defined in the Backing Bean as well as the items
list is binded to the list of products in the Backing Bean. The
component also contains reference to the converter which should be used,
which we have before prepared in the faces-config.xml. If you try this
example right now, it will not work, instead you will obtain a strange
validation error in your GlassFish log.
To be used by custom JSF converter, the entity class needs to override
the **Equals** method. This is because the selected item is bound to
the Backing Bean and when the selected item changes, it is compared to
the bound field in the Backing Bean. So now you have to override the
equals method for your "Product" class. To distinguish two products you
can simply compare their IDs.

```java
@Override
public boolean equals(Object obj){
  if (obj == null) {
    return false;
  }
  if (getClass() != obj.getClass()) {
     return false;
}

final Product other = (Product) obj;
if (this.getId() != other.getId()) {
return false;
}
return true;
}
```

To try out your example you need some simple testing data. You can run this script (just check the names of the fields in the database tables before).

```sql
INSERT INTO SALES.COMPANY (companyname, companydescription) values('First company', 'Sales bananas');
INSERT INTO SALES.COMPANY (companyname, companydescription) values('Second company', 'Sales oranges');

INSERT INTO SALES.PRODUCT (productname,productdescription) values ('Product 1','Orange');
INSERT INTO SALES.PRODUCT (productname,productdescription) values ('Product 2','Banana');
```

[**Download the source code**](https://skydrive.live.com/redir?resid=F8AFB4F072D6DB62!4913)

- [Part 1](http://www.hoonzis.com/enterprise-java-netbeans/)
- [Part 2](http://www.hoonzis.com/netbeans-enterprise-java-2-some-more-of/)
- [Part 3](http://www.hoonzis.com/j2ee-netbeans-jsf-persistence-api-part/)
