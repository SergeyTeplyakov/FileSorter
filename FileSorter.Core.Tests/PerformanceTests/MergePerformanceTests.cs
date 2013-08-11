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
//    public class MergePerformanceTests
//    {
//        [TestMethod]
//        public void Test_Imperative_Merge_For_50_Large_Sequences()
//        {
//            int seqCount = 50;
//            int elementsCount = 1 * 1000 * 1000;
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

//        [TestMethod]
//        public void Test_Recursive_Merge_For_2_Large_Sequences()
//        {
//            int elementsCount = 10 * 1000 * 1000;
//            var seq = Enumerable.Range(1, elementsCount);

//            var sw = Stopwatch.StartNew();

//            var result = MergeSort.recursiveMerge(seq, seq).Count();
//            sw.Stop();
//            Console.WriteLine("Duration: {0}ms, Count: {1}", sw.ElapsedMilliseconds, result);
//            sw.Stop();
//        }

//        [TestMethod]
//        public void Test_Imperative_Merge_For_2_Large_Sequences()
//        {
//            int elementsCount = 10 * 1000 * 1000;
//            var seq = Enumerable.Range(1, elementsCount);

//            var sw = Stopwatch.StartNew();

//            var result = MergeSort.imperativeMerge(seq, seq).Count();
//            sw.Stop();
//            Console.WriteLine("Duration: {0}ms, Count: {1}", sw.ElapsedMilliseconds, result);
//            sw.Stop();
//        }


//    }
//}
