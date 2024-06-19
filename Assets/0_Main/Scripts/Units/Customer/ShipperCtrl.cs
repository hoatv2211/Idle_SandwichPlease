using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShipperCtrl : MonoBehaviour
{
    public ShipperUnit shipperUnit;
    public YoutuberUnit youtuberUnit;
    public Transform trStart;
    public Transform trEnd;


    private void OnEnable()
    {
        DOVirtual.DelayedCall(5,()=> 
        {
            if (MapController.Instance.mapData.IsUnlockByName("table_04"))
            {
                DOVirtual.DelayedCall(GameManager.Instance.m_DataConfigRemote.time_delay_special - 5, StartSpecialShiper);
            }

        });
    }

    public void StartSpecialShiper()
    {
        int _random = Module.EasyRandom(0, 2);
        if (shipperUnit.gameObject.activeInHierarchy || youtuberUnit.gameObject.activeInHierarchy)
        {
            DelayShiperShow(60);
            return;
        }



        if (_random == 0)
        {
            shipperUnit.gameObject.SetActive(true);
            shipperUnit.transform.position = trStart.position;
            shipperUnit.CallStart();
        }
        else
        {
            youtuberUnit.gameObject.SetActive(true);
            youtuberUnit.transform.position = trStart.position;
            youtuberUnit.CallStart();

        }

        //youtuberUnit.gameObject.SetActive(true);
        //youtuberUnit.transform.position = trStart.position;
        //youtuberUnit.CallStart();
    }

    public void DelayShiperShow(int _time)
    {
        DOVirtual.DelayedCall(_time, () => StartSpecialShiper());
    }
}
