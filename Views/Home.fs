namespace HtmxWCSample.Views

open Giraffe
open Microsoft.AspNetCore.Http
open Giraffe.ViewEngine
open HtmxWCSample.BaseViews

module private Scripts =
    let private getFullPageScript (content: string) =
        js
            $"""
            window.addEventListener('DOMContentLoaded', () => {{
                {content}
            }});
            """

    let fsMessageClose (inPartial: bool) =
        let jsScript =
            let content =
                js
                    """
                    const msgContainer = document.querySelector("#message-container");

                    msgContainer.addEventListener('fs-close-message', (e) => {
                        e.target.remove();
                    });
                    """

            match inPartial with
            | false -> getFullPageScript content
            | true -> content

        script [ _defer ] [ rawText jsScript ]

    let fsTabClose (inPartial: bool) =
        let jsScript =
            let content =
                js
                    """
                    const page = document.querySelector(".page");
                    page.addEventListener('on-fs-tab-close', (e) => {
                        e.target.remove();
                    });
                    """

            match inPartial with
            | false -> getFullPageScript content
            | true -> content

        script [ _defer ] [ rawText jsScript ]

type private Partials() =
    static member Index(?extras: XmlNode list) =
        let extras = defaultArg extras []

        div [] [
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
            yield! extras
        ]

    static member Tabs(?extras: XmlNode list) =
        let extras = defaultArg extras []

        div [] [
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
            yield! extras
        ]

    static member TabForName(name: string) =
        article [ _class "tab-content" ] [
            str $"I'm the tab for {name}"
        ]

open Scripts
open type Partials

type Home() =
    static member Index(?isPartial: bool) : HttpHandler =
        (fun (next, (ctx: HttpContext)) ->
            match isPartial with
            | Some true ->
                ctx.SetHttpHeader("HX-Push", "/")

                Index([ fsMessageClose true ])
                |> RenderView.AsString.htmlNode
                |> htmlString
            | Some false
            | None ->
                Layout.Default([ Index() ], "Home", [ fsMessageClose false ])
                |> htmlView)
        |> warbler


    static member Tabs(?isPartial: bool) : HttpHandler =
        (fun (next, ctx: HttpContext) ->
            match isPartial with
            | Some true ->
                ctx.SetHttpHeader("HX-Push", "/server-tabs")

                Tabs([ fsTabClose true ])
                |> RenderView.AsString.htmlNode
                |> htmlString
            | Some false
            | None ->
                Layout.Default([ Tabs([ fsTabClose false ]) ], "Tabs", [ fsTabClose false ])
                |> htmlView)
        |> warbler

    static member TabContent(tabName: string) : HttpHandler =
        (fun _ ->
            TabForName(tabName)
            |> RenderView.AsString.htmlNode
            |> htmlString)
        |> warbler
