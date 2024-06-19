using DG.Tweening;
using Firebase.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterSpawn : MonoBehaviour
{
    private void OnEnable()
    {
        DOVirtual.DelayedCall(1, () =>
        {
            if (!MapController.Instance.booterCtrl.trSpawns.Contains(this.transform))
            {
                MapController.Instance.booterCtrl.trSpawns.Add(this.gameObject.transform);
                //Debug.LogError("enable new booster");
            }
        });
       
       
    }

    private void OnDisable()
    {
        if (MapController.Instance.booterCtrl.trSpawns.Contains(this.transform))
        {
            MapController.Instance.booterCtrl.trSpawns.Remove(this.gameObject.transform);
            //Debug.LogError("disable new booster");

        }
       
    }
}
