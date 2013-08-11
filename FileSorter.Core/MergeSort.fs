namespace FileSorter.Core

open System.Linq
open System
open System.Diagnostics.Contracts
open System.Collections.Generic

open Iterators

module MergeSort =

    /// Merge two sorted sequence. 
    /// (simple imperative implementation)
    let imperativeMerge (l: 'a seq) (r: 'a seq) =
        // Its interesting that imperative implementation much smaller and faster (like 10 times)
        let li = IteratorR<_>.fromSeq l
        let ri = IteratorR<_>.fromSeq r

        seq {
            while not (li.Finished) || not (ri.Finished) do
                let iter = 
                    if li.Finished then ri
                    else if ri.Finished then li
                    else if li.Iter.Current < ri.Iter.Current then li else ri
                
                yield iter.Current
                iter.MoveNext()
        }


    let unfoldMerge (l: 'a seq) (r: 'a seq) =
        // It seems that we have two allocations per each iteration!

        // TODO: can we cache Some(ri), Some(li) or this is not necessary?
        // TODO: try to create computation expression for this!!!
        // Cached version still performs better!! Why?? Maybe ther is some caching in Some alread?
        // Compare memory consumption!
        let getCurrent (li: IteratorR<_>) (ri: IteratorR<_>) =
            match (li.Finished, ri.Finished) with
            | (true, true) -> None
            | (true, false) -> Some(ri)
            | (false, true) -> Some(li)
            // Extract compare to an argument
            | (false, false) -> if li.Iter.Current < ri.Iter.Current then Some(li) else Some(ri)

        let li = IteratorR<_>.fromSeq l
        let ri = IteratorR<_>.fromSeq r
        
        let initState = getCurrent li ri
        
        // TODO: This version will not call dispose on interators!!!
        Seq.unfold(fun (st: IteratorR<'a> option) -> 
            // If current state is not None, we have at least one element in output sequence
            st |> Option.bind(fun iter -> 
                let curValue = iter.Iter.Current
                iter.MoveNext()
                
                // We're capturing li and ri, I don't think that this is "pure" fuctional style!
                let newState = getCurrent li ri
                
                // TODO: we have additional allocation per each iteration!!
                // Result for unfold function contains new valud for the sequence and modified state
                Some((curValue, newState)))
            ) initState


    let recursiveMerge (l: 'a seq) (r: 'a seq) =
        let rec merge li ri = seq {
            
            if li.Finished && ri.Finished then
                yield! Seq.empty
            else
                let iter = 
                    if li.Finished then ri
                    else if ri.Finished then li
                    else if li.Current < ri.Current then li else ri

                yield iter.Current
                iter.MoveNext()
                yield! merge li ri
            
//            let getCurrent (li: IteratorR<_>) (ri: IteratorR<_>) =
//                match (li.Finished, ri.Finished) with
//                | (true, true) -> None
//                | (true, false) -> Some(ri)
//                | (false, true) -> Some(li)
//                // Extract compare to an argument
//                | (false, false) -> if li.Iter.Current < ri.Iter.Current then Some(li) else Some(ri)


//            let cur = getCurrent li ri
//            match cur with
//            | None -> yield! Seq.empty
//            | Some(x) -> yield x.Iter.Current; x.MoveNext(); yield! merge li ri

//            match(li.Finished, ri.Finished) with
//            | (true, true) -> yield! Seq.empty
//            | (_, _) ->
//                let iter = if li.Finished then ri
//                           else if ri.Finished then li
//                           else if li.Iter.Current < ri.Iter.Current then li else ri
//                yield iter.Iter.Current
//                iter.MoveNext()
//                yield! merge li ri
        }

//        // This function is recursive without tail recursion optimization!
//        let rec mergeImpl li ri (ci: Iterator<'a> option) = seq {
//            
//            if ci.IsSome then
//                yield ci.Value.Iter.Current
//                ci.Value.MoveNext()
////                ci.Value.Finished <- not (ci.Value.Iter.MoveNext())
//                // This will lead to one additional allocation per each element!
//                yield! mergeImpl li ri (getCurrent li ri)
//            else
//                yield! Seq.empty
//        }
    
        let li = IteratorR<_>.fromSeq l
        let ri = IteratorR<_>.fromSeq r
        merge li ri
//        mergeImpl li ri (getCurrent li ri)

    /// Merge array of sorted sequences
    /// (more functional implementation)
    let unfoldMergeSequences(sequences: 'a seq array) =
        
        let getCurrent (iters: IteratorC<'a> seq) =
            let notFinished = iters |> Seq.filter(fun i -> not i.Finished)
            if Seq.isEmpty notFinished then None
            else Some(notFinished |> Seq.minBy(fun i -> i.Iter.Current))
        
        let iters = sequences |> Array.map(fun s -> IteratorC<_>.fromSeq s)
        
        let initState = iters //:> Iterator<'a> seq

        Seq.unfold(fun (st: IteratorC<'a> array) ->
            
            let notFinished = iters |> Seq.filter(fun i -> not i.Finished)
            if Seq.isEmpty notFinished then None
            else
                let curIter = notFinished |> Seq.minBy(fun i -> i.Iter.Current)
                let curVal = curIter.Iter.Current
                curIter.MoveNext()
                Some(curVal, st)
                ) initState

    /// Merge sequence that contains sequence of sorted elements
    let imperativeMergeSequences<'a when 'a :> System.IComparable<'a>> (sequences: 'a seq array) =
        // TODO: I'm not disposing those iterators
        
        // Store all iterators in a sorted order (by their content). 
        // SortedList can't contain duplicates, thats why we're using 
        // list of iterators as a associated value
        let listOfIters = SortedList<'a, List<IteratorC<'a>>>(sequences.Length)

        let iters = 
            sequences 
            |> Seq.map IteratorC<_>.fromSeq 
            |> Seq.filter(fun i -> not i.Finished)
            |> Seq.iter(fun i -> SortedList.add listOfIters i.Current i)

        // It would be nice to wrap seq {} into something like "post call" where we can
        // dispose all iterators
        seq {
            
            while listOfIters.Count <> 0 do
                
                // First element contains least element
                let (cur, iter) = SortedList.getFirst listOfIters
                
                // Yielding current value
                yield cur
                
                // Moving least iterator forward and att it once more if it not finished
                iter.MoveNext()
                if not iter.Finished then
                    SortedList.add listOfIters iter.Current iter
        }
        

    

    let insertionSort (s: 'a seq) =
//        let a = s |> Seq.toArray

        //let list = List<'a>(s)

        // TODO: Check and implement optimizing merge sort that uses insertion sort when
        // subarrays of S are reached where S is the number of data items fitting into a CPU's cache!
        // Se wiki for more details!
        Seq.sort(s)



