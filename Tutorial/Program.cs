using System;
using System.Collections.Generic;
using System.IO;
using Tutorial.Case;
using Tutorial.Case.Clustering;
using Tutorial.Case.Data;
using Tutorial.Case.Model;
using Tutorial.Case.Other;
using Tutorial.Case.Validation;

namespace Tutorial
{
    class Program
    {
        static void Main(string[] args)
        {
            // output
            var sw = new StreamWriter("report.txt", true);

            // data
//            DataStructures.RunInstance(args);
//            SparseVector.RunInstance(args);
//            SparseMatrix.RunInstance(args);
//            Sateful.RunInstance(args);
//            Cloning.RunInstance(args);
//            Serialization.RunInstance(args);

            // model
//            Bow.RunInstance(args);
//            BinarySvm.RunInstance(args);

            // clustering
//            KMeans.RunInstance(args);
            
            // validation
            NFold.RunInstance(sw, args);

            // other
//            Searching.RunInstance(args);
        }
    }
}
