module Auth

open Fable.OidcClient
open Fable.Core.JsInterop
open Browser

let mgr: UserManager = Oidc.UserManager.Create !!{| response_mode = Some "query" |}

promise {
    console.log "mgr.signinRedirectCallback()"
    let! user = mgr.signinRedirectCallback()
    console.log (sprintf "%A" user)
    window.location.href <- "index.html"
} |> ignore
