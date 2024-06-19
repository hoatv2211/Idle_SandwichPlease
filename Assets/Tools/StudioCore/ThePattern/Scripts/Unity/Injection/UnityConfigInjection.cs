using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ThePattern.Common.Injection;

namespace ThePattern.Unity.Injection
{
    public class UnityConfigInjection : IConfigInjection, IInjection
    {
        private string _cachePath = string.Empty;

        public string CachePath
        {
            get
            {
                if(string.IsNullOrEmpty(_cachePath))
                {
                    _cachePath = Application.persistentDataPath;
                    if(string.IsNullOrEmpty(_cachePath))
                        Debug.LogError("Disk if Full");
                }
                return _cachePath;
            }
        }
        
        public bool EnableVerboseLog => false;
    }
}
