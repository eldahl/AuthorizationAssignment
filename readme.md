# Authorization Assignment
This is a small assignment to demonstrate how to do authentication using JWTs.  
See assignment description here: ![Secure Software](https://rpede.github.io/SecureSoftwareDevelopment/assignments/authorization)

## Instructions for testing functionality
Run the project and either use the built in Swagger tool in Development mode, or use an external tool to run a POST request at `/login`.
This will yield a JWT bearer token which is needed to access and execute API endpoints.

Credentials are:
|Username|Password|
|---|---|
|EditorUser|EditorPassword|
|JournalistUser|JournalistPassword|
|RegisteredUser|RegisteredPassword|

`/login` JSON Format:
```
{
  "username": "JournalistUser",
  "password": "JournalistPassword"
}
```

All routes can be found in the ![RoutesController.cs](SampleApp/BackEnd/RoutesController.cs) file.

Note: The `DatabaseSeed.sql` script is run at startup, if the file `Database.db` does not exist.

## Running in Codespaces

1. **📤 One-click setup**: [Open a new Codespace](https://codespaces.new/eldahl/AuthorizationAssignment), giving you a fully configured cloud developer environment.
2. **▶️ Run all, one-click again**: Use VS Code's built-in *Run* command and open the forwarded port *8080* in your browser. 
![](images/RunAll.png)
