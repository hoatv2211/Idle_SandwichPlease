using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using TMPro;
using System;

#region Info Data
[System.Serializable]
public class SpecialData
{
    public float timeCountdown = 300;
    public bool isCashOut = false;
    public List<SpecialProductInfo> specialProductInfos;
    public int moneyValue => specialProduct.total * 45;

    public SpecialProductInfo specialProduct => specialProductInfos[0];
}

[System.Serializable]
public class SpecialProductInfo
{
    public EProductType type;
    public int total=10;
    public int numberfill = 0;

    public bool isFull => numberfill >= total;
    public string strTextProcess => numberfill + "/" + total;
}

#endregion

[RequireComponent(typeof(NavMeshAgent))]
public class ShipperUnit : UnitBase
{

    [Header("HUD")]
    public SpecialData specialData;
    [SerializeField] private GameObject hud_Order;
    [SerializeField] private TextMeshPro txtTimeCD;
    [SerializeField] private GameObject objOderSandwich;
    [SerializeField] private GameObject doneSandwich;
    [SerializeField] private TextMeshPro txtSandwich;
    [SerializeField] private GameObject objOderSandwich_package;
    [SerializeField] private GameObject doneSandwich_package;
    [SerializeField] private TextMeshPro txtSandwich_package;

    [SerializeField] private GameObject objHappy;
    [SerializeField] private GameObject objAngry;
    [SerializeField] private GameObject objSleep;

    public CustomerData m_CustomerData;
    [Header("CUSTOMER UNIT - MOVING")]
    [SerializeField] protected NavMeshAgent m_navMeshAgent;
    [SerializeField] private Animator m_animator;

    [SerializeField] private ECustomerState m_customerState;
    public float waitingTime = 300;

    [SerializeField] private Transform trStart;
    [SerializeField] private Transform trEnd;
    [SerializeField] private Transform moneyArea;
    [SerializeField] private CashBox cashBox;

    [SerializeField] private ShipperStorage shipperStorage;

    public ECustomerState CurrentState
    {
        get => m_customerState;
        set
        {
            m_customerState = value;
        }
    }

    public override void CallStart()
    {
       
        specialData.timeCountdown = 300;
        specialData.specialProductInfos[0].numberfill = 0;
        specialData.specialProductInfos[0].total = MapController.Instance.levelProcess.value_quest[0];

        objHappy.SetActive(false);
        objAngry.SetActive(false);
        objSleep.SetActive(false);
        shipperStorage.Clean();
        MapController.Instance.cameraCtrl.SetCamTargetFocusNoAction(this.transform);
        DOVirtual.DelayedCall(5, () => _corState = StartCoroutine(IeState()));

        FirebaseManager.Instance.LogEvent_Quest("shiper", "start");
    }

    Coroutine _corState;
    private IEnumerator IeState()
    {
        CurrentState = ECustomerState.Move;
        specialData.isCashOut = false;
        while (CurrentState != ECustomerState.End)
        {
            switch (CurrentState)
            {
                case ECustomerState.Move:
                    hud_Order.gameObject.SetActive(false);
                    txtSandwich.text = specialData.specialProduct.strTextProcess;
                    Move(trEnd.position, true);

                    yield return new WaitUntil(() => ReachedDestinationOrGaveUp());
                    Debug.Log("Done to target point");
                    m_navMeshAgent.enabled = false;
                    m_animator.SetBool("Run", false);

                    //Debug.LogError("1");
                    yield return null;
                    waitingTime = specialData.timeCountdown;
                    CurrentState = ECustomerState.WaitingCheckOut;
                    hud_Order.gameObject.SetActive(true);
                    transform.eulerAngles = Vector3.zero;
                    break;
                case ECustomerState.WaitingCheckOut:
                    waitingTime -= Time.deltaTime;
                    txtTimeCD.text = Module.SecondCustomToTime((int)waitingTime);
                    txtSandwich.text = specialData.specialProduct.strTextProcess;
                    if(waitingTime<=0)
                    {
                        //Next state
                        hud_Order.gameObject.SetActive(false);
                        CurrentState = ECustomerState.Exiting;
                        FirebaseManager.Instance.LogEvent_Quest("shiper", "fail");
                    }

                    if (specialData.specialProduct.isFull)
                    {
                        hud_Order.gameObject.SetActive(false);
                        objSleep?.SetActive(true);
                        yield return new WaitUntil(() => specialData.isCashOut);
                        CurrentState = ECustomerState.Exiting;
                        FirebaseManager.Instance.LogEvent_Quest("shiper", "complete");
                    }

                    break;
          
                case ECustomerState.Exiting:
                    Move(trStart.position, true);
                    hud_Order.gameObject.SetActive(false);
                    objSleep?.SetActive(false);
                    objHappy?.SetActive(specialData.specialProduct.isFull);
                    objAngry?.SetActive(!specialData.specialProduct.isFull);
                    yield return new WaitUntil(() => ReachedDestinationOrGaveUp());
                    m_navMeshAgent.enabled = false;
                    m_animator.SetBool("Run", false);

                    yield return null;
                    MapController.Instance.shipperCtrl.DelayShiperShow(Module.EasyRandom(60,100));
                    CurrentState = ECustomerState.End;
                   
                    break;

                default:
                    break;
            }


            yield return null;
        }

        yield return null;
        gameObject.SetActive(false);
    }


    public bool ReachedDestinationOrGaveUp()
    {
        try
        {
            if (m_navMeshAgent.enabled && !m_navMeshAgent.pathPending)
            {
                if (m_navMeshAgent.remainingDistance <= m_navMeshAgent.stoppingDistance)
                {
                    if (!m_navMeshAgent.hasPath || m_navMeshAgent.velocity.sqrMagnitude == 0f)
                        return true;
                }

            }


            return false;
        }
        catch (Exception e)
        {

            Debug.LogError(e.Message);
        }

        return false;
    }

    public void Move(Vector3 targetPos, bool _isNav = false)
    {
        if (!m_navMeshAgent)
        {
            Debug.LogError("Check NavMeshAgent");
            return;
        }
        m_navMeshAgent.enabled = _isNav;
        if (!m_navMeshAgent.enabled)
        {
            Debug.LogWarning("NavMesh is disable");
            return;
        }

        m_navMeshAgent.speed = m_CustomerData.moveSpeed;
        m_navMeshAgent.SetDestination(targetPos);
        m_animator.SetBool("Run", true);
    }

    public void CarryState(int _value = 0)
    {
        m_animator.SetInteger("Carry", _value);
    }

}
