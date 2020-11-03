module App

open Browser.Dom
open OpenIdConnectClient
open Fable.Core.JsInterop
open Thoth.Fetch
open Thoth.Json
open Shared

let settings: UserManagerSettings = 
    !!{| 
        authority = Some "https://localhost:5001"
        client_id = Some "js"
        redirect_uri = Some "https://localhost:5003/callback.html"
        response_type = Some "code"
        scope = Some "openid profile scope1"
        post_logout_redirect_uri = Some "https://localhost:5003/index.html"
        
        filterProtocolClaims = Some true
        loadUserInfo = Some true
    |}

let mgr: UserManager = Oidc.UserManager.Create settings

// Get a reference to our button and cast the Element to an HTMLButtonElement
let loginButton = document.getElementById("login") :?> Browser.Types.HTMLButtonElement
let apiButton = document.getElementById("api") :?> Browser.Types.HTMLButtonElement
let logoutButton = document.getElementById("logout") :?> Browser.Types.HTMLButtonElement

let log (arguments: string list) =
    let results = document.getElementById("results")
    results.innerText <- "";

    List.map (fun (msg) -> 
        results.innerText <- results.innerText + msg + "\r\n";
    ) arguments |> ignore
    ()

loginButton.onclick <- fun _ ->
    mgr.signinRedirect()

apiButton.onclick <- fun _ -> promise {
    let! user = mgr.getUser(): Fable.Core.JS.Promise<User option>

    match user with
    | Some u -> 
        let authHeader: Fetch.Types.HttpRequestHeaders = Fetch.Types.HttpRequestHeaders.Authorization ("Bearer " + u.access_token)
        console.log (sprintf "%A" authHeader)
        let! claims = Fetch.get<_, Claim list> (url = "https://localhost:6001/identity", headers = [authHeader], caseStrategy=CamelCase)
        let json = Encode.Auto.toString(4, claims)
        log([json])
    | _ -> log(["Log in first!"])
}
logoutButton.onclick <- fun _ ->
    mgr.signoutRedirect()

promise {
    let! user = mgr.getUser(): Fable.Core.JS.Promise<User option>
    match user with
    | Some u -> 
        let profile = Encode.toString 4 u.profile
        log(["User logged in:\n" + profile])
    | _ -> log(["User not logged in"])
} |> ignore
