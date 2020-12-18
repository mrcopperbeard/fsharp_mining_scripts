#r "./lib/FSharp.Data.dll"
#r "./lib/System.Text.Encoding.CodePages.dll"

open System.Text
open FSharp.Data

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

type Episode = {
    Title: string;
    IsAvailable: bool;
}

let parse (item: HtmlNode) =
    let title = 
        item
        |> HtmlNode.descendantsNamed false [ "td" ]
        |> Seq.item 2
        |> HtmlNode.innerText

    { Title = title; IsAvailable = item.HasClass ""; }

let log episode =
    let available =
        match episode.IsAvailable with
        | true -> "доступна"
        | false -> "недоступна"
    printfn "%s %s" episode.Title available

let fetch () = 
    HtmlDocument.Load("https://www.lostfilm.tv/series/The_Mandalorian/seasons/")
    |> HtmlDocument.descendants true (fun body -> HtmlNode.hasClass "movie-parts-list" body)
    |> Seq.head
    |> HtmlNode.descendantsNamed false [ "tr" ]
    |> Seq.map parse

let check () =
    let items = fetch ()
    items |> Seq.iter log
    items |> Seq.length

let mutable availableSeries: Option<int> = None;
let mutable alarm = false;

while not alarm do
    System.DateTime.Now.ToString ("HH:mm:ss")
    |> printfn "%s Проверка..."
    let currentAvailable = check ()
    alarm <- availableSeries.IsSome && currentAvailable > availableSeries.Value
    System.Threading.Thread.Sleep(30_000)
    System.Console.Clear()

System.DateTime.Now.ToString ("HH:mm:ss")
|> printfn "\r\n\r\n\r\n%s Кажется вышла новая серия!"