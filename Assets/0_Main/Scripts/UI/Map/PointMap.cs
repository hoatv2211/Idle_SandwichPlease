using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public enum EMapState
{
    Locked,
    AdsUnlock, //can use ads for unlock
    Unlock,   // can unlock
    Unlocked,
    Select
}
public class PointMap : MonoBehaviour
{
    public int idMap;

    [SerializeField] private DOTweenAnimation twCanUnlockMap;
    [SerializeField] private GameObject[] objLocked;
    [SerializeField] private GameObject[] objUnlock;
    [SerializeField] private TextMeshProUGUI txtTitle;

    [SerializeField] private UIButton btnSelect;
    UIMap uIMap;
    public void CallStart(UIMap map)
    {
        uIMap = map;
        switch (UserModel.Instance.mapState(idMap))
        {
            case EMapState.Locked:
                break;
            case EMapState.AdsUnlock:
                btnSelect.SetUpEvent(() => uIMap.ShowPopupOpenAStore(idMap, true));
                break;
            case EMapState.Unlock:
                btnSelect.SetUpEvent(() => uIMap.ShowPopupOpenAStore(idMap, false));
                break;
            case EMapState.Unlocked:
                btnSelect.SetUpEvent(() => uIMap.Action_SelectMap(idMap));
                break;
            case EMapState.Select:
                break;
            default:
                break;
        }

    }

}
