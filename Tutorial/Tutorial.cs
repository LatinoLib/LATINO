namespace Tutorial
{
    public static partial class Tutorial
    {
    }

    public abstract class Tutorial<T> where T : Tutorial<T>, new()
    {
        public object Result { get; set; }
        
        public static T LastInstance { get; set; }
        public static object LastResult { get; set; }

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