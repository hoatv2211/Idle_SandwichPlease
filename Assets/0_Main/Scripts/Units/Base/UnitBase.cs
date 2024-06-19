using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : MonoBehaviour
{
    public string IdUnit = "";
    public UnitData m_UnitData;
    public WorkerSlot _inputWorkerSlot;
    public WorkerSlot _outputWorkerSlot;

    private string idmap => MapController.Instance._idMap == 1 ? string.Empty : MapController.Instance._idMap.ToString();
    public virtual void Load_Unlock() 
    {
      
        JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(idmap+IdUnit), m_UnitData);


        if (MapController.Instance.mapData.IsUnlock(IdUnit))
        {
            m_UnitData = MapController.Instance.mapData.GetUnlockUnitData(IdUnit);
            m_UnitData.isUnlocked = true;

        }
       

        CallStart();

    }

    public virtual void CallStart() { }

    public virtual void Save_Unlock()  
    {
        PlayerPrefs.SetString(idmap+IdUnit, JsonUtility.ToJson(m_UnitData));
    }
    public virtual void Action_UnlockUnit() { m_UnitData.isUnlocked = true; }
    public virtual void Action_UpgradeUnit() { }
    public virtual void Action_OnWorkUnit() { }
    public virtual void Action_OffWorkUnit() { }
    public virtual void Action_AddProduct(ProductUnit _unit) { }
    public virtual void Action_RemoveProduct(ProductUnit _unit) { }

    public virtual void GenerateInMap()
    {

    }
}

[System.Serializable]
public class UnitData
{
    public string mapID;
    public string idUnit;
    public string name;

    public DataMapProcess dataMapProcess;

    [Space]
    public bool isUnlocked = false;     //save
    public int costUnlock = 10;
    public int residualPrice = 10;
    public int crLevel = 1;

    [Space]
    public int moneySave = 0;

    public UnitData Clone()
    {
        UnitData _cl        = new UnitData();
        _cl.mapID           = mapID;
        _cl.idUnit          = idUnit;
        _cl.name            = name;

        _cl.dataMapProcess  = this.dataMapProcess;

        _cl.isUnlocked      = isUnlocked;
        _cl.costUnlock      = costUnlock;
        _cl.residualPrice   = residualPrice;
        _cl.crLevel         = crLevel;
        _cl.moneySave       = moneySave;


        return _cl;
    }
  
}

[System.Serializable]
public class UnlockLevel
{
    public int level = 0;
    public List<GameObject> listOffs;
    public List<GameObject> listOns;
}

[System.Serializable]
public class MachineData : UnitData
{
    public EObjectType objectType;
    public int id;

    public int speedLvl;
    public int stackLvl;

    public float Speed => 2;
    public int CarryStack =>10;
}

#region Saved

[Serializable]
public class SavedMapData
{
    public bool isUnlocked;
    public long nextIncomeTime;
    //public List<SavedMapObject> savedMapObjects;

    //public SavedMapData()
    //{
    //    isUnlocked = true;
    //    nextIncomeTime = 0;
    //    savedMapObjects = new List<SavedMapObject>();
    //}

    //public bool ContainsMapObject(EObjectType objectType, string objectName, int id)
    //{
    //    return savedMapObjects.FindIndex(o =>
    //        o.objectType == objectType
    //        && string.Compare(o.objectName.ToLower(), objectName.ToLower()) == 0
    //        && o.id == id) >= 0;
    //}
    //public int GetMapObjectIndex(EObjectType objectType, string objectName, int id)
    //{
    //    int foundIndex = savedMapObjects.FindIndex(o =>
    //        o.objectType == objectType
    //        && string.Compare(o.objectName.ToLower(), objectName.ToLower()) == 0
    //        && o.id == id);
    //    return foundIndex;
    //}
    //public SavedMapObject GetSavedMapObject(EObjectType objectType, string objectName, int id)
    //{
    //    if (!ContainsMapObject(objectType, objectName, id))
    //    {
    //        savedMapObjects.Add(new SavedMapObject(objectType, objectName, id));
    //    }
    //    int foundIndex = GetMapObjectIndex(objectType, objectName, id);
    //    return savedMapObjects[foundIndex];
    //}
}



#endregion