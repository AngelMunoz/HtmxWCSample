module HtmxWCSample.Program

open Giraffe
open Saturn
open Saturn.Endpoint
open HtmxWCSample.Views

let browserPipeline =
    pipeline {
        plug acceptHtml
        plug putSecureBrowserHeaders
        set_header "x-pipeline-type" "Browser"
    }

let htmlApiPipeline =
    pipeline {
        plug putSecureBrowserHeaders
        plug (requireHeader "HX-Request" "true")
        set_header "x-pipeline-type" "HtmlApi"
    }

let partialsPipeline =
    pipeline {
        plug putSecureBrowserHeaders
        plug (requireHeader "HX-Request" "true")
        set_header "x-pipeline-type" "Partials"
    }

let browserRouter =
    router {
        pipe_through browserPipeline
        get "/" (Home.Index())
        get "/index.html" (redirectTo false "/")
        get "/default.html" (redirectTo false "/")
        get "/server-tabs" (Home.Tabs())
    }

// basically a copy of the browserRouter, but returning partials instead of full pages
let partialsRouter =
    router {
        pipe_through partialsPipeline
        get "/" (Home.Index(true))
        get "/server-tabs" (Home.Tabs(true))
        getf "/tab-content/%s" Home.TabContent
    }


let apiRouter =
    router {
        pipe_through htmlApiPipeline
        post "/server-messages" (Messages.RandomMessage())
        post "/new-tab" (Tabs.NewTab())

    }

let appRouter =
    router {
        forward "" browserRouter
        forward "/partials" partialsRouter
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
