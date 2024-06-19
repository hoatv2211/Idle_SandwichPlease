using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMap : MonoBehaviour
{
    [SerializeField] private UIButton btnExit;
    [SerializeField] private UIButton btnSelectMap;

    public List<PointMap> pointMaps;

    [Header("Popup")]
    public PopupStoreClaim storeClaim;
    public PopupStoreUnlock storeUnlock;

    public void CallStart()
    {
        btnExit.SetUpEvent(Action_btnExit);
        btnSelectMap.SetUpEvent(Action_btnExit);

        foreach(var k in pointMaps)
        {
            k.CallStart(this);
        }
    }

    public void Action_btnExit()
    {
        gameObject.SetActive(false);
    }

    public void Action_SelectMap(int _idMap)
    {
        storeClaim.gameObject.SetActive(true);
        storeClaim.CallStart(_idMap);
    }

    public void ShowPopupOpenAStore(int _idMap,bool _isShowAds)
    {
        storeUnlock.gameObject.SetActive(true);
        storeUnlock.CallStart(_idMap, _isShowAds);
    }
}
