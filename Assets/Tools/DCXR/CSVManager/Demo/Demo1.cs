using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DCXR.CSVManagerDemo
{
    public class Demo1 : MonoBehaviour
    {
        [SerializeField] CSVManager manager;
        // Start is called before the first frame update
        void Start()
        {

            // download the csv file if needed.
            Debug.Log("Please Download the example csv file from: https://github.com/quincyhuang/CSVManager/tree/main/CSV");
            Debug.Log($"File load from: {Application.dataPath}/Tools/DCXR/CSV/TechCrunchcontinentalUSA.csv");

            // load
            manager.GetDatabase("csv").LoadFromFile($"{Application.dataPath}/Tools/DCXR/CSV", "TechCrunchcontinentalUSA.csv");
            Debug.Log($"[0,category] = {manager.GetDatabase("csv").GetCell(0, "category")}");

            manager.GetDatabase("save").LoadFromFile($"{Application.dataPath}/Tools/DCXR/CSV", "save.csv");

            // create a new csv file
            manager.RemoveDatabase("mycsv");
            manager.CreateDatabase("mycsv");

            // create cols
            manager.GetDatabase("mycsv").CreateCol("ID");
            manager.GetDatabase("mycsv").CreateCol("Name");
            manager.GetDatabase("mycsv").CreateCol("Age");

            // create cells
            for (int i = 0; i < 5; i++)
            {
                manager.GetDatabase("mycsv").CreateRow();
                manager.GetDatabase("mycsv").UpdateCell(i, "ID", i.ToString());
                manager.GetDatabase("mycsv").UpdateCell(i, "Name", UnityEngine.Random.Range(1000,9999).ToString());
                manager.GetDatabase("mycsv").UpdateCell(i, "Age", UnityEngine.Random.Range(20, 50).ToString());
            }

            // remove row
            //manager.GetDatabase("mycsv").RemoveRow(3);

            // remove col
            //manager.GetDatabase("mycsv").RemoveCol("Name");

            // save
            manager.GetDatabase("mycsv").SaveToFile($"{Application.dataPath}/Tools/DCXR/CSV", "save.csv");
            Debug.Log($"File saved to: {Application.dataPath}/Tools/DCXR/CSV/save.csv");
        }

    }
}