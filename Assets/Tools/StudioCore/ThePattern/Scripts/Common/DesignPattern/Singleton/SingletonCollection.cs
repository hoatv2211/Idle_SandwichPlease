using System;
using System.Collections.Generic;

namespace ThePattern
{
    public class SingletonCollection
    {
        private static readonly Dictionary<Type, object> _instances = new Dictionary<Type, object>();
        public static Dictionary<Type, object> Instances => _instances;
    }
}