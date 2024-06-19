namespace ThePattern
{
    public class BaseSingleton<T> where T : BaseSingleton<T>, new()
    {
        public static T Instance
        {
            get
            {
                object obj;
                if (!SingletonCollection.Instances.TryGetValue(typeof(T), out obj))
                {
                    obj = (object)new T();
                    SingletonCollection.Instances.Add(typeof(T), obj);
                    ((T)obj).Init();
                }
                return (T)obj;
            }
        }

        protected virtual void Init()
        {
        }

        public virtual void Load()
        {
        }

        public static void ResetSingleton()
        {
            if (!SingletonCollection.Instances.ContainsKey(typeof(T)))
                return;
            SingletonCollection.Instances.Remove(typeof(T));
        }
    }
}

