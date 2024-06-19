using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum ETypeReward
{
    money,
    ticket,
    skin
}

[CreateAssetMenu(menuName = "MAD/BattlePassModel", fileName = "BattlePassModel")]
public class BattlePassModel : ScriptableObject
{
    public static BattlePassModel Instance => Resources.Load<BattlePassModel>("BattlePassModel");

    public List<Level_User> level_Users;
    public List<Reward_Info> reward_Battles;

    public Level_User cr_level => level_Users.Find(x => x.level == UserModel.Instance.level);
    public Level_User next_level => level_Users.Find(x => x.level == UserModel.Instance.level+1);

    public bool isNoticeReward()
    {
        foreach(var k in level_Users)
        {
            if(k.level<= UserModel.Instance.level)
            {
                if (!UserModel.Instance.isRewarded(k.bonus_free))
                {
                    return true;
                }

                if (UserModel.Instance.isManagerSub)
                {
                    if (!UserModel.Instance.isRewarded(k.bonus_manager))
                        return true;
                }

                if(UserModel.Instance.isCEOSub)
                {
                    if (!UserModel.Instance.isRewarded(k.bonus_ceo))
                        return true;

                }

            }

        }

        return false;
    }

    [Button("Generate")]
    public void Generate()
    {
        foreach(var k in level_Users)
        {
            k.reward_free       = reward_Battles.Find(x => x.id == k.bonus_free);
            k.reward_manager    = reward_Battles.Find(x => x.id == k.bonus_manager);
            k.reward_ceo        = reward_Battles.Find(x => x.id == k.bonus_ceo);
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();

#endif
    }


}

[System.Serializable]
public class Level_User
{
    public int level;
    public int exp_req;
    public int bonus_money;
    public int value_quest;
    public int booster_money;

    public string bonus_free;
    public string bonus_manager;
    public string bonus_ceo;

    public Reward_Info reward_free;
    public Reward_Info reward_manager;
    public Reward_Info reward_ceo;

    public string[] feature_unlocks;
}


[System.Serializable]
public class Reward_Info
{
    public string id;
    public ETypeReward type;
    public int value;
}
