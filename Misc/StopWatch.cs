using System;
using System.IO;

namespace Latino
{
    public class StopWatch
    {
        DateTime mStartTime
            = DateTime.Now;

        public double TotalMilliseconds
        {
            get { return (DateTime.Now - mStartTime).TotalMilliseconds; }
        }

        public void Reset()
        {
            mStartTime = DateTime.Now;
        }

        public void Save(string fileName, int count)
        {
            StreamWriter writer = new StreamWriter(fileName, /*append=*/true);
            writer.WriteLine("{0}\t{1}", count, TotalMilliseconds);
            writer.Close();
        }

        public void Save(string fileName, int count, string info)
        {
            StreamWriter writer = new StreamWriter(fileName, /*append=*/true);
            writer.WriteLine("{0}\t{1}\t{2}", count, TotalMilliseconds, info);
            writer.Close();
        }
    }
}
