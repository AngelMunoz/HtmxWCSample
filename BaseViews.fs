namespace HtmxWCSample.BaseViews

open Giraffe.ViewEngine

module private PartialBaseContent =
    let augmentNodes (nodes: XmlNode list option) (defaults: XmlNode list) =
        nodes
        |> Option.map (fun extras -> List.concat [ defaults; extras ])
        |> Option.orElse (Some defaults)

    let NavBar =
        lazy
            (nav [ _class "navbar" ] [
                section [ _class "navbar-brand" ] [
                    a [ _class "navbar-burger" ] [
                        span [ attr "aria-hidden" "true" ] []
                        span [ attr "aria-hidden" "true" ] []
                        span [ attr "aria-hidden" "true" ] []
                    ]
                ]
                section [ _class "navbar-menu" ] [
                    section [ _class "navbar-start" ] [
                        a [ _hxGet "/partials/"
                            _hxTarget "main"
                            _class "navbar-item" ] [
                            str "Home"
                        ]
                        a [ _hxGet "/partials/server-tabs"
                            _hxTarget "main"
                            _class "navbar-item" ] [
                            str "Tabs Example"
                        ]
                    ]
                ]
             ])

    let OffCanvas (offCanvasContent: XmlNode list option) =
        let offCanvasContent = defaultArg offCanvasContent []

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
                    _hxTarget "main"
                    _class "navbar-item" ] [
                    str "Tabs Example"
                ]
            ]
            yield! offCanvasContent
        ]

    let DefaultStyles () : XmlNode list =
        [ link [ _rel "stylesheet"
                 _href "https://cdn.jsdelivr.net/npm/bulma@0.9.3/css/bulma.min.css" ]
          link [ _rel "stylesheet"
                 _href "/css/styles.css" ] ]

    let DefaultScripts () : XmlNode list =
        [ script [ _type "module"; _src "/js/index.js" ] [] ]

open PartialBaseContent

type Layout() =

    /// <summary>
    /// This is the default layout for this application.
    /// Most of the time it will be used only for the first load, anything after
    /// will be requested to the server.
    /// </summary>
    /// <param name="content">The content to be rendered inside the `main` tag</param>
    /// <param name="offCanvasContent">Extra nodes to render in the offcanvas piece.</param>
    /// <param name="styles">Any extra style that any particular page might require</param>
    /// <param name="scripts">any extra script that a particular page may require</param>
    /// <param name="_title">A title to assign to the document's head</param>
    static member Default
        (
            content: XmlNode list,
            ?_title: string,
            ?scripts: XmlNode list,
            ?styles: XmlNode list,
            ?offCanvasContent: XmlNode list
        ) =
        let _title = defaultArg _title "Welcome"

        let scripts =
            augmentNodes scripts (DefaultScripts())
            |> Option.defaultValue []

        let styles =
            augmentNodes styles (DefaultStyles())
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
                NavBar.Value
                OffCanvas offCanvasContent
                main [] [ yield! content ]
                yield! scripts
            ]
        ]

    static member NotFound(?content: XmlNode list) =
        Layout.Default(
            content
            |> Option.defaultValue [ NavBar.Value
                                     OffCanvas None
                                     str "Not Found" ]
        )
