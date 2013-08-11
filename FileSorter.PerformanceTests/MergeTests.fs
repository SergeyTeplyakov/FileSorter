namespace FileSorter.PerformanceTests

open System.Linq
open System.Diagnostics
open FileSorter.Core

module MergeTests =

    let mergeAndMeasure fn =
        let sw = Stopwatch.StartNew()
        let r = fn()
        sw.Stop()
        (r, sw.Elapsed)
        
    //-----------------------------------------------------------------------------------------------
    // Merge two sequences
    //-----------------------------------------------------------------------------------------------
    let measureMergeTwoSequences() =
        let seqLength = 10*1000*1000
        let seq1 = Enumerable.Range(1, seqLength)

        let mergeMethods = 
            [|(MergeSort.imperativeMerge, "imperative merge"); // 25 - release
              (MergeSort.recursiveMerge, "recursive merge"); // 34 - release
              (MergeSort.unfoldMerge, "unfold merge")|] // 32 - release

        printfn "testing %d merge implementations..." mergeMethods.Length
        
        // Warming up merge functions
        printfn "warming up..."
        mergeMethods |> Array.iter(fun (m, name) -> m (Seq.empty<int>) (Seq.empty<int>) |> ignore)
        
        printfn "start testing..."
        mergeMethods 
            |> Array.iter(fun (m, name) -> 
                printf "%s started for %d elements..." name seqLength
                let sw = Stopwatch.StartNew()
                let count = (m seq1 seq1) |> Seq.length
                sw.Stop()
                printfn "done for %O" (sw.Elapsed)
                )

        printfn "done."

    //-----------------------------------------------------------------------------------------------
    // Merge 10 sequences
    //-----------------------------------------------------------------------------------------------
    let measureMergeOf10Sequences() =
        
        let seqCount = 10
        let seqLength = 1 * 1000 * 1000
        let sequence = Enumerable.Range(1, seqLength)
        let sequences = Enumerable.Repeat(sequence, seqCount).ToArray()

        let mergeMethods = 
            [|(MergeSort.imperativeMergeSequences, "imperative merge");
              (MergeSort.unfoldMergeSequences, "unfold merge")|]

        printfn "testing %d merge implementations..." mergeMethods.Length
        
        // Warming up merge functions
        printfn "warming up..."
        let emptySequences = Enumerable.Repeat(sequence, seqCount).ToArray();
        mergeMethods |> Array.iter(fun (m, name) -> m (emptySequences) |> ignore)
        
        printfn "start testing..."
        mergeMethods 
            |> Array.iter(fun (m, name) -> 
                printf "%s started for %d elements..." name seqLength
                let sw = Stopwatch.StartNew()
                let count = (m sequences) |> Seq.length
                sw.Stop()
                printfn "done for %O" (sw.Elapsed)
                )

        printfn "done."

    

    

