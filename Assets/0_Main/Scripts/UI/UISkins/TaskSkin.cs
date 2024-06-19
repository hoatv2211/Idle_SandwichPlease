using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class TaskSkin : MonoBehaviour
{
    public ETypeSkin typeSkin;
    public InfoSkin infoSkin;

    [SerializeField] private GameObject objUnlock;
    [SerializeField] private UIButton btnSelect;

    [SerializeField] private GameObject objLocked;
    [SerializeField] private UIButton btnAds;
    [SerializeField] private TextMeshProUGUI txtAds;

    [SerializeField] private UIButton btnCash;
    [SerializeField] private TextMeshProUGUI txtCash;

    [SerializeField] private GameObject imgSelected;
    [SerializeField] private GameObject imgCheck;

    [SerializeField] private Image imgIcon;
    [SerializeField] private List<Sprite> sprIcons;

   

    public void CallStart()
    {
        infoSkin = GameManager.Instance.skinModel.GetInfoSkin(typeSkin);
        imgIcon.sprite = sprIcons.Find(x => x.name == "icon_" + typeSkin.ToString());

        ShowInfo();
        Selected(typeSkin == GameManager.Instance.skinModel.typeSkin);

        btnAds.SetUpEvent(Action_btnAds);
        btnCash.SetUpEvent(Action_btnCash);
        btnSelect.SetUpEvent(Action_btnSelect);
    }

    private void Action_btnAds()
    {
        AdsAppLovinController.Instance.ShowRewardedAd(() => 
        {
            infoSkin.ads_process++;
            if (infoSkin.ads_process >= infoSkin.ads_cost)
            {
                infoSkin.isUnlock = true;

                Action_selected();
            }

            ShowInfo();

        }, "skin","skin_"+ typeSkin.ToString());
    }

    private void Action_btnCash()
    {
        if (Module.money_currency < infoSkin.money_cost)
            return;

        Module.money_currency -= infoSkin.money_cost;
        infoSkin.isUnlock = true;

        Action_selected();

        ShowInfo();

        int money = Module.money_currency;
        FirebaseManager.Instance.LogEvent_spend("money", "money",
                 infoSkin.money_cost, money + infoSkin.money_cost, money,
                 infoSkin.id_skin, "skin", infoSkin.money_cost);
    }
    
    private void Action_btnSelect()
    {
        if (typeSkin == GameManager.Instance.skinModel.typeSkin)
            return;

        Action_selected();
    }

    public void ShowInfo()
    {
        
        objUnlock.SetActive(infoSkin.isUnlock);
        objLocked.SetActive(!infoSkin.isUnlock);


        txtAds.text = infoSkin.ads_process + "/" + infoSkin.ads_cost;
        txtCash.text = infoSkin.money_cost.ToString();

       
        //imgCheck.SetActive(typeSkin == GameManager.Instance.skinModel.typeSkin);
    }

    public void Selected(bool _isSelect)
    {
        imgSelected.SetActive(_isSelect);
        imgCheck.SetActive(_isSelect);
       
    }

    public void Action_selected()
    {
        GameManager.Instance.skinModel.SetSkin(typeSkin);
        Module.Action_Event_SkinChange(typeSkin);
    }

    private void OnEnable()
    {
        Module.Event_SkinChange += Module_Event_SkinChange;
    }

    private void Module_Event_SkinChange(ETypeSkin _type)
    {
        Selected(typeSkin == _type);
    }

    private void OnDisable()
    {
        Module.Event_SkinChange -= Module_Event_SkinChange;
    }

}



