using DG.Tweening;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class UILevelUp : MonoBehaviour
{

    public Level_User info_reward;

    [SerializeField] private TextMeshProUGUI txtLevel;
    [SerializeField] private GameObject[] listUnlocks;  //0.Zone,1.Map, 2.stucture
    [SerializeField] private TextMeshProUGUI txtMoney;

    [SerializeField] private UIButton btnTicket;
    [SerializeField] private UIButton btnAds;
    [SerializeField] private UIButton btnReward;


    public void CallStart()
    {
        info_reward = BattlePassModel.Instance.cr_level;

        btnTicket.SetUpEvent(Action_btnTicket);
        btnAds.SetUpEvent(Action_btnAds);
        btnReward.SetUpEvent(Action_btnReward);

        ShowInfo();

    }

    public void ShowInfo()
    {
        txtLevel.text = info_reward.level.ToString();
        txtMoney.text = "+" + info_reward.bonus_money.ToString();

        listUnlocks[0].SetActive(info_reward.feature_unlocks.ToList().Contains("zone"));
        listUnlocks[1].SetActive(info_reward.feature_unlocks.ToList().Contains("newmap"));
        listUnlocks[2].SetActive(info_reward.feature_unlocks.ToList().Contains("stucture"));



        switch (UserModel.Instance.level)
        {
            case 2:
                DOVirtual.DelayedCall(5f, () => UIMainGame.Instance.Show_UIDaily());
                UIMainGame.Instance.btnCompass.gameObject.SetActive(true);
                UIMainGame.Instance.btnDaily.gameObject.SetActive(true);
                break;

            case 3:
                UIMainGame.Instance.ShowAdsInter();
                UIMainGame.Instance.btnRemoveAds.gameObject.SetActive(true);
                UIMainGame.Instance.btnShop.gameObject.SetActive(true);
                break;
            case 4:
                //MapController.Instance.shipperCtrl.StartSpecialShiper();

                break;
            case 5:
                MapController.Instance.objCleanBot.SetActive(true);
                UIMainGame.Instance.btnCleanBot.gameObject.SetActive(true);
                UIMainGame.Instance.btnSkin.gameObject.SetActive(true);
                break;
            case 9:
                UIMainGame.Instance.btnMap.gameObject.SetActive(true);
                UIMainGame.Instance.popupStoreUnlock.gameObject.SetActive(true);
                UIMainGame.Instance.popupStoreUnlock.CallStart(2, false);
                break;

            default:
                break;
        }

        Module.Action_Event_RefreshNotice();
    }

    private void Action_btnTicket()
    {
        if (Module.ticket_currency < 1)
            return;

        DOVirtual.DelayedCall(2.2f, () => gameObject.SetActive(false));
        int _value = info_reward.bonus_money * 2;
        int _vl = _value / 10;
        if (_vl > 40)
            _vl = 40;
        UIMainGame.Instance.Show_Effect_MoneyBooter(txtMoney.transform, _vl);

        int money = Module.money_currency;
        DOVirtual.Int(money, money + _value, 2, (x) => Module.money_currency = x);

        int moneyTracking = Module.money_currency;
        FirebaseManager.Instance.LogEvent_receive("money", "money",
          _value, moneyTracking - _value, moneyTracking,
          "level_up", "level_up_" + info_reward.level, _value);

    }

    private void Action_btnAds()
    {
        AdsAppLovinController.Instance.ShowRewardedAd(() =>
        {
            DOVirtual.DelayedCall(2.2f, () => gameObject.SetActive(false));
            int _value = info_reward.bonus_money * 2;
            int _vl = _value / 10;
            if (_vl > 40)
                _vl = 40;
            UIMainGame.Instance.Show_Effect_MoneyBooter(txtMoney.transform, _vl);

            int money = Module.money_currency;
            DOVirtual.Int(money, money + _value, 2, (x) => Module.money_currency = x);

            int moneyTracking = Module.money_currency;
            FirebaseManager.Instance.LogEvent_receive("money", "money",
          _value, moneyTracking - _value, moneyTracking,
          "level_up", "level_up_" + info_reward.level, _value);


        }, "level_up", "level_" + info_reward.level);

        if(UserModel.Instance.level == 2 )
            StartCoroutine("WaitThenShow");
    }

    private void Action_btnReward()
    {
        DOVirtual.DelayedCall(2.2f, () => gameObject.SetActive(false));
        int _value = info_reward.bonus_money;
        int _vl = _value / 10; 
        if (_vl > 40)
            _vl = 40;
        UIMainGame.Instance.Show_Effect_MoneyBooter(txtMoney.transform, _vl);

        int money = Module.money_currency;
        DOVirtual.Int(money, money + _value, 2, (x) => Module.money_currency = x);

        int moneyTracking = Module.money_currency;
        FirebaseManager.Instance.LogEvent_receive("money", "money",
         _value, moneyTracking - _value, moneyTracking,
         "level_up", "level_up_" + info_reward.level, _value);

    }

}
