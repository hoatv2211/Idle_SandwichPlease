using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UICleanBot : MonoBehaviour
{
    [SerializeField] private UIButton btnAdsCharge;
    [SerializeField] private UIButton btnTicket;
    [SerializeField] private UIButton btnClose;

    [SerializeField] private TextMeshProUGUI txtProcess;
    [SerializeField] private Image imgProcess;

    Coroutine coroutine;
    private void OnEnable()
    {
        CallStart();
    }

    private void OnDisable()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
    }

    public void CallStart()
    {
        btnAdsCharge.SetUpEvent(Action_btnCharge);
        btnTicket.SetUpEvent(Ation_btnTicket);
        btnClose.SetUpEvent(Action_btnClose);
       
        if (coroutine != null)
            StopCoroutine(coroutine);

        ShowInfo();
        coroutine = StartCoroutine(IECountdownTime());

        btnTicket.gameObject.SetActive(Module.ticket_currency > 0);
        btnAdsCharge.gameObject.SetActive(Module.ticket_currency <= 0);
    }

    IEnumerator IECountdownTime()
    {
        ShowInfo();
        while (Module.timeCleanBot > 0)
        {
            yield return new WaitForSeconds(1f);
            ShowInfo();
        }
    }

    private void Action_btnCharge()
    {
        AdsAppLovinController.Instance.ShowRewardedAd(() => 
        {
            Module.Action_Event_CleanBot(180);
            CallStart();
        }, "cleanbot", "cleanbot");
    }

    private void Ation_btnTicket()
    {
        Module.Action_Event_CleanBot(180);

        CallStart();
    }

    public void ShowInfo()
    {
        txtProcess.text = (100*Module.timeCleanBot / 180f).ToString("00") + "%";
        imgProcess.fillAmount = Module.timeCleanBot/180f;
    }

    private void Action_btnClose()
    {
        gameObject.SetActive(false);
        FirebaseManager.Instance.LogEvent_click_button("close_cleanbot");
    }

}
