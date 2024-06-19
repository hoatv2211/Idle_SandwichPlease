using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;
using System.ComponentModel;

public class UISpecialOrder : MonoBehaviour
{
    [SerializeField] private SpecialData specialData { get; set; }
    [SerializeField] private UIButton btnExit;

    [SerializeField] private GameObject btnGetOFF;
    [SerializeField] private UIButton btnGet;
    [SerializeField] private UIButton btnAds;
    [SerializeField] private UIButton btnInstant;
    [SerializeField] private TextMeshProUGUI txtTimeProcess;
    [SerializeField] private TextMeshProUGUI txtMoneyGet;


    [SerializeField] private Slider processSanwich;
    [SerializeField] private TextMeshProUGUI txtProcess;

    [SerializeField] private ShipperUnit shipperUnit;
    public void CallStart(SpecialData _data,int _time)
    {
        specialData = _data;
        btnExit.SetUpEvent(Action_btnExit);
        btnGet.SetUpEvent(Action_btnGet);
        btnAds.SetUpEvent(Action_btnAds);
        btnInstant.SetUpEvent(Action_btnInstant);
        btnGet.enabled = true;
        btnAds.enabled = true;
        btnInstant.enabled = true;
        timeCD = _time;

        if (corTime != null)
            StopCoroutine(corTime);

        //timeCD = (int)MapController.Instance.shipperCtrl.shipperUnit.waitingTime;
        corTime = StartCoroutine(ICountdown());

        Module_Event_Special_Quest(_data);
    }

    private void OnEnable()
    {
        Module.Event_Special_Quest += Module_Event_Special_Quest;
    }

    private void Module_Event_Special_Quest(SpecialData data)
    {
        txtProcess.text = data.specialProduct.strTextProcess;
        processSanwich.maxValue = data.specialProduct.total;
        processSanwich.value = data.specialProduct.numberfill;
        txtMoneyGet.text = data.moneyValue.ToString();

        if (data.specialProduct.isFull )
        {
            btnAds.gameObject.SetActive(true);
            btnGet.gameObject.SetActive(true);
            btnGetOFF.gameObject.SetActive(false);
            btnInstant.gameObject.SetActive(false);
        }
        else
        {
            btnAds.gameObject.SetActive(false);
            btnGet.gameObject.SetActive(false);
            btnGetOFF.gameObject.SetActive(true);
            btnInstant.gameObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        Module.Event_Special_Quest -= Module_Event_Special_Quest;
        if (corTime != null)
            StopCoroutine(corTime);
    }

    private void Action_btnInstant()
    {
        AdsAppLovinController.Instance.ShowRewardedAd(() => {

            //btnGet.enabled = false;
            //btnAds.enabled = false;
            btnInstant.enabled = false;


            //PickUp

            DOVirtual.DelayedCall(2.5f, () => { gameObject.SetActive(false); specialData.isCashOut = true; });
                     
            txtProcess.text = processSanwich.maxValue +"/" + processSanwich.maxValue;
            processSanwich.value = processSanwich.maxValue;
            specialData.specialProduct.total = (int)processSanwich.maxValue;
            specialData.specialProduct.numberfill = specialData.specialProduct.total;

            int _value = specialData.moneyValue;
            int _vl = _value / 10;
            if (_vl > 40)
                _vl = 40;
            UIMainGame.Instance.Show_Effect_MoneyBooter(txtMoneyGet.transform, _vl);

            int money = Module.money_currency;
            DOVirtual.Int(money, money + _value, 2, (x) => Module.money_currency = x);

            int moneyTracking = Module.money_currency;
            FirebaseManager.Instance.LogEvent_receive("money", "money",
              _value, moneyTracking - _value, moneyTracking,
              "shiper_ads", "shiper_ads", _value);
        }, "shiper", "shiper_ads");
    }

    private void Action_btnExit()
    {
        gameObject.SetActive(false);
    }

    private void Action_btnGet()
    {
        DOVirtual.DelayedCall(2.5f, () => {
            gameObject.SetActive(false);
            specialData.isCashOut = true;
            shipperUnit.CurrentState = ECustomerState.Exiting;
        });

        btnGet.enabled = false;
        btnAds.enabled = false;
        btnInstant.enabled = false;

        int _value = specialData.moneyValue;
        int _vl = _value / 10;
        if (_vl > 40)
            _vl = 40;
        UIMainGame.Instance.Show_Effect_MoneyBooter(txtMoneyGet.transform, _vl);

        int money = Module.money_currency;
        DOVirtual.Int(money, money + _value, 2, (x) => Module.money_currency = x);

        int moneyTracking = Module.money_currency;
        FirebaseManager.Instance.LogEvent_receive("money", "money",
          _value, moneyTracking - _value, moneyTracking,
          "shiper_normal", "shiper_normal", _value);
    }

    private void Action_btnAds()
    {
        AdsAppLovinController.Instance.ShowRewardedAd(() => {

            btnGet.enabled = false;
            btnAds.enabled = false;

            DOVirtual.DelayedCall(2.5f, () => {
                gameObject.SetActive(false);
                specialData.isCashOut = true;
                shipperUnit.CurrentState = ECustomerState.Exiting;
            });
            int _value = specialData.moneyValue *3;
            int _vl = _value / 10;
            if (_vl > 40)
                _vl = 40;
            UIMainGame.Instance.Show_Effect_MoneyBooter(txtMoneyGet.transform, _vl);

            int money = Module.money_currency;
            DOVirtual.Int(money, money + _value, 2, (x) => Module.money_currency = x);

            int moneyTracking = Module.money_currency;
            FirebaseManager.Instance.LogEvent_receive("money", "money",
              _value, moneyTracking - _value, moneyTracking,
              "shiper_ads", "shiper_ads", _value);
        }, "shiper", "shiper_ads");
    }

    Coroutine corTime;
    int timeCD = 300;
    IEnumerator ICountdown()
    {
        while (timeCD>0)
        {
            txtTimeProcess.text = Module.SecondCustomToTime(timeCD);
            yield return new WaitForSeconds(1);
            timeCD--;
        }
      
    }
}
