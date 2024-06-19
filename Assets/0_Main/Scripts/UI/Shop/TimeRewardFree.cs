using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

public class TimeRewardFree : MonoBehaviour
{
    public List<ItemTimeFree> itemTimeFrees;


    [SerializeField] private TextMeshProUGUI txtTimeLeft;
    [SerializeField] private UIButton btnAdsFree;

    [SerializeField] private GameObject objCountdown;
    [SerializeField] private int timeCD;


    private void OnEnable()
    {
        btnAdsFree.SetUpEvent(Action_btnAdsFree);

        if (!string.IsNullOrEmpty(Module.datetime_reward_free))
            timeCD = 20 * 60 -(int)Convert.ToDateTime(DateTime.Now).Subtract(Convert.ToDateTime(Module.datetime_reward_free)).TotalSeconds;
        else
            timeCD = 0;

        if (timeCD <= 0)
            timeCD = 0;

        foreach (var k in itemTimeFrees)
        {
            k.CallStart(this, timeCD <= 0, Module.reward_free_turn);
        }

        ShowInfo();
        StartCoroutine(ITimeCD());
    }

    private void OnDisable()
    {
        StopCoroutine(ITimeCD());
    }

    private void Action_btnAdsFree()
    {
        AdsAppLovinController.Instance.ShowRewardedAd(()=> 
        {
            timeCD = 0;
            StopCoroutine(ITimeCD());
            ShowInfo();

            foreach (var k in itemTimeFrees)
            {
                k.CallStart(this, timeCD <= 0, Module.reward_free_turn);
            }
        },"time_reward_free","turn_"+Module.reward_free_turn);
    }

    public void ShowInfo()
    {
        objCountdown.SetActive(timeCD > 0);
        txtTimeLeft.text = Module.SecondCustomToTime2(timeCD);

        if (timeCD <= 0)
        {
            foreach (var k in itemTimeFrees)
            {
                k.CallStart(this, timeCD <= 0, Module.reward_free_turn);
            }
        }
    }

    IEnumerator ITimeCD()
    {
        yield return null;
        ShowInfo();
        while (timeCD > 0)
        {
            yield return new WaitForSeconds(1);
            timeCD--;
            ShowInfo();
        }
    }

    public void Action_GetItem(int _index)
    {
        Module.reward_free_turn++;
        timeCD = 20 * 60;
        Module.datetime_reward_free = System.DateTime.Now.ToString();
        int _value = 0;
        switch (_index)
        {
            case 0:
                _value = 50;
                Module.money_currency += 50;
                break;
            case 1:
                _value = 200;
                Module.money_currency += 200;
                break;
            case 2:
                Module.money_currency += 700;
                _value = 700;
                break;
            case 3:
                Module.ticket_currency += 1;
                break;

            default:
                break;
        }

        if (_value != 0)
        {
            int _vl = _value / 10;
            if (_vl > 40)
                _vl = 40;
            UIMainGame.Instance.Show_Effect_MoneyBooter(itemTimeFrees[_index].transform, _vl);

            int money = Module.money_currency;
            DOVirtual.Int(money, money + _value, 2, (x) => Module.money_currency = x);

            int moneyTracking = Module.money_currency;
            FirebaseManager.Instance.LogEvent_receive("money", "money",
              _value, moneyTracking - _value, moneyTracking,
              "reward_free", "reward_free", _value);
        }

        if (Module.reward_free_turn >= 4)
        {
            Module.reward_free_turn = 0;
            foreach (var k in itemTimeFrees)
            {
                k.CallStart(this, timeCD <= 0, Module.reward_free_turn);
            }
        }


        ShowInfo();
        StartCoroutine(ITimeCD());

        UIMainGame.Instance.notice_TimeFree.gameObject.SetActive(false);
    }
}
