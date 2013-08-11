using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSorter.Core.Tests
{
    [TestFixture]
    public class MergeTests
    {

        [TestCaseSource("GetMergeTwoSeqSource")]
        public int[] Test_ImperativeMerge_Two_Seqs(int[] left, int[] right)
        {
            return MergeSort.imperativeMerge(left, right).ToArray();
        }

        [TestCaseSource("GetMergeTwoSeqSource")]
        public int[] Test_RecursiveMerge_Two_Seqs(int[] left, int[] right)
        {
            return MergeSort.recursiveMerge(left, right).ToArray();
        }

        [TestCaseSource("GetMergeTwoSeqSource")]
        public int[] Test_UnfoldMerge_Two_Seqs(int[] left, int[] right)
        {
            return MergeSort.unfoldMerge(left, right).ToArray();
        }

        [TestCaseSource("GetMergeFewSequencesSource")]
        public int[] Test_ImperativeMergeSequences(int[][] sequences)
        {
            var seq = sequences.Select(a => (IEnumerable<int>)a);
            return MergeSort.imperativeMergeSequences(seq.ToArray()).ToArray();
        }

        [TestCaseSource("GetMergeFewSequencesSource")]
        public int[] Test_UnfoldeMergeSequences(int[][] sequences)
        {
            var seq = sequences.Select(a => (IEnumerable<int>)a);
            return MergeSort.unfoldMergeSequences(seq.ToArray()).ToArray();
        }

        public IEnumerable<TestCaseData> GetMergeTwoSeqSource()
        {
            yield return new TestCaseData(new[] { 3, 7, 22 }, new[] { 1, 4, 11 })
                .Returns(new[] { 1, 3, 4, 7, 11, 22 });
            
            yield return new TestCaseData(new int[] { }, new int[] { }).Returns(new int[] { });
            
            yield return new TestCaseData(new int[] { 1, 2, 3 }, new int[] { 1, 2, 3 })
                .Returns(new int[] { 1, 1, 2, 2, 3, 3 });
        }

        public IEnumerable<TestCaseData> GetMergeFewSequencesSource()
        {
            var s1 = new[] {1, 7, 21, 77};
            var s2 = new[] {3, 6, 12, 51};
            var s3 = new[] {2, 5, 11, 12};
            yield return new TestCaseData(arg: new[] {s1, s2, s3})
                .Returns(new int[]{1,2,3,5,6,7,11,12,12,21,51,77});

            var s2_1 = Enumerable.Range(1, 10).ToArray();
            yield return new TestCaseData(arg: new[] { s2_1, s2_1, s2_1 })
                .Returns(s2_1.SelectMany(n => Enumerable.Repeat(n, 3).ToArray()));
        }
    }
}
