using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ItemTimeFree : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private UIButton btnGet;
    [SerializeField] private GameObject objRewarded;

    private TimeRewardFree timeReward;
    public void CallStart(TimeRewardFree _timeReward,bool _isReward,int _idReward)
    {
        timeReward = _timeReward;


        if (id > _idReward)
        {
            btnGet.gameObject.SetActive(false);
            objRewarded.gameObject.SetActive(false);
        }
        else if(id<_idReward)
        {
            btnGet.gameObject.SetActive(false);
            objRewarded.gameObject.SetActive(true);
        }
        else
        {
            btnGet.gameObject.SetActive(_isReward);
            objRewarded.gameObject.SetActive(false);
        }

        btnGet.SetUpEvent(Action_btnGet);
    }

    private void Action_btnGet()
    {
        
        btnGet.gameObject.SetActive(false);
        objRewarded.gameObject.SetActive(true);
        timeReward.Action_GetItem(id);
    }



}
