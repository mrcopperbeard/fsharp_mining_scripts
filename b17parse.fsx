#r "./lib/FSharp.Data.dll"
#r "./lib/System.Text.Encoding.CodePages.dll"

open System.Text
open FSharp.Data

type Person = {
    Position : int
    Name : string
}

let nodeToPerson index (node : HtmlNode) : Person =
    let name =
        node
        |> HtmlNode.descendants false (fun div -> HtmlNode.hasClass "text" div)
        |> Seq.head
        |> HtmlNode.descendantsNamed false [ "a" ]
        |> Seq.head
        |> HtmlNode.innerText

    {
        Position = index
        Name = name
    }

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

let persons =
    HtmlDocument.Load("https://www.b17.ru/psiholog/sankt-peterburg/?page_plus=300", Encoding.GetEncoding("windows-1251"))
    |> HtmlDocument.descendants true (fun body -> HtmlNode.hasId "items_list_main" body)
    |> Seq.head
    |> HtmlNode.descendants true (fun node -> HtmlNode.hasId "items_list" node)
    |> Seq.head
    |> HtmlNode.descendants true (fun node -> HtmlNode.hasName "div" node && HtmlNode.hasClass "list" node)
    |> Seq.mapi nodeToPerson

// persons
//     |> Seq.iter (fun person -> printfn "Name is %s" person.Name)

persons
    |> Seq.tryFind (fun person -> person.Name.Contains("Ильина"))
    |> function
        | Some person -> printfn "Нашли: %s, позиция %d" person.Name person.Position
        | None -> printfn "Не нашли"
