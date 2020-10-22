# dotnet-core-demo

This is a simple demo app based on .NET core which allows you to manage "Projects", which uses MySQL as data store. 

Visual Studio version: VS 2019

# Bring up the database

I use docker for the database. If you use the desktop version, make sure you modify `Data\ApplicationDbContext.cs`. 

If you use docker, in PowerShell, type:

```
docker-compose up
```
Then, in packet management shell:

```
Update-Database
```
# Roles

By default, there is an "Administrator" role, which allows the administrator to see all projects. By default, users can see only their own project. 

## Administrator Credentials

The administrator account and role are seeded when the server is up the first time.  

```
Username: admin@example.com
Password: password
```

## Localization
The default locale is `zh-CH`. You can change it in `Startup.cs`. 
