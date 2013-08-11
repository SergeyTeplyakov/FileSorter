namespace FileSorter.Core

open System
open System.Collections.Generic
open System.Diagnostics.Contracts

type Subject<'T> () =
    // TODO: I'm not sure about thread-safety!
    let hasSubscribers = new System.Threading.ManualResetEvent(false)
    let sync = obj()
    // TODO: interestingly enough, but F# has no Volatile keywords, so ...
    let mutable stopped = false
    let observers = List<IObserver<'T>>()
    // Why this function is not using lock?
    let iter f = observers |> Seq.iter f
    
    let onCompleted () =
       if not stopped then
          stopped <- true
          iter (fun observer -> observer.OnCompleted())
    
    let onError ex () =
       if not stopped then
          stopped <- true
          iter (fun observer -> observer.OnError(ex))
    
    let next value () =
       Contract.Assert(not stopped, "We should not call Next on finished observable sequence!")

       if not stopped then
          iter (fun observer -> observer.OnNext(value))
    
    let remove observer () =
       observers.Remove observer |> ignore
    
    member x.Next value = lock sync <| next value
    member x.Error ex = lock sync <| onError ex
    member x.Completed () = lock sync <| onCompleted
    
    member x.HasSubscribers = hasSubscribers

    interface IObserver<'T> with
       member x.OnCompleted() = x.Completed()
       member x.OnError ex = x.Error ex
       member x.OnNext value = x.Next value
    
    interface IObservable<'T> with
       member this.Subscribe(observer:IObserver<'T>) =
          // Setting event that we have at least one observer
          hasSubscribers.Set() |> ignore

          observers.Add observer
          { new IDisposable with
             member this.Dispose() =
                lock sync <| remove observer
          }

