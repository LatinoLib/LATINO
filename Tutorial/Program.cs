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
            // disable latino logger
            Logger.GetRootLogger().LocalOutputType = Logger.OutputType.Custom;
            Logger.GetRootLogger().LocalProgressOutputType = Logger.ProgressOutputType.Custom;

            // output
            var sw = new StreamWriter("report.txt", true); // use file for output
            sw.WriteLine("************");

            //TestAll(args);

            // data
            //DataStructures.RunInstanceWr(sw, args);
            //SparseVector.RunInstanceWr(sw, args);
            //SparseMatrix.RunInstanceWr(sw, args);
            //Sateful.RunInstanceWr(sw, args);
            //Cloning.RunInstanceWr(sw, args);
            //Serialization.RunInstanceWr(sw, args);

            // model
            //Bow.RunInstanceWr(sw, args);
            //BinarySvm.RunInstanceWr(sw, args);

            // clustering
            //KMeans.RunInstanceWr(sw, args);
            
            // validation
            //NFold.RunInstanceWr(sw, args);
            //NFoldClass.RunInstanceWr(sw, args);
            //NFoldParallel.RunInstanceWr(sw, args);

            // other
            //Searching.RunInstanceWr(sw, args);

            // text processing
            TextProcessing.RunInstance(sw, args);
        }

        static void TestAll(string[] args)
        {
            // data
            DataStructures.RunInstanceNull(args);
            SparseVector.RunInstanceNull(args);
            SparseMatrix.RunInstanceNull(args);
            Sateful.RunInstanceNull(args);
            Cloning.RunInstanceNull(args);
            Serialization.RunInstanceNull(args);

            // model
            Bow.RunInstanceNull(args);
            BinarySvm.RunInstanceNull(args);

            // clustering
            KMeans.RunInstanceNull(args);

            // validation
            NFold.RunInstanceNull(args);
            NFoldClass.RunInstanceNull(args);

            // other
            Searching.RunInstanceNull(args);
        }

    }
}
