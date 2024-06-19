using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

public class UIBooster : MonoBehaviour
{
    [SerializeField] private ETypeBooster typeBooster;
    [SerializeField] private UIButton btnExit;
    [SerializeField] private UIButton btnAds;
    [SerializeField] private UIButton btnTicket;

    [SerializeField] private TextMeshProUGUI txtmoney;
    [SerializeField] private Transform trBaseMoney;

    [SerializeField] private TextMeshProUGUI txtTitle;
    [SerializeField] private List<GameObject> listTasks;
    [SerializeField] private GameObject taskCoinBag;
    [SerializeField] private GameObject taskSpeedUp;
    [SerializeField] private GameObject taskCapacityUp;
    [SerializeField] private GameObject taskSpeedEat;

    private Action callback;
    public void CallStart(ETypeBooster _type, Action _callback)
    {
        typeBooster = _type;
        callback = _callback;
        btnExit.SetUpEvent(Action_btnExit);
        
        txtmoney.text = MapController.Instance.levelProcess.booster_money[0].ToString();

        foreach (var k in listTasks)
            k.gameObject.SetActive(false);

        switch (typeBooster)
        {
            case ETypeBooster.CoinBag:
                txtTitle.text = "Coin Bag!!";
                taskCoinBag.SetActive(true);
                btnAds.SetUpEvent(Action_btnAdsMoney);
                btnTicket.SetUpEvent(Action_btnAdsMoney);
                break;
            case ETypeBooster.Capacity:
                txtTitle.text = "Capacity Up!!";
                taskCapacityUp.SetActive(true);
                btnAds.SetUpEvent(Action_btnAds_CapatityUp);
                btnTicket.SetUpEvent(Action_btnAds_CapatityUp);
                break;
            case ETypeBooster.MoveSpeed:
                txtTitle.text = "Speed Up!!";
                taskSpeedUp.SetActive(true);
                btnAds.SetUpEvent(Action_btnAds_Speedup);
                btnTicket.SetUpEvent(Action_btnAds_Speedup);
                break;
            case ETypeBooster.EatSpeed:
                txtTitle.text = "Eat Speed!!";
                taskSpeedEat.SetActive(true);
                btnAds.SetUpEvent(Action_btnAds_SpeedEat);
                btnTicket.SetUpEvent(Action_btnAds_SpeedEat);
                break;
            default:
                txtTitle.text = "Speed Up!!";
                break;
        }

        btnTicket.gameObject.SetActive(Module.ticket_currency > 0);
        btnAds.gameObject.SetActive(Module.ticket_currency <= 0);
    }

    private void Action_btnExit()
    {
        gameObject.SetActive(false);
    }

    private void Action_btnAds_Speedup()
    {
        if (Module.ticket_currency > 0)
        {
            Module.ticket_currency--;
            callback?.Invoke();
            gameObject.SetActive(false);
            UIMainGame.Instance.Show_HUDBooster(typeBooster);
            btnTicket.gameObject.SetActive(Module.ticket_currency > 0);
            btnAds.gameObject.SetActive(Module.ticket_currency <= 0);
            return;
        }


        AdsAppLovinController.Instance.ShowRewardedAd(() =>
        {
            callback?.Invoke();
            gameObject.SetActive(false);
            UIMainGame.Instance.Show_HUDBooster(typeBooster);
          
        },"booster","move_speed");
    }

    private void Action_btnAds_SpeedEat()
    {
        if (Module.ticket_currency > 0)
        {
            callback?.Invoke();
            gameObject.SetActive(false);
            UIMainGame.Instance.Show_HUDBooster(typeBooster);
            btnTicket.gameObject.SetActive(Module.ticket_currency > 0);
            btnAds.gameObject.SetActive(Module.ticket_currency <= 0);
            return;
        }

        AdsAppLovinController.Instance.ShowRewardedAd(() =>
        {
            callback?.Invoke();
            gameObject.SetActive(false);
            UIMainGame.Instance.Show_HUDBooster(typeBooster);

        }, "booster", "eat_speed");
    }

    private void Action_btnAds_CapatityUp()
    {
        if (Module.ticket_currency > 0)
        {
            callback?.Invoke();
            gameObject.SetActive(false);
            UIMainGame.Instance.Show_HUDBooster(typeBooster);

            return;
        }

        AdsAppLovinController.Instance.ShowRewardedAd(() =>
        {
            callback?.Invoke();
            gameObject.SetActive(false);
            UIMainGame.Instance.Show_HUDBooster(typeBooster);
        }, "booster", "capacity");
    }

    private void Action_btnAdsMoney()
    {
        if (Module.ticket_currency > 0)
        {
            DOVirtual.DelayedCall(2.2f, () => gameObject.SetActive(false));
            int _value = MapController.Instance.levelProcess.booster_money[0];
            int _vl = _value / 10;
            if (_vl > 40)
                _vl = 40;
            UIMainGame.Instance.Show_Effect_MoneyBooter(trBaseMoney, _vl);

            int money = Module.money_currency;
            DOVirtual.Int(money, money + _value, 2, (x) => Module.money_currency = x);

            int moneyTracking = Module.money_currency;
            FirebaseManager.Instance.LogEvent_receive("money", "money",
              _value, moneyTracking - _value, moneyTracking,
              "booster", "booster_ads", _value);
            callback?.Invoke();

            btnTicket.gameObject.SetActive(Module.ticket_currency > 0);
            btnAds.gameObject.SetActive(Module.ticket_currency <= 0);
            return;
        }


        AdsAppLovinController.Instance.ShowRewardedAd(() => {

            DOVirtual.DelayedCall(2.2f, () => gameObject.SetActive(false));
            int _value = MapController.Instance.levelProcess.booster_money[0];
            int _vl = _value / 10;
            if (_vl > 40)
                _vl = 40;
            UIMainGame.Instance.Show_Effect_MoneyBooter(trBaseMoney, _vl);

            int money = Module.money_currency;
            DOVirtual.Int(money, money + _value, 2, (x) => Module.money_currency = x);

            int moneyTracking = Module.money_currency;
            FirebaseManager.Instance.LogEvent_receive("money", "money",
              _value, moneyTracking - _value, moneyTracking,
              "booster", "booster_ads", _value);
            callback?.Invoke();
        }, "booster", "coin_bag");
    }

}
