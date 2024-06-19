using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
///
/// MIT license
/// Created by Haikun Huang
/// Date: 2021
/// </summary>
///
#if ODIN_INSPECTOR_3 || ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.Serialization;
#else
using OdinSerializer;
#endif

namespace DCXR
{

    [HelpURL("https://www.notion.so/CSV-Manager-af6deb14f93a4d65ba9d42d911f6884d")]
    [CreateAssetMenu(menuName = "DCXR/CSV Manager", fileName = "CSV Manager")]
    public class CSVManager : SerializedScriptableObject
    {

#if ODIN_INSPECTOR || ODIN_INSPECTOR_3
        [ShowInInspector]
#endif
        [OdinSerialize] Dictionary<string, CSVReader> data = new Dictionary<string, CSVReader>();

        // Add a new database
        public void AddDatabase(string key, CSVReader csv)
        {
            if (!data.ContainsKey(key))
                data.Add(key, csv);
        }


        // Create a new database
        public void CreateDatabase(string key)
        {
            AddDatabase(key, new CSVReader());
        }

        // Remove a database
        public void RemoveDatabase(string key)
        {
            data.Remove(key);
        }

        // Has database
        public bool HasDatabase(string key)
        {
            return data.ContainsKey(key);
        }


        [ContextMenu("GetDatabase")]
        // Get database
        public CSVReader GetDatabase(string key)
        {
            if (data.ContainsKey(key))
                return data[key];

            CreateDatabase(key);
            return data[key];
        }

        // Clear all database
        public void ClearAllDatabase()
        {
            data.Clear();
        }


    }

}
