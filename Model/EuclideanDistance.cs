using System;

namespace Latino.Model
{
    public class EuclideanDistance : IDistance<SparseVector<double>>
    {
        public static readonly EuclideanDistance Instance = new EuclideanDistance();

        public double GetDistance(SparseVector<double> bow1, SparseVector<double> bow2)
        {
            double sum = 0;
            int i = 0, j = 0;
            while (i < bow1.Count || j < bow2.Count)
            {
                while (i < bow1.Count && (j == bow2.Count || bow1.InnerIdx[i] < bow2.InnerIdx[j]))
                {
                    sum += bow1.InnerDat[i] * bow1.InnerDat[i];
                    i++;
                }
                while (i < bow1.Count && j < bow2.Count && bow1.InnerIdx[i] == bow2.InnerIdx[j])
                {
                    sum += (bow1.InnerDat[i] - bow2.InnerDat[j]) * (bow1.InnerDat[i] - bow2.InnerDat[j]);
                    i++; j++;
                }
                while (j < bow2.Count && (i == bow1.Count || bow2.InnerIdx[j] < bow1.InnerIdx[i]))
                {
                    sum += bow2.InnerDat[j] * bow2.InnerDat[j];
                    j++;
                }
            }
            return Math.Sqrt(sum);
        }

        public void Save(BinarySerializer writer)
        {
            throw new NotImplementedException();
        }
    }
}