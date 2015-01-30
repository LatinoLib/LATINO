using System.Collections.Generic;

namespace Tutorial
{
    public abstract class Tutorial<T> where T : Tutorial<T>, new()
    {
        private readonly Dictionary<string, object> mResult = new Dictionary<string, object>();

        public Dictionary<string, object> Result
        {
            get { return mResult; }
        }

        public static T LastInstance { get; private set; }
        public static Dictionary<string, object> LastResult { get; private set; }

        public abstract void Run(string[] args);

        public static T RunInstance(string[] args)
        {
            var instance = new T();
            instance.Run(args);
            LastResult = instance.Result;
            LastInstance = instance;
            return instance;
        }
    }
}