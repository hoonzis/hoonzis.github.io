open System.IO
open System.Text
open System.Text.RegularExpressions

let toReplace = [
    (@"\*\\","")
    (@"\\(\n|\r|\r\n)","")
    ("{.brush:.*csharp}","")
    ("{.brush:.*java}","")
    ("{.brush:.*xml}","")
    ("<div(.*)>","")
    ("</div>","")
    ("</?span(.*)>","")
    ("%20","")
    ("{.prettyprint}","")
    ("%}\)","}\)")
    ("{%post","{post")
    ("{.brush:prettyprint}","")
    (" {.prittyprint}","")
    (" {.brush: .py; .auto-links: .true; .collapse: .false; .first-line: .1; .gutter: .true; .html-script: .false; .light: .false; .ruler: .false; .smart-tabs: .true; .tab-size: .4; .toolbar: .true;}","")
]

let startProcess fileName output=
    let p = new System.Diagnostics.Process();

    p.StartInfo.FileName <- "pandoc";
    p.StartInfo.Arguments <- sprintf "-f html -t markdown %s -o %s" fileName output
    p.StartInfo.UseShellExecute <- false
    p.Start() |> ignore
    p.WaitForExit()

let createMdFilesFromHtml files =
    files |> Seq.iter (fun filename ->
        let content = File.ReadAllText(filename)
        let outputFile = filename.Replace(".html",".md")
        let yamlHeader = Regex.Match(content,"(?s)---\s.*---")

        if yamlHeader.Success then
            printfn "Found header: %s" yamlHeader.Value
            let withtoutHeader = Regex.Replace(content,"(?s)---.*---","")

            let modifiedHtml = filename.Replace(".html","_modified.html")
            File.WriteAllText(modifiedHtml,withtoutHeader)

            startProcess modifiedHtml outputFile
            let mdContent = File.ReadAllText(outputFile)

            let replaced = toReplace |> Seq.fold (fun v acc ->
                let testMatch = Regex.Match(v,fst(acc))
                if testMatch.Success then
                    printfn "Found: %s" testMatch.Value
                Regex.Replace(v,fst(acc),snd(acc))) mdContent

            let withHeader = yamlHeader.Value + "\n" + replaced

            File.WriteAllText(outputFile,withHeader)
            File.Delete filename
            File.Delete modifiedHtml
        else
            printfn "Header not found"
    )


let generateLinkingTemplateCode filesDirectory =

    let files = Directory.GetFiles(filesDirectory, ".html",SearchOption.AllDirectories)

    let addToTemplate acc filename =
        let parsedValues = Regex.Match(filename,"(\d{4})-(\d{2})-(\d{2})-(.*).html")
        let year = parsedValues.Groups.[1].Value
        let month = parsedValues.Groups.[2].Value
        let name = parsedValues.Groups.[4].Value

        printfn "Year: %s month: %s name:%s" year month name

        let originalUrl = sprintf "http://hoonzis.blogspot.com/%s/%s/%s.html" year month name
        let newUrl = sprintf "http://www.hoonzis.com/%s" name

        let ifStatement = match acc with
                            | "" -> "<b:if",""
                            | _ -> "<b:elseif","/"


        let template = sprintf "%s cond='data:blog.canonicalUrl == \"%s\"'%s>\n\t<link rel=\"canonical\" href=\"%s\"/>\n\t<meta http-equiv=\"refresh\" content=\"0; url=%s\"/>\n" (fst ifStatement) originalUrl (snd ifStatement) newUrl newUrl
        sprintf "%s\n%s" acc template

    let templatesPrefix = sprintf "%s\n</b:if>" (files |> Seq.fold addToTemplate "")
    File.WriteAllText(@"C:\\temp\\xmlprefix.xml",templatesPrefix)


let filesDirectory = "d:\\projects\\hoonzis.github.io\\bloggerfiles"
let files = Directory.GetFiles(filesDirectory)
let geturlmap acc filename =
    let parsedValues = Regex.Match(filename,"(\d{4})-(\d{2})-(\d{2})-(.*).html")
    let year = parsedValues.Groups.[1].Value
    let month = parsedValues.Groups.[2].Value
    let name = parsedValues.Groups.[4].Value

    printfn "Year: %s month: %s name:%s" year month name

    let originalUrl = sprintf "http://hoonzis.blogspot.com/%s/%s/%s.html" year month name
    let newUrl = sprintf "http://www.hoonzis.com/%s/" name
    sprintf "%s\n%s, %s" acc originalUrl newUrl

let completeMap = files |> Seq.fold (geturlmap) ""
File.WriteAllText(@"C:\\temp\\urlmap.csv",completeMap)
