namespace HtmxWCSample.Views

open Giraffe.ViewEngine
open HtmxWCSample.BaseViews

[<RequireQualifiedAccess>]
module Home =

    let Index () =
        let content =
            [ Partials.Navbar(
                leftLinks =
                    [ a [ _href "/todos"; _class "navbar-item" ] [
                          str "Check the Todos!"
                      ] ]
              )
              article [ _class "page" ] [
                  section [ _id "message-container" ] []
                  header [] [
                      h1 [] [ str "Welcome to Saturn!" ]
                  ]
                  button [ attr "hx-post" "/api/server-messages"
                           attr "hx-target" "#message-container"
                           attr "hx-swap" "beforeend"
                           _class "button is-info" ] [
                      str "Send me some server messages!"
                  ]
                  p [] [
                      str
                          """
                          Saturn is an F# web framework for asp.net
                          """
                  ]
              ] ]

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

        Layout.Default(content, "Home", [ script [] [ rawText jsScript ] ])
