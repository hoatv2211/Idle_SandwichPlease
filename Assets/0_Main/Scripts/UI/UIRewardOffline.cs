using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIRewardOffline : MonoBehaviour
{
    [SerializeField] private UIButton btnClaim;
    [SerializeField] private UIButton btnAdsClaim;
    [SerializeField] private TextMeshProUGUI txtMoney;

    [SerializeField] private int moneyReceive;
    public void CallStart(int _hour)
    {
        moneyReceive = _hour * (MapController.Instance.levelProcess.booster_money[0] / 2);
        //txtMoney.text = moneyReceive.ToString();
        btnClaim.SetUpEvent(Action_btnClaim);
        btnAdsClaim.SetUpEvent(Action_btnAdsClaim);

        btnClaim.gameObject.SetActive(false);
        DOVirtual.Int(0, moneyReceive, 1.5f, (x) => { txtMoney.text = x.ToString(); });
        DOVirtual.DelayedCall(1.5f, () => btnClaim.gameObject.SetActive(true));
    }

    private void Action_btnClaim()
    {
        DOVirtual.DelayedCall(2.5f, () => {
            gameObject.SetActive(false);

        });

        btnClaim.enabled = false;
        btnAdsClaim.enabled = false;

        int _value = moneyReceive;
        int _vl = _value / 10;
        if (_vl > 40)
            _vl = 40;
        UIMainGame.Instance.Show_Effect_MoneyBooter(txtMoney.transform, _vl);

        int money = Module.money_currency;
        DOVirtual.Int(money, money + _value, 2, (x) => Module.money_currency = x);

        int moneyTracking = Module.money_currency;
        FirebaseManager.Instance.LogEvent_receive("money", "money",
          _value, moneyTracking - _value, moneyTracking,
          "reward_offline", "reward_normal", _value);

        Module.time_offline = System.DateTime.Now.ToString();
       
    }

    private void Action_btnAdsClaim() 
    {
        AdsAppLovinController.Instance.ShowRewardedAd(() => {
            btnClaim.enabled = false;
            btnAdsClaim.enabled = false;

            DOVirtual.DelayedCall(2.5f, () => {
                gameObject.SetActive(false);

            });
            int _value = moneyReceive * 2;
            txtMoney.text = _value.ToString();

            int _vl = _value / 10;
            if (_vl > 40)
                _vl = 40;
            UIMainGame.Instance.Show_Effect_MoneyBooter(txtMoney.transform, _vl);

            int money = Module.money_currency;
            DOVirtual.Int(money, money + _value, 2, (x) => Module.money_currency = x);

            int moneyTracking = Module.money_currency;
            FirebaseManager.Instance.LogEvent_receive("money", "money",
              _value, moneyTracking - _value, moneyTracking,
              "reward_offline", "reward_ads", _value);

            Module.time_offline = System.DateTime.Now.ToString();
        }, "offline", "reward_offline");
    }



}
