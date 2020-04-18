namespace ConsoleTool
{
    public abstract class SingletonManager<T> : ISingletonManager where T : class, ISingletonManager, new()
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                }
                return instance;
            }
        }

        public virtual void Init()
        {

        }
    }
}
