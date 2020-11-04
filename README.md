# Fable App with OpenId Connect authentication

This repository reproduces the exact effect achieved by following the "[Adding a JavaScript client](https://identityserver4.readthedocs.io/en/latest/quickstarts/4_javascript_client.html)" tutorial of IdentityServer4, but in F#. [Here's](https://mjarosie.github.io/dev/2020/11/04/fable-with-openid-connect.html) a post explaining the client configuration and code step by step.

## Requirements

* [dotnet SDK](https://www.microsoft.com/net/download/core) 3.0 or higher
* [node.js](https://nodejs.org) with [npm](https://www.npmjs.com/)
* An F# editor like Visual Studio, Visual Studio Code with [Ionide](http://ionide.io/) or [JetBrains Rider](https://www.jetbrains.com/rider/).

## Building and running the app

* `dotnet tool restore` to install [Fake](https://fake.build/)
* `dotnet fake build -t Run` to spin up a [webpack dev server](https://webpack.js.org/configuration/dev-server/), API and [IdentityServer4](https://identityserver4.readthedocs.io/en/latest/) services.
* After the first compilation is finished, in your browser open: https://localhost:5003/

You might need to accept the risk of invalid local certificate in your browser.

## Project structure

### F#

The solution consists of 4 projects:

- `Api`: [Saturn framework](https://saturnframework.org/) web server that returns the user identity when called under `/identity`, works exactly the same as the API defined in "[Protecting an API using Client Credentials](https://identityserver4.readthedocs.io/en/latest/quickstarts/1_client_credentials.html) tutorial of IdentityServer4.
- `IdentityServer`: Identity Provider service, works exactly the same as the Identity Provider defined in "[Protecting an API using Client Credentials](https://identityserver4.readthedocs.io/en/latest/quickstarts/1_client_credentials.html) tutorial of IdentityServer4.
- `Client` directory contains these two projects:
	- `App`: Main logic of the SPA (single page application)
	- `Auth`: Small bit of code needed for handling the OpenId Protocol redirection.

### npm

JS dependencies are declared in `package.json`, while `package-lock.json` is a lock file automatically generated.

### Webpack

[Webpack](https://webpack.js.org) is a JS bundler with extensions, like a static dev server that enables hot reloading on code changes.
Fable interacts with Webpack through the `fable-loader`. Configuration for Webpack is defined in the `webpack.config.js` file.
Note this sample only includes basic Webpack configuration for development mode,
if you want to see a more comprehensive configuration check the [Fable webpack-config-template](https://github.com/fable-compiler/webpack-config-template/blob/master/webpack.config.js).

This configuration produces two separate outputs:
- `app` - main code of the SPA (single page application)
- `auth` - small bit of code needed for handling the OpenId Protocol redirection.

### Web assets

Assets like html pages or an icon can be found in the `public` folder.
The `index.html` file wraps the application (uses the code from `src/Client/App`),
`callback.html` is used for handling the OpenId Protocol redirection (uses the code from `src/Client/Auth`).
