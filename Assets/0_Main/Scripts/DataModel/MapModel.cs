using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "MAD/MapModel", fileName = "MapModel")]
public class MapModel : ScriptableObject
{
    public int mapID;
    public MapData mapData;

    public DataMapProcess[] mapObjects;

    [Header("Upgrade DATA")]
    public List<UpgradeData> PU_Speed;
    public List<UpgradeData> PU_Capacity;
    public List<UpgradeData> PU_Profit;
    public List<UpgradeData> Staff_Speed;
    public List<UpgradeData> Staff_Capacity;
    public List<UpgradeData> Staff_Employ;

    [Header("Units Base")]
    public List<UnitDataBase> unitDataBases;

    [Header("Map Level Process")]
    public List<LevelProcess> levelProcesses;

    public UnitDataBase GetUnitDataBase(string _id)
    {
        UnitDataBase unitData = unitDataBases.Find(x => x.idUnit == _id); 

        return unitData;
    }

    public DataMapProcess GetDataMapProcess(string _id)
    {
        return mapObjects.ToList().Find(x => x.id == _id);
    }

    public LevelProcess GetLevelProcess(int _point)
    {
        LevelProcess _lv = new LevelProcess();
        _lv = levelProcesses.Where(x => (x.point_space[0] <= _point&&x.point_space[1]>_point)).First();

        if (_lv == null)
            _lv = levelProcesses[0];
        //Debug.LogError(JsonUtility.ToJson(_lv));
        return _lv;
    }


    [ContextMenu("LoadRefData")]
    public void LoadRefData()
    {
        UpgradeModel gm = Resources.Load<UpgradeModel>("UpgradeModel");
        PU_Speed        = gm.PU_Speed       .Where(x => x.map_id==mapID).ToList();
        PU_Capacity     = gm.PU_Capacity    .Where(x => x.map_id == mapID).ToList();
        PU_Profit       = gm.PU_Profit      .Where(x => x.map_id == mapID).ToList();
        Staff_Speed     = gm.Staff_Speed    .Where(x => x.map_id == mapID).ToList();
        Staff_Capacity  = gm.Staff_Capacity .Where(x => x.map_id == mapID).ToList();
        Staff_Employ    = gm.Staff_Employ   .Where(x => x.map_id == mapID).ToList();
        levelProcesses  = gm.levelProcesses .Where(x => x.map_id == mapID).ToList();

        UnitModel unit  = Resources.Load<UnitModel>("UnitModel");
        unitDataBases   = unit.unitDataBases.Where(x => x.map_id == mapID).ToList();

        LoadRefToMapData();
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(gm);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }

    public void LoadRefToMapData()
    {
        //
        mapData.upgradePlayer.capacities    = PU_Capacity.ToList();
        mapData.upgradePlayer.speeds        = PU_Speed.ToList();
        mapData.upgradePlayer.profitups     = PU_Profit.ToList();
        mapData.upgradeStaff.capacity       = Staff_Capacity.ToList();
        mapData.upgradeStaff.speed          = Staff_Speed.ToList();
        mapData.upgradeStaff.employ         = Staff_Employ.ToList();
    }
}


[System.Serializable]
public class DataMapProcess
{
    public string id;
    public string name;
    
    public string[] unlockreq;
    public string[] unlocknext;

    public int cost;
    public int exp;
    public int levelreq;
    public ETypeUnlock type;
}

[System.Serializable]
public class LevelProcess
{
    public int level;
    public int map_id;
    public int[] point_space;
    public int[] value_customer;
    public int[] value_quest;
    public int[] booster_money;
}
