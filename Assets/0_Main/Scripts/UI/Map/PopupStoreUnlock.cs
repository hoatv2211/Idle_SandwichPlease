using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupStoreUnlock : MonoBehaviour
{
    public int idMap;
    [SerializeField] private UIButton btnExit;
    [SerializeField] private UIButton btnOpenStore;
    [SerializeField] private UIButton btnAdsOpenStore;

    [SerializeField] private Image imgIcon;

    public void CallStart(int _idMap,bool _isShowAds)
    {
        idMap = _idMap;
        imgIcon.sprite = AssetConfigs.Instance.GetIconStore(_idMap.ToString());
        btnExit.SetUpEvent(Action_btnExit);
        btnOpenStore.SetUpEvent(Action_btnOpenStore);
        btnAdsOpenStore.SetUpEvent(Action_btnAds);

        btnOpenStore.gameObject.SetActive(!_isShowAds);
        btnAdsOpenStore.gameObject.SetActive(_isShowAds);
    }

    private void Action_btnExit()
    {
        gameObject.SetActive(false);
    }

    private void Action_btnOpenStore()
    {
        gameObject.SetActive(false);
        UserModel.Instance.Add_UnlockMap(idMap);
        FirebaseManager.Instance.LogEvent_map_unlock(idMap);
        Module.LoadScene(idMap);
    }

    private void Action_btnAds()
    {
        AdsAppLovinController.Instance.ShowRewardedAd(() =>
        {
            UserModel.Instance.Add_UnlockMap(idMap);
            btnAdsOpenStore.gameObject.SetActive(false);
            btnOpenStore.gameObject.SetActive(true);

        }, "open_new_map", idMap.ToString());
    }
}
