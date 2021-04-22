# Authentication Repository

Repository for the Authentication API

## Instructions

baseurl: to be determined

Headers: {Authorization: `Bearer ${token}`}

### example headers using RestSharp

- request.AddHeader("Authorization", token);
- look at https://github.com/NNKamel/testapi for "complete" example on using this as a backend api
- look at https://github.com/NNKamel/auth0-angular-test for "complete" example on using this as a frontend SPA


### Available requests:

GET request:

- {baseurl} --> if user is signed in (token is correct), returns {access: "granted"}, else returns Forbidden Result (status code: 403).

GET request:

- {baseurl}/userdata --> if ok, returns user Auth0 Dictionary<string, string>, else returns Forbidden Result (status code: 403).

GET request:

- {baseurl}/isadmin --> if user is signed in and an admin (in auth0), returns {access: "granted"}, else returns Forbidden Result.

GET request:

- {baseurl}/ismoderator --> if user is signed in and a moderator (in auth0), returns {access: "granted"}, else returns Forbidden Result.

