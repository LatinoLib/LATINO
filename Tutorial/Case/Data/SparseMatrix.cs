/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    SparseMatrix.cs
 *  Desc:    Tutorial 2.2: SparseMatrix
 *  Created: Dec-2009
 *
 *  Authors: Miha Grcar
 *
 ***************************************************************************/

using System;
using Latino;

namespace Tutorial.Case.Data
{
    public class SparseMatrix : Tutorial<SparseMatrix>
    {
        public override void Run(string[] args)
        {
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
            int idx = matrix[0].GetDirectIdx(4);
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
