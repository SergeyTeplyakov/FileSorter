namespace FileSorter.Core


type RawChunk = {Data: char array; Length: int}
type ParsedChunk = {ParsedData: int array}
type SortedChunk = {SortedData: int array}
type PreparedChunk = {PreparedData: byte array; Length: int}

type SavedChunk = {Number: int; FileName: string}

///// Chunk is a basic block for all processing pipeline.
///// First, raw data reads from the file, than it parses from byte array to chunk of ints.
//type Chunk = 
//    // Raw chunk contains a byte array with a length
//    | RawChunk of char array * int
//    | ParsedChunk of int array
//
