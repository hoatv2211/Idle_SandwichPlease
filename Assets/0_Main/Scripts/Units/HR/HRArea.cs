using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ETypeHR
{
   Player,
   Staff
}
public class HRArea : MonoBehaviour
{
    public ETypeHR typeHR;

    private void OnTriggerEnter(Collider other)
    {
        PlayerCtrl player = other.gameObject.GetComponent<PlayerCtrl>();
        if (player)
        {
            switch (typeHR)
            {
                case ETypeHR.Player:
                    UIMainGame.Instance.Show_UIUpgradePlayer();
                    break;
                case ETypeHR.Staff:
                    UIMainGame.Instance.Show_UIUpgradeStaff();
                    break;
                default:
                    break;
            }
           
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerCtrl player = other.gameObject.GetComponent<PlayerCtrl>();
        if (player)
        {
            switch (typeHR)
            {
                case ETypeHR.Player:
                    UIMainGame.Instance.m_UIUpgradePlayer.gameObject.SetActive(false);
                    break;
                case ETypeHR.Staff:
                    UIMainGame.Instance.m_UIUpgradeStaff.gameObject.SetActive(false);
                    break;
                default:
                    break;
            }

        }
    }
}
