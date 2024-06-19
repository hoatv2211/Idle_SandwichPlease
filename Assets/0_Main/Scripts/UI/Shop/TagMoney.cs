using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class TagMoney : MonoBehaviour
{
    public int _baseValue;
    public TextMeshProUGUI txtMoney;

    [SerializeField] private ProductButton productButton;
    private void OnEnable()
    {
        CallStart();
    }

    public void CallStart()
    {
        int _value =(int)(_baseValue * MapController.Instance.levelProcess.level * 1.5f);
        txtMoney.text = "+" + _value.ToString();
        productButton?.SetActionCallBack(Action_BuySuccess);
    }

    public void Action_BuySuccess()
    {
        int _value = (int)(_baseValue * MapController.Instance.levelProcess.level * 1.5f);

        int _vl = _value / 10;
        if (_vl > 40)
            _vl = 40;
        UIMainGame.Instance.Show_Effect_MoneyBooter(this.transform, _vl);

        int money = Module.money_currency;
        DOVirtual.Int(money, money + _value, 2, (x) => Module.money_currency = x);

        int moneyTracking = Module.money_currency;
        FirebaseManager.Instance.LogEvent_receive("money", "money",
          _value, moneyTracking - _value, moneyTracking,
          "buy_IAP", productButton.productID, _value);
    }
}
