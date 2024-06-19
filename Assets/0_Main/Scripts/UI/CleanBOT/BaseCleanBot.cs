using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCleanBot : MonoBehaviour
{
    bool isCanShow = false;
    private void OnTriggerEnter(Collider other)
    {
        PlayerCtrl player = other.gameObject.GetComponent<PlayerCtrl>();
        if (player)
        {
            isCanShow = true;
            
        }
    }

    private void OnTriggerStay(Collider other)
    {
        PlayerCtrl player = other.gameObject.GetComponent<PlayerCtrl>();
        if (player && isCanShow)
        {
            if (!MapController.Instance.joyStick.isTouching)
            {
                isCanShow = false;
                UIMainGame.Instance.Show_UICleanBOT();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerCtrl player = other.gameObject.GetComponent<PlayerCtrl>();
        if (player)
        {
            isCanShow = false;
        }
    }
}
