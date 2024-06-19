using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class UIRemoveAds : MonoBehaviour
{
    [SerializeField] private UIButton btnPurchase;
    [SerializeField] private UIButton btnExit;

    private void Start()
    {
        //btnPurchase.SetUpEvent(Action_btnRemoveAds);
        btnExit.SetUpEvent(Action_btnExit);

    }

    private void Action_btnExit()
    {
        gameObject.SetActive(false);

        FirebaseManager.Instance.LogEvent_click_button("close_removeads");
    }



}
