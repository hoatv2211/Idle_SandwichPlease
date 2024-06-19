using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.ComponentModel;

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
    [System.Serializable]
    public class CSVReader
    {
#if ODIN_INSPECTOR || ODIN_INSPECTOR_3
        [ShowInInspector]
#endif
        // mapping table
        // use this table for mapping header to index
        [OdinSerialize] Dictionary<string, int> indexTable;

#if ODIN_INSPECTOR || ODIN_INSPECTOR_3
        [ShowInInspector]
#endif
        // context List
        [OdinSerialize] List<List<string>> contextList;

        public CSVReader()
        {
            indexTable = new Dictionary<string, int>();
            contextList = new List<List<string>>();
        }

        public void Clear()
        {
            indexTable.Clear();
            contextList.Clear();
        }


        public bool LoadFromFile(string fullPath)
        {
            // check if exist
            if (!File.Exists(fullPath))
            {
                return false;
            }

            StreamReader sr = new StreamReader(fullPath);
            LoadFromRaw(sr.ReadToEnd());
            sr.Close();

            return true;

        }

        public bool LoadFromFile(string folderPath, string fileName)
        {
            string fullPath = Path.Combine(folderPath, fileName);
            // check if exist
            if (!File.Exists(fullPath))
            {
                return false;
            }

            StreamReader sr = new StreamReader(fullPath);
            LoadFromRaw(sr.ReadToEnd());
            sr.Close();

            return true;

        }


        public bool LoadFromRaw(string rawData)
        {
            Clear();

            Queue<string> lines = new Queue<string>(rawData.Split('\n'));     

            // head
            string line = lines.Dequeue();
            string[] headers = line.Split(',');
            for (int i = 0; i < headers.Length; i++)
            {
                // replace "\"" to ""
                headers[i] = headers[i].Replace("\"", "");
                // Debug.Log ("header: " + headers[i]);
                indexTable.Add(headers[i], i);
            }

            // context
            while (lines.Count >0)
            {
                line = lines.Dequeue();
                string[] data = line.Split(',');

                List<string> current = new List<string>();

                for (int i = 0; i < data.Length;)
                {
                    // string
                    if (!data[i].StartsWith("\""))
                    {
                        current.Add((data[i++].Trim()));
                    }
                    // array e.g: "a,a,a,a"
                    else
                    {
                        string str = "";
                        for (; i < data.Length;)
                        {
                            str += data[i++];
                            if (data[i - 1].EndsWith("\""))
                            {
                                break;
                            }
                            else
                            {
                                str += ",";
                            }
                        }

                        // replace "\"" to ""
                        str = str.Replace("\"", "");
                        current.Add(str.Trim());
                    }

                }

                contextList.Add(current);

            }
            return true;
        }

        public string GetRaw()
        {
            string s = "";

            // header
            List<string> k = new List<string>(indexTable.Keys);
            for (int i = 0; i < k.Count; i++)
            {
                if (i > 0)
                {
                    s += ",";
                }
                s += $"\"{k[i]}\"";
            }
            s += "\n";

            // cells
            foreach (var c in contextList)
            {
                for(int i=0; i< c.Count; i++)
                {
                    if(i>0)
                    {
                        s += ",";
                    }
                    s += $"\"{c[i]}\"";
                }
                s += "\n"; 
            }


            return s;
        }

        public void SaveToFile(string folderPath, string fileName)
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string fullpath = Path.Combine(folderPath, fileName);

            StreamWriter sw = new StreamWriter(fullpath, false);

            sw.Write(GetRaw());
            sw.Close();

        }


        public int RowCount()
        {
            return contextList.Count;
        }

        public int ColCount()
        {
            return indexTable.Count;
        }


        // get the origin record
        public List<string> GetRow(int row)
        {
            return contextList[row];
        }


        public List<string> GetCol(string colName)
        {
            List<string> res = new List<string>();
            foreach (var r in contextList)
            {
                res.Add(r[indexTable[colName]]);
            }

            return res;
        }

        // get the data by the given column name
        public string GetCell(int row, string colName)
        {
            return GetRow(row)[indexTable[colName]];
        }


        public void CreateRow()
        {
            List<string> newRow = new List<string>();
            for(int i=0; i<ColCount(); i++)
            {
                newRow.Add("");
            }
            contextList.Add(newRow);
        }

        public void CreateCol(string colName)
        {
            foreach(var r in contextList)
            {
                r.Add("");
            }

            // add index table
            if(!string.IsNullOrEmpty(colName))
            {
                int index = ColCount();
                indexTable.Add(colName, index);
            }
        }

        public void UpdateCell(int row, string colName, string value)
        {
            contextList[row][indexTable[colName]] = value;
        }

        public void RemoveRow(int row)
        {
            contextList.RemoveAt(row);
        }

        public void RemoveCol(string colName)
        {
            if (!HasCol(colName))
                return;

            int index = indexTable[colName];
            foreach(var r in contextList)
            {
                r.RemoveAt(index);
            }
            indexTable.Remove(colName);

        }

        public bool HasCol(string colName)
        {
            return indexTable.ContainsKey(colName);
        }
    }
}