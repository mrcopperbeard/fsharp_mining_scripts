open System.IO
#r "./../lib/FSharp.Data.dll"
#r "./../lib/System.Text.Encoding.CodePages.dll"

open FSharp.Data

type Sex = Male | Female

type User = {
    Sex: Sex;
    Age: int;
}

type Answer = {
    Group: string;
    RawValue: int;
    Value: int;
}

let csvPath = @"C:\Source\F#\MiningScripts\AnxietyTest\tables"

type GroupValue = {
    Name: string;
    Value: int;
    Stinine: int;
}

let currentUser = { Sex = Female; Age = 20 }

let computingTable = CsvFile.Load(Path.Combine [|csvPath; "table.csv"|], ?separators=Some(" "), ?hasHeaders=Some(false))
let answersTable = CsvFile.Load(Path.Combine [|csvPath; "answers.csv"|], ?separators=Some(" "), ?hasHeaders=Some(false))

let stininesTable =
    match currentUser with
    | { Sex = Female; Age = age} when age < 21 -> "youngLadyStinines.csv"
    | _ -> "commonStinines.csv"
    |> fun fileName -> Path.Combine [|csvPath; fileName|]
    |> fun path -> CsvFile.Load(path, ?separators=Some(" "), ?hasHeaders=Some(false))

let getStinine (table: CsvFile) name value =
    table.Rows
    |> Seq.find (fun row -> (row.GetColumn 0) = name)
    |> fun row -> row.Columns
    |> Seq.findIndexBack (fun v -> v |> int <= value)

let getAnswer (answerRow:CsvRow, infoRow:CsvRow) =
    let rawValue = answerRow.[1] |> int

    let value =
        rawValue
        |> fun n -> infoRow.[n + 2]
        |> int

    {
        Group = infoRow.GetColumn 1;
        RawValue = rawValue;
        Value = value;
    }

let answers =
    computingTable.Rows
    |> Seq.zip answersTable.Rows 
    |> Seq.map (getAnswer)

let grouped =
    answers
    |> Seq.groupBy (fun a -> a.Group)
    |> Seq.map (fun (name, items) ->
        let value = items |> Seq.sumBy (fun item -> item.Value)
        {
            Name = name;
            Value = items |> Seq.sumBy (fun item -> item.Value);
            Stinine = getStinine stininesTable name value;
        })

grouped
|> Seq.iter (fun group -> printfn "%s: %d (%d станайн)" group.Name group.Value group.Stinine)