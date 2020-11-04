module Auth

open Fable.OidcClient
open Fable.Core.JsInterop
open Browser

let mgr: UserManager = Oidc.UserManager.Create !!{| response_mode = Some "query" |}

promise {
    let! user = mgr.signinRedirectCallback()
    window.location.href <- "index.html"
} |> ignore
