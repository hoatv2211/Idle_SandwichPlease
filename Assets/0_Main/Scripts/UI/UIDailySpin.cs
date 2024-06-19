using DG.Tweening;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIDailySpin : MonoBehaviour
{
    [SerializeField] private UIButton btnSpin;
    [SerializeField] private UIButton btnSpinAds;
    [SerializeField] private UIButton btnExit;

    [SerializeField] private GameObject Circle;
    [SerializeField] private GameObject Arrow;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private BoosterCD boosterCD; // khai báo booster
    [SerializeField] public GameObject EFShow;

    private bool spinning = false;
    private float speed = 1000f;
    private float countdownTime = 6 * 60 * 60; // cho 6h coutdown thôi để nó còn quay lại chơi
    private int SpinLeft
    {
        get { return PlayerPrefs.GetInt("spinads", 4); }
        set { PlayerPrefs.GetInt("spinads", value); }

    }

    private float timeLeft = 0;
    private string getTimeSpinFree // cái này sẽ lưu thời gian e quay cái spin free
    {
        get { return PlayerPrefs.GetString("getTimeSpinFree", string.Empty); }
        set { PlayerPrefs.SetString("getTimeSpinFree", value); }
    }
    public double timeRemain() 
    {
        if (string.IsNullOrEmpty(getTimeSpinFree))
            return 0;

        double time = Convert.ToDateTime(DateTime.Now).Subtract(Convert.ToDateTime(getTimeSpinFree)).TotalSeconds;

        return time;
    }

    public void CallStart()
    {
        btnExit.SetUpEvent(btn_Exit);
        btnSpin.SetUpEvent(Spin);
        btnSpinAds.SetUpEvent(SpinAds);

        if(timeRemain() == 0)
        {
            timeLeft = 0;
        }
        else
        {
            timeLeft = countdownTime - (float)timeRemain(); // tính toán thời gian còn lại
        }

        btnSpinAds.gameObject.SetActive(SpinLeft > 0);

        if (timeLeft <= 0)
        {
            //show free spin
            btnSpin.gameObject.SetActive(true);
            btnSpin.enabled= true;
            countdownText.text = "Ready";
        }
        else
        {
            btnSpin.gameObject.SetActive(false);
            //show countdown
            StartCoroutine(IECountdown());

        }
    }

    public void btn_Exit()
    {
        gameObject.SetActive(false);
        StopCoroutine(IECountdown());
    }

    public void Spin()
    {
        if (!spinning)
        {
            spinning = true;
            //StartCoroutine("SpinCoroutine"); //code thế này thì ref kiểu liz gì
            StartCoroutine(IESpinCoroutine());


            btnSpin.gameObject.SetActive(false);
            getTimeSpinFree = DateTime.Now.ToString();
            timeLeft = countdownTime;
            StartCoroutine(IECountdown());
        }

    }
    
    public void SpinAds()
    {
        if (SpinLeft > 0)
        {
            AdsAppLovinController.Instance.ShowRewardedAd(() =>
            {
                SpinLeft -= 1;
                //SaveData() ;
                //StartCoroutine("SpinCoroutine"); //code thế này thì ref kiểu liz gì
                StartCoroutine(IESpinCoroutine());
                if (SpinLeft <= 0)
                    btnSpinAds.enabled=false;
            }
            , null, null);
        }

    }

    IEnumerator IESpinCoroutine()
    {
        float randomSpeed = UnityEngine.Random.Range(900f, speed);
        while (randomSpeed > 0)
        {
            Circle.transform.Rotate(Vector3.forward * randomSpeed * Time.deltaTime);
            randomSpeed -= Time.deltaTime * 100;
            btnSpin.enabled = false;
            btnSpinAds.enabled = false;
            btnExit.enabled = false;
            yield return null;
        }
        //Debug.LogError("chieu z : " + Circle.transform.eulerAngles.z);

        if (Circle.transform.eulerAngles.z - 30 > 0 && Circle.transform.eulerAngles.z - 30 <= 60)
        {
            UIMainGame.Instance.Show_HUDBooster(ETypeBooster.EatSpeed);
        }
        else if (Circle.transform.eulerAngles.z - 30 > 60 && Circle.transform.eulerAngles.z - 30 <= 120)
        {
            UIMainGame.Instance.Show_Effect_MoneyBooter(EFShow.transform, 30);
            int money = Module.money_currency;
            DOVirtual.Int(money, money + 400, 2, (x) => Module.money_currency = x);
        }
        else if (Circle.transform.eulerAngles.z - 30 > 120 && Circle.transform.eulerAngles.z - 30 <= 180)
        {
            UIMainGame.Instance.Show_HUDBooster(ETypeBooster.Capacity);
        }
        else if (Circle.transform.eulerAngles.z - 30 > 180 && Circle.transform.eulerAngles.z - 30 <= 240)
        {
            UIMainGame.Instance.Show_Effect_MoneyBooter(EFShow.transform, 40);
        
            int money = Module.money_currency;
            DOVirtual.Int(money, money + 1200, 2, (x) => Module.money_currency = x);
        }
        else if (Circle.transform.eulerAngles.z - 30 > 240 && Circle.transform.eulerAngles.z - 30 <= 300)
        {
            UIMainGame.Instance.Show_HUDBooster(ETypeBooster.MoveSpeed);
        }
        else if (Circle.transform.eulerAngles.z - 30 > 300 && Circle.transform.eulerAngles.z - 30 <= 360)
        {
            UIMainGame.Instance.Show_Effect_MoneyBooter(EFShow.transform, 20);
            int money = Module.money_currency;
            DOVirtual.Int(money, money + 200, 2, (x) => Module.money_currency = x);
        }


        btnSpinAds.enabled = SpinLeft > 0;
        spinning = false;
        btnExit.enabled = true;
    }




    IEnumerator IECountdown()
    {
        while (timeLeft > 0)
        {
            btnSpin.gameObject.SetActive(false);

            int hours = Mathf.FloorToInt((timeLeft / (60 * 60)));
            int minutes = Mathf.FloorToInt((timeLeft / 60) % 60);
            int seconds = Mathf.FloorToInt(timeLeft % 60);

            countdownText.text = string.Format(" Refresh : {0:00}:{1:00}:{2:00}", hours, minutes, seconds);

            yield return new WaitForSeconds(1f); // Chờ 1 giây

            timeLeft -= 1f;
           
            if(timeLeft <= 0)
            {
                countdownText.text = " Ready ";
                btnSpin.gameObject.SetActive(true);
                btnSpin.enabled = true;
            }
        }

    }

}
