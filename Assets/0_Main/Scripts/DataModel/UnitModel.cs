using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "MAD/UnitModel", fileName = "UnitModel")]
public class UnitModel : ScriptableObject
{
    public UnitDataBase[] unitDataBases;
}

[System.Serializable]
public class UnitDataBase
{
    public string idUnit;
    public int map_id;
    public float[] speed_lvs;
    public float[] stack_lvs;
    public float[] capacity_lvs;
    public float[] revenue_lvs;

    #region Ref
    public float Speed(int _lv)
    {
        if (_lv > speed_lvs.Length)
            _lv = speed_lvs.Length;

        return speed_lvs[_lv-1];
    }

    public float Stack(int _lv)
    {
        if (_lv > stack_lvs.Length)
            _lv = stack_lvs.Length;

        return stack_lvs[_lv - 1];
    }

    public float Capacity(int _lv)
    {
        if (_lv > capacity_lvs.Length)
            _lv = capacity_lvs.Length;

        return capacity_lvs[_lv - 1];
    }

    public float Revenue(int _lv)
    {
        if (_lv > revenue_lvs.Length)
            _lv = revenue_lvs.Length;

        return revenue_lvs[_lv - 1];
    }

    #endregion
}