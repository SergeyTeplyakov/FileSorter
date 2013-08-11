namespace FileSorter.Core

open System
open System.IO
open System.Diagnostics.Contracts
open System.Threading.Tasks


(*

General approach

  IObservable              IObservable     /------->                      ------>   IObservable           IObservable
  or raw chunks          of parsed chunks / IObservable of Task of Result        \ of Task of R           of Result (one by one)
  ------------->(Transform)-------------->---------->(Parallel processing)-------->------------->(Transform) --------> Then Parallel Save (with other degree of parallelism)
                                         \                                       /
                                          \-------->                      ------>

*)



module ChunkReader =

//    let readChunks (fileName: string, chunkSize: int) =
//        use fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, chunkSize * 2, useAsync = true)
//        
//        ()
    
    let split<'a> (idx: int) (a: 'a array) =
        Contract.Requires(idx < a.Length, "idx should be less than array length")
        (Array.sub a 0 (idx - 1), Array.sub a (idx - 1) (a.Length - idx + 1))

    let isInt32 s =
        fst (Int32.TryParse s)

    /// Appends two arrays, but uses specified length for second array.
    let appendArray (lhs: 'a array) (rhs: 'a array) (rhsLen: int) =
        let res = Array.zeroCreate<'a> (lhs.Length + rhsLen)
        Array.Copy(lhs, res, lhs.Length)
        Array.Copy(rhs, 0, res, lhs.Length, rhsLen)
        res

    /// Parses specified array by separator, returns sequence of ints and reminder.
    /// For example, if we'll try to parse "123sep12sep21" with "sep" as a seprator
    /// this function should return 123, 12 and 21 as a reminder.
    let parse (separator: string) (data: char array) : (int seq * char array) =
        // ctor(separator)
        // IEnumerable<int> Parse(char[])
        // 
        // Tuple<IEnumerable<int>, char[]> Parse(string separator, data char[])
        // TODO: naive implementation!
        let s = String(data)
        
        let strings = s.Split([|separator|], StringSplitOptions.None)
        
        // Last element of strings contains a reminder for parsing next time
        let (toParse, reminder) = split strings.Length strings
            
        // Converting first part to ints, and reminder to char array
        (toParse |> Seq.map Int32.Parse, reminder |> Array.collect(fun r -> r.ToCharArray()))

    let parseList(data: string array, separator: string) : int seq =
        
        let parseSep = parse separator
        //let r = Array.scan (fun s e -> s) Array.empty<char> data
        seq {
            let tmp = ref Array.empty<char>
            for v in data do
                let r = parseSep (Array.append !tmp (v.ToCharArray()))
                tmp := snd r
                yield! fst r

            let tmpString = String(tmp.Value)
            if isInt32 tmpString then
                yield Int32.Parse(tmpString)
        }

    /// Reads chunks with specified size from the text reader.
    /// Produces "stream" of parsed chunks.
    let readChunks (tr: TextReader) (chunkSize) : IObservable<RawChunk> =
        let e = Subject<_>()

        

        let a = async {
            
            // Waiting for at least one subscriber
            // TODO this is hack but I don't know how to solve this in other way!
            // Maybe there is a way for generic approach to wait for the first observer!
            let! r = Async.AwaitWaitHandle e.HasSubscribers
            
            let (buffer: char array) = Array.zeroCreate chunkSize
            let cond = ref true
            
            // TODO: how to rewrite this code with functional style?
            while cond.Value do
                //printfn "starting reading stuff..."
                let tsk = tr.ReadBlockAsync(buffer, 0, Array.length buffer)
                //let tsk = Task.Run(fun _ -> System.Threading.Thread.Sleep(1000); 42)
                let! (r: int) = Async.AwaitTask(tsk)
                //printfn "task finished? %s" (tsk.Status.ToString())
                //printfn "read some stuff %d..." r
                // ReadBlock will return 0 when it reaches end of the stream
                cond := r <> 0

                if r <> 0 then
                    let chunk = {Data = buffer; Length = r}
                    e.Next(chunk)
                else
                    // Notify that sequence completed
                    e.Completed()
        }
        Async.Start a

        e :> IObservable<_>


//    let readStream (tr: Stream) (chunkSize) : IObservable<RawChunk> =
//        let e = Subject<_>()
//
//        
//
//        let a = async {
//            
//            // Waiting for at least one subscriber
//            // TODO this is hack but I don't know how to solve this in other way!
//            // Maybe there is a way for generic approach to wait for the first observer!
//            let! r = Async.AwaitWaitHandle e.HasSubscribers
//            
//            let (buffer: byte array) = Array.zeroCreate chunkSize
//            let cond = ref true
//            
//            // TODO: how to rewrite this code with functional style?
//            while cond.Value do
//                printfn "starting reading stuff..."
//                let tsk = tr.ReadAsync(buffer, 0, Array.length buffer)
//                //let tsk = Task.Run(fun _ -> System.Threading.Thread.Sleep(1000); 42)
//                let! (r: int) = Async.AwaitTask(tsk)
//                printfn "read some stuff %d..." r
//                // ReadBlock will return 0 when it reaches end of the stream
//                cond := r <> 0
//
//                if r <> 0 then
//                    let chunk = {Data = buffer; Length = r}
//                    e.Next(chunk)
//                else
//                    // Notify that sequence completed
//                    e.Completed()
//        }
//        Async.Start a
//
//        e :> IObservable<_>

    let parseChunks (separator: string) (source: IObservable<RawChunk>) : IObservable<ParsedChunk> =
        
        let parseSep = parse separator
        
        // TODO: I don't know how to solve this in functional way!!
        let reminder = ref Array.empty<char>
        let s = Subject<_>()
        
        source.Subscribe(
            {new IObserver<RawChunk> with
                
                member x.OnCompleted() =
                    let reminder = String(!reminder)
                    if isInt32 reminder then
                        s.Next({ParsedData = [|Int32.Parse reminder|]})
                    s.Completed()
                
                member x.OnError e =
                    s.Error e

                member x.OnNext(rc) =
                    //System.Threading.Thread.Sleep(1000)
                    // TODO: add exception handling and call to OnError (BTW should we stop our processing in this way?)
                    let toParse = appendArray !reminder rc.Data rc.Length
                    let r = parseSep (toParse)
                    reminder := snd r
                    let parsedData = {ParsedData = (fst r) |> Seq.toArray}
                    
                    if parsedData.ParsedData.Length <> 0 then
                        s.Next(parsedData)
                    })
            |> ignore

        s :> IObservable<_>


    let sortChunk (chunk) =
        Task.Run(fun d -> {SortedData = MergeSort.insertionSort(Seq.ofArray chunk.ParsedData) |> Seq.toArray})


    /// Transforms sorted chunk into the sequence of chunks prepared for saving
    let prepareForSaving (separator: string) (prepChunkSize: int) (chunk: SortedChunk) : PreparedChunk =
        // 1mb took 1 second
        let sw = System.Diagnostics.Stopwatch.StartNew()
        let encoding = System.Text.ASCIIEncoding()
        let encoder = System.Text.ASCIIEncoding.Default.GetEncoder();
//        let bufferManager = System.ServiceModel.Channels.BufferManager.CreateBufferManager(50*1000*1000L, prepChunkSize)
//        let buffer = bufferManager.
//        let separator2 = separator.Chars

// TODO: this implementation is much slower!!!! maybe implementation without closures 
//        let ar = Array.zeroCreate<byte>(chunk.SortedData.Length * 14)
//        let separator' = separator.ToCharArray()
//        let position = ref 0
//        let charUsed = ref 0
//        let bytesUsed = ref 0
//        let completed = ref false
//
//        let encode (c: char array) (pos: int) =
//            encoder.Convert(c, 0, c.Length, ar, pos, ar.Length - pos, false, charUsed, bytesUsed, completed)
//            pos + bytesUsed.Value
//
//        let (_, bytesUsed) = 
//            chunk.SortedData
//                // Transform every int to char array
//                |> Array.map(fun d -> d.ToString().ToCharArray())
//                // Encode every
//                |> Array.fold(fun (isFirst, pos) elem -> 
//                    // Encode separator for non-first elements
//                    let pos' = if isFirst = true then pos else encode separator' pos
//                    // Encode content
//                    let pos' = encode elem pos'
//                    (false, pos')) (true, 0)
        
                    
            //|> Array.iteri(fun (idx, data) -> 
//        chunk.SortedData 
//            |> Array.map(fun d -> d.ToString().ToCharArray())
//            |> Array.map(fun ca -> )
        //encoder.Convert(
        let s = String.Join(separator, chunk.SortedData |> Seq.map(fun d -> d.ToString()))
        let buffer = encoding.GetBytes(s)
        let r = {PreparedData = buffer; Length = buffer.Length}
        //let r = {PreparedData = ar; Length = bytesUsed}
        sw.Stop()
        printfn "prepare for saving took: %d ms" (sw.ElapsedMilliseconds)
        r
//        encoder.Convert(
        //let outputBuffer = Array

    let saveChunk (separator: string) (chunk: SortedChunk) (sw: System.IO.StreamWriter) (fn: string) : Task<SavedChunk> =
        // TODO: this code is too imperative!!
        let a = async {
            let preparedChunk = prepareForSaving separator (10*1000*1000) chunk
            let tsk = sw.BaseStream.WriteAsync(preparedChunk.PreparedData, 0, preparedChunk.Length)
            do! Async.AwaitIAsyncResult(tsk) |> Async.Ignore
            do! Async.AwaitIAsyncResult(sw.FlushAsync()) |> Async.Ignore
            return {Number = preparedChunk.PreparedData.Length; FileName = fn}
        }

        Async.StartAsTask a

    let chunkNum = ref 0

    let getNextFileName() =
        "C:\\Temp\\FS\\tmp_" + System.Threading.Interlocked.Increment(chunkNum).ToString() + ".chk"


        

    let saveChunkToFile (separator: string) (chunk: SortedChunk) : Task<SavedChunk> =
        let fileName = getNextFileName()
        let file = new System.IO.FileStream(fileName, FileMode.Create)
        let tw = new System.IO.StreamWriter(file)
        let r = saveChunk separator chunk tw fileName
        r.ContinueWith(fun (t:Task<SavedChunk>) -> file.Dispose(); tw.Dispose()) |> ignore
        r


    






