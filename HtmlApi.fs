namespace HtmxWCSample

open Giraffe
open Giraffe.ViewEngine

[<RequireQualifiedAccess>]
module Tabs =
    let private random = System.Random()

    let private tabNames =
        [ "Tab 😁"
          "Tab ☕"
          "Tab 🌮"
          "Tab 🤣"
          "Tab 👀" ]

    let NewTab () : HttpHandler =
        warbler
            (fun _ ->
                let tabName () =
                    tabNames.[random.Next(0, tabNames.Length)]

                let kind () =
                    match random.Next(0, 7) with
                    | 0 -> Primary
                    | 1 -> Info
                    | 2 -> Link
                    | 3 -> Success
                    | 4 -> Warning
                    | 5 -> Danger
                    | _ -> Default

                let name = tabName ()

                fsTabItem [ _fsKind (kind ())
                            _fsTabItemLabel (name)
                            _fsTabItemTabName (name)
                            _hxGet $"/partials/tab-content/{name}"
                            _hxTarget "#tab-content"
                            _isClosable ] []
                |> RenderView.AsString.htmlNode
                |> htmlString)

[<RequireQualifiedAccess>]
module Messages =
    let private random = System.Random()

    let private headers =
        [ "I'm header 1"
          "I'm header 2"
          "I'm header 3"
          "I'm header 4" ]

    let RandomMessage () : HttpHandler =
        warbler
            (fun _ ->
                let header () =
                    headers.[random.Next(0, headers.Length)]

                let kind () =
                    match random.Next(0, 7) with
                    | 0 -> Primary
                    | 1 -> Info
                    | 2 -> Link
                    | 3 -> Success
                    | 4 -> Warning
                    | 5 -> Danger
                    | _ -> Default

                fsMessage [ _isOpen
                            _fsMsgHeader (header ())
                            _fsKind (kind ()) ] [
                    str "This might me something you need to look at"
                ]
                |> RenderView.AsString.htmlNode
                |> htmlString)
