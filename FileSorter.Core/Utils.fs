namespace FileSorter.Core

open System.Collections.Generic

module Utils =
        
    let withMeasure f a =
        let sw = System.Diagnostics.Stopwatch.StartNew()
        f a
        sw.Stop()
        printfn "Elapsed %dms" (sw.ElapsedMilliseconds)

    let memoize fn arg =
        let d = Dictionary<_, _>()
        
        let (k, b) = d.TryGetValue(arg)

        match d.TryGetValue(arg) with
        | true, cached -> cached
        | false, _ -> 
            let res = fn arg
            d.Add(arg, res)
            res

    let cachedSome<'a when 'a : equality> () : ('a -> 'a option) =
        let d = Dictionary<_,_>()
        fun x ->
            //printfn "count: %d" (d.Count)

            match d.TryGetValue(x) with
            | true, cached -> cached
            | false, _ ->
                let r = Some(x)
                //printfn "adding to the dictionary"
                d.Add(x, r)
                r


