using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ITaskWorker
{
    bool IsDoing();
    void InitTask();
    void ResetTask();
    WorkerTask GetWorkerTask();
}

public enum EWorkerTask
{
    None =0,
    Fill_Counter = 2,
    Fill_PackageTable=3,
    Fill_DriveCar=4,
    Trash=5,
    Clean_Table =1
}

[System.Serializable]
public class WorkerTask 
{
    public bool isDoneTask = false;
    public UnitBase unitBase;
    public ITaskWorker taskWorker;
    public EWorkerTask typeWorkerTask;
    public EProductType productType;
    public WorkerSlot inputSlot;
    public WorkerSlot outputSlot;
    public WorkerUnit workerUnit;
   
    
    public void CallStart(EWorkerTask _typeTask)
    {
        typeWorkerTask = _typeTask;
    }

}
