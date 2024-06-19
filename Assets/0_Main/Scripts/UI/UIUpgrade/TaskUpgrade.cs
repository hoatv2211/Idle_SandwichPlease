using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

//Tracking helper
public enum ETypeTaskUpgrade
{
    Player,
    Staff
}
public class TaskUpgrade : MonoBehaviour
{
    [SerializeField] private ETypeTaskUpgrade typeTask;
    [SerializeField] private string eventType = "";
    [SerializeField] private UpgradeData upgradeData;
    [SerializeField] private List<Image> imgLevels;
    [SerializeField] private List<Sprite> sprtCashs;


    [SerializeField] private UIButton btnTicket;
    [SerializeField] private UIButton btnAds;
    [SerializeField] private UIButton btnAds_Off;
    [SerializeField] private UIButton btnCash;
    [SerializeField] private UIButton btnCash_Off;
    [SerializeField] private TextMeshProUGUI txtCash;

    [SerializeField] private GameObject maxLevel;
   


    private void OnEnable()
    {  
        btnAds.SetUpEvent(Action_btnAds);
        btnTicket.SetUpEvent(Action_btnTicket);
        btnCash.SetUpEvent(Action_btnCash);
    }

    private Action callback;
    public void CallStart(Action _callback,UpgradeData _data=null)
    {
        callback = _callback;
        SetDisplay(_data);

    }

    public void ShowLocked()
    {
        btnAds_Off.gameObject.SetActive(true);
        btnCash_Off.gameObject.SetActive(true);
    }

    public void ShowUnlocked()
    {
        btnAds_Off.gameObject.SetActive(false);
        btnCash_Off.gameObject.SetActive(false);
       
    }

    public void SetDisplay(UpgradeData _data)
    {
        if (_data != null)
            upgradeData = _data;
        maxLevel.SetActive(false);

        if (upgradeData.cost <= 0)
            txtCash.text = "FREE";
        else
            txtCash.text = upgradeData.cost.ToString();
        for(int i =0;i< imgLevels.Count; i++)
        {
            if (i < upgradeData.level-1)
                imgLevels[i].color= Color.white;
            else
                imgLevels[i].color = Color.gray;
        }

        btnTicket.gameObject.SetActive(Module.ticket_currency > 0);

        if (_data.cost > Module.money_currency)
        {
            btnCash.GetComponent<Image>().sprite = sprtCashs[1];
            btnCash.enabled = false;
        }
        else
        {
            btnCash.GetComponent<Image>().sprite = sprtCashs[0];
            btnCash.enabled = true;
        }

    }

    public void MaxLevel()
    {
        for (int i = 0; i < imgLevels.Count; i++)
            imgLevels[i].color = Color.white;
        maxLevel.SetActive(true);
        btnAds.gameObject.SetActive(false);
        btnTicket.gameObject.SetActive(false);
        btnCash.gameObject.SetActive(false);
    }

    private void Action_btnTicket()
    {
        if (Module.ticket_currency > 0)
        {
            Module.ticket_currency--;
            callback?.Invoke();
        }

    
    }

    private void Action_btnAds()
    {
        AdsAppLovinController.Instance.ShowRewardedAd(() =>
        {
            switch (typeTask)
            {
                case ETypeTaskUpgrade.Player:
                    FirebaseManager.Instance.LogEvent_player(eventType, upgradeData.level, "ads");
                    break;
                case ETypeTaskUpgrade.Staff:
                    FirebaseManager.Instance.LogEvent_staff(eventType, upgradeData.level, "ads");
                    break;
                default:
                    break;
            }

            callback?.Invoke();
        }, "upgrade", upgradeData.id);

    }

    private void Action_btnCash()
    {
        if (Module.money_currency >= upgradeData.cost )
        {
            Module.money_currency -= upgradeData.cost;
           
           

            int money = Module.money_currency;
            FirebaseManager.Instance.LogEvent_spend("money", "money",
                     upgradeData.cost, money + upgradeData.cost, money,
                     upgradeData.id, "upgrade", upgradeData.cost);

            switch (typeTask)
            {
                case ETypeTaskUpgrade.Player:
                    FirebaseManager.Instance.LogEvent_player(eventType, upgradeData.level, "money");
                    break;
                case ETypeTaskUpgrade.Staff:
                    FirebaseManager.Instance.LogEvent_staff(eventType, upgradeData.level, "money");
                    break;
                default:
                    break;
            }

            callback?.Invoke();
        }
    }


}
