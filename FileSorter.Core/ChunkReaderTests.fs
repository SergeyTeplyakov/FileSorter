namespace FileSorter.Core

open System
open FileSorter.Core.ChunkReader
open System.Threading.Tasks

module ChunkReaderTests =

    let testBySingleChar() =
        let separator = "SEP"
        let data = "1234SEP2123SEP231".ToCharArray() |> Array.map(fun v -> String([|v|]))
        
        let parsed = parseList(data, separator)
        
        let d = parsed |> Seq.map(fun n -> n.ToString())
        let s = System.String.Join(", ", d)
        printfn "%s" s
        ()


    let testBySubStrings() =
        let separator = "SEP"
        let data = [|"123"; "4SE"; "P2"; "123"; "SE"; "P23"; "1"|]
        //let data = "1234SEP2123SEP231".ToCharArray() |> Array.map(fun v -> String([|v|]))
        
        let parsed = parseList(data, separator)
        
        let d = parsed |> Seq.map(fun n -> n.ToString())
        let s = System.String.Join(", ", d)
        printfn "%s" s
        ()

    let testBySubStringsWithSpaces() =
        let separator = " "
        let data = [|"123"; "4"; " "; "123"; " 23"; "1"|]
        
        let parsed = parseList(data, separator)
        
        let d = parsed |> Seq.map(fun n -> n.ToString())
        let s = System.String.Join(", ", d)
        printfn "%s" s
        ()

//    let toSeq(o: IObservable<'a>) : 'a seq =
//
//        o.Subscribe(
//            new IObserver<_> with
//                member OnCompleted() =
//                    
//        //Observable.subscribe(
//        seq {
//            
//        }

    let testReadChunksFull() =
        //let s = "14SEP23SEP2" + "7SEP11SEP1" + "4SEP31SEP233" + "1SEP46SEP12";
        let path = "C:\\Temp\\FS\\source.txt"
        use file = new System.IO.FileStream(path, System.IO.FileMode.Open)
        use tr = new System.IO.StreamReader(file)
        let chunkSize = 10000000
        file.Position = 0L |> ignore
        printfn "Starting..."

        readChunks tr chunkSize
            |> parseChunks ","
            //|> ObservableEx.collect(fun d -> d.ParsedData |> Seq.ofArray)
            |> Observable.map sortChunk
            // Transform sequence of non-finished tasks into the sequence of finished tasks
            |> ObservableEx.processOneByOne
            
            // Saving all this stuff to the files
            |> Observable.map(fun tsk -> tsk.Result)
            |> Observable.map(fun chk -> saveChunkToFile(", ") (chk))
            
            // Chunks are saved
            |> ObservableEx.processOneByOne
            |> Observable.map(fun tsk -> tsk.Result)
            |> Observable.map(fun d -> 
                
                //printfn "in map"
                //System.String(d.Data, 0, d.Length))
                sprintf "Number: %d, FileName: %s" (d.Number) (d.FileName))
                //String.Join(", ", d.SortedData |> Seq.map(fun n -> n.ToString())))
            |> ObservableEx.toSeq
            |> Seq.iter(fun v -> printfn "%s %s" (DateTime.Now.ToLongTimeString()) v)
        ()



    let test_save_chunk_async() =
        let s = [| {SortedData = [|1; 5; 21|] };
                   {SortedData = [|6; 12; 19|] };
                   {SortedData = [|2; 3; 11|] }; |]

        let r = s 
                |> Seq.map(fun chk -> (chk, new System.IO.StreamWriter(new System.IO.MemoryStream()), getNextFileName()))
                |> ObservableEx.fromSeq 
                |> Observable.map(fun (chk, sw, fn) -> ((saveChunk ", " chk sw fn), sw))
                |> ObservableEx.processOneByOne2(fun (tsk, sw) -> tsk)
                |> ObservableEx.toSeq
                |> Seq.map(fun (tsk, sw) -> sw.ToString())
                |> Seq.toArray
        
        // TODO: add test for this stuff!! that output sequence contains all input data!
        printfn "%s" (String.Join("; ", r))
        //|> ChunkReader.saveChunk(fun tp -> tp.)
        ()

    let prepareTestFile() =
        let rnd = Random(1)
        let s = seq {
            for i in 1 .. 10000000 -> rnd.Next()
            //for i in 1 .. 100 -> rnd.Next(1000000)

        }
        let path = "C:\\Temp\\FS\\source.txt"
        use file = new System.IO.FileStream(path, System.IO.FileMode.Create)
        use tw = new System.IO.StreamWriter(file)

        s |> Seq.iteri(fun i n -> 
            if i <> 0 then tw.Write(","); 
            tw.Write(n);)

        ()

    let testReadChunks() =
        let s = "14SEP23SEP2" + "7SEP11SEP1" + "4SEP31SEP233" + "1SEP46SEP12";
        use tr = new System.IO.StringReader(s)

        
        readChunks tr 11
            |> parseChunks "SEP"
            //|> ObservableEx.collect(fun d -> d.ParsedData |> Seq.ofArray)
            |> Observable.map sortChunk
            // Transform sequence of non-finished tasks into the sequence of finished tasks
            |> ObservableEx.processOneByOne
            
            // Saving all this stuff to the files
            |> Observable.map(fun tsk -> tsk.Result)
            |> Observable.map(fun chk -> saveChunkToFile ", " chk)
            
            |> ObservableEx.processOneByOne
            
            |> Observable.map(fun tsk -> 
                sprintf "Number: %d, FileName: %s" (tsk.Result.Number) (tsk.Result.FileName))
//            |> Observable.map(fun d -> 
//                
//                //printfn "in map"
//                //System.String(d.Data, 0, d.Length))
////                sprintf "Number: %d, FileName: %s" (d.) (d.FileName))
//                String.Join(", ", d.SortedData |> Seq.map(fun n -> n.ToString())))
//            |> ObservableEx.toSeq
            |> ObservableEx.toSeq
            |> Seq.iter(fun v -> printfn "%s %s" (DateTime.Now.ToLongTimeString()) v)
            //|> Observable.subscribe(fun s -> printfn "%s" s)
            //|> ignore
//        readChunks tr 3
//            |> parseChunks "SEP"
//            |> ObservableEx.collect(fun d -> d.ParsedData |> Seq.ofArray)
//            |> Observable.map(fun d -> 
//                
//                //printfn "in map"
//                //System.String(d.Data, 0, d.Length))
//                d.ToString())
//            |> Observable.subscribe(fun s -> printfn "%s" s)
//            |> ignore

//        System.Threading.Thread.Sleep(10000)
        
        ()


    let foo() =
        let e = new Event<_>()

        e.Trigger(42)
        //let lst = System.Collections.Generic.List<_>()
//        lst.Add(42)
//        lst.Add("foo")
        
        e

        //()

    let boo() =
        let e = foo()
        
        


        ()


    type CustomType() =
        let e = new Event<_>()

        member x.Fire() =
            e.Trigger 42

        member x.E = e.Publish
