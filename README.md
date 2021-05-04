# Authentication Repository

An API used to facilitate authentication across a group of APIs. Handles login authentication and access permissions across the collective solution.

## Technologies Used

C#, Azure DevOps, ASP.NET Core Web API, Auth0

## Instructions and Installation

baseurl: to be determined

Headers: {Authorization: `Bearer ${token}`}

### Example headers using RestSharp

- request.AddHeader("Authorization", token);
- look at https://github.com/NNKamel/testapi for "complete" example on using this as a backend api
- look at https://github.com/NNKamel/auth0-angular-test for "complete" example on using this as a frontend SPA

### Available requests:

GET request: (used in Authentication Middleware automatically)

- {baseurl} -->

* if user is signed in (token is correct), returns {access: "granted", permissions: [a list of string permissions]}
* else returns Forbidden Result (status code: 403).

GET request:

- {baseurl}/userdata -->

* if ok, returns user Auth0 Dictionary<string, string>
* else returns Forbidden Result (status code: 403).

POST request:

- {baseurl}/role/{roleName} -->

* if ok, adds the role from the current user token returns True
* if an error occurs in the process returns false
* if unauthenticated returns Forbidden Result (status code: 403).

DELETE request:

- {baseurl}/role/{roleName} -->

[REQUIRED BODY]

> userid: string

- if ok, deletes the role from the current user token returns True
- if an error occurs in the proces returns false
- if unauthenticated returns Forbidden Result (status code: 403).

## Licenses

This project is under the MIT License.

## Questions

If you have any questions, please reach out to the creators.
