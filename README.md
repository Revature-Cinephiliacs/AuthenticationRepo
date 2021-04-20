# Authentication Repository

Repository for the Authentication API

## Instructions

baseurl: to be determined

Headers: {Authorization: `Bearer ${token}`}

### example headers using RestSharp

- request.AddHeader("Authorization", token);

### Available requests:

GET request:

- {baseurl} --> status code 200 if token is correct

GET request:

- {baseurl}/userdata --> if ok, returns user Auth0 Dictionary<string, string>, else returns Forbidden Result (status code: 403)
