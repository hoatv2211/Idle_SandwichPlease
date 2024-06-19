using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Sirenix.OdinInspector;
using System.Linq;

public class GameManager : Singleton<GameManager>
{

    [Header("Datas Game")]
    public SkinModel skinModel;
    public UnitModel unitModel;
    public UpgradeModel upgradeModel;
    public UserModel userModel;
    public List<MapModel> mapModels;
   

    [TabGroup("Config")]
    public DataConfigRemote m_DataConfigRemote = new DataConfigRemote();

    [TabGroup("SkinModel")]
    public Remote_SkinModel remote_SkinModel;

    [TabGroup("UnitModel")]
    public Remote_UnitModel remote_UnitModel;

    [TabGroup("UpgradeModel")]
    public Remote_UpgradeModel Remote_UpgradeModel;

    [Header("Cheater")]
    public GameObject ingameDebug;

    public MapModel mapModel => mapModels[0];

    public MapModel GetMapModel(int _idMap)
    {
        return mapModels.Find(x => x.mapID == _idMap);
    }

    #region Base

    protected override void Awake()
    {
        base.Awake();
        if (string.IsNullOrEmpty(Module.time_first_open))
        {
            Module.time_first_open = Module.TimestampNow();
        }

        if (string.IsNullOrEmpty(Module.datetime_first_open))
        {
            Module.datetime_first_open = DateTime.Now.ToString();
        }
        else
        {
            DateTime _old = Convert.ToDateTime(Module.datetime_first_open);
            if (_old.Day != DateTime.Now.Day)
            {
                Module.isFirstDay = false;
                //Debug.LogError("Not firstday!");
            }
        }
    }

    private void OnEnable()
    {
        Module.Event_RemoteConfigs += Module_Event_RemoteConfigs;
        LoadDataGame();
        ResetInter();


        DOVirtual.DelayedCall(3 * 60, () => FirebaseManager.Instance.LogEvent_PlayTime("3"));
        DOVirtual.DelayedCall(5 * 60, () => FirebaseManager.Instance.LogEvent_PlayTime("5"));
        DOVirtual.DelayedCall(10 * 60, () => FirebaseManager.Instance.LogEvent_PlayTime("10"));
        DOVirtual.DelayedCall(15 * 60, () => FirebaseManager.Instance.LogEvent_PlayTime("15"));
        DOVirtual.DelayedCall(20 * 60, () => FirebaseManager.Instance.LogEvent_PlayTime("20"));
    }

    private void Module_Event_RemoteConfigs(string _key, string _value)
    {
        Debug.Log(_key + " : " + _value);
        switch (_key)
        {
            case "gab012_configs":
                JsonUtility.FromJsonOverwrite(_value, m_DataConfigRemote);
                ResetInter();
                break;
            default:

                break;
        }
    }

    private void OnDisable()
    {
        SaveDataGame();

        Module.Event_RemoteConfigs -= Module_Event_RemoteConfigs;
    }


    private void LateUpdate()
    {
        if (timeInterDelay > 0)
        {
            timeInterDelay -= Time.deltaTime;
            if (timeInterDelay <= 0)
            {
                UIMainGame.Instance.ShowAdsInter();
            }
        }
    }

#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Module.money_currency += 10000;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            MapController.Instance.mapData.upgradePlayer.lvSpeed = 5;
            MapController.Instance.mapData.upgradePlayer.lvCapacity = 5;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            MapController.Instance.shipperCtrl.StartSpecialShiper();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            MapController.Instance.ShowUnit_UpgradeAds();
        } 
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
            Debug.LogError("1");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            UIMainGame.Instance.m_UICheater.gameObject.SetActive(true) ;
        }
    }


#endif

    #endregion

    #region Help Inter ads
    // ads inter
    public float timeInterDelay = 0;
    public bool isCanShowInter()
    {
        if (MapController.Instance._idMap != 1)
            return true;
        else
            return MapController.Instance.mapData.IsUnlockByName(m_DataConfigRemote.idUnlock_ads_inter);
    }

    public void ResetInter()
    {
        if (Module.isFirstDay)
            timeInterDelay = m_DataConfigRemote.ads_delay_inter;
        else
            timeInterDelay = m_DataConfigRemote.ads_delay_inter_day2;

        Debug.Log("Reset inter " + timeInterDelay);

    }

    public void CheckDelayRewardandInter()
    {
        if(timeInterDelay< m_DataConfigRemote.ads_delay_inter_and_reward)
        {
            timeInterDelay = m_DataConfigRemote.ads_delay_inter_and_reward;
        }
    }
    #endregion

    #region Aplication
    private void OnApplicationQuit()
    {
        SaveDataGame();
        FirebaseManager.Instance.LogEvent_AppExit();
    }

    private void OnApplicationPause(bool pause)
    {
        if(pause)
        {
            SaveDataGame();
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            SaveDataGame();
        }
    }


    #endregion

    public void LoadDataGame()
    {
        //currentMap.LoadData();
        skinModel.LoadData();
        userModel.LoadData();

        foreach(var k in mapModels)
        {
            k.mapData.LoadData();
        }

        CheckNewDay();
    }

    public void SaveDataGame()
    {
        //currentMap.SaveData();
        userModel.SaveData();
        skinModel.SaveData();

        //foreach (var k in mapModels)
        //{
        //    k.mapData.SaveData();
        //}
    }

    public void CheckNewDay()
    {
        if (DateTime.Now.Day != Module.datetime_lastday)
        {
            Module.reward_free_turn = 0;
            Module.datetime_lastday = DateTime.Now.Day;
            PlayerPrefs.SetFloat("spinads", 4);

        }
    }

    [ContextMenu("EditorDebug")]
    public void EditorDebug() 
    {
       Debug.LogError(m_DataConfigRemote.Json());
    }

    [Button("SetBaseRemote")]
    public void SetBaseRemote()
    {
        remote_SkinModel.infoSkins = skinModel.infoSkins.ToList();
        remote_UnitModel.unitDataBases = unitModel.unitDataBases.ToList();

        Debug.LogError("SetBaseRemote");
    }

}

#region Map data

[System.Serializable]
public class MapData
{
    public int mapID;
    public int crProcess;

    public bool isMoneyBonus = true;

    public float percentProcess => crProcess / 100f;

    [Space, Header("Player")]
    public PlayerData playerData;
    public WorkerData workerData;

    [Space, Header("Upgrade")]
    public UpgradePlayer upgradePlayer;
    public UpgradeStaff upgradeStaff;

    [Header("Unlocked Units")]
    public List<CustomerData> customerDatas;

    public List<UnitData> unlocked_UnitDatas;
    public List<UnitData> crUnlocks;

    public List<string> crTut;
    public List<string> tutCleans;


    public string tutlated => tutCleans.Count > 0 ? tutCleans[tutCleans.Count - 1] : "none";
    public UnitData GetUnlockUnitData(string _id)
    {
        //Debug.LogError(_id);
        return unlocked_UnitDatas.Find(x => x.idUnit == _id);
    }

    public UnitData GetCurrentUnlock(string _id)
    {
        return crUnlocks.Find(x => x.idUnit == _id);
    }

    public bool IsUnlock(string _id)
    {
        var _data = unlocked_UnitDatas.Find(x => x.idUnit == _id);

        return _data != null;
    }

    public bool IsUnlockByName(string _id)
    {
        var _data = unlocked_UnitDatas.Find(x => x.name == _id);

        return _data != null;
    }

    public void Action_UnlockUnit(UnitData _unit)
    {
        if (!unlocked_UnitDatas.Contains(_unit))
        {
            unlocked_UnitDatas.Add(_unit);
        }

        SaveData();
    }

    public void SetCurrentUnlock(UnitData _unit)
    {
        if (crUnlocks.Find(o => o.idUnit == _unit.idUnit)==null)
        {
            crUnlocks.Add(_unit);
        }

        SaveData();
    }

    public void RemoveCrUnitUnlock(UnitData _unit)
    {
        int foundIndex = crUnlocks.FindIndex(o => o.idUnit == _unit.idUnit);

        if (foundIndex >= 0)
        {
            crUnlocks.RemoveAt(foundIndex);
        }

    }


    public void ShowTut(string _idTut)
    {
        if (tutCleans.Contains(_idTut))
            return;
        MapController.Instance.tutNodes.Find(x => x.idTut == _idTut).gameObject.SetActive(true);
    }

    public void AddTut(string _idTut)
    {
        //Debug.LogError(_idTut);
        if (tutCleans.Contains(_idTut))
            return;
        tutCleans.Add(_idTut);
    }

    string keySave => "DataGameMap" + mapID;
    public void SaveData()
    {
       
        string json = JsonUtility.ToJson(this);
        PlayerPrefs.SetString(keySave, json);
        Debug.Log("SaveData: \n" + json);
    }

    public void LoadData()
    {
        if (PlayerPrefs.HasKey(keySave))
        {
            string json = PlayerPrefs.GetString(keySave);
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(keySave), this);
            Debug.Log("LoadData: \n" + json);
        }
        else
        {
            //Clean
            crProcess = 0;
            isMoneyBonus = true;
            upgradePlayer = new UpgradePlayer();
            upgradeStaff = new UpgradeStaff();
            unlocked_UnitDatas.Clear();
            crUnlocks.Clear();
            crTut.Clear();
            tutCleans.Clear();
        }

      
    }

  
}

#endregion


#region Upgrade data

[System.Serializable]
public class UpgradePlayer
{
    public int lvCapacity = 0;
    public int lvSpeed = 0;
    public int lvProfit = 0;

    public List<UpgradeData> capacities;
    public List<UpgradeData> speeds;
    public List<UpgradeData> profitups;

    #region Action
    public bool Upgrade_Capacity()
    {
        if (isMaxCapacity)
            return false;

        lvCapacity++;
        return true;
    }

    public bool Upgrade_Speed()
    {
        if (isMaxSpeed)
            return false;

        lvSpeed++;
        return true;
    }

    public bool Upgrade_Profit()
    {
        if (isMaxProfit)
            return false;

        lvProfit++;
        return true;
    }

    #endregion

    #region Ref
    public bool isMaxCapacity => lvCapacity >= capacities.Count - 1;
    public bool isMaxSpeed => lvSpeed >= speeds.Count - 1;
    public bool isMaxProfit => lvProfit >= profitups.Count - 1;
    public bool isMaxAll => isMaxCapacity && isMaxSpeed && isMaxProfit;

    public UpgradeData crCapacity() { return capacities[lvCapacity]; }

    public UpgradeData crSpeed() { return speeds[lvSpeed]; }

    public UpgradeData crProfitup() { return profitups[lvProfit]; }

    public UpgradeData nextCapacity() { return capacities[lvCapacity + 1]; }

    public UpgradeData nextSpeed() { return speeds[lvSpeed + 1]; }

    public UpgradeData nextProfitup() { return profitups[lvProfit + 1]; }

    #endregion

    public bool isNotice()
    {
        if (!isMaxCapacity)
        {
            if (nextCapacity()?.cost <= Module.money_currency)
                return true;
        }

        if (!isMaxSpeed)
        {
            if (nextSpeed()?.cost <= Module.money_currency)
                return true;
        }

        if (!isMaxProfit)
        {
            if (nextProfitup()?.cost <= Module.money_currency)
                return true;
        }

        return false;
    }
}


[System.Serializable]
public class UpgradeStaff
{
    public int lvCapacity = 0;
    public int lvSpeed = 0;
    public int lvEmploy = 0;

    public List<UpgradeData> capacity;
    public List<UpgradeData> speed;
    public List<UpgradeData> employ;

    #region Action
    public bool Upgrade_Capacity()
    {
        if (isMaxCapacity)
            return false;

        lvCapacity++;
        return true;
    }

    public bool Upgrade_Speed()
    {
        if (isMaxSpeed)
            return false;

        lvSpeed++;
        return true;
    }

    public bool Upgrade_Employ()
    {
        if (isMaxEmploy)
            return false;

        lvEmploy++;
        return true;
    }

    #endregion

    #region Ref
    public bool isMaxCapacity   => lvCapacity >= capacity.Count - 1;
    public bool isMaxSpeed      => lvSpeed >= speed.Count - 1;
    public bool isMaxEmploy     => lvEmploy >= employ.Count - 1;

    public bool isMaxAll => isMaxCapacity && isMaxSpeed && isMaxEmploy;

    public UpgradeData crCapacity() { return capacity[lvCapacity]; }

    public UpgradeData crSpeed() { return speed[lvSpeed]; }

    public UpgradeData crlvEmploy() { return employ[lvEmploy]; }

    public UpgradeData nextCapacity() { return capacity[lvCapacity + 1]; }

    public UpgradeData nextSpeed() { return speed[lvSpeed + 1]; }

    public UpgradeData nextlvEmploy() { return employ[lvEmploy + 1]; }

    #endregion

    public bool isNotice()
    {

        if(!isMaxCapacity)
        {
            if (nextCapacity()?.cost <= Module.money_currency)
                return true;
        }

        if (!isMaxSpeed)
        {
            if (nextSpeed()?.cost <= Module.money_currency)
                return true;
        }

        if (!isMaxEmploy)
        {
            if (nextlvEmploy()?.cost <= Module.money_currency)
                return true;
        }

        return false;
    }
}


[System.Serializable]
public class UpgradeData
{
    public int map_id;
    public string id;
    public int level;
    public float value;
    public ETypeUpgrade type_upgrade;
    public int cost;
}

public enum ETypeUpgrade
{
    free,
    coin,
    ads
}

#endregion


#region Remote Config
[System.Serializable]
public class DataConfigRemote
{
    public string version = "v001";
    public string googleIAP = "";

    [Header("Ads")]
    public float delay_loading = 1f;
    public string idUnlock_ads_inter = "table_02";
    public int ads_delay_inter = 30;
    public int ads_delay_inter_day2 = 60;
    public int ads_delay_inter_and_reward = 30;
    public int time_delay_special = 300;

    [Header("Add on v0.10")]
    public float speedBonus         = 0;
    public float speedBonus_Eat     = 0;
    public float rateJuice          = 25;
    public bool isShowWelcome       = false;
    public bool isShowInternet      = false;
   

    public string Json()
    {
        return JsonUtility.ToJson(this);
    }
}

[System.Serializable]
public class Remote_SkinModel
{
    public List<InfoSkin> infoSkins;

    public string Json()
    {
        return JsonUtility.ToJson(this);
    }
}

[System.Serializable]
public class Remote_UnitModel
{
    public List<UnitDataBase> unitDataBases;

    public string Json()
    {
        return JsonUtility.ToJson(this);
    }
}

[System.Serializable]
public class Remote_UpgradeModel
{
    public List<UpgradeData> PU_Speed;
    public List<UpgradeData> PU_Capacity;
    public List<UpgradeData> PU_Profit;
    public List<UpgradeData> Staff_Speed;
    public List<UpgradeData> Staff_Capacity;
    public List<UpgradeData> Staff_Employ;

    public List<LevelProcess>levelProcesses;

    public string Json()
    {
        return JsonUtility.ToJson(this);
    }
}

#endregion