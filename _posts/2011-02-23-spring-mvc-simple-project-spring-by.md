---
layout: post
title: Spring MVC tutorial
date: '2011-02-23T15:50:00.002-08:00'
author: Jan Fajfr
tags:
- J2EE
- spring
modified_time: '2014-06-27T02:24:23.011-07:00'
thumbnail: http://1.bp.blogspot.com/-_dTtQK_AOfI/TWWcttkD9uI/AAAAAAAAAI8/bIA2WbWE2ts/s72-c/students.PNG
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-5496261131107564255
blogger_orig_url: http://hoonzis.blogspot.com/2011/02/spring-mvc-simple-project-spring-by.html
---
This semester I had to do a bigger project using Spring Framework & Hibernate. On a side of this project I had a smaller one which I have created just to always try out certain parts which later I applied to the bigger one. Here I just present the simple project which should be easy to understand, but it contains all the important parts/frameworks/design patterns of enterprise applications: MVC, ORM, Validations, Security, Internalization, Views/ Tiles, Unit Tests.

I do not have much experience working with J2EE, or writing enterprise Java applications, so this will maybe help someone who is on the same Java level as I am.

#### The application description
[Download the project here](https://onedrive.live.com/redir.aspx?cid=f8afb4f072d6db62&resid=F8AFB4F072D6DB62!4915&parId=F8AFB4F072D6DB62!4912&authkey=!ALHYiWWtLLCdhKw)

This is standard CRUD application, here are the 3 mains screens:

[![](http://1.bp.blogspot.com/-_dTtQK_AOfI/TWWcttkD9uI/AAAAAAAAAI8/bIA2WbWE2ts/s320/students.PNG)](http://1.bp.blogspot.com/-_dTtQK_AOfI/TWWcttkD9uI/AAAAAAAAAI8/bIA2WbWE2ts/s1600/students.PNG)


[![](http://4.bp.blogspot.com/-oOhl2KRvNu0/TWWcz_jtCQI/AAAAAAAAAJE/dsyyVl5BbHQ/s320/courses.PNG)](http://4.bp.blogspot.com/-oOhl2KRvNu0/TWWcz_jtCQI/AAAAAAAAAJE/dsyyVl5BbHQ/s1600/courses.PNG)

[![](http://3.bp.blogspot.com/-QXf0RVF3iSU/TWWc4nHhLcI/AAAAAAAAAJM/fruZNxQct-0/s320/studentcourses.PNG)](http://3.bp.blogspot.com/-QXf0RVF3iSU/TWWc4nHhLcI/AAAAAAAAAJM/fruZNxQct-0/s1600/studentcourses.PNG)

You can see, that it is a basic application containing: Student and Courses. There is one Many To Many relationship, while each student can have many courses and each course can be followed by many students.

### Basic setup of the project

This is Eclipse "Dynamic Web Application" Project. I have used the latest Eclipse 3.6, Java 1.6 and Tomcat 6. However this should not limit to run the project with different configurations, however changing to Java 1.5 would require some additional changes (@Override annotations). In the following image you can see the two groups first the source files and second all the important configuration files. Here is brief overview:

[![](http://3.bp.blogspot.com/-I_ffD86c_ZE/TWGaCcuafFI/AAAAAAAAAIo/gloMHFTlC3E/s320/project_structure.PNG)](http://3.bp.blogspot.com/-I_ffD86c_ZE/TWGaCcuafFI/AAAAAAAAAIo/gloMHFTlC3E/s1600/project_structure.PNG)

-   **web.xml** - Web Deployment Descriptor, the base of the
    configuration for all the parts that compose the web application.
-   **tiles.xml** - Configuration of Appache Tiles - templating
    framework, used to define reutilisable parts of web pages.
-   **spring-servlet.xml** - Spring configuration part. Described lower.
-   **jdbc.properties** - This file just holds the properties of the
    database connection.
-   **applicationContext-security.xml** - Configuration of Spring
    Security Framework.

### Package and project structure

The project has a common structure:
**View &lt;--&gt; Controller &lt;--&gt; Service &lt;--&gt; Repository &lt;--&gt; DB**.
The Repository pattern (implemented by the classes in the DAO package)
allows the abstraction of the DB technology. The Service layer has
implemented all the functions of the project. For example in the
StudetsService class you can find all methods concerning the students
(add a course to student, create new student..).
The Controller layer uses the Service layer to fill the model needed
behind the view and to perform the tasks desired by the user.

### Model View Controller with Spring

The MVC software architecture permits separate the domain model (M), the
logic (C) and the user interface (V). Spring implementation of MVC is
based on DispatcherServlet. DispatcherServlet is classical HttpServlet
which decides to which Controller the current request should be sent and
which View should be used to render the response.
Please refer to the [Official Documentation](http://static.springsource.org/spring/docs/3.0.x/reference/mvc.html)
which contains great explanation.
In the web.xml the DispatcherServlet is defined to handle all the
requests coming to the web site (see url-pattern).

``` 
<servlet>
   <servlet-name>spring</servlet-name>
   <servlet-class>org.springframework.web.servlet.DispatcherServlet</servlet-class>
   <load-on-startup>1</load-on-startup>
  </servlet>
  <servlet-mapping>
   <servlet-name>spring</servlet-name>
   <url-pattern>/</url-pattern>
  </servlet-mapping>

  <context-param>
  <param-name>contextConfigLocation</param-name><param-value>/WEB-INF/spring-servlet.xml
   /WEB-INF/applicationContext-security.xml
  </param-value></context-param>
  
  <listener>
    <listener-class>org.springframework.web.context.ContextLoaderListener</listener-class>
  </listener>
```

In the servlet mapping part, the servlet-name is set to "spring". This
servlet needs some special configuration and this will be provided by
spring-servlet.xml file. But in order to tell Spring where to search te
configuration we set the path of this file as the parameter to the Spring
ContextLoader. There is also a link to the file which provides security
configuration. Spring-servlet.xml contains a lot of configuration. Lets
now take a look at the part concerning the view.

Apache Tiles
------------

With spring you can use several APIs or technologies to build you user
interface: standard JSP, Java Server Faces, Apache Tiles (they do not
compete, they serve for different purposes and sometimes overlap). In
this project I am using Apache Tiles. That is a framework which allows
you to define a fragments or parts which can be reused in several web
pages (typically for example you have a menu bar, which is always
present).
In the Spring-servlet.xml this is the part concerning Tiles:

``` 
<bean class="org.springframework.web.servlet.view.UrlBasedViewResolver" id="viewResolver">
     <property name="viewClass">
         <value>
             org.springframework.web.servlet.view.tiles2.TilesView
         </value>
     </property>
 </bean>
 <bean class="org.springframework.web.servlet.view.tiles2.TilesConfigurer" id="tilesConfigurer">
     <property name="definitions">
         <list>
             <value>/WEB-INF/tiles.xml</value>
         </list>
     </property>
 </bean>
```

That tells Spring, that all the views should be resolved using TilesView
class and the configuration can be found in the tiles.xml file.

Tiles Configuration
-------------------

First declared in the Tiles configuration file is the layout. So a
specific page(layout.jsp) which defines the layout of our pages and
links to jsp pages which represent these parts. After this definition
come the declarations of each view. Each view specifies which layout it
uses and the parts which should be imported.

``` 
<tiles-definitions>
    <definition name="base.definition" template="/WEB-INF/jsp/layout.jsp">
        <put-attribute name="title" value="">
        <put-attribute name="header" value="/WEB-INF/jsp/header.jsp">
        <put-attribute name="menu" value="/WEB-INF/jsp/menu.jsp">
        <put-attribute name="body" value="">
        <put-attribute name="footer" value="/WEB-INF/jsp/footer.jsp">
    </put-attribute></put-attribute></put-attribute></put-attribute></put-attribute></definition>
 
    <definition extends="base.definition" name="students">
        <put-attribute name="title" value="Users management">
        <put-attribute name="body" value="/WEB-INF/jsp/students.jsp">
    </put-attribute></put-attribute></definition>
</tiles-definitions>
```

Take a look at the ***layout.jsp*** file and you will fast understand
how it works.

MVC implementation
------------------

To understand how MVC works open one of the Controller classes, for
example: **StudentsController*

``` 
@Controller
public class StudentsController {

 @Autowired
 private StudentsService studentsService;

 @RequestMapping("/students")
 public String listUsers(ModelMap model){
  model.addAttribute("student",new Student());
  List<student> users = studentsService.listStudents();
  model.addAttribute("studentsList",users);
  return "students";
 }
 
  @RequestMapping(value = "/addstudent", method = RequestMethod.POST)
    public String addContact(@ModelAttribute("student")
    Student user, BindingResult result) {
 
   studentsService.addStudent(user);
        return "redirect:/students";
    }
 
    @RequestMapping("/delete/{studentID}")
    public String deleteContact(@PathVariable("studentID")
    Integer studentID) {
     studentsService.removeStudent(studentID);
        return "redirect:/students";
    }
}
```

There is a private field of type StudentsService. You can see that it is
annotated with @Autowired. This service provides all the methods needed
to manipulate students. The @Autowire annotation tells Spring, that it
should automatically instantiate this field from it's Dependency
Injection container (in other words you do not have to take care about
the creation of object).
Next there are 3 methods with different signatures all of them annotated
with @RequestMapping. The MVC framework binds the URL given in the
decoration with the controller and with the execution of the decorated
method. In other words when the user navigates to
"..TestingProject/students.do" this method will be processed to
construct the Model and to return the View to the user.
As said before the methods have different signatures depending on their needs but the are some rules which have to be kept. ([see the documentation](http://static.springsource.org/spring/docs/3.0.x/spring-framework-reference/html/mvc.html#mvc-ann-requestmapping-uri-templates))).

The most straightforward is the method **listStudents**. This method fills the given Model (which is passed to the method by MVC framework) and returns the name of the view which should be used to show this model. You can take a look at the Tiles configuration to see which JSP will be shown after returning "students" as the name of the view.

### Object Relational Mapping - the Domain

In the Domain package you can find just two classes: Student and Course. This is the Student class:

``` 
@Entity
@Table(name="student")
public class Student {

 @Id
 @GeneratedValue
 private Integer id;
 
 @ManyToMany
 @JoinTable(name="student_course"
  ,joinColumns=@JoinColumn(name="student_id")
  ,inverseJoinColumns=@JoinColumn(name="course_id"))
 private List follows;
 
 @Column
 private String firstname;
 
 @Column
 private String lastname;

 @Column
 private String email;
 
 @Column
 private Integer phone;
 public Integer getId() {
  return id;
 }
  ...getters and setters...
}
```

The @Entity annotation tells hibernate that this class represents and
entity which should be stored in the database, the @Table annotation
details the name of the table used to store these entities.
Each field of this class is annotated with @Column annotation - this
tells Hibernate that each field should be persisted as separated
column.
The @Id and @GeneratedValue annotations over the id field specify that
this field represents the primary key and that the value of this key
should be automatically generated whenever persisting a new entity in
the table.
And last @ManyToMany and @JointTable annotations represent the N to N
relationship between students and Courses. In common words Student can
have several Courses and a Course can have several students.
The @JoinTable annotation specifies exactly which table should
represents the N - N relation ship and declare the foreign keys in this
table. Note that there are different ways to manage this using Hibernate
(look for @ManyToMany and @MappedBy annotations).
The course class is quite similar and I will not describe it here.

### Internalization / Localization

It is quite common demand to make the application supporting multiple languages. Spring framework provides LocalChangeInterceptor, which based on parameter in the url of the page decides, which languages version of the page should be loaded. To configure this feature take a look at the **spring-servlet.xml** file which contains the 3 following beans:

``` 
<!-- Messages and Internalization -->
    <bean id="messageSource"
        class="org.springframework.context.support.ReloadableResourceBundleMessageSource">
        <property name="basename" value="classpath:messages" />
        <property name="defaultEncoding" value="UTF-8" />
    </bean>
    
    <bean id="localeChangeInterceptor"
     class="org.springframework.web.servlet.i18n.LocaleChangeInterceptor">
        <property name="paramName" value="lang"/>
    </bean>
 
    <bean id="localeResolver"
   class="org.springframework.web.servlet.i18n.CookieLocaleResolver">
   <property name="defaultLocale" value="en"/>
    </bean>
```

The first bean defines the store for the strings in multiple languages.
Here it aims at all files which look like "messages\*" placed in the
classpath, defining the UTF-8 as the default encoding to save the
strings.
The second bean is the above mention language interceptor. The
**paramName property** tells spring to look at the "lang" url parameter
before composing the final web page.
The last bean defines which implementation of LocalResolver interface
should be used to define the language. Here it is CookieLocalResolver.
An implementation which uses a cookie sent to the user, which later can
be retrieved to determine the language.
Now in the resources you can localize two files:
**messages\_en.properties** and **messages\_fr.properties**. These two
files contain a simple list of key-value pairs defining the strings used
in the application and the translations.
Now the last step is to take a look in how these strings are references
in the web pages. For that take a look for example at **menu.jsp** which
defines the left side menu of the application.

``` 
<%@taglib uri="http://www.springframework.org/tags" prefix="spring"%>
<p><spring:message code="label.menu"/></p><ul><li><a href="/TestingProject/students"><spring:message code="label.students"/></a></li>
<li><a href="/TestingProject/courses"><spring:message code="label.courses"/></a></li>
</ul>
```

Here you can see that instead of writing the string inside of the A tag,
the **spring:message** tag is used which loads the correct string
identified by the "label.students" key.

### Security

Now the security is taken care of by Spring Security (former Acegi
Security) framework and I use the very basic settings. Spring Security
in its core uses basic Filter and FilterChain interfaces which are
defined in the core of Java Servlet application and are generally code
parts responsible for composing or caching the content to be send to the
user.
In the **web.xml** file the following parts instruct the application to
use the org.springframework.web.filter.DelegatingFilterProxy class for
filtering all url (pattern set in the filter-mapping part). This filter
is later responsible for allowing access to certain parts only to users
with certain rights. To get more information refer to this official
["The Security Filter Chain"
page.](http://static.springsource.org/spring-security/site/docs/3.0.x/reference/security-filter-chain.html).
For more information about filters in general [visit this
page.](http://onjava.com/pub/a/onjava/2003/11/19/filters.html).
The first part of the security settings is defined in separated
**applicationContext-security.xml** file.
First the following **authentication-manager** tells spring that the
user is authentified against the database. The same data source is
already used for ORM mapping.

``` 
<authentication-manager>
  <authentication-provider>
    <jdbc-user-service data-source-ref="dataSource"/>
  </authentication-provider>
</authentication-manager>
```


However the precedent snippet does not define how the security data is
stored in the database. That is because it uses the default
implementation. In other words Spring expects you to provide the data
source with default structure. This structure is composed of two tables:
**users** and **authorities**. To see the details take a look at the
[Apendix A of Spring
documentation.](http://static.springsource.org/spring-security/site/docs/3.0.x/reference/appendix-schema.html)
And if you just want to have the DB structure you can create it with
**security\_db.sql** script which is part of the project:

``` 
create table users(
      username varchar(50) not null primary key,
      password varchar(50) not null,
      enabled boolean not null);

  create table authorities (
      username varchar(50) not null,
      authority varchar(50) not null,
      constraint fk_authorities_users foreign key(username) references users(username));
      create unique index ix_auth_username on authorities (username,authority);
```


So we have the users which have roles and the data is stored in the
database. Now the way it is used is specified in the second part of
"applicationContext-security.xml".

``` 
<http auto-config="true">
        <intercept-url pattern="/admin/*.do" access="ROLE_ADMIN"/>
        <intercept-url pattern="/**" access="ROLE_USER,ROLE_ADMIN,ROLE_TEACHER"/>
        <intercept-url pattern="/login.jsp*" filters="none"/>
        <form-login login-page='/login.jsp'/>
</http>
```

Here we assign to url patterns the roles that users need to have in
order to access these urls.
Well that's it. But this is just a very simple way of implementing
security. There are more advanced manners of securing the method calls
or visibility of html elements always based on user roles.

### Hints for the other aspects enterprise application

Unfortunately I did not have time to include in this simple project some
other aspects of enterprise applications.

#### Validations

To implement data validations spring contains
org.springframework.validation space, which contains a Validator
interface containing "validate" method. Here is a short example of how
validator could be implemented for a "User" class:

``` 
public class UserValidator implements Validator {
  
 @Override
 public boolean supports(Class<?> arg0) {
  return User.class.isAssignableFrom(arg0);
 }

 @Override
 public void validate(Object arg0, Errors arg1) {
  User user = (User) arg0;
  if(!isValidEmailAddress(user.getAdresse_mail())){
   arg1.rejectValue("adresse_mail","adresse_mail.notValid","Not a valid email");
  }
  
 }
 
 public boolean isValidEmailAddress(String emailAddress){  
     String  expression="^[\\w\\-]([\\.\\w])+[\\w]+@([\\w\\-]+\\.)+[A-Z]{2,4}$";  
     CharSequence inputStr = emailAddress;
     Pattern pattern = Pattern.compile(expression,Pattern.CASE_INSENSITIVE);  
     Matcher matcher = pattern.matcher(inputStr);  
     return matcher.matches();
   } 
}
```

In this example I am using Regular Expressions to determine whether the
specified email is in valid form.
Another way of validating would be to use the JSR 303 validation
specification, which defines the validation model based on annotations.
This allows you to annotate field of you domain class and thus specify
the way this field should look like. In the following example the @Email
annotation would take care of what the above implementation does.


``` 
public class User {

@Email
protected String adresse_mail;

@Basic
@Column(length=50)
public String getAdresse_mail() {
 return adresse_mail;
}
public void setAdresse_mail(String adresse_mail) {
 this.adresse_mail = adresse_mail;
}
}
```

However there are cases where you will always like to implement your own
validators. For example when you want to reach to the database and
decided whether there is not already a user with a given name, or
whether the student is not already signed in specified class.
To get more information about JSR 303 validation refer to [this blog
post.](http://www.openscope.net/2010/02/08/spring-mvc-3-0-and-jsr-303-aka-javax-validation/)

#### Advanced Security Approaches

When working on our application I was having hard time to find out how
to force Spring Security to you my own database model and perform the
authentication my way. I found that the easiest way was to implement
**UserDetailsService** interface. This interface contains one method
called: **loadUserByUsername** which returns **UserDetails** class, also
defined by Spring. This class should contain the user information. So
basically its up to you to implement this method that way, that you will
give it the user name and it will return filled UserDetails class
containing users password and users roles. Here is a simple
implementation:


``` 
@Service("userDetailsService") 
public class UserDetailsServiceImpl implements UserDetailsService {

  @Autowired
  private UsersDAO usersDAO;

  @Transactional(readOnly = true)
  public UserDetails loadUserByUsername(String username)
      throws UsernameNotFoundException, DataAccessException {

    User user = usersDAO.findByLogin(username);
    
    if (user == null)
      throw new UsernameNotFoundException("user not found");

    Collection authorities = new ArrayList();
    for (openschool.domain.model.Role role : user.getRoles()) {
      authorities.add(new GrantedAuthorityImpl(role.getRole()));
    }
    
    String password = user.getPassword();
    boolean enabled = user.isEnabled();
    boolean accountNonExpired = true;
    boolean credentialsNonExpired = true;
    boolean accountNonLocked = true;
    
    
    org.springframework.security.core.userdetails.User springUser;
    springUser = new User(username, password, enabled, accountNonExpired, credentialsNonExpired, accountNonLocked, authorities);
    
    return springUser;
  }
}
```



#### Unit Tests

When using Spring IOC you need to tell to spring what is the application
context for the unit tests. Normally the application context is
specified by the **web.xml** file. But the Unit Tests do not have
anything together with the web application, so you have to tell to
Spring where to find all the definitions in order to wire all you
injected objects.
This can be done by annotating the UnitTest class by following
annotations:

``` 
@RunWith(SpringJUnit4ClassRunner.class)
@ContextConfiguration(locations={"classpath:/applicationContext.xml"})
```

The ContextConfiguration annotations provides the information to spring
where to find the configuration xml. Here it points to the
applicationContext.xml on the classpath. Just two provide some
information on how the complete test class can look:


``` 
@RunWith(SpringJUnit4ClassRunner.class)
@ContextConfiguration(locations={"classpath:/applicationContext.xml"})
public class UsersDAOTest{

 @Autowired
 UsersDAO usersDao;
 
 private Long beforeUserID;
 
 @Before
 @Transactional
 public void initDB(){
  Administratif user = new Administratif();
  user.setLastName("User_before");
  user.setAdresse_mail("email@email.com");
  user.setLogin("login");
  
  
  usersDao.addUser(user);
  beforeUserID = user.getIdUser();
 }
 
 @Test
 @Transactional
 public void testAddUser() {
  
  User user = new User();
  user.setLastName("Lastname");
  user.setAdresse_mail("email@email.com");
  
  usersDao.addUser(user);
  
  User user2 = usersDao.getUser(user.getIdUser());
  assertEquals(user, user2); 
       }
```

You might also want to create Test Suites which allow to run multiple tests in the same time, by creating simple class with @Suite annotation:

``` 
@RunWith(Suite.class)
@Suite.SuiteClasses({
 MyTest1.class,
 UsersDAOTest.class,
 MyTest2.class
})
public class TestSuite {
  //intentionally empty
}
```