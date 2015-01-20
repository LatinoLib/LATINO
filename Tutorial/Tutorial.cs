namespace Tutorial
{
    public abstract class Tutorial<T> where T : Tutorial<T>, new()
    {
        public static T Instance
        {
            get
            {
                return new T();
            }
        }

        public abstract void Run(string[] args);
    }
}