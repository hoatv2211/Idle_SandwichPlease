using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class BoosterUnit : MonoBehaviour
{
    [SerializeField] public ETypeBooster m_ETypeBooster;
    [SerializeField] private TextMeshPro txtValue;
    [SerializeField] private float _duration = 2f;
    [SerializeField] private Transform process;

    [SerializeField] private List<GameObject> listModel;
    [SerializeField] private GameObject objCoinBag;
    [SerializeField] private GameObject objCapacity;
    [SerializeField] private GameObject objMoveSpeed;

    private Tween _tweenerCD;
    private void OnEnable()
    {
        _tweenerCD= DOVirtual.DelayedCall(120, () => {
            if (gameObject.activeInHierarchy)
            {
                gameObject.SetActive(false);
            }
        });
    }

    private void OnDisable()
    {
        _tweenerCD.Kill();
    }

    public void CallStart(ETypeBooster _type)
    {
        m_ETypeBooster = _type;
        foreach (var k in listModel)
            k.SetActive(false);
        switch (m_ETypeBooster)
        {
            case ETypeBooster.CoinBag:
                objCoinBag.SetActive(true);
                txtValue.gameObject.SetActive(true);
                DisplayInfo(MapController.Instance.levelProcess.booster_money[0]);
                break;
            case ETypeBooster.Capacity:
                objCapacity.SetActive(true);
                //txtValue.gameObject.SetActive(false);
                txtValue.text = "180%";
                break;
            case ETypeBooster.MoveSpeed:
                objMoveSpeed.SetActive(true);
                //txtValue.gameObject.SetActive(false);
                txtValue.text = "180%";
                break;
            default:
                break;
        }

        FirebaseManager.Instance.LogEvent_Booster(_type.ToString(), "start");
    }

    public void DisplayInfo(int _vl)
    {
        txtValue.text = _vl.ToString();

    }

    bool isCanShow = false;
    private void OnTriggerEnter(Collider other)
    {
        PlayerCtrl player = other.gameObject.GetComponent<PlayerCtrl>();
        if (player)
        {
            isCanShow = true;
            CallStart(m_ETypeBooster);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        PlayerCtrl player = other.gameObject.GetComponent<PlayerCtrl>();
        if (player&&isCanShow)
        {
            if (!MapController.Instance.joyStick.isTouching)
            {
                isCanShow = false;
                CallStart(m_ETypeBooster);
                UIMainGame.Instance.Show_UIBooster(m_ETypeBooster, ()=> { gameObject.SetActive(false); });
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

public enum ETypeBooster
{
    CoinBag,
    Capacity,
    MoveSpeed,
    EatSpeed,
    Hoverboard,
    Hopter
}