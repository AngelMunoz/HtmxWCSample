namespace HtmxWCSample.BaseViews

open Giraffe.ViewEngine

[<RequireQualifiedAccess>]
module private Html =
    let augmentNodes (nodes: XmlNode list option) (defaults: XmlNode list) =
        nodes
        |> Option.map (fun extras -> List.concat [ defaults; extras ])
        |> Option.orElse (Some defaults)

type Partials() =
    static member Navbar(?attrs: XmlAttribute list, ?leftLinks: XmlNode list, ?rightLinks: XmlNode list) =
        let leftLinks =
            Html.augmentNodes
                leftLinks
                [ a [ _hxGet "/partials/"
                      _hxTarget "main"
                      _class "navbar-item" ] [
                      str "Home"
                  ] ]

        let attrs = defaultArg attrs []
        let left = defaultArg leftLinks []
        let right = defaultArg rightLinks []

        nav [ yield! attrs; _class "navbar" ] [
            section [ _class "navbar-brand" ] [
                a [ _class "navbar-burger" ] [
                    span [ attr "aria-hidden" "true" ] []
                    span [ attr "aria-hidden" "true" ] []
                    span [ attr "aria-hidden" "true" ] []
                ]
            ]
            section [ _class "navbar-menu" ] [
                section [ _class "navbar-start" ] [
                    yield! left
                ]
                section [ _class "navbar-end" ] [
                    yield! right
                ]
            ]
        ]

type Layout() =

    static member DefaultStyles() : XmlNode list =
        [ link [ _rel "stylesheet"
                 _href "https://cdn.jsdelivr.net/npm/bulma@0.9.3/css/bulma.min.css" ]
          link [ _rel "stylesheet"
                 _href "/css/styles.css" ] ]

    static member DefaultScripts() : XmlNode list =
        [ script [ _type "module" ] [
            rawText (
                js
                    """
                    import { registerAll } from 'https://cdn.skypack.dev/pin/fsharp-components@v0.0.4-vrcdq1fAiAHCYER7N99I/mode=imports,min/optimized/fsharp-components.js';
                    registerAll();
                    """
            )
          ]
          script [ _type "module"; _src "/js/index.js" ] [] ]

    static member Default(content: XmlNode list, ?_title: string, ?scripts: XmlNode list, ?styles: XmlNode list) =
        let _title = defaultArg _title "Welcome"

        let scripts =
            Html.augmentNodes scripts (Layout.DefaultScripts())
            |> Option.defaultValue []

        let styles =
            Html.augmentNodes styles (Layout.DefaultStyles())
            |> Option.defaultValue []


        html [ _lang "en-US" ] [
            head [] [
                meta [ _charset "utf-8" ]
                meta [ _name "viewport"
                       _content "width=device-width, initial-scale=1" ]
                title [] [ str $"{_title} | Saturn" ]
                yield! styles
                // Add HTMX
                script [ _type "module"
                         _src "https://unpkg.com/htmx.org@1.5.0" ] []
                // Add Constructed Stylesheets Polyfill for fs-components
                script [ _type "module"
                         _src "https://unpkg.com/construct-style-sheets-polyfill" ] []
            ]
            body [] [
                /// use our helper tags from `Extensions.fs`
                fsOffCanvas [ _closable ] [
                    h3 [ _slot "header-text" ] [
                        str "Web Components from the server"
                    ]
                    h4 [] [
                        str "Enhanced on the client side!"
                    ]
                    ul [] [
                        a [ _hxGet "/partials/"
                            _hxTarget "main"
                            _class "navbar-item" ] [
                            str "Home"
                        ]
                        a [ _hxGet "/partials/server-tabs"
                            _hxTarget "main" ] [
                            str "Tabs Example"
                        ]
                    ]
                    ul [] [
                        li [] [ str "Content!" ]
                        li [] [ str "From the server!" ]
                        li [] [ str "Neat right?!" ]
                    ]
                ]
                main [] [ yield! content ]
                yield! scripts
            ]
        ]

    static member NotFound(?content: XmlNode list) =
        Layout.Default(
            content
            |> Option.defaultValue [ Partials.Navbar()
                                     str "Not Found" ]
        )
