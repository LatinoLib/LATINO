/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    LSqrDotNet.cs
 *  Desc:    C# wrapper for LSQR DLL
 *  Created: Oct-2007
 *
 *  Author:  Miha Grcar 
 *
 ***************************************************************************/

using System;
using System.Runtime.InteropServices;

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |		 
       |  Class LSqrDll
       |
       '-----------------------------------------------------------------------
    */
    public static class LSqrDll
    {
#if DEBUG
        private const string LSQR_DLL = "LSqrDebug.dll";
#else
        private const string LSQR_DLL = "LSqr.dll";
#endif

        // *** External functions ***

        [DllImport(LSQR_DLL)]
        public static extern int NewMatrix(int rowCount);

        [DllImport(LSQR_DLL)]
        public static extern void DeleteMatrix(int id);

        [DllImport(LSQR_DLL)]
        public static extern void InsertValue(int matId, int rowIdx, int colIdx, double val);

        [DllImport(LSQR_DLL)]
        public static extern IntPtr DoLSqr(int matId, int matTranspId, double[] initSol, double[] rhs, int maxIter);

        // *** Wrappers for external DoLSqr ***

        public static double[] DoLSqr(int numCols, LSqrSparseMatrix mat, LSqrSparseMatrix matTransp, double[] rhs, int maxIter)
        {
            return DoLSqr(numCols, mat, matTransp, /*initSol=*/null, rhs, maxIter);
        }

        public static double[] DoLSqr(int numCols, LSqrSparseMatrix mat, LSqrSparseMatrix matTransp, double[] initSol, double[] rhs, int maxIter)
        {
            IntPtr solPtr = DoLSqr(mat.Id, matTransp.Id, initSol, rhs, maxIter);
            GC.KeepAlive(mat); // avoid premature garbage collection
            GC.KeepAlive(matTransp);
            double[] sol = new double[numCols];
            Marshal.Copy(solPtr, sol, 0, sol.Length);
            Marshal.FreeHGlobal(solPtr);
            return sol;
        }
    }

    /* .-----------------------------------------------------------------------
       |		 
       |  Class LSqrSparseMatrix
       |
       '-----------------------------------------------------------------------
    */
    public class LSqrSparseMatrix : IDisposable
    {
        private int mId;

        public LSqrSparseMatrix(int rowCount)
        {
            mId = LSqrDll.NewMatrix(rowCount);
        }

        ~LSqrSparseMatrix()
        {
            Dispose();
        }

        public int Id
        {
            get { return mId; }
        }

        public void InsertValue(int rowIdx, int colIdx, double val)
        {
            LSqrDll.InsertValue(mId, rowIdx, colIdx, val);
        }

        public static LSqrSparseMatrix FromDenseMatrix(double[,] mat)
        {
            LSqrSparseMatrix lsqrMat = new LSqrSparseMatrix(mat.GetLength(0));
            for (int row = 0; row < mat.GetLength(0); row++)
            {
                for (int col = 0; col < mat.GetLength(1); col++)
                {
                    if (mat[row, col] != 0)
                    {
                        lsqrMat.InsertValue(row, col, mat[row, col]);
                    }
                }
            }
            return lsqrMat;
        }

        public static LSqrSparseMatrix TransposeFromDenseMatrix(double[,] mat)
        {
            LSqrSparseMatrix lsqrMat = new LSqrSparseMatrix(mat.GetLength(1));
            for (int col = 0; col < mat.GetLength(1); col++)
            {
                for (int row = 0; row < mat.GetLength(0); row++)
                {
                    if (mat[row, col] != 0)
                    {
                        lsqrMat.InsertValue(col, row, mat[row, col]);
                    }
                }
            }
            return lsqrMat;
        }

        // *** IDisposable interface implementation ***

        public void Dispose()
        {
            if (mId >= 0)
            {
                LSqrDll.DeleteMatrix(mId);
                mId = -1;
            }
        }
    }
}