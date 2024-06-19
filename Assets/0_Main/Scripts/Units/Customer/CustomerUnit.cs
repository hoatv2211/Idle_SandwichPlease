using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public enum ECustomerState
{
    Move,
    WaitingInLine,
    SortInLine,
    Buying,
    WaitingCheckOut,
    CheckingOut,
    GoToSeat,
    Eating,
    Exiting,
    TakeAway,
    End
}

[RequireComponent(typeof(NavMeshAgent))]
public class CustomerUnit : UnitBase
{
    public CustomerData m_CustomerData;
    [Space, Header("CUSTOMER UNIT - MOVING")]
    [SerializeField] protected NavMeshAgent m_navMeshAgent;

    [Space, Header("CUSTOMER UNIT - PICKUP AND DROP")]
    [SerializeField] private CustomerPickup m_customerPickup;
    public CustomerPickup CustomerPickup => m_customerPickup;

    [Space, Header("CUSTOMER TROLLEY")]
    [SerializeField] private Transform m_trolley;
    [SerializeField] private Transform m_posBox;
    [SerializeField] private Animator m_animator;
    [SerializeField] private List<GameObject> m_listSkins;

    [SerializeField] private GameObject[] objTrays;

    [Space, Header("BALLOONS")]
    [SerializeField] private Transform trBalloon;
    [SerializeField] private GameObject objSleepBalloon;
    [SerializeField] private GameObject objAngryBalloon;
    [SerializeField] private GameObject objTableCleanBalloon;
    [SerializeField] private GameObject objEatBalloon;
    [SerializeField] private GameObject objCheckingOut;
    [SerializeField] public CashBox cashBox;


    [Space, Header("HIDE LATE")]
    [SerializeField] private EProductType  m_productType;
    [SerializeField] private ECustomerState m_customerState;
    private CustomerPoint m_targetPoint { get; set; }
    private int cashValue
    {
        get 
        { 
            int _value= m_CustomerData.isVip? CustomerPickup.ProductUnits.Count * 7 : CustomerPickup.ProductUnits.Count * 5;
            _value += (int)GameManager.Instance.skinModel.infoSkin.single_price_bonus;

            return _value; 
        }
        
    } 

    private float waitimeResetTask = 0;
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
        CurrentState = ECustomerState.Move;
        _corScanNextCustomerPoint= StartCoroutine(IeScanNextCustomerPoint());

        m_CustomerData.stackProduct = Module.EasyRandom(MapController.Instance.levelProcess.value_customer[0], MapController.Instance.levelProcess.value_customer[1]+1);
        m_customerPickup.CallStart(m_CustomerData);

        //Random skin
        int indexSkin = Module.EasyRandom(0, m_listSkins.Count);
        for (int i = 0; i < m_listSkins.Count; i++)
        {
            if (i == indexSkin)
            {
                m_listSkins[i].SetActive(true);
            }
            else
            {
                m_listSkins[i].SetActive(false);
            }
        }

        m_customerPickup.ResetTray();
        m_animator.SetBool("isEat", false);
        m_animator.SetInteger("Carry", 0);

        //tray
        objTrays[0].SetActive(MapController.Instance._idMap == 1);
        objTrays[1].SetActive(MapController.Instance._idMap == 2);
    }

    public void CallVip()
    {
        CurrentState = ECustomerState.Move;
        _corScanNextCustomerPoint = StartCoroutine(IeScanNextCustomerPoint());

        m_CustomerData.stackProduct = Module.EasyRandom(MapController.Instance.levelProcess.value_customer[0], MapController.Instance.levelProcess.value_customer[1]);
        m_CustomerData.isVip = true;
        m_customerPickup.CallStart(m_CustomerData);
     
    }

    public void InitTargetPoint(CustomerPoint _target)
    {
        m_targetPoint = _target;
        m_targetPoint.IsFilled = true;
        _target.IsFilled = true;
    }

    public void SetNewTarget(CustomerPoint _target)
    {
        
        m_targetPoint.IsFilled = false;
        m_targetPoint = _target;
        m_navMeshAgent.enabled = false;
        _target.IsFilled = true;
        if (_corScanNextCustomerPoint!=null)
        {
            StopCoroutine(_corScanNextCustomerPoint);
            _corScanNextCustomerPoint = null;
        }
        CurrentState = ECustomerState.SortInLine;
        _corScanNextCustomerPoint = StartCoroutine(IeScanNextCustomerPoint());
    }

    CounterStorage counterStorage => MapController.Instance.counterStorage;
    Juice_storage juice_Storage => MapController.Instance.juice_Machine.juice_Storage;
    public SeatPoint seatPoint;
    Coroutine _corScanNextCustomerPoint;
    private IEnumerator IeScanNextCustomerPoint()
    {

        while (CurrentState!=ECustomerState.End)
        {
            switch (CurrentState)
            {
                case ECustomerState.Move:
                    if (Vector3.Distance(this.transform.position, MapController.Instance.customerPath.trPath1.position) >
                       Vector3.Distance(this.transform.position, m_targetPoint.transform.position))
                    {
                        Move(m_targetPoint.transform.position, true);
                    }
                    else
                    {
                        Move(MapController.Instance.customerPath.trPath1.position, true);
                        yield return new WaitUntil(() => ReachedDestinationOrGaveUp());

                        Move(m_targetPoint.transform.position, true);
                    }

                    yield return new WaitUntil(() => ReachedDestinationOrGaveUp());
                    //Debug.Log("Done to target point");
                    m_navMeshAgent.enabled = false;
                    m_animator.SetBool("Run", false);

                  
                    if (m_targetPoint.IsPayPoint)
                    {
                        CurrentState = ECustomerState.Buying;
                    }
                    else
                        CurrentState = ECustomerState.WaitingInLine;

                    break;
                case ECustomerState.SortInLine:

                    yield return null;
                    Move(m_targetPoint.transform.position, true);
                    yield return new WaitUntil(() => ReachedDestinationOrGaveUp());
                    //Debug.Log("Done to target point");
                    m_navMeshAgent.enabled = false;
                    m_animator.SetBool("Run", false);

                    if (m_targetPoint.IsPayPoint)
                    {
                        waitimeResetTask = 0;
                        CurrentState = ECustomerState.Buying;
                    }
                    else
                        CurrentState = ECustomerState.WaitingInLine;
                    break;

                case ECustomerState.WaitingInLine:
                    //Debug.Log("WaitingInLine: " + gameObject.name);
                    //yield return new WaitUntil(() => m_targetPoint != null);
                    //yield return new WaitUntil(() => m_targetPoint.IsPayPoint);
                    //CurrentState = ECustomerState.Buying;
                    //yield return new WaitForSeconds(1f);

                    break;
                case ECustomerState.Buying:
                    //Debug.Log("Buying: " + gameObject.name);
                    MapController.Instance.orderBalloons.balloonCustomer.ShowBalloon(CustomerPickup.productLeft, trBalloon);
                    waitimeResetTask += 0.25f;
                    if (waitimeResetTask >= 5)
                    {
                        //Debug.LogError("Reset couternode task");
                        MapController.Instance.counterNode.ResetTask();
                        waitimeResetTask = 0;
                    }
                    yield return null;

                    yield return new WaitUntil(() => MapController.Instance.counterNode.isPay());
                    while (!CustomerPickup.IsMaxItem())
                    {
                        if(MapController.Instance.counterNode.m_UnitData.crLevel>1)
                            yield return new WaitForSeconds(MapController.Instance.counterNode.speed);

                        yield return new WaitUntil(() => MapController.Instance.counterNode._counterStorage.ProductUnits.Count > 0);
                        counterStorage.Action_GetProduct(CustomerPickup);
                        yield return null;

                        MapController.Instance.orderBalloons.balloonCustomer.ShowBalloon(CustomerPickup.productLeft, trBalloon);
                        m_animator.SetInteger("Carry", CustomerPickup.ProductUnits.Count);
                    }



                    CurrentState = ECustomerState.WaitingCheckOut;
                    MapController.Instance.CompleteTut("tut_08");
                    MapController.Instance.orderBalloons.balloonCustomer.HideBalloon();
                    Transform trMoneyArea = MapController.Instance.counterNode.moneyArea();
                    if (trMoneyArea.childCount > 0)
                    {
                        try
                        {
                            CashBox _cashbox = trMoneyArea.GetComponentsInChildren<CashBox>().First(x => !x.isPickingCash);
                            if (_cashbox != null)
                            {
                                _cashbox.AddOn(cashValue);
                            }
                            else
                            {
                                GameObject g = Instantiate(cashBox.gameObject, trMoneyArea.position, Quaternion.identity);
                                g.transform.SetParent(trMoneyArea);

                                _cashbox = g.GetComponent<CashBox>();
                                _cashbox.AddOn(cashValue);
                            }
                        }
                        catch /*(Exception e)*/
                        {
                            //Debug.LogError(e.Message);
                            continue;
                        }

                    }
                    else
                    {
                        GameObject g = Instantiate(cashBox.gameObject, trMoneyArea.position, Quaternion.identity);
                        g.transform.SetParent(trMoneyArea);

                        CashBox _cashbox = g.GetComponent<CashBox>();
                        _cashbox.AddOn(cashValue);
                    }
                    SoundManager.Instance.PlayOnCamera(2);
                  
                    yield return null;
                   

                    break;
                case ECustomerState.WaitingCheckOut:
                    //Debug.Log("WaitingCheckOut: " + gameObject.name);
                    yield return null;
                    yield return null;
                    yield return null;
                    seatPoint = null;
                    while (seatPoint == null)
                    {
                        if (!Module.isYoutuberSeat)
                            seatPoint = MapController.Instance.GetSeatPointFree();
                        else
                        {
                            seatPoint = null;
                        }

                        objTableCleanBalloon.SetActive(seatPoint == null);
                        yield return null;
                        yield return null;
                        yield return null;
                        
                    }

                    seatPoint.SetCustomer(this);
                    transform.SetParent(seatPoint.transform);
                    objTableCleanBalloon.SetActive(false);
                    MapController.Instance.RemoveCustomer(this);
                    m_targetPoint.IsFilled = false;
                    m_targetPoint = null;
                    CurrentState = ECustomerState.CheckingOut;

                    yield return null;
                    break;
                case ECustomerState.CheckingOut:
                    //Debug.Log("CheckingOut: " + gameObject.name);
                    //Free slot
                    DOVirtual.DelayedCall(0.5f, () => MapController.Instance.SortLineupCustomer());
                    CurrentState = ECustomerState.GoToSeat;
                    yield return null;
           
                    break;
                case ECustomerState.GoToSeat:
                    //Debug.Log("GoToSeat: " + gameObject.name);

                    Move(seatPoint.transform.position, true);
                    yield return new WaitUntil(() => ReachedDestinationOrGaveUp());
                    m_animator.SetBool("isEat", true);
                    m_navMeshAgent.enabled = false;
                    transform.LookAt(seatPoint.LookAtTransform);

                    transform.SetParent(seatPoint.transform);
                    transform.DOLocalMove(Vector3.zero,0.1f);
                    transform.eulerAngles = new Vector3(0,transform.eulerAngles.y, transform.eulerAngles.z);
                    //m_customerPickup.tray_Customer.transform.SetParent(seatPoint.tableArea.tableStorage.transform);
                    //m_customerPickup.tray_Customer.transform.localPosition = Vector3.zero;

                    while (m_customerPickup.ProductUnits.Count > 0)
                    {
                        ProductUnit product = m_customerPickup.ProductUnits[0];
                        seatPoint.tableArea.tableStorage.AddProduct(product);
                        m_customerPickup.RemoveProduct(product);
                        yield return new WaitForSeconds(0.1f);
                    }


                    yield return new WaitForSeconds(0.5f);
                    seatPoint.isEating = true;

                    yield return new WaitUntil(() => seatPoint.tableArea.isFullSeatEating());
                    yield return null;

                    CurrentState = ECustomerState.Eating;
                    seatPoint.tableArea.piecesGroup._parent.gameObject.SetActive(false);
                    break;
                case ECustomerState.Eating:
                    Debug.Log("Eating: " + gameObject.name);  
                    objEatBalloon.SetActive(true);
                    yield return new WaitForSeconds(Module.EasyRandom(0.3f,0.5f));
                    seatPoint.tableArea.tableStorage.Eating();
                    //StartCoroutine(CustomerPickup.IEatting());
                    yield return new WaitUntil(() => seatPoint.tableArea.tableStorage.isDoneEat);
                    seatPoint.isEating = false;

                    //yield return new WaitUntil(() => seatPoint.tableArea.isDoneEat());
                    objEatBalloon.SetActive(false);
                    //trash here
                    seatPoint.tableArea.piecesGroup._parent.gameObject.SetActive(true);
                    MapController.Instance.mapData.ShowTut("tut_09");


                    yield return null;
                    seatPoint.tableArea.ResetTask();
                    seatPoint.tableArea.SetAnimChairOff();

                    
                    yield return new WaitForSeconds(0.1f);

                    transform.SetParent(null);
                    m_customerPickup.ResetTray();
                    m_animator.SetBool("isEat", false);
                    m_animator.SetInteger("Carry", 0);
                    if (MapController.Instance.isHaveTakeAway)
                    {
                        if (Module.EasyRandom(100) < GameManager.Instance.m_DataConfigRemote.rateJuice)
                        {
                            CurrentState = ECustomerState.TakeAway;
                        }
                        else
                        {
                            CurrentState = ECustomerState.Exiting;
                        }
                    }
                    else
                    {
                        CurrentState = ECustomerState.Exiting;
                    }

                    


                    break;
                case ECustomerState.TakeAway:
       
                    Move(juice_Storage.transform.position, true);
                    yield return new WaitUntil(() => ReachedDestinationOrGaveUp());

                    m_customerPickup.Action_TakeAway(juice_Storage.transform);
                    yield return null;
                   
                    yield return new WaitUntil(() => m_customerPickup.ProductUnits.Count>0);
                    m_animator.SetInteger("Carry", CustomerPickup.ProductUnits.Count);
                    MapController.Instance.juice_Machine.Action_MoneyArea();

                    yield return new WaitForSeconds(0.2f);
                    CurrentState = ECustomerState.Exiting;

                    break;

                case ECustomerState.Exiting:
                    
                    //Debug.Log("Exiting: " + gameObject.name);
                    Move(MapController.Instance.customerPath.trPath3.position, true);
                    yield return new WaitUntil(() => ReachedDestinationOrGaveUp());
                    Debug.Log("End " + gameObject.name);
                    CurrentState = ECustomerState.End;
                   
                    SimplePool.Despawn(this.gameObject);
                    break;
                default:
                    break;
            }


            yield return null;
            yield return null;
            yield return null;
        }
    }


    public bool ReachedDestinationOrGaveUp()
    {
        if (!m_navMeshAgent.pathPending)
        {
            if (m_navMeshAgent.remainingDistance <= m_navMeshAgent.stoppingDistance)
            {
                if (!m_navMeshAgent.hasPath || m_navMeshAgent.velocity.sqrMagnitude == 0f)
                    return true;
            }
        }
        return false;
    }


    public void Move(Vector3 targetPos,bool _isNav =false)
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

    public void GetPayedProduct(GameObject box, Action oncompleted)
    {
        m_trolley.gameObject.SetActive(false);
        box.transform.SetParent(m_posBox);
        box.transform.DOLocalJump(Vector3.zero, 1, 1, 0.4f)
            .OnComplete(() =>
            {
                oncompleted.Invoke();
                CurrentState = ECustomerState.Exiting;
            });
    }
}
