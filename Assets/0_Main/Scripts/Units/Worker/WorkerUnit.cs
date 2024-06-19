using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EWorkerState
{
    Waiting,
    GetInput,
    DeliveryOuput,
    Gaveup,
    Sleep
}

[RequireComponent(typeof(NavMeshAgent))]
public class WorkerUnit : UnitBase
{
    [SerializeField] private EWorkerType workerType;
    [SerializeField] private EWorkerState _workerState;

    public WorkerData m_WorkerData => MapController.Instance.mapData.workerData;
    [Space, Header("UNIT - MOVING")]
    [SerializeField] protected NavMeshAgent m_navMeshAgent;
    [SerializeField] private GameObject _fxRunSmoke;

    [Space, Header("CUSTOMER SKINs")]
    [SerializeField] private Animator _animator;
    [SerializeField] private List<GameObject> m_listSkins;
    [SerializeField] private GameObject[] objTrays;

    [Space, Header("CUSTOMER UNIT - PICKUP AND DROP")]
    [SerializeField] protected WorkerPickUp _workerPickup;
    public WorkerTask workerTask;
    public WorkerPickUp WorkerPickUp => _workerPickup;
    [SerializeField] protected float _maxWaitime = 5f;

    [SerializeField] private GameObject _fxSleep;
  
    public EWorkerState EWorkerState
    {
        get { return _workerState; }
        set
        {
            _workerState = value;
        }
    }

    protected WorkerSlot _targetSlot;
    public WorkerSlot TargetSlot => _targetSlot;
    public bool _isDoneTask = false;
    public bool IsDoneTask => _workerPickup.ProductUnits.Count >= m_WorkerData.CarryStack || _isDoneTask;


    public override void CallStart()
    {
        EWorkerState = EWorkerState.Waiting;
        //tray
        objTrays[0].SetActive(MapController.Instance._idMap == 1);
        objTrays[1].SetActive(MapController.Instance._idMap == 2);

        StartCoroutine(IeWorkTask());
    }

    private void LateUpdate()
    {
        if(EWorkerState== EWorkerState.GetInput&& ReachedDestinationOrGaveUp())
        {
            _currentWaitTime += Time.deltaTime;
            if (_currentWaitTime > 6)
            {
                //Debug.LogError("xxxxxxxx");
                _currentWaitTime = 0;
                if (_workerPickup.ProductUnits.Count > 0)
                {
                    StopCoroutine(IeWorkTask());
                    EWorkerState = EWorkerState.DeliveryOuput;
                    StartCoroutine(IeWorkTask());
                }
                else
                {
                    StopCoroutine(IeWorkTask());
                    EWorkerState = EWorkerState.Waiting;
                    StartCoroutine(IeWorkTask());
                }

             
            }

           
        }
    }

    protected bool _isWaitingReachedPoint = false;
    protected float _currentWaitTime = 0;
    EWorkerState _crState = EWorkerState.Waiting;
    IEnumerator IeWorkTask()
    {
        yield return new WaitForSeconds(Module.EasyRandom(2f,5f));
        _currentWaitTime = 0;
        _animator?.SetBool("Run", false);
        while (true)
        {

            switch (EWorkerState)
            {
                case EWorkerState.Waiting:
                
                    if (WorkerPickUp.ProductUnits.Count > 0)
                    {
                        EWorkerState = EWorkerState.DeliveryOuput;
                        continue;
                    }

                    //Clear
                    m_navMeshAgent.enabled = false;
                    _animator?.SetBool("Run", false);
                    workerTask = null;
                    _isDoneTask = false;

                    switch (Module.EasyRandom(0,1))
                    {
                        case 1:
                            _animator?.SetBool("isWelcome", true);
                            _fxSleep?.SetActive(false);
                            break;
                        default:
                            _animator?.SetBool("isWelcome", false);
                            _fxSleep?.SetActive(true);
                            break;
                    }

                
                    yield return new WaitForSeconds(0.5f);
                    workerTask = MapController.Instance.GetTaskWorker();
                  
                    yield return null;
                    //yield return new WaitUntil(() => MapController.Instance.workerTasks.Count>0);

                    //workerTask = MapController.Instance.PickTask();

                    if (workerTask != null)
                    {
                        workerTask.workerUnit = this;
                        _animator?.SetBool("isWelcome", false);
                        _fxSleep?.SetActive(false);
                        if (workerTask.typeWorkerTask == EWorkerTask.None)
                            continue;

                        //workerTask.inputSlot = MapController.Instance.GetWorkerSlot_Product(workerTask.productType);
                        //workerTask.outputSlot = MapController.Instance.GetWorkerSlot_Ouput(workerTask.productType);

                        try
                        {
                            _inputWorkerSlot = workerTask.inputSlot ? workerTask.inputSlot : null;
                            _outputWorkerSlot = workerTask.outputSlot ? workerTask.outputSlot : null;
                        }
                        catch
                        {
                            Debug.LogError(name);
                        }
                    
                        //_targetSlot = workerTask.targetSlot;
                        EWorkerState = EWorkerState.GetInput;
                    }
                  

                    break;
                case EWorkerState.GetInput:
                    _fxSleep?.SetActive(false);
                    //workerTask.inputSlot = MapController.Instance.GetWorkerSlot_Product(workerTask.productType);
                    //Debug.Log(name + "GetInput");
                    //_workerPickup.productType = workerTask.productType;
                    if (workerTask == null)
                    {
                        EWorkerState = EWorkerState.Sleep;
                        continue;
                    }

                    while (workerTask.inputSlot == null)
                    {
                        workerTask.inputSlot = MapController.Instance.GetWorkerSlot_Product(workerTask.productType);
                        yield return null;
                    }
                    


                    //yield return new WaitUntil(() => workerTask.inputSlot != null);
                    Move(workerTask.inputSlot.transform.position, true);
                    yield return new WaitUntil(() => ReachedDestinationOrGaveUp());
                    //Get items
                    _animator?.SetBool("Run", false);
                    yield return null;

                    yield return new WaitUntil(() => IsDoneTask);
                    EWorkerState = EWorkerState.DeliveryOuput;


                    yield return null;
                    //GetNewEndpointTask();
                    break;
                case EWorkerState.DeliveryOuput:
                    _fxSleep?.SetActive(false);
                    //Debug.Log(name + "DeliveryOuput");


                    if (WorkerPickUp.ProductUnits.Count > 0)
                    {
                        if (workerTask == null)
                        {
                            workerTask = new WorkerTask();
                            workerTask.productType = WorkerPickUp.ProductUnits[0].typeProduct;
                            workerTask.typeWorkerTask = MapController.Instance.GetWorkerTaskType(workerTask.productType);
                            workerTask.outputSlot = MapController.Instance.GetWorkerSlot_Ouput(workerTask.productType);
                        }
                        else
                        {
                            if(workerTask.outputSlot==null)
                                workerTask.outputSlot = MapController.Instance.GetWorkerSlot_Ouput(workerTask.productType);
                        }
                     
                    }
                    else
                    {
                        //Debug.LogError(name + "workerTask");
                        EWorkerState = EWorkerState.Waiting;
                        continue;
                    }

                    if (workerTask == null)
                        continue;

                    while (workerTask.outputSlot == null)
                    {
                        workerTask.outputSlot = MapController.Instance.GetWorkerSlot_Ouput(workerTask.productType);
                        yield return null;
                    }

                    yield return null;
                    Move(workerTask.outputSlot.transform.position, true);
                    yield return new WaitUntil(() => ReachedDestinationOrGaveUp());
                  
                    _animator.SetBool("Run", false);

                    //Debug.Log("endSlot");
                    yield return null;
                    m_navMeshAgent.enabled = false;
                    yield return new WaitForSeconds(1f);

                    if (_workerPickup.ProductUnits.Count > 0)
                    {
                        //workerTask.outputSlot = MapController.Instance.GetWorkerSlot_Ouput(_workerPickup.ProductUnits[0].typeProduct);
                        EWorkerState = EWorkerState.DeliveryOuput;
                    }
                    else
                    {
                        EWorkerState = EWorkerState.Waiting;
                    }

                    //Reset
                    if (workerTask != null)
                    {
                        workerTask.taskWorker?.ResetTask();
                        workerTask.workerUnit = null;
                        workerTask = null;

                    }


                    break;
                case EWorkerState.Gaveup:
                    _fxSleep?.SetActive(false);
                    //GetNewTrashBin();
                    Move(MapController.Instance.m_trashBinUnits[0].workerSlot.position, true);
                    yield return new WaitUntil(() => ReachedDestinationOrGaveUp());
                    m_navMeshAgent.enabled = false;
                    _animator?.SetBool("Run", false);

                    if (workerTask != null)
                    {
                        workerTask.taskWorker?.ResetTask();
                        workerTask.workerUnit = null;
                        workerTask = null;

                    }

                    yield return new WaitForSeconds(Module.EasyRandom(0f, 1f));
                    EWorkerState = EWorkerState.Waiting;
                    break;
                case EWorkerState.Sleep:
                    //yield return new WaitUntil(() => ReachedDestinationOrGaveUp());
                    _fxSleep?.SetActive(true);
                    _animator?.SetBool("Run", false);
                    _animator?.SetBool("isSleep", true);
                    yield return new WaitForSeconds(1);
                    _animator?.SetBool("isSleep", false);
                    EWorkerState = EWorkerState.Waiting;
                    break;
                default:
                    break;
            }

            yield return null;
        
        }

    }

    public virtual void GetNewTask()
    {
        
    }
    public virtual void GetNewEndpointTask()
    {
        if (_workerPickup.ProductUnits.Count <= 0)
        {
            _workerState = EWorkerState.Waiting;
            return;
        }
     
    }

    public virtual void CheckTrigger(Collider other)
    {
        //if (!_targetSlot)
        //{
        //    return;
        //}
        //if (!ReachedDestinationOrGaveUp())
        //{
        //    return;
        //}
        BasePickAndDrop pickupAndDropBase = other.GetComponent<BasePickAndDrop>();
        if (pickupAndDropBase != null)
        {
            if (pickupAndDropBase is CounterPickup
            || pickupAndDropBase is PackagePickup)
            {
                for (int i = _workerPickup.ProductUnits.Count - 1; i >= 0; i--)
                {
                    if (_workerPickup.ProductUnits[i].IsMoving)
                    {
                        continue;
                    }

                    bool check = pickupAndDropBase.PickUp(_workerPickup.ProductUnits[i], m_WorkerData.CarryStack);
                    if (check)
                    {
                        _workerPickup.RemoveProduct(_workerPickup.ProductUnits[i]);
                        if (_workerPickup.ProductUnits.Count <= 0)
                        {
                            // Debug.Log("Done Task");
                            //_workerState = EWorkerState.Waiting;
                            ResetTargetSlot();
                            return;
                        }
                    }
                    else
                    {
                        if (!pickupAndDropBase.IsCountdown)
                        {
                            if (_workerPickup.ProductUnits.Count > 0)
                            {
                                //_workerState = EWorkerState.Gaveup;
                                ResetTargetSlot();
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        //CheckTrigger(other);
    }
    public virtual bool Pickup(ProductUnit productUnit, int maxQuantity)
    {
       // Debug.LogError(productUnit.unitName);

        bool check = _workerPickup.PickUp(productUnit, maxQuantity);
        if (IsDoneTask)
        {
            ResetTargetSlot();
        }

        return check;
    }

    public void ResetTargetSlot()
    {
        if (_targetSlot != null)
        {
            _targetSlot.IsFilled = false;
            _targetSlot = null;
        }
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
        _fxSleep?.SetActive(false);
        m_navMeshAgent.speed = m_WorkerData.Speed;
        m_navMeshAgent.SetDestination(targetPos);
        _animator.SetBool("Run", true);
        _animator.speed = 1 + m_navMeshAgent.speed/10f;
        _animator.SetFloat("speedRun", m_navMeshAgent.speed);
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

    public void CarryState(int _value = 0)
    {
        _animator.SetInteger("Carry", _value);
    }
}

[System.Serializable]
public class WorkerData : UnitData
{
    //
    public float Speed => MapController.Instance.mapData.upgradeStaff.crSpeed().value + GameManager.Instance.skinModel.infoSkin.staff_speed;
    public int CarryStack => (int)MapController.Instance.mapData.upgradeStaff.crCapacity().value + (int)GameManager.Instance.skinModel.infoSkin.staff_capacity;
}