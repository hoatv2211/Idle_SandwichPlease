using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MAD/UserModel", fileName = "UserModel")]
public class UserModel : ScriptableObject
{
    public static UserModel Instance => Resources.Load<UserModel>("UserModel");

    public string m_Name = "Newplayer";
    public string m_ID = string.Empty;

    public int level = 1;
    public int exp_current = 0;
    public bool isVIPSub  => iap_bought.Contains("com.sandwich.vipsubscription");
    public bool isPremium => iap_bought.Contains("com.sandwich.premiumpro");

    public bool isManagerSub => iap_bought.Contains("com.sandwich.submanager");
    public bool isCEOSub => iap_bought.Contains("com.sandwich.subceo");

    public List<string> iap_bought;
    public List<int> mapUnlock;

    public List<string> listRewared; //battle pass
    
    public bool isRewarded(string _id)
    {
        if (listRewared.Find(x => x == _id) != null)
            return true;

        return false;
    }

    public void Action_Reward(Reward_Info _reward)
    {
        if (!listRewared.Contains(_reward.id))
            listRewared.Add(_reward.id);
    }

    public bool isCheckLevelUp()
    {
        if (exp_current >= BattlePassModel.Instance.cr_level.exp_req)
        {
            level++;

            return true;
        }


        return false;
    }

    public void Add_IAP(string _idIAP)
    {
        if (!iap_bought.Contains(_idIAP))
            iap_bought.Add(_idIAP);
    }

    public void Add_UnlockMap(int _idMap)
    {
        if (!mapUnlock.Contains(_idMap))
            mapUnlock.Add(_idMap);
    }

    public void SaveData()
    {
        string json = JsonUtility.ToJson(this);
        PlayerPrefs.SetString("usermodel_prefs", json);
    }

    public void LoadData()
    {
        string json = PlayerPrefs.GetString("usermodel_prefs", string.Empty);

        if (string.IsNullOrEmpty(json))
        {
            //Clean data
            mapUnlock.Clear();
            iap_bought.Clear();
            listRewared.Clear();
            level = 1;
            exp_current = 0;

            SaveData();
        }
        else
        {
            JsonUtility.FromJsonOverwrite(json, this);
        }


        m_ID = Module.idUser;
        if (mapUnlock.Count == 0)
            mapUnlock.Add(1);

    }

    public bool isUnlockedMap(int _id)
    {
        return mapUnlock.Contains(_id);
    }
    
    public EMapState mapState(int _id)
    {
        switch (_id)
        {
            case 1:
                return EMapState.Unlocked;
            case 2:
                if (level >= 8)
                    return EMapState.Unlocked;
                break;
            default:

                break;
        }

      

        return EMapState.Locked;
    }
}
