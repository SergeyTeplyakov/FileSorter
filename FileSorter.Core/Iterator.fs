namespace FileSorter.Core

open System
open System.Collections.Generic


// Using simple wrapper around build-in iterators (a.k.a. enumerables) that would store
// iterators state (whether iterator finished or not) greately simplify implementation
// for merge
module Iterators =
    
    // To play with different approach I'm using few different implementations for iteratrs,
    // like use record, class or struct

    type IteratorC<'a>(s: 'a seq) =
        let mutable finished = false
        let iter = s.GetEnumerator()
        
        do
            finished <- not (iter.MoveNext())

        member x.Finished = finished
        member x.Current = iter.Current
        member x.MoveNext() = finished <- not (iter.MoveNext())
        member x.Iter = iter

        interface IDisposable with
            member x.Dispose() =
                // TODO: discover this stuff: why I can't access from interface implementation to let bindings?!?!
                x.Iter.Dispose()
        
        /// Factory method for creating iterator
        static member fromSeq (s: seq<'a>) =
            new IteratorC<_>(s)

    // Record-based implementation (I got some issues because of its immutability, I guess)
    type IteratorR<'a> = {mutable Finished: bool; Iter: IEnumerator<'a>} with

        static member fromSeq (s: seq<'a>) =
            let en = s.GetEnumerator()
            {Finished = not (en.MoveNext()); Iter = en}

        member x.Current = x.Iter.Current

        member x.MoveNext() =
            x.Finished <- not (x.Iter.MoveNext())

        interface IDisposable with
            member x.Dispose() =
                x.Iter.Dispose()


    // (struct implementation is not finished and never used yet (because of nature: its really hard to use
    // mutable value types in F#))
    [<Struct>]
    type IteratorS<'a> = // (s: 'a seq) =
        
        [<DefaultValue(false)>]
        val mutable finished: bool
        //[<DefaultValue>]
        val mutable iter: IEnumerator<'a>
        
        //[<DefaultValue>]
        val mutable sq: 'a seq

//        new() = {finished = true; iter = Seq.empty<'a>.GetEnumerator()}
        new(s: 'a seq) = { sq = s; iter = s.GetEnumerator() } then
            // How to initialize something else here???
            // finished <- iter.MoveNext()
            ()

        member x.Iter = x.iter
        member x.Current = x.iter.Current
        member x.Finished = x.finished

        member x.MoveNext() =
            x.finished <- not (x.iter.MoveNext())

        static member fromSeq (s: seq<'a>) =
            let mutable i = new IteratorS<_>(s)
            i.MoveNext()
            i
        

