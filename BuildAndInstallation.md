# Minimum System Requirements #
  1. MS.NET 2.0
  1. Visual Studio Express (Web developer)
  1. SQL Server 2005 (This is the one I've tested this app with but others should work too)

# Build Setup #

  1. If not already existing then setup the ASP.NET Membership DB on an SQL Server instance. It can be easily done via Aspnet\_regsql.exe (http://msdn.microsoft.com/en-us/library/ms229862(VS.80).aspx)
  1. Checkout the code from trunk here
  1. Edit the connection string in Web.config such that it points to the above Membership DB on your SQL Server
  1. For this application to work you need to setup at least one user with "admin" role in the target membership DB. This can be done either via:
    1. Visual Studio through Project->ASP.NET Configuration menu
    1. Directly adding entries into DB via SQL
  1. Build the application in Visual Studio
  1. Launch and logon using the above "admin" user. You should be able to see the application, user and role management options.