//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace FileSorter.Core.Tests
//{
    

//    [TestClass]
//    public class MergeMSTests
//    {

//        [TestMethod]
//        public void Test_Merge_50_Large_Sequences_with_mergeSequences()
//        {
//            int seqCount = 50;
//            int elementsCount = 100*1000;
//            var lst = Enumerable.Range(1, elementsCount).ToList();
//            var rnd = new Random(42);
//            var seqs =
//                Enumerable.Range(1, seqCount)
//                    .Select(_ => lst).ToArray();

//            //var arg = seqs.ToArray();
//            var sw = Stopwatch.StartNew();

//            var result = MergeSort.imperativeMergeSequences(seqs).Count();
//            sw.Stop();
//            Console.WriteLine("Duration: {0}ms, Count: {1}", sw.ElapsedMilliseconds, result);
//            sw.Stop();
//        }


//        //[TestMethod]
//        //public void Test_Merge_3_Simple_Sequences()
//        //{
//        //    var s1 = Enumerable.Range(1, 10);
//        //    var merged = MergeSort.imperativeSequenceMerge(new[] { s1, s1, s1 }).ToList();
//        //    Console.WriteLine(string.Join(", ", merged.Select(n => n.ToString())));

//        //    Assert.AreEqual(30, merged.Count);
//        //}

//        // We can use parameterized tests easily! for all those corner cases!
//        [TestMethod]
//        public void Test_Merge_Two_Empty_Seq_Return_Empty_Seq()
//        {
//            var merged = MergeSort.imperativeMerge(Enumerable.Empty<int>(), Enumerable.Empty<int>());
//            Assert.AreEqual(merged.Count(), 0);
//        }

//        [TestMethod]
//        public void Test_Merge_two_sequence_with_2_elements_produces_sequence_with_4()
//        {
//            var seq1 = Enumerable.Range(1, 2);
//            var seq2 = Enumerable.Range(1, 2);
//            var merged = MergeSort.imperativeMerge(seq1, seq2);
//            Assert.AreEqual(4, merged.Count());
//        }

//        [TestMethod]
//        public void Test_Merge_two_sequence_with_10_elements_produces_sequence_with_20()
//        {
//            var seq1 = Enumerable.Range(1, 10);
//            var seq2 = Enumerable.Range(1, 10);
//            var merged = MergeSort.imperativeMerge(seq1, seq2);
//            Assert.AreEqual(20, merged.Count());
//        }

//        //[TestMethod]
//        //public void Test_Merge_with_unfold_On_Large_Sequence()
//        //{
//        //    var seq1 = Enumerable.Range(1, 100000000);
//        //    var seq2 = Enumerable.Range(1, 100000000);
//        //    var sw = Stopwatch.StartNew();
//        //    var merged = MergeSort.unfoldeMerge(seq1, seq2).Count();
//        //    sw.Stop();
//        //    Console.WriteLine("Merge took: {0}ms", sw.ElapsedMilliseconds);
//        //    //Console.WriteLine("Merged sequence length is {0}", merged.Count);
//        //    //Console.WriteLine(string.Join(", ", merged.Select(n => n.ToString())));
//        //}

//        [TestMethod]
//        public void Test_Merge_with_implerative_On_Large_Sequence()
//        {
//            var count = 10000000;
//            var seq1 = Enumerable.Range(1, count);
//            var seq2 = Enumerable.Range(1, count);
//            var sw = Stopwatch.StartNew();
//            var merged = MergeSort.imperativeMerge(seq1, seq2).Count();
//            sw.Stop();
//            Console.WriteLine("Merge took: {0}ms", sw.ElapsedMilliseconds);
//            //Console.WriteLine("Merged sequence length is {0}", merged.Count);
//            //Console.WriteLine(string.Join(", ", merged.Select(n => n.ToString())));
//        }

//        [TestMethod]
//        public void Test_Merge_with_recursion_On_Large_Sequence()
//        {
//            var seq1 = Enumerable.Range(1, 100000000);
//            var seq2 = Enumerable.Range(1, 100000000);

//            var merged = MergeSort.recursiveMerge(seq1, seq2).Skip(10000000).Take(10).ToList();
//            Console.WriteLine("Merged sequence length is {0}", merged.Count);
//            Console.WriteLine(string.Join(", ", merged.Select(n => n.ToString())));
//        }
//    }
//}
