using System;
using System.Collections.Generic;
using System.IO;

namespace Tutorial
{
    public abstract class Tutorial<T> where T : Tutorial<T>, new()
    {
        private readonly Dictionary<string, object> mResult = new Dictionary<string, object>();

        public StreamWriter Output { get; set; }
        public Dictionary<string, object> Result
        {
            get { return mResult; }
        }

        public static T LastInstance { get; private set; }
        public static Dictionary<string, object> LastResult { get; private set; }

        public abstract void Run(object[] args);

        public static T RunInstance(params object[] args)
        {
            var writer = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };
            Console.SetOut(writer);
            return RunInstanceWr(writer, args);
        }

        public static T RunInstanceNull(params object[] args)
        {
            return RunInstanceWr(StreamWriter.Null, args);
        }

        public static T RunInstanceWr(StreamWriter writer, params object[] args)
        {
            var instance = new T { Output = writer };
            instance.Run(args);
            LastResult = instance.Result;
            LastInstance = instance;
            instance.Output.Flush();
            return instance;
        }
    }
}