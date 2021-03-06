[<AutoOpen>]
module Extensions

open Giraffe.ViewEngine

// get VSCode support for JavaScript strings with
// https://github.com/alfonsogarciacaro/vscode-template-fsharp-highlight
// https://marketplace.visualstudio.com/items?itemName=alfonsogarciacaro.vscode-template-fsharp-highlight
let js (js: string) = js

// add strong typing for your web components' attributes and tags

type OffcanvasPosition =
    | Left
    | Right

type Kind =
    | Primary
    | Info
    | Link
    | Success
    | Warning
    | Danger
    | Default

// register your tags
let fsOffCanvas = tag "fs-off-canvas"
let fsMessage = tag "fs-message"
let fsTabHost = tag "fs-tab-host"
let fsTabItem = tag "fs-tab-item"

// regiser your attributes

// HTMX helpers
let _hxGet = attr "hx-get"
let _hxPost = attr "hx-post"
let _hxPut = attr "hx-put"
let _hxPushUrl = attr "hx-push-url" "true"
let _hxDelete = attr "hx-delete"
let _hxTarget = attr "hx-target"
let _hxSwap = attr "hx-swap"

// fs-components attributes

let _isOpen = attr "is-open" ""
let _closable = attr "closable" ""
let _isSelected = attr "is-selected" ""
let _isClosable = attr "is-closable" ""


let _fsTabItemLabel = attr "label"
let _fsTabItemTabName = attr "tab-name"
let _fsMsgHeader = attr "header"
let _fsOffCanvasNoOverlay = attr "no-overlay" ""

// use type safety for you attributes rather than strings!
let _fsOffCanvasPosition (value: OffcanvasPosition) =
    let pos =
        match value with
        | Left -> "left"
        | Right -> "right"

    attr "position" pos

let _fsKind (value: Kind) =
    let kind =
        match value with
        | Primary -> "primary"
        | Info -> "info"
        | Link -> "link"
        | Success -> "success"
        | Warning -> "warning"
        | Danger -> "danger"
        | Default -> ""

    attr "kind" kind

let _slot = attr "slot"
