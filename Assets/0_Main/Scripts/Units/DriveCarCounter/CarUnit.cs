using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using System.Linq;
using System;

public class CarUnit : UnitBase
{
    public CustomerData m_CustomerData;
    [Space, Header("UNIT - MOVING")]
    [SerializeField] protected NavMeshAgent m_navMeshAgent;

    [Space, Header("UNIT - PICKUP AND DROP")]
    [SerializeField] private CustomerPickup m_customerPickup;
    public CustomerPickup CustomerPickup => m_customerPickup;

    [Space, Header("CUSTOMER TROLLEY")]
    [SerializeField] private Animator m_animator;
    [SerializeField] private List<GameObject> m_listSkins;

    [Space, Header("Balloons")]
    [SerializeField] private Transform trBalloon;
    [SerializeField] private GameObject objSleepBalloon;
    [SerializeField] private GameObject objAngryBalloon;
    [SerializeField] private GameObject objCheckout;
    [SerializeField] private CashBox cashBox;


    [Space, Header("Hide Late")]
    [SerializeField] private CarPoint m_targetPoint;
    [SerializeField] private EProductType m_productType;
    [SerializeField] private ECustomerState m_customerState;
    //private int cashValue => m_CustomerData.isVip? CustomerPickup.ProductUnits.Count*45: CustomerPickup.ProductUnits.Count * 30;
    private int cashValue
    {
        get
        {
            int _value = m_CustomerData.isVip ? CustomerPickup.ProductUnits.Count * 45 : CustomerPickup.ProductUnits.Count * 30;
            _value += (int)GameManager.Instance.skinModel.infoSkin.single_price_bonus;

            return _value;
        }
    }

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

        if (MapController.Instance.m_CarUnits.Count <= 1)
        {
            m_CustomerData.stackProduct = 1;
        }

        //Random skin
        int indexSkin = Module.EasyRandom(0, m_listSkins.Count);
        for(int i = 0; i < m_listSkins.Count; i++)
        {
            if(i == indexSkin)
            {
                m_listSkins[i].SetActive(true);
            }
            else
            {
                m_listSkins[i].SetActive(false);
            }
        }

        List<int> _indexVip = new List<int> { 3,4,5 };
        if (_indexVip.Contains(indexSkin))
            m_CustomerData.isVip = true;
        else
            m_CustomerData.isVip = false;

        m_customerPickup.CallStart(m_CustomerData);
        SoundManager.Instance.PlayOnCamera(3);
        // transform.DOLocalMoveY(0, 0.1f);
    }

    public void InitTargetPoint(CarPoint _target)
    {
        m_targetPoint = _target;
        _target.IsFilled = true;
    }

    public void SetNewTarget(CarPoint _target)
    {
        m_targetPoint.IsFilled = false;
        m_targetPoint = _target;
        _target.IsFilled = true;
        if (_corScanNextCustomerPoint != null)
        {
            StopCoroutine(_corScanNextCustomerPoint);
            _corScanNextCustomerPoint = null;
        }
        CurrentState = ECustomerState.SortInLine;
        _corScanNextCustomerPoint = StartCoroutine(IeScanNextCustomerPoint());
    }

    CounterStorage packageStorage => MapController.Instance.driveCarStorage;
    Coroutine _corScanNextCustomerPoint;
    private IEnumerator IeScanNextCustomerPoint()
    {

        while (CurrentState!=ECustomerState.End)
        {
            switch (CurrentState)
            {
                case ECustomerState.Move:
                    if (Vector3.Distance(this.transform.position, MapController.Instance.drivePath.trPath1.position) >
                        Vector3.Distance(this.transform.position, m_targetPoint.transform.position))
                    {
                        Move(m_targetPoint.transform.position, true);
                    }
                    else
                    {
                        Move(MapController.Instance.drivePath.trPath1.position, true);
                        yield return new WaitUntil(() => ReachedDestinationOrGaveUp());

                        Move(m_targetPoint.transform.position, true);
                    }
                    yield return new WaitUntil(() => ReachedDestinationOrGaveUp());
                    m_navMeshAgent.enabled = false;
                    if (m_targetPoint.LookAtTransform != null)
                    {
                        this.transform.LookAt(m_targetPoint.LookAtTransform);
                        Vector3 eulerAngles = this.transform.eulerAngles;
                        eulerAngles.x = 0;
                        eulerAngles.z = 0;
                        this.transform.eulerAngles = eulerAngles;
                        //_isWaitingLook = false;
                    }
                   

                    if (m_targetPoint.IsPayPoint)
                    {
                        CurrentState = ECustomerState.Buying;
                        SoundManager.Instance.PlayOnCamera(3);
                    }  
                    else
                        CurrentState = ECustomerState.WaitingInLine;
                    yield return null;
                    break;
                case ECustomerState.WaitingInLine:
                    //Debug.Log("WaitingInLine: " + gameObject.name);
                    //yield return new WaitForSeconds(0.5f);
                    break;
                case ECustomerState.SortInLine:
                    Move(m_targetPoint.transform.position, true);
                    yield return new WaitUntil(() => ReachedDestinationOrGaveUp());
                    Debug.Log("Done to target point");
                    m_navMeshAgent.enabled = false;

                    if (m_targetPoint.LookAtTransform != null)
                    {
                        this.transform.LookAt(m_targetPoint.LookAtTransform);
                        Vector3 eulerAngles = this.transform.eulerAngles;
                        eulerAngles.x = 0;
                        eulerAngles.z = 0;
                        this.transform.eulerAngles = eulerAngles;
                        //_isWaitingLook = false;
                    }

                    if (m_targetPoint.IsPayPoint)
                    {
                        CurrentState = ECustomerState.Buying;
                        SoundManager.Instance.PlayOnCamera(3);
                    }   
                    else
                        CurrentState = ECustomerState.WaitingInLine;
                    
                    break;
                case ECustomerState.Buying:
                    Debug.Log("Buying: " + gameObject.name);
                    

                    MapController.Instance.orderBalloons.balloonCar.ShowBalloon(CustomerPickup.productLeft, trBalloon);
                    yield return new WaitUntil(() => MapController.Instance.driveCarCounterNode.isPay());

                    while (!CustomerPickup.IsMaxItem())
                    {                    
                        if (MapController.Instance.counterNode.m_UnitData.crLevel > 1)
                            yield return new WaitForSeconds(MapController.Instance.counterNode.speed);

                        yield return new WaitUntil(() => MapController.Instance.driveCarCounterNode.m_Storage.ProductUnits.Count > 0);
                        packageStorage.Action_GetProduct(CustomerPickup);
                        yield return null;

                        MapController.Instance.orderBalloons.balloonCar.ShowBalloon(CustomerPickup.productLeft, trBalloon);
                        yield return null;
                    }

                    CurrentState = ECustomerState.CheckingOut;
                    MapController.Instance.CompleteTut("tut_21");
                    MapController.Instance.orderBalloons.balloonCar.HideBalloon();
                    Transform trMoneyArea = MapController.Instance.driveCarCounterNode.moneyArea();
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
                        catch (Exception e)
                        {
                            Debug.LogError(e.Message);
                        }
                    }
                    else
                    {
                        GameObject g = SimplePool.Spawn(cashBox.gameObject, trMoneyArea.position, Quaternion.identity);
                        g.transform.SetParent(trMoneyArea);

                        CashBox _cashbox = g.GetComponent<CashBox>();
                        _cashbox.AddOn(cashValue);
                    }

                    SoundManager.Instance.PlayOnCamera(2);

                    yield return null;
                    yield return null;
                    yield return null;
                    break;
                case ECustomerState.CheckingOut:
                    //Free slot
                    yield return null;
                    m_targetPoint.IsFilled = false;
                    m_targetPoint = null;
                    MapController.Instance.RemoveCar(this);

                    CurrentState = ECustomerState.Exiting;
                    Move(MapController.Instance.drivePath.trPath3.position, true);
                    yield return new WaitForSeconds(1f);
                    MapController.Instance.SortLineupCar();
                  
                    break;
                case ECustomerState.Exiting:
                    Debug.Log("Exiting: " + gameObject.name);
                    yield return null;
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
        if (m_navMeshAgent.enabled == false)
            return false;

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
        //m_animator.SetBool("Run", true);
    }

    public void Checkout()
    {
        CurrentState = ECustomerState.CheckingOut;
    }

}
