using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.AI;
using System;

public class YoutuberUnit : UnitBase
{
    [Space]
    public CustomerData m_CustomerData;

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
    [SerializeField] private GameObject objLaptop;

   
    [Header("CUSTOMER UNIT - MOVING")]
    [SerializeField] protected NavMeshAgent m_navMeshAgent;
    [SerializeField] private Animator m_animator;

    [SerializeField] private ECustomerState m_customerState;
    public float waitingTime = 300;

    [SerializeField] public TableCreateArea table;
    [SerializeField] private Transform trStart;
    [SerializeField] private Transform trEnd;
    [SerializeField] private Transform moneyArea;
    [SerializeField] private CashBox cashBox;

    [SerializeField] private YoutuberStorage storage;

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
        storage.Clean();

        MapController.Instance.cameraCtrl.SetCamTargetFocusNoAction(this.transform);


        _corState = StartCoroutine(IeState());
   

        FirebaseManager.Instance.LogEvent_Quest("youtuber", "start");
    }

    Coroutine _corState;
    private IEnumerator IeState()
    {
        table = null;
        Module.isYoutuberSeat = true;
        specialData.isCashOut = false;
        while (table == null)
        {
            table = MapController.Instance.GetTableIndex();
            if (!table.isEmpty())
                table = null;
            yield return null;
           
        }
      
       
        table.SetStage(ETableState.Seating);
        table.isBlocked = true;
        trEnd = table.seatPoints[0].transform;

        foreach (var k in table.seatPoints)
            k.IsFilled = true;

        yield return null;
        Module.isYoutuberSeat = false;

        yield return new WaitForSeconds(5);

        CurrentState = ECustomerState.Move;
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

                    yield return null;
                    m_animator.SetBool("isTyping", true);
                    objLaptop.SetActive(true);
                    //m_animator.SetBool("isEat", true);
                    m_navMeshAgent.enabled = false;
                    transform.LookAt(table.seatPoints[0].LookAtTransform);
                    transform.SetParent(table.seatPoints[0].transform);
                    transform.DOLocalMove(Vector3.zero, 0.1f);
                    transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);

                    yield return null;
                    waitingTime = specialData.timeCountdown;
                    CurrentState = ECustomerState.WaitingCheckOut;
                    hud_Order.gameObject.SetActive(true);

                    break;
                case ECustomerState.WaitingCheckOut:
                    waitingTime -= Time.deltaTime;
                    txtTimeCD.text = Module.SecondCustomToTime((int)waitingTime);
                    txtSandwich.text = specialData.specialProduct.strTextProcess;
                    if (waitingTime <= 0)
                    {
                        //Next state
                        hud_Order.gameObject.SetActive(false);
                        CurrentState = ECustomerState.Exiting;
                        FirebaseManager.Instance.LogEvent_Quest("youtuber", "fail");
                    }

                    if (specialData.specialProduct.isFull)
                    {
                        hud_Order.gameObject.SetActive(false);
                        m_animator.SetBool("isTyping", false);
                        m_animator.SetBool("isClap", true);
                        objHappy?.SetActive(true);
                        yield return new WaitUntil(() => specialData.isCashOut);
                       
                        CurrentState = ECustomerState.Eating;
                       
                    }
                    break;

                case ECustomerState.Eating:
                    m_animator.SetBool("isClap", false);
                    m_animator.SetBool("isTypeAndEat", true);
                    table.piecesGroup._parent.gameObject.SetActive(false);
                    storage.Eating();
                  
                    yield return new WaitUntil(() => storage.isDoneEat);
                    m_animator.SetBool("isTypeAndEat", false);
                    CurrentState = ECustomerState.Exiting;
                  
                   
                    break;

                case ECustomerState.Exiting:
                    table.SetAnimChairOff();
                    m_animator.SetBool("isClap", false);
                    m_animator.SetBool("isTyping", false);
                    m_animator.SetBool("isTypeAndEat", false);
                    m_animator.SetBool("Run", true);
                    FirebaseManager.Instance.LogEvent_Quest("youtuber", "complete");
                    table.piecesGroup._parent.gameObject.SetActive(true);
                    table.SetStage(ETableState.GotTrash);
                    table.isBlocked = false;

                    storage.Clean();
                    table = null;
                    objLaptop.SetActive(false);
                    Move(trStart.position, true);
                    hud_Order.gameObject.SetActive(false);
                    objSleep?.SetActive(false);
                    objHappy?.SetActive(specialData.specialProduct.isFull);
                    objAngry?.SetActive(!specialData.specialProduct.isFull);
                    yield return new WaitUntil(() => ReachedDestinationOrGaveUp());
                    m_navMeshAgent.enabled = false;
                    m_animator.SetBool("Run", false);

                    transform.SetParent(MapController.Instance.shipperCtrl.transform);
                    yield return null;
                    MapController.Instance.shipperCtrl.DelayShiperShow(Module.EasyRandom(60, 100));
                    //MapController.Instance.shipperCtrl.DelayShiperShow(10);
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
