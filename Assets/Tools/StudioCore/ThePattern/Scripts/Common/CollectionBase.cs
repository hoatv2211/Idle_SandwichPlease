using System;
using System.Collections.Generic;
using UnityEngine;

namespace ThePattern
{
    public class CollectionBase
    {
        protected static readonly Dictionary<Type, object> _instances = new Dictionary<Type, object>();

        protected CollectionBase() { }
    }
}

