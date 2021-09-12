module HtmxWCSample.Program

open Giraffe
open Saturn
open Saturn.Endpoint
open HtmxWCSample.Views

let browser =
    pipeline {
        plug acceptHtml
        plug putSecureBrowserHeaders
        set_header "x-pipeline-type" "Browser"
    }

let htmlApi =
    pipeline {
        plug putSecureBrowserHeaders
        plug (requireHeader "HX-Request" "true")
        set_header "x-pipeline-type" "HtmlApi"
    }

let partialPipeline =
    pipeline {
        plug putSecureBrowserHeaders
        plug (requireHeader "HX-Request" "true")
        set_header "x-pipeline-type" "Partials"
    }

let browserRouter =
    router {
        pipe_through browser
        get "/" (Home.Index())
        get "/index.html" (redirectTo false "/")
        get "/default.html" (redirectTo false "/")
        get "/server-tabs" (Home.Tabs())
    }

// basically a copy of the browserRouter, but returning partials instead of full pages
let partials =
    router {
        pipe_through partialPipeline
        get "/" (Home.Index(true))
        get "/server-tabs" (Home.Tabs(true))
        getf "/tab-content/%s" Home.TabContent
    }


let apiRouter =
    router {
        pipe_through htmlApi
        post "/server-messages" (Messages.RandomMessage())
        post "/new-tab" (Tabs.NewTab())

    }

let appRouter =
    router {
        forward "" browserRouter
        forward "/partials" partials
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
