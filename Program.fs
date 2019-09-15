// Learn more about F# at http://fsharp.org

open System
open CommandLine
open System.IO
open FSharp.Text.Lexing

type options = {
  [<Option('r', "read", HelpText = "Input files.")>] files : seq<string>;
  [<Option(HelpText = "Prints all messages to standard output.")>] verbose : bool;
}

let parseTextPrint text = 
    let lexbuf = LexBuffer<char>.FromString text
    try 
       let result = Parser.Expr Lexer.tokenstream lexbuf
       printfn "Expr=%A" result
    with
       | _ as e ->
         printfn "Exception=%A" e
         ()

let parseTextPrintFile file =
    let lines = File.ReadAllLines file
    Array.iter (fun s -> parseTextPrint s) lines

let rec evalLoop () =
    let text = Console.ReadLine()
    parseTextPrint text
    evalLoop ()


[<EntryPoint>]
let main argv =
  let result = CommandLine.Parser.Default.ParseArguments<options>(argv)
  match result with
  | :? Parsed<options> as parsed ->
      let files = parsed.Value.files
      if (Seq.length files) > 0 then
         Seq.iter (fun f -> parseTextPrintFile f ) files
      else
          evalLoop ()
      1
  | :? NotParsed<options> as notParsed ->
      printfn "%s" <| notParsed.Errors.ToString()
      1
  | _ ->
      1

