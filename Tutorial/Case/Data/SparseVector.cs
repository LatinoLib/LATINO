/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    SparseVector.cs
 *  Desc:    Tutorial 2.1: SparseVector 
 *  Created: Dec-2009
 *
 *  Authors: Miha Grcar
 *
 ***************************************************************************/

using System;
using Latino;

namespace Tutorial.Case.Data
{
    public class SparseVector : Tutorial<SparseVector>
    {
        public override void Run(string[] args)
        {
            // create SparseVector
            Output.WriteLine("Create SparseVector ...");
            SparseVector<string> vec = new SparseVector<string>(new IdxDat<string>[] {
                new IdxDat<string>(1, "a"),
                new IdxDat<string>(3, "b"),
                new IdxDat<string>(4, "c"),
                new IdxDat<string>(6, "d") });
            Output.WriteLine(vec);
            // add more items
            Output.WriteLine("Add more items ...");
            // ... at the end
            vec.Add("E");
            vec.AddRange(new string[] { "F", "G" });
            // ... at specific places 
            vec.AddRange(new IdxDat<string>[] {
                new IdxDat<string>(2, "AB"),
                new IdxDat<string>(10, "H") });
            vec[11] = "i";
            Output.WriteLine(vec);
            // get items
            Output.WriteLine("Get items ...");
            Output.WriteLine(vec[1]);
            Output.WriteLine(vec.TryGet(2, "missing"));
            Output.WriteLine(vec.TryGet(5, "missing"));
            // set items
            Output.WriteLine("Set items ...");
            vec[2] = "ab";
            vec[10] = "h";
            Output.WriteLine(vec);
            // check for items
            Output.WriteLine("Check for items ...");
            Output.WriteLine(vec.ContainsAt(2));
            Output.WriteLine(vec.ContainsAt(5));
            // get first and last non-empty index
            Output.WriteLine("Get first and last non-empty index ...");
            Output.WriteLine(vec.FirstNonEmptyIndex);
            Output.WriteLine(vec.LastNonEmptyIndex);
            // get first and last item
            Output.WriteLine("Get first and last item ...");
            Output.WriteLine(vec.First);
            Output.WriteLine(vec.Last);
            // create another SparseVector
            Output.WriteLine("Create another SparseVector ...");
            SparseVector<string> vec2 = new SparseVector<string>(new IdxDat<string>[] {
                new IdxDat<string>(0, "!"),
                new IdxDat<string>(2, "@"),
                new IdxDat<string>(3, "#"),
                new IdxDat<string>(5, "$") });
            Output.WriteLine(vec2);
            // concatenate 
            Output.WriteLine("Concatenate ...");
            vec.Append(vec2, vec.LastNonEmptyIndex + 1);
            Output.WriteLine(vec);
            vec2.Append(vec, vec2.LastNonEmptyIndex + 1);
            Output.WriteLine(vec2);
            // get number of items
            Output.WriteLine("Get number of items ...");
            Output.WriteLine(vec.Count);
            // remove item
            Output.WriteLine("Remove item ...");
            vec.RemoveAt(2);
            Output.WriteLine(vec);
            // directly access to items
            Output.WriteLine("Directly access to items ...");
            int idx = vec.GetDirectIdx(3);
            Output.WriteLine(idx);
            vec.SetDirect(idx, "bbb");
            Output.WriteLine(vec);
            Output.WriteLine(vec.GetIdxDirect(idx));
            Output.WriteLine(vec.GetDatDirect(idx));
            Output.WriteLine(vec.GetDirect(idx));
            vec.RemoveDirect(idx);
            Output.WriteLine(vec);
            // perform unary operation
            Output.WriteLine("Perform unary operation ...");
            vec.PerformUnaryOperation(delegate(string item) { return item.ToUpper(); });
            Output.WriteLine(vec);
            // merge
            Output.WriteLine("Merge ...");
            vec.Merge(vec2, delegate(string a, string b) { return string.Format("{0}+{1}", a, b); });
            Output.WriteLine(vec);
            // purge
            Output.WriteLine("Purge items ...");
            vec.PurgeAt(1);
            Output.WriteLine(vec);
            vec.PurgeAt(1);
            Output.WriteLine(vec);
            Output.WriteLine();

            // *** SparseMatrix ***
            Output.WriteLine("*** SparseMatrix ***");
            Output.WriteLine();
            // create SparseMatrix
            Output.WriteLine("Create SparseMatrix ...");
            SparseMatrix<string> matrix = new SparseMatrix<string>();
            matrix[0] = new SparseVector<string>(new IdxDat<string>[] {
                new IdxDat<string>(1, "a"),
                new IdxDat<string>(3, "b"),
                new IdxDat<string>(4, "c") });
            matrix[2] = new SparseVector<string>(new IdxDat<string>[] {
                new IdxDat<string>(2, "d"),
                new IdxDat<string>(4, "e"),
                new IdxDat<string>(5, "f") });
            matrix[3] = new SparseVector<string>(new IdxDat<string>[] {
                new IdxDat<string>(0, "g"),
                new IdxDat<string>(3, "h"),
                new IdxDat<string>(5, "i") });
            matrix[4] = new SparseVector<string>(new IdxDat<string>[] {
                new IdxDat<string>(1, "j"),
                new IdxDat<string>(2, "k"),
                new IdxDat<string>(4, "l") });
            Output.WriteLine(matrix.ToString("E"));
            // get rows
            Output.WriteLine("Get rows ...");
            Output.WriteLine(matrix[0]);
            Output.WriteLine(matrix[3]);
            // set rows
            Output.WriteLine("Set rows ...");
            matrix[1] = new SparseVector<string>(new IdxDat<string>[] { new IdxDat<string>(0, "j"), new IdxDat<string>(3, "k") });
            matrix[2] = null;
            matrix[4] = null;
            Output.WriteLine(matrix.ToString("E"));
            // count rows
            Output.WriteLine("Count rows ...");
            Output.WriteLine("{0} != {1}", matrix.GetRowCount(), matrix.GetLastNonEmptyRowIdx() + 1);
            // trim rows
            Output.WriteLine("Trim rows ...");
            matrix.TrimRows();
            Output.WriteLine(matrix.ToString("E"));
            // add more items
            Output.WriteLine("Add more items ...");
            matrix[0].Add("*");
            matrix[3].AddRange(new IdxDat<string>[] {
                new IdxDat<string>(1, "!"),
                new IdxDat<string>(2, "?"),
                new IdxDat<string>(4, "&") });
            matrix[2] = new SparseVector<string>(new IdxDat<string>[] {
                new IdxDat<string>(2, "d"),
                new IdxDat<string>(4, "e"),
                new IdxDat<string>(5, "f") });
            Output.WriteLine(matrix.ToString("E"));
            // get items
            Output.WriteLine("Get items ...");
            Output.WriteLine(matrix[0, 1]);
            Output.WriteLine(matrix[2, 2]);
            Output.WriteLine(matrix[2][4]);
            Output.WriteLine(matrix.TryGet(2, 4, "missing"));
            Output.WriteLine(matrix.TryGet(2, 6, "missing"));
            // set items
            Output.WriteLine("Set items ...");
            matrix[0, 1] = "l";
            matrix[2, 3] = "m";
            matrix[3][4] = "n";
            Output.WriteLine(matrix.ToString("E"));
            // check for items
            Output.WriteLine("Check for items ...");
            Output.WriteLine(matrix.ContainsAt(0, 1));
            Output.WriteLine(matrix.ContainsAt(1, 1));
            Output.WriteLine(matrix.Contains("c"));
            Output.WriteLine(matrix.Contains("C"));
            int rowIdx = -1, colIdx = -1;
            matrix.IndexOf("c", ref rowIdx, ref colIdx);
            Output.WriteLine("{0}, {1}", rowIdx, colIdx);
            // check for rows and columns
            Output.WriteLine("Check for rows and columns ...");
            Output.WriteLine(matrix.ContainsColAt(0));
            Output.WriteLine(matrix.ContainsColAt(100));
            Output.WriteLine(matrix.ContainsRowAt(0));
            Output.WriteLine(matrix.ContainsRowAt(100));
            // get first and last non-empty row and column index
            Output.WriteLine("Get first and last non-empty row and column index ...");
            Output.WriteLine(matrix.GetFirstNonEmptyRowIdx());
            Output.WriteLine(matrix.GetLastNonEmptyRowIdx());
            Output.WriteLine(matrix.GetFirstNonEmptyColIdx());
            Output.WriteLine(matrix.GetLastNonEmptyColIdx());
            // get first and last item in row
            Output.WriteLine("Get first and last item in row ...");
            Output.WriteLine(matrix[0].First);
            Output.WriteLine(matrix[3].Last);
            // create another SparseMatrix
            Output.WriteLine("Create another SparseMatrix ...");
            SparseMatrix<string> matrix2 = new SparseMatrix<string>();
            matrix2[0] = new SparseVector<string>(new IdxDat<string>[] {
                new IdxDat<string>(0, "A"),
                new IdxDat<string>(2, "B"),
                new IdxDat<string>(3, "C") });
            matrix2[2] = new SparseVector<string>(new IdxDat<string>[] {
                new IdxDat<string>(1, "D"),
                new IdxDat<string>(3, "E") });
            matrix2[3] = new SparseVector<string>(new IdxDat<string>[] {
                new IdxDat<string>(0, "G"),
                new IdxDat<string>(1, "H"),
                new IdxDat<string>(2, "I") });
            Output.WriteLine(matrix2.ToString("E"));
            // concatenate
            Output.WriteLine("Concatenate ...");
            matrix.AppendCols(matrix2, matrix.GetLastNonEmptyColIdx() + 1);
            Output.WriteLine(matrix.ToString("E"));
            // remove items
            Output.WriteLine("Remove items ...");
            matrix.RemoveAt(0, 1);
            matrix.RemoveAt(3, 5);
            Output.WriteLine(matrix.ToString("E"));
            // directly access to items
            Output.WriteLine("Directly access to items ...");
            idx = matrix[0].GetDirectIdx(4);
            Output.WriteLine(idx);
            Output.WriteLine(matrix[0].GetDirect(idx));
            matrix[0].SetDirect(idx, "C");
            Output.WriteLine(matrix[1].GetDirect(0));
            matrix[1].RemoveDirect(0);
            Output.WriteLine(matrix.ToString("E"));
            // get properties
            Output.WriteLine("Get properties ...");
            Output.WriteLine("{0:0.00}%", matrix.GetSparseness(matrix.GetLastNonEmptyRowIdx() + 1, matrix.GetLastNonEmptyColIdx() + 1) * 100.0);
            Output.WriteLine(matrix.IsSymmetric());
            Output.WriteLine(matrix.ContainsDiagonalElement());
            Output.WriteLine(matrix.CountValues());
            // perform unary operation
            Output.WriteLine("Perform unary operation ...");
            matrix.PerformUnaryOperation(delegate(string item) { return item.ToUpper(); });
            Output.WriteLine(matrix.ToString("E"));
            // merge
            Output.WriteLine("Merge ...");
            matrix.Merge(matrix2, delegate(string a, string b) { return string.Format("{0}+{1}", a, b); });
            Output.WriteLine(matrix.ToString("E"));
            // clear row and column
            Output.WriteLine("Clear row and column ...");
            matrix.RemoveRowAt(2);
            matrix.RemoveColAt(1);
            Output.WriteLine(matrix.ToString("E"));
            // purge row and column
            Output.WriteLine("Purge row and column ...");
            matrix.PurgeRowAt(2);
            matrix.PurgeColAt(1);
            Output.WriteLine(matrix.ToString("E"));
            // get column copy
            Output.WriteLine("Get column copy ...");
            Output.WriteLine(matrix.GetColCopy(0));
            // transpose
            Output.WriteLine("Transpose ...");
            Output.WriteLine(matrix.GetTransposedCopy().ToString("E"));
            // set diagonal 
            Output.WriteLine("Set diagonal ...");
            matrix.SetDiagonal(matrix.GetLastNonEmptyColIdx() + 1, "X");
            Output.WriteLine(matrix.ToString("E"));
            // make symmetric
            Output.WriteLine("Make symmetric ...");
            matrix.Symmetrize(delegate(string a, string b) { return string.Format("{0}+{1}", a, b); });
            Output.WriteLine(matrix.ToString("E"));
        }
    }
}
