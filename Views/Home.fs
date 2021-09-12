namespace HtmxWCSample.Views

open Giraffe
open Giraffe.ViewEngine
open HtmxWCSample.BaseViews

module private Home =
    let private navLinks =
        lazy
            ([ a [ _hxGet "/partials/server-tabs"
                   _hxTarget "main"
                   _class "navbar-item" ] [
                   str "Tabs Example"
               ] ])

    module Scripts =
        let fsMessageClose =
            let jsScript =
                js
                    """
                window.addEventListener('DOMContentLoaded', () => {
                    const msgContainer = document.querySelector("#message-container");

                    msgContainer.addEventListener('fs-close-message', (e) => {
                        e.target.remove();
                    });
                });
            """

            script [] [ rawText jsScript ]

        let fsTabClose =
            let jsScript =
                js
                    """
                window.addEventListener('DOMContentLoaded', () => {
                    const page = document.querySelector(".page");
                    page.addEventListener('on-fs-tab-close', (e) => {
                        e.target.remove();
                    });
                });
            """

            script [] [ rawText jsScript ]

    module Partials =
        let Index () =
            div [] [
                Partials.Navbar(leftLinks = navLinks.Value)
                article [ _class "page" ] [
                    section [ _id "message-container" ] []
                    header [] [
                        h1 [] [ str "Welcome to Saturn!" ]
                    ]
                    button [ _hxPost "/api/server-messages"
                             _hxTarget "#message-container"
                             _hxSwap "beforeend"
                             _class "button is-info" ] [
                        str "Send me some server messages!"
                    ]
                    p [] [
                        str
                            """
                          Saturn is an F# web framework for asp.net
                          """
                    ]
                ]
                Scripts.fsMessageClose
            ]

        let Tabs () =
            div [] [
                Partials.Navbar(leftLinks = navLinks.Value)
                article [ _class "page" ] [
                    fsTabHost [ _fsKind Info ] [
                        nav [ _slot "tabs"; _id "tab-bar" ] [
                            fsTabItem [ _isSelected
                                        _fsKind Primary
                                        _fsTabItemLabel "Music"
                                        _fsTabItemTabName "music"
                                        _hxGet "/partials/tab-content/music"
                                        _hxTarget "#tab-content" ] []
                            fsTabItem [ _fsKind Info
                                        _fsTabItemLabel "Settings"
                                        _fsTabItemTabName "settings"
                                        _hxGet "/partials/tab-content/settings"
                                        _hxTarget "#tab-content" ] []
                            button [ _class "is-small is-ghost button always-last"
                                     _hxPost "/api/new-tab"
                                     _hxSwap "beforebegin" ] [
                                str "Add Tab"
                            ]
                        ]
                        section [ _id "tab-content" ] []
                    ]
                ]
                Scripts.fsTabClose
            ]

        let TabForName (name: string) =
            article [ _class "tab-content" ] [
                str $"I'm the tab for {name}"
            ]

open Home

type Home() =
    static member Index(?isPartial: bool) : HttpHandler =
        fun next ctx ->
            match isPartial with
            | Some true -> warbler (fun _ -> htmlString (Partials.Index() |> RenderView.AsString.htmlNode)) next ctx
            | Some false
            | None -> htmlView (Layout.Default([ Partials.Index() ], "Home")) next ctx


    static member Tabs(?isPartial: bool) : HttpHandler =
        fun next ctx ->
            match isPartial with
            | Some true -> warbler (fun _ -> htmlString (Partials.Tabs() |> RenderView.AsString.htmlNode)) next ctx
            | Some false
            | None -> htmlView (Layout.Default([ Partials.Tabs() ], "Tabs")) next ctx

    static member TabContent(tabName: string) : HttpHandler =
        warbler
            (fun _ ->
                Partials.TabForName(tabName)
                |> RenderView.AsString.htmlNode
                |> htmlString)
