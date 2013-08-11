namespace System.Collections.Generic

open System.Collections.Generic
open System.Diagnostics.Contracts
open System.Linq

/// Set of helper methods that helps to store duplicate values in the sorted list.
module SortedList =
        
        // Add element to sorted list that allows to store multiple values for the key
        let add (list: SortedList<_,List<_>>) (key) (value) =
            
            if list.ContainsKey(key) then
                list.[key].Add(value)
            else
                // We can use List<_>([|iter|]) but it will lead to additional allocation
                let lst = List<_>()
                lst.Add(value)
                list.Add(key, lst)


        // Returns first element and remove it from the sorted list
        let getFirst (list: SortedList<_, List<_>>) =
            
            Contract.Requires(list.Count <> 0, "List should not be empty")
            
            let fst = list.First()
            let value = fst.Value.[0]
            
            if fst.Value.Count = 1 then
                // This is last element in the value!
                list.Remove(fst.Key) |> ignore
            else
                fst.Value.RemoveAt(0)

            (fst.Key, value)


