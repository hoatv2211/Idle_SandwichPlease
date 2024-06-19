using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Ads_Area : UnitBase
{
    public ETypeUnlock typeUnlock;
    public DriveCarCounterNode driveCarCounterNode;

    [SerializeField] private BoardNode boardNode;
    [SerializeField] private float _duration = 2f;

    float timeDelay = 0;
    private Tweener _tweener;


    private void OnTriggerEnter(Collider other)
    {
        PlayerCtrl player = other.gameObject.GetComponent<PlayerCtrl>();
        if (player)
        {
            timeDelay = 0;

        }
    }

    private void OnTriggerStay(Collider other)
    {
        PlayerCtrl player = other.gameObject.GetComponent<PlayerCtrl>();
        if (player)
        {
            timeDelay += Time.deltaTime;
            if (timeDelay >= 0.3f)
            {

                if (!player.IsMoving())
                {
                    AdsAppLovinController.Instance.ShowRewardedAd(() =>
                    {
                        driveCarCounterNode.UnlockStaff();
                        boardNode.isCanPay = true;
                    }, "ads_staff", m_UnitData.idUnit);
                    return;
                }
                else
                {
                    //animator.SetBool("isPay", false);
                }
            }

        }
    }


    private void OnTriggerExit(Collider other)
    {
       
        PlayerCtrl player = other.gameObject.GetComponent<PlayerCtrl>();
        if (player)
        {
            if (_tweener != null)
            {
                _tweener.Kill();
                _tweener = null;
            }
            
        }
    }

    
}
