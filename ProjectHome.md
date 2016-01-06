Short Description: This project aims to provide a web application which will allow managing ASP.NET Membership database artifacts such as users and roles for ASP.NET web applications.

More Details:
ASP.NET SQL Server Registration tool (aspnet\_regsql.exe) creates a standard set of database artifacts to allow ASP.NET web applications to make use of ASP.NET Membership API for authentication and authorization.

These users and roles etc. for a web application can only be added/updated by directly running DML in SQL Server or via Visual Studio (VS uses a web app internally which may be used outside of VS, but such method is hackish).

Web application provided in this project can be deployed like any other ASP.NET web application and allows you to manage the ASP.NET Membership data (roles and users).