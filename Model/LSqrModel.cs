/*==========================================================================;
 *
 *  This file is part of LATINO. See http://www.latinolib.org
 *
 *  File:    LSqrModel.cs
 *  Desc:    Least-squares linear regression model
 *  Created: Nov-2007
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

using System;

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |
       |  Class LSqrModel
       |
       '-----------------------------------------------------------------------
    */
    public class LSqrModel : IModel<double, SparseVector<double>>
    {
        private ArrayList<double> mSol
            = null;
        private int mNumIter
            = -1;
        private double[] mInitSol
            = null;

        public LSqrModel()
        {
        }

        public LSqrModel(int numIter)
        {
            mNumIter = numIter;
        }

        public LSqrModel(BinarySerializer reader)
        {
            Load(reader); // throws ArgumentNullException, serialization-related exceptions
        }

        public ArrayList<double>.ReadOnly Solution
        {
            get
            {
                Utils.ThrowException(mSol == null ? new InvalidOperationException() : null);
                return mSol;
            }
        }

        public int NumIter
        {
            get { return mNumIter; }
            set
            {
                Utils.ThrowException(value <= 0 ? new ArgumentOutOfRangeException("NumIter") : null);
                mNumIter = value;
            }
        }

        public double[] InitialSolution
        {
            get { return mInitSol; }
            set { mInitSol = value; }
        }

        // *** IModel<double, SparseVector<double>> interface implementation ***

        public Type RequiredExampleType
        {
            get { return typeof(SparseVector<double>); }
        }

        public bool IsTrained
        {
            get { return mSol != null; }
        }

        public void Train(ILabeledExampleCollection<double, SparseVector<double>> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(dataset.Count == 0 ? new ArgumentValueException("dataset") : null);
            LSqrSparseMatrix mat = new LSqrSparseMatrix(dataset.Count);
            double[] rhs = new double[dataset.Count];
            int solSize = -1;
            int i = 0;
            foreach (LabeledExample<double, SparseVector<double>> labeledExample in dataset)
            {
                if (labeledExample.Example.LastNonEmptyIndex + 1 > solSize) 
                { 
                    solSize = labeledExample.Example.LastNonEmptyIndex + 1; 
                }
                foreach (IdxDat<double> item in labeledExample.Example)
                {
                    mat.InsertValue(i, item.Idx, item.Dat);
                }
                rhs[i++] = labeledExample.Label;
            }
            Utils.ThrowException((mInitSol != null && mInitSol.Length != solSize) ? new ArgumentValueException("InitialSolution") : null);
            LSqrSparseMatrix matT = new LSqrSparseMatrix(solSize);
            i = 0;
            foreach (LabeledExample<double, SparseVector<double>> labeledExample in dataset)
            {
                foreach (IdxDat<double> item in labeledExample.Example)
                {
                    matT.InsertValue(item.Idx, i, item.Dat);
                }
                i++;
            }
            int numIter = mNumIter < 0 ? solSize + dataset.Count + 50 : mNumIter;
            mSol = new ArrayList<double>(LSqrDll.DoLSqr(solSize, mat, matT, mInitSol, rhs, numIter));
            mat.Dispose();
            matT.Dispose();
        }

        void IModel<double>.Train(ILabeledExampleCollection<double> dataset)
        {
            Utils.ThrowException(dataset == null ? new ArgumentNullException("dataset") : null);
            Utils.ThrowException(!(dataset is ILabeledExampleCollection<double, SparseVector<double>>) ? new ArgumentTypeException("dataset") : null);
            Train((ILabeledExampleCollection<double, SparseVector<double>>)dataset); // throws ArgumentValueException
        }

        public Prediction<double> Predict(SparseVector<double> example)
        {
            Utils.ThrowException(mSol == null ? new InvalidOperationException() : null);
            Utils.ThrowException(example == null ? new ArgumentNullException("example") : null);
            double result = 0;
            foreach (IdxDat<double> item in example)
            {
                result += mSol[item.Idx] * item.Dat;
            }
            return new Prediction<double>(new KeyDat<double, double>[] { new KeyDat<double, double>(result, result) });
        }

        Prediction<double> IModel<double>.Predict(object example)
        {
            Utils.ThrowException(example == null ? new ArgumentNullException("example") : null);
            Utils.ThrowException(!(example is SparseVector<double>) ? new ArgumentTypeException("example") : null);
            return Predict((SparseVector<double>)example); // throws InvalidOperationException
        }

        // *** ISerializable interface implementation ***

        public void Save(BinarySerializer writer)
        {
            Utils.ThrowException(writer == null ? new ArgumentNullException("writer") : null);
            // the following statements throw serialization-related exceptions  
            writer.WriteInt(mNumIter);
            writer.WriteObject(mSol);
        }

        public void Load(BinarySerializer reader)
        {
            Utils.ThrowException(reader == null ? new ArgumentNullException("reader") : null);
            // the following statements throw serialization-related exceptions
            mNumIter = reader.ReadInt();
            mSol = reader.ReadObject<ArrayList<double>>();
        }
    }
}