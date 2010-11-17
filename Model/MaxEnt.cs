/*==========================================================================;
 *
 *  This file is part of LATINO. See http://latino.sf.net
 *
 *  File:    MaxEnt.cs
 *  Desc:    Maximum entropy classifier 
 *  Created: Sep-2009
 *
 *  Authors: Jan Rupnik, Miha Grcar
 *
 ***************************************************************************/

using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Latino.Model
{
    /* .-----------------------------------------------------------------------
       |
       |  Class MaxEnt
       |
       '-----------------------------------------------------------------------
    */
    internal static class MaxEnt
    {
        private static SparseMatrix<double> CreateObservationMatrix<LblT>(ILabeledExampleCollection<LblT, BinaryVector<int>.ReadOnly> dataset, ref LblT[] idxToLbl)
        {
            ArrayList<LblT> tmp = new ArrayList<LblT>();
            Dictionary<LblT, int> lblToIdx = new Dictionary<LblT, int>();
            foreach (LabeledExample<LblT, BinaryVector<int>.ReadOnly> labeledExample in dataset)
            {
                if (!lblToIdx.ContainsKey(labeledExample.Label))
                {
                    lblToIdx.Add(labeledExample.Label, lblToIdx.Count);
                    tmp.Add(labeledExample.Label);
                }
            }            
            // prepare struct for fast computation
            Dictionary<int, int>[] counter = new Dictionary<int, int>[tmp.Count];
            for (int j = 0; j < counter.Length; j++) { counter[j] = new Dictionary<int, int>(); }
            // count features
            int i = 0;
            foreach (LabeledExample<LblT, BinaryVector<int>.ReadOnly> labeledExample in dataset)
            {
                Utils.VerboseProgress("{0} / {1}", ++i, dataset.Count);
                int lblIdx = lblToIdx[labeledExample.Label];
                int val;
                foreach (int idx in labeledExample.Example)
                {
                    if (counter[lblIdx].TryGetValue(idx, out val))
                    {
                        counter[lblIdx][idx] = val + 1;
                    }
                    else
                    {
                        counter[lblIdx].Add(idx, 1);
                    }
                }
            }            
            // create sparse matrix
            SparseMatrix<double> mtx = new SparseMatrix<double>();
            for (int j = 0; j < counter.Length; j++)
            {
                SparseVector<double> vec = new SparseVector<double>();
                foreach (KeyValuePair<int, int> item in counter[j])
                {
                    vec.InnerIdx.Add(item.Key);
                    vec.InnerDat.Add(item.Value);
                }
                vec.Sort();
                mtx[j] = vec;
            }
            idxToLbl = tmp.ToArray();
            return mtx;
        }

        private static SparseMatrix<double> CutOff(SparseMatrix<double>.ReadOnly mtx, int cutOff)
        {
            SparseMatrix<double> newMtx = new SparseMatrix<double>();
            foreach (IdxDat<SparseVector<double>.ReadOnly> row in mtx)
            {
                ArrayList<IdxDat<double>> tmp = new ArrayList<IdxDat<double>>();
                foreach (IdxDat<double> item in row.Dat)
                {
                    if (item.Dat > cutOff) { tmp.Add(item); }
                }
                newMtx[row.Idx] = new SparseVector<double>(tmp);
            }
            return newMtx;
        }

        private static SparseMatrix<double> CopyStructure(SparseMatrix<double>.ReadOnly mtx)
        {
            SparseMatrix<double> newMtx = new SparseMatrix<double>();
            foreach (IdxDat<SparseVector<double>.ReadOnly> row in mtx)
            {
                ArrayList<IdxDat<double>> tmp = new ArrayList<IdxDat<double>>();
                foreach (IdxDat<double> item in row.Dat)
                {
                    tmp.Add(new IdxDat<double>(item.Idx, 0));
                }
                newMtx[row.Idx] = new SparseVector<double>(tmp);
            }
            return newMtx;
        }

        private static void Reset(SparseMatrix<double> mtx)
        {
            foreach (IdxDat<SparseVector<double>> row in mtx)
            {
                for (int i = 0; i < row.Dat.Count; i++)
                {
                    row.Dat.SetDirect(i, 0);
                }
            }
        }

        private static void GisUpdate(SparseMatrix<double> lambda, SparseMatrix<double>.ReadOnly expectations, SparseMatrix<double>.ReadOnly observations, double f)
        {
            foreach (IdxDat<SparseVector<double>.ReadOnly> row in observations)
            {
                int i = 0;
                foreach (IdxDat<double> item in row.Dat)
                {
                    double newVal = lambda[row.Idx].GetDirect(i).Dat + 1.0 / f * Math.Log(observations[row.Idx].GetDirect(i).Dat / expectations[row.Idx].GetDirect(i).Dat);
                    lambda[row.Idx].SetDirect(i, newVal);
                    i++;
                }
            }
        }

        private static double GisFindMaxF<LblT>(ILabeledExampleCollection<LblT, BinaryVector<int>.ReadOnly> dataset)
        {
            double maxVal = 0;
            foreach (LabeledExample<LblT, BinaryVector<int>.ReadOnly> item in dataset)
            {
                if (item.Example.Count > maxVal) { maxVal = item.Example.Count; }
            }
            return maxVal;
        }
        
        private static void UpdateExpectationMatrixPass1(object _args)
        {
            object[] args = (object[])_args;
            int startIdx = (int)args[0];
            int endIdx = (int)args[1];
            SparseMatrix<double>.ReadOnly trainMtxTr = (SparseMatrix<double>.ReadOnly)args[2];
            IdxDat<SparseVector<double>.ReadOnly>[] rows = (IdxDat<SparseVector<double>.ReadOnly>[])args[3];
            double[][] mtx = (double[][])args[4];
            Ref<int> progress = (Ref<int>)args[5];
            for (int i = startIdx; i <= endIdx; i++)
            {
                IdxDat<SparseVector<double>.ReadOnly> row = rows[i];                
                foreach (IdxDat<double> item in row.Dat)
                {
                    if (trainMtxTr.ContainsRowAt(item.Idx))
                    {
                        SparseVector<double>.ReadOnly trainMtxRow = trainMtxTr[item.Idx];
                        foreach (IdxDat<double> trainMtxItem in trainMtxRow)
                        {
                            mtx[row.Idx][trainMtxItem.Idx] += trainMtxItem.Dat * item.Dat;
                        }
                    }
                }
                progress.Val++;
            }
        }

        private static void UpdateExpectationMatrixPass2(object _args)
        {
            object[] args = (object[])_args;
            int startIdx = (int)args[0];
            int endIdx = (int)args[1];
            SparseMatrix<double>.ReadOnly trainMtxTr = (SparseMatrix<double>.ReadOnly)args[2];
            IdxDat<SparseVector<double>>[] rows = (IdxDat<SparseVector<double>>[])args[3];
            double[][] mtx = (double[][])args[4];
            double[] z = (double[])args[5];
            Ref<int> progress = (Ref<int>)args[6];
            for (int i = startIdx; i <= endIdx; i++)
            {
                IdxDat<SparseVector<double>> row = rows[i];     
                int itemIdx = 0;
                foreach (IdxDat<double> item in row.Dat)
                {
                    SparseVector<double>.ReadOnly pom = trainMtxTr[item.Idx];
                    foreach (IdxDat<double> pomItem in pom)
                    {
                        row.Dat.SetDirect(itemIdx, row.Dat.GetDatDirect(itemIdx) + mtx[row.Idx][pomItem.Idx] / z[pomItem.Idx] * pomItem.Dat);
                    }
                    itemIdx++;
                }
                progress.Val++;
            }
        }

        private static void UpdateExpectationMatrix(int numClasses, int trainSetSize, SparseMatrix<double>.ReadOnly trainMtxTr, SparseMatrix<double>.ReadOnly lambda, SparseMatrix<double> expectations, int numThreads)
        {
            double[][] mtx = new double[numClasses][];
            for (int j = 0; j < numClasses; j++) { mtx[j] = new double[trainSetSize]; }
            double[] z = new double[trainSetSize];
            Utils.VerboseLine("Initiating {0} threads ...", numThreads);
            int lambdaRowCount = lambda.GetRowCount();
            IdxDat<SparseVector<double>.ReadOnly>[] aux = new IdxDat<SparseVector<double>.ReadOnly>[lambdaRowCount];
            int i = 0;
            foreach (IdxDat<SparseVector<double>.ReadOnly> row in lambda)
            {
                aux[i++] = row;
            }
            int chunkSz = (int)Math.Round((double)lambdaRowCount / (double)numThreads); // *** this load balancing is not so good; should I count values instead of rows?
            Thread[] threads = new Thread[numThreads];
            Ref<int>[] progressInfo = new Ref<int>[numThreads];
            int startIdx = 0;            
            for (i = 0; i < numThreads; i++)
            {
                int endIdx = startIdx + chunkSz - 1;
                if (i == numThreads - 1) { endIdx = aux.Length - 1; }
                progressInfo[i] = new Ref<int>();
                threads[i] = new Thread(new ParameterizedThreadStart(UpdateExpectationMatrixPass1));
                threads[i].Start(new object[] { startIdx, endIdx, trainMtxTr, aux, mtx, progressInfo[i] });
                startIdx += chunkSz;
            }
            bool isAlive = true;
            while (isAlive)
            {
                int aggrProgress = 0;
                foreach (Ref<int> progress in progressInfo)
                {
                    aggrProgress += progress;
                }
                Utils.Verbose("Pass 1: {0} / {1}\r", aggrProgress, lambdaRowCount);
                isAlive = false;
                foreach (Thread thread in threads)
                {
                    isAlive = isAlive || thread.IsAlive;
                }
                Thread.Sleep(100);               
            }
            Utils.VerboseLine("Pass 1: {0} / {0}\r", lambdaRowCount);
            for (i = 0; i < numClasses; i++)
            {
                for (int j = 0; j < trainSetSize; j++)
                {
                    mtx[i][j] = Math.Exp(mtx[i][j]);
                    z[j] += mtx[i][j];
                }
            }
            int expeRowCount = expectations.GetRowCount();
            IdxDat<SparseVector<double>>[] aux2 = new IdxDat<SparseVector<double>>[expeRowCount];
            i = 0;
            foreach (IdxDat<SparseVector<double>> row in expectations)
            {
                aux2[i++] = row;
            }
            startIdx = 0;
            for (i = 0; i < numThreads; i++)
            {
                int endIdx = startIdx + chunkSz - 1;
                if (i == numThreads - 1) { endIdx = aux.Length - 1; }
                progressInfo[i] = 0;
                threads[i] = new Thread(new ParameterizedThreadStart(UpdateExpectationMatrixPass2));
                threads[i].Start(new object[] { startIdx, endIdx, trainMtxTr, aux2, mtx, z, progressInfo[i] });
                startIdx += chunkSz;
            }
            isAlive = true;
            while (isAlive)
            {
                int aggrProgress = 0;
                foreach (Ref<int> progress in progressInfo)
                {
                    aggrProgress += progress;
                }
                Utils.Verbose("Pass 2: {0} / {1}\r", aggrProgress, expeRowCount);
                isAlive = false;
                foreach (Thread thread in threads)
                {
                    isAlive = isAlive || thread.IsAlive;
                }
                Thread.Sleep(100);
            }
            Utils.VerboseLine("Pass 2: {0} / {0}\r", expeRowCount);
        }

        private static void UpdateExpectationMatrix(int numClasses, int trainSetSize, SparseMatrix<double>.ReadOnly trainMtxTr, SparseMatrix<double>.ReadOnly lambda, SparseMatrix<double> expectations)
        {
            double[][] mtx = new double[numClasses][];
            for (int j = 0; j < numClasses; j++) { mtx[j] = new double[trainSetSize]; }
            double[] z = new double[trainSetSize];
            foreach (IdxDat<SparseVector<double>.ReadOnly> row in lambda)
            {
                Utils.Verbose("Pass 1: {0} / {1}\r", row.Idx + 1, lambda.GetLastNonEmptyRowIdx() + 1);
                foreach (IdxDat<double> item in row.Dat)
                {
                    if (trainMtxTr.ContainsRowAt(item.Idx))
                    {
                        SparseVector<double>.ReadOnly trainMtxRow = trainMtxTr[item.Idx];
                        foreach (IdxDat<double> trainMtxItem in trainMtxRow)
                        {
                            mtx[row.Idx][trainMtxItem.Idx] += trainMtxItem.Dat * item.Dat;
                        }
                    }
                }
            }
            Utils.VerboseLine();
            for (int i = 0; i < numClasses; i++)
            {
                for (int j = 0; j < trainSetSize; j++)
                {
                    mtx[i][j] = Math.Exp(mtx[i][j]);
                    z[j] += mtx[i][j];
                }
            }
            foreach (IdxDat<SparseVector<double>> row in expectations)
            {
                Utils.Verbose("Pass 2: {0} / {1}\r", row.Idx + 1, expectations.GetLastNonEmptyRowIdx() + 1);
                int itemIdx = 0;
                foreach (IdxDat<double> item in row.Dat)
                {
                    SparseVector<double>.ReadOnly pom = trainMtxTr[item.Idx];
                    foreach (IdxDat<double> pomItem in pom)
                    {
                        row.Dat.SetDirect(itemIdx, row.Dat.GetDatDirect(itemIdx) + mtx[row.Idx][pomItem.Idx] / z[pomItem.Idx] * pomItem.Dat);
                    }
                    itemIdx++;
                }
            }
            Utils.VerboseLine();
        }

        private static SparseMatrix<double> TransposeDataset<LblT>(ILabeledExampleCollection<LblT, BinaryVector<int>.ReadOnly> dataset, bool clearDataset)
        {
            SparseMatrix<double> aux = new SparseMatrix<double>();
            int i = 0;
            if (clearDataset)
            {
                foreach (LabeledExample<LblT, BinaryVector<int>.ReadOnly> item in dataset)
                {
                    aux[i++] = ModelUtils.ConvertExample<SparseVector<double>>(item.Example);
                    item.Example.Inner.Clear(); // *** clear read-only vectors to save space
                }
            }
            else
            {
                foreach (LabeledExample<LblT, BinaryVector<int>.ReadOnly> item in dataset)
                {
                    aux[i++] = ModelUtils.ConvertExample<SparseVector<double>>(item.Example);
                }
            }
            return aux.GetTransposedCopy();
        }

        public static SparseMatrix<double> Gis<LblT>(ILabeledExampleCollection<LblT, BinaryVector<int>.ReadOnly> dataset, int cutOff, int numIter, bool clearDataset, string mtxFileName, ref LblT[] idxToLbl, int numThreads, double allowedDiff) 
        {
            Utils.VerboseLine("Creating observation matrix ...");
            SparseMatrix<double> observations = null;
            if (Utils.VerifyFileNameOpen(mtxFileName))
            {
                BinarySerializer reader = new BinarySerializer(mtxFileName, FileMode.Open);
                idxToLbl = new ArrayList<LblT>(reader).ToArray();
                observations = new SparseMatrix<double>(reader);
                reader.Close();
            }
            else
            {
                observations = CreateObservationMatrix(dataset, ref idxToLbl);
                if (Utils.VerifyFileNameCreate(mtxFileName))
                {
                    BinarySerializer writer = new BinarySerializer(mtxFileName, FileMode.Create);
                    new ArrayList<LblT>(idxToLbl).Save(writer);
                    observations.Save(writer);
                    writer.Close();
                }
            }
            int numClasses = observations.GetLastNonEmptyRowIdx() + 1;
            int numExamples = dataset.Count;
            if (cutOff > 0)
            {
                Utils.VerboseLine("Performing cut-off ...");
                observations = CutOff(observations, cutOff);
            }
            Utils.VerboseLine("Preparing structures ...");
            SparseMatrix<double> lambda = CopyStructure(observations);
            SparseMatrix<double> expectations = CopyStructure(observations);
            double f = GisFindMaxF(dataset);
            SparseMatrix<double> trainMtxTr = TransposeDataset(dataset, clearDataset);            
            Utils.VerboseLine("Entering main loop ...");
            double[] oldLambda = null;
            if (allowedDiff > 0)
            {
                oldLambda = new double[lambda.CountValues()];
            }
            for (int i = 0; i < numIter; i++)
            {
                Utils.VerboseLine("Iteration {0} / {1} ...", i + 1, numIter);
                Utils.VerboseLine("Updating expectations ...");
                if (numThreads > 1)
                {
                    UpdateExpectationMatrix(numClasses, numExamples, trainMtxTr, lambda, expectations, numThreads);
                }
                else
                {
                    UpdateExpectationMatrix(numClasses, numExamples, trainMtxTr, lambda, expectations);
                }
                Utils.VerboseLine("Updating lambdas ...");
                GisUpdate(lambda, expectations, observations, f);
                Reset(expectations);
                // check lambda change
                if (allowedDiff > 0)
                {
                    int j = 0;
                    double maxDiff = 0;
                    foreach (IdxDat<SparseVector<double>> row in lambda)
                    {
                        foreach (IdxDat<double> item in row.Dat)
                        {
                            double diff = Math.Abs(item.Dat - oldLambda[j]);
                            if (diff > maxDiff) { maxDiff = diff; }
                            oldLambda[j] = item.Dat;
                            j++;
                        }
                    }
                    Utils.VerboseLine("Max lambda diff: {0:0.0000}", maxDiff);
                    if (maxDiff <= allowedDiff)
                    {
                        Utils.VerboseLine("Max lambda diff is small enough. Exiting optimization loop.");
                        break;
                    }
                }
            }
            return lambda;
        }

        public static Prediction<LblT> Classify<LblT>(BinaryVector<int>.ReadOnly binVec, SparseMatrix<double>.ReadOnly lambdas, LblT[] idxToLbl, bool normalize)
        {
            SparseVector<double> vec = ModelUtils.ConvertExample<SparseVector<double>>(binVec);
            Prediction<LblT> scores = new Prediction<LblT>();
            double sum = 0;
            foreach (IdxDat<SparseVector<double>.ReadOnly> row in lambdas)
            {
                double score = Math.Exp(DotProductSimilarity.Instance.GetSimilarity(row.Dat, vec));
                scores.Inner.Add(new KeyDat<double, LblT>(score, idxToLbl[row.Idx]));
                sum += score;
            }
            if (normalize && sum > 0)
            {
                for (int i = 0; i < scores.Count; i++)
                {
                    KeyDat<double, LblT> score = scores[i];
                    scores.Inner[i] = new KeyDat<double, LblT>(score.Key / sum, score.Dat);
                }
            }
            scores.Inner.Sort(DescSort<KeyDat<double, LblT>>.Instance);
            return scores;
        }

        public static Dictionary<int, double>[] PrepareForFastPrediction(SparseMatrix<double>.ReadOnly lambdas)
        {
            Dictionary<int, double>[] retVal = new Dictionary<int, double>[lambdas.GetLastNonEmptyRowIdx() + 1];
            foreach (IdxDat<SparseVector<double>.ReadOnly> row in lambdas)
            {
                Dictionary<int, double> vec = new Dictionary<int, double>();
                foreach (IdxDat<double> item in row.Dat)
                {
                    vec.Add(item.Idx, item.Dat);
                }
                retVal[row.Idx] = vec;
            }
            return retVal;
        }

        public static Prediction<LblT> Classify<LblT>(BinaryVector<int>.ReadOnly binVec, Dictionary<int, double>[] lambdas, LblT[] idxToLbl, bool normalize)
        { 
            Prediction<LblT> scores = new Prediction<LblT>();
            double sum = 0;
            int i = 0;
            foreach (Dictionary<int, double> row in lambdas)
            {
                if (row != null)
                {
                    double score = 0;
                    foreach (int idx in binVec)
                    {
                        double val;
                        if (row.TryGetValue(idx, out val)) { score += val; }
                    }
                    score = Math.Exp(score);
                    scores.Inner.Add(new KeyDat<double, LblT>(score, idxToLbl[i]));
                    sum += score;
                }
                i++;
            }
            if (normalize && sum > 0)
            {
                for (i = 0; i < scores.Count; i++)
                {
                    KeyDat<double, LblT> score = scores[i];
                    scores.Inner[i] = new KeyDat<double, LblT>(score.Key / sum, score.Dat);
                }
            }
            scores.Inner.Sort(DescSort<KeyDat<double, LblT>>.Instance);
            return scores;
        }
    }
}