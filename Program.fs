module HtmxWCSample.Program

open Giraffe
open Saturn
open Saturn.Endpoint
open HtmxWCSample.Views

let browser =
    pipeline {
        plug acceptHtml
        plug putSecureBrowserHeaders
        plug fetchSession
        set_header "x-pipeline-type" "Browser"
    }

let htmlApi =
    pipeline {
        plug putSecureBrowserHeaders
        plug (requireHeader "HX-Request" "true")
        set_header "x-pipeline-type" "HtmlApi"
    }

let defaultView =
    router {
        get "/" (htmlView (Home.Index()))
        get "/index.html" (redirectTo false "/")
        get "/default.html" (redirectTo false "/")
    }

let browserRouter =
    router {
        pipe_through browser

        forward "" defaultView
    }

let apiRouter =
    router {
        pipe_through htmlApi
        post "/server-messages" (warbler (fun _ -> htmlView (Messages.RandomMessage())))
    }

let appRouter =
    router {
        forward "" browserRouter
        forward "/api" apiRouter
    }

[<EntryPoint>]
let main args =
    let app =
        application {
            use_developer_exceptions
            use_endpoint_router appRouter
            use_static "wwwroot"
        }

    run app
    0
