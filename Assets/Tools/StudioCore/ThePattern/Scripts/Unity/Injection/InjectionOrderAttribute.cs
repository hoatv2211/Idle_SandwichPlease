using System;

namespace ThePattern.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class InjectionOrderAttribute : Attribute
    {
        public int Order { get; private set; }

        public InjectionOrderAttribute(int order) => this.Order = order;
    }
}

