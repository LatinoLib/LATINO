using System.IO;
using Latino;
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
//            Logger.GetRootLogger().LocalOutputType = Logger.OutputType.Custom; // disable latino logger
            var sw = new StreamWriter("report.txt", true);
            sw.WriteLine("************");

            // data
//            DataStructures.RunInstanceWr(sw, args);
//            SparseVector.RunInstanceWr(sw, args);
//            SparseMatrix.RunInstanceWr(sw, args);
//            Sateful.RunInstanceWr(sw, args);
//            Cloning.RunInstanceWr(sw, args);
//            Serialization.RunInstanceWr(sw, args);

            // model
//            Bow.RunInstanceWr(sw, args);
            BinarySvm.RunInstanceWr(sw, args);

            // clustering
//            KMeans.RunInstanceWr(sw, args);
            
            // validation
//            NFold.RunInstanceWr(sw, args);

            // other
//            Searching.RunInstanceWr(sw, args);
        }

    }
}
