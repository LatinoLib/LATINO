/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    DifferentialEvolution.cs 
 *  Desc:    Differential evolution stochastic optimizer
 *  Created: Oct-2008
 *
 *  Author:  Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

namespace Latino.Optimization
{
    /* .-----------------------------------------------------------------------
       |		 
       |  Class DifferentialEvolution
       |
       '-----------------------------------------------------------------------
    */
    public class DifferentialEvolution : IOptimizer
    {
        private Random mRandom
            = new Random();

        private double mMinParamVal
            = 0;
        private double mMaxParamVal
            = 1;
        private int mMinNoChangeIter
            = 100;
        private double mCrossover
            = 0.9;
        private double mWgtFactor
            = 0.8;

        private ArrayList<Pair<double, ArrayList<double>>> mPopul;

        private static Logger mLogger
            = Logger.GetLogger(typeof(DifferentialEvolution));
        
        private void SetInitPopul(int numParams, int populSize)
        {
            mPopul = new ArrayList<Pair<double, ArrayList<double>>>(populSize);
            for (int i = 0; i < populSize; i++)
            {
                ArrayList<double> indiv = new ArrayList<double>(numParams);
                for (int j = 0; j < numParams; j++)
                {
                    indiv.Add(mRandom.NextDouble() * (mMaxParamVal - mMinParamVal) + mMinParamVal); // *** this never reaches mMaxParamVal
                }
                mPopul.Add(new Pair<double, ArrayList<double>>(0, indiv));
            }
        }
        
        private void GetRandomIndiv(ArrayList<double> indiv, ref ArrayList<double> rndIndiv1, ref ArrayList<double> rndIndiv2, ref ArrayList<double> rndIndiv3)
        {
            Set<ArrayList<double>> indivSet = new Set<ArrayList<double>>();
            indivSet.Add(indiv);
            for (int i = 0; i < 3; i++)
            {
                int rndIdx;
                do
                {
                    rndIdx = mRandom.Next(0, mPopul.Count);
                }
                while (indivSet.Contains(mPopul[rndIdx].Second));
                indivSet.Add(mPopul[rndIdx].Second);
            }
            indivSet.Remove(indiv);
            ArrayList<ArrayList<double>> tmp = new ArrayList<ArrayList<double>>(indivSet);
            rndIndiv1 = tmp[0];
            rndIndiv2 = tmp[1];
            rndIndiv3 = tmp[2];
        }
        
        private static ArrayList<double> ComputeInitialCandidate(ArrayList<double> indiv1, ArrayList<double> indiv2, ArrayList<double> indiv3, double wgtFactor)
        {
            ArrayList<double> cand = new ArrayList<double>(indiv1.Count);
            for (int i = 0; i < indiv1.Count; i++)
            { 
                cand.Add(indiv1[i] + wgtFactor * (indiv2[i] - indiv3[i]));
            }
            return cand;
        }
        
        private void SetNextPopul(IEval eval, double wgtFactor, double crossover, ref double bestVal, ref ArrayList<double> bestParamVec)
        {
            ArrayList<Pair<double, ArrayList<double>>> nextPopul = new ArrayList<Pair<double, ArrayList<double>>>(mPopul.Count);
            // for each individual in the population ...
            foreach (Pair<double, ArrayList<double>> indivInfo in mPopul)
            {
                ArrayList<double> indiv = indivInfo.Second;
                ArrayList<double> rndIndiv1 = null, rndIndiv2 = null, rndIndiv3 = null;
                // randomly select parents 
                GetRandomIndiv(indiv, ref rndIndiv1, ref rndIndiv2, ref rndIndiv3);
                // create initial candidate v
                ArrayList<double> v = ComputeInitialCandidate(rndIndiv1, rndIndiv2, rndIndiv3, wgtFactor);
                // create final candidate u
                ArrayList<double> u = indiv.Clone();
                for (int i = 0; i < u.Count; i++)
                {
                    if (mRandom.NextDouble() < crossover)
                    {
                        u[i] = v[i];
                    }
                }
                // accept or reject candidate u
                double newVal = eval.Eval(u);
                if (newVal > indivInfo.First) 
                {
                    nextPopul.Add(new Pair<double, ArrayList<double>>(newVal, u));
                    if (newVal > bestVal) { bestVal = newVal; bestParamVec = u; }
                }
                else
                {
                    nextPopul.Add(indivInfo);
                    if (indivInfo.First > bestVal) { bestVal = indivInfo.First; bestParamVec = indivInfo.Second; }
                }
            }
            mPopul = nextPopul;
        }
        
        public void SetRndSeed(int seed)
        {
            mRandom = new Random(seed); // throws OverflowException
        }
        
        public void SetParamRange(double minVal, double maxVal)
        {
            Utils.ThrowException(minVal >= maxVal ? new ArgumentValueException("minVal") : null);
            mMinParamVal = minVal;
            mMaxParamVal = maxVal;
        }
        
        public double MinParamVal
        {
            get { return mMinParamVal; }
        }
        
        public double MaxParamVal
        {
            get { return mMaxParamVal; }
        }
        
        public int MinNoChangeIter 
        {
            get { return mMinNoChangeIter; }
            set 
            {
                Utils.ThrowException(value < 0 ? new ArgumentValueException("MinNoChangeIter") : null);
                mMinNoChangeIter = value; 
            }
        }
        
        public double WgtFactor
        {
            get { return mWgtFactor; }
            set 
            {
                Utils.ThrowException(value <= 0.0 /*|| value > 1.0*/ ? new ArgumentValueException("WgtFactor") : null);
                mWgtFactor = value;
            }
        }
        
        public double Crossover
        {
            get { return mCrossover; }
            set
            {
                Utils.ThrowException(value <= 0.0 || value > 1.0 ? new ArgumentValueException("Crossover") : null);
                mCrossover = value;
            }
        }
        
        // *** IOptimizer interface implementation ***
        
        public ArrayList<double> Optimize(double[] initParamVec, IEval eval)
        {
            Utils.ThrowException(initParamVec == null ? new ArgumentNullException("initParamVec") : null);
            Utils.ThrowException(eval == null ? new ArgumentNullException("eval") : null);
            ArrayList<double> paramVec = new ArrayList<double>(initParamVec);
            Utils.ThrowException(paramVec.Count == 0 ? new ArgumentValueException("initParamVec") : null);
            SetInitPopul(paramVec.Count, paramVec.Count * 10); // *** make this multiplier configurable
            mPopul[0] = new Pair<double, ArrayList<double>>(0, paramVec);
            double bestGlobalVal = double.MinValue;
            ArrayList<double> bestParamVec = null;
            // evaluate initial population
            for (int i = 0; i < mPopul.Count; i++)
            {
                Pair<double, ArrayList<double>> indiv = mPopul[i];
                double localVal = eval.Eval(indiv.Second);
                mPopul[i] = new Pair<double, ArrayList<double>>(localVal, indiv.Second);
                if (localVal > bestGlobalVal) { bestGlobalVal = localVal; bestParamVec = indiv.Second; }
            }
            // optimize
            int numNoChangeIter = 0;
            double bestVal = bestGlobalVal;
            while (numNoChangeIter < mMinNoChangeIter)
            {
                SetNextPopul(eval, mWgtFactor, mCrossover, ref bestVal, ref bestParamVec);
                if (bestVal > bestGlobalVal) { numNoChangeIter = 0; bestGlobalVal = bestVal; } else { numNoChangeIter++; }
                mLogger.Info("Optimize", "Iteration status:\r\n" +
                    "No-change iterations: {0} / {1}\r\n" +
                    "Current best solution vector: {2}\r\n" +
                    "Current best solution score:  {3}", numNoChangeIter, mMinNoChangeIter, bestParamVec, bestGlobalVal);
            }
            return bestParamVec;
        }
    }
}