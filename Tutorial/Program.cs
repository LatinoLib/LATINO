using System;
using System.Collections.Generic;
using Tutorial.Case;
using Tutorial.Case.Data;
using Tutorial.Case.Model;

namespace Tutorial
{
    class Program
    {
        static void Main(string[] args)
        {
            DataStructures.RunInstance(args);
//            SparseVector.Instance.Run(args);
//            SparseMatrix.Instance.Run(args);
//            C04_Sateful.Instance.Run(args);
//            C03_Cloning.Instance.Run(args);
//            Serialization.Instance.Run(args);
//            Bow.Instance.Run(args);
//            KMeans.Instance.Run(args);
//            Searching.Instance.Run(args);
            var results = new List<double>();
            for (int i = 0; i < 1; i++)
            {
                results.Add((double)Svm.RunInstance(args).Result);
            }
            Console.WriteLine("\n\n\n");
            foreach (double result in results)
            {
                Console.WriteLine("Accurracy: {0}", result);
            }
        }
    }
}
