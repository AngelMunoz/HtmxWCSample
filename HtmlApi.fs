namespace HtmxWCSample


open Giraffe.ViewEngine

[<RequireQualifiedAccess>]
module Messages =
    let private random = System.Random()

    let private headers =
        [ "I'm header 1"
          "I'm header 2"
          "I'm header 3"
          "I'm header 4" ]

    let RandomMessage () =
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
