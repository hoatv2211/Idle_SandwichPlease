using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePattern.Common.Injection
{
    public interface IConfigInjection : IInjection
    {
        string CachePath { get; }
        bool EnableVerboseLog { get; }
    }
}
