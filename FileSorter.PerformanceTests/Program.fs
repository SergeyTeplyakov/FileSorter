// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open System.Linq

open FileSorter.PerformanceTests

[<EntryPoint>]
let main argv = 
    
//    MergeTests.measureMergeTwoSequences()
    MergeTests.measureMergeOf10Sequences()
    
//    let s = Enumerable.Range(1, 10)
//    let r = s.Zip(s, System.Func<_,_,_>(fun l r -> l)).Zip(s, System.Func<_,_,_>(fun l r -> l)).ToArray()
//    printfn "%A" r


    0 // return an integer exit code
