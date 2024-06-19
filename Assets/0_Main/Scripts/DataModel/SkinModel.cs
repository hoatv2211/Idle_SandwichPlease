using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;



[CreateAssetMenu(menuName = "MAD/SkinModel", fileName = "SkinModel")]
public class SkinModel : ScriptableObject
{
    public ETypeSkin typeSkin = ETypeSkin.normal;
    public InfoSkin infoSkin;

    [Header("Base")]
    public InfoSkin[] infoSkins;

    [Header("Load/Save")]
    public List<InfoSkin> cr_InfoSkins;
    
    public InfoSkin GetInfoSkin(ETypeSkin eType)
    {
        return cr_InfoSkins.Find(x => x.typeSkin == eType);
    }

    public void SetSkin(ETypeSkin _typeSkin)
    {
        typeSkin = _typeSkin;
        infoSkin = infoSkins.ToList().Find(x => x.typeSkin == _typeSkin);
    }

    public void UnlockSkin(ETypeSkin _typeSkin)
    {
        InfoSkin info = infoSkins.ToList().Find(x => x.typeSkin == _typeSkin);
        info.isUnlock = true;
        cr_InfoSkins.ToList().Find(x => x.typeSkin == _typeSkin).isUnlock=true;
        SaveData();
    }

    public void SaveData()
    {
        string json = JsonUtility.ToJson(this);
        PlayerPrefs.SetString("skinmodel_prefs", json);
    }

    public void LoadData()
    {
        string json = PlayerPrefs.GetString("skinmodel_prefs",string.Empty);

        if (string.IsNullOrEmpty(json))
        {
            cr_InfoSkins.Clear();
            cr_InfoSkins = infoSkins.ToList();
            foreach(var k in cr_InfoSkins)
            {
                k.isUnlock = false;
            }
            cr_InfoSkins.Find(x => x.typeSkin == ETypeSkin.normal).isUnlock = true;
            SetSkin(ETypeSkin.normal);
            SaveData();
        }
        else
        {
            JsonUtility.FromJsonOverwrite(json, this);
        }

        if (cr_InfoSkins.Count == 0)
        {
            cr_InfoSkins = infoSkins.ToList();
            foreach (var k in cr_InfoSkins)
            {
                k.isUnlock = false;
            }
            cr_InfoSkins.Find(x => x.typeSkin == ETypeSkin.normal).isUnlock = true;
            SetSkin(ETypeSkin.normal);
            SaveData();
        }
       
    }

}

[System.Serializable]
public class InfoSkin
{
    public string id_skin;
    public string name;
    public ETypeSkin typeSkin;

    [Header("Cost")]
    public int ads_cost;
    public int money_cost;

    [Header("Bonus")]
    public float speed_bonus;
    public float capaticy_bonus;
    public float staff_employ;
    public float staff_speed;
    public float staff_capacity;
    public float single_price_bonus;
    public float package_price_bonus;

    [Header("Save/Load")]
    public int ads_process  = 0;
    public bool isUnlock    = false;


}

public enum ETypeSkin
{
    normal,
    clown,
    chef,
    fairy,
    ceo,
    chicken
}
