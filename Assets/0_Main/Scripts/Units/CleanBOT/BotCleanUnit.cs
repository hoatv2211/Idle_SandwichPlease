using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EBotState
{
    Off,
    Waiting,
    DeliveryOuput,
    Sleep
}

[RequireComponent(typeof(NavMeshAgent))]
public class BotCleanUnit : UnitBase
{
    [Space, Header("UNIT - MOVING")]
    [SerializeField] private EBotState _botState;
    [SerializeField] protected NavMeshAgent m_navMeshAgent;
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _fxSleep;

    public WorkerSlot TargetSlot;
    public Transform baseClean;
    public GameObject objBattery;
    [SerializeField] private float speed=0.1f;
    [SerializeField] private float jumpForce=2;
    public EBotState EBotState
    {
        get { return _botState; }
        set
        {
            _botState = value;
        }
    }

    private void OnEnable()
    {
        Module.Event_CleanBot += Module_Event_CleanBot;
        _fxSleep?.SetActive(true);
        _animator?.SetBool("isRun", false);
        _animator?.SetBool("isSleep", true);
    }

    private void Module_Event_CleanBot(int _timeLeft)
    {
        if (_timeLeft > 0)
        {
            CallStart();
        }
    }

    private void OnDisable()
    {
        Module.Event_CleanBot -= Module_Event_CleanBot;
    }


    public override void CallStart()
    {
        //Debug.LogError("Waiting");
        EBotState = EBotState.Waiting;
        if (coroutine != null)
            StopCoroutine(coroutine);
        coroutine = StartCoroutine(IeWorkTask());
    }

    public TableCreateArea _tableCr;
    Coroutine coroutine;
    IEnumerator IeWorkTask()
    {
        yield return null;

        _animator?.SetBool("Run", false);
        _animator?.SetBool("isSleep", false);
        objBattery?.SetActive(false);
        
        while (Module.timeCleanBot>0)
        {
            switch (EBotState)
            {
                case EBotState.Off:

                    break;
                case EBotState.Waiting:
                    m_navMeshAgent.enabled = false;
                    yield return null;
                    if (MapController.Instance.tableCreateAreas.Find(x => x.stage == ETableState.GotTrash) == null)
                    {
                        EBotState = EBotState.Sleep;
                    }
                    else
                    {
                        yield return new WaitUntil(() => MapController.Instance.tableCreateAreas.Find(x => x.stage == ETableState.GotTrash) != null);
                       
                        _tableCr = MapController.Instance.tableCreateAreas.Find(x => x.stage == ETableState.GotTrash);
                        Move(_tableCr.piecesGroup.transform.position, true);
                        EBotState = EBotState.DeliveryOuput;
                    }
                   
                    break;
                case EBotState.DeliveryOuput:
                    _animator?.SetBool("isSleep", false);
                    _fxSleep?.SetActive(false);

                    
                    while(_tableCr.stage == ETableState.GotTrash)
                    {
                        if (Vector3.Distance(transform.position, _tableCr.piecesGroup.transform.position) > 0.1f)
                        {
                            Move(_tableCr.piecesGroup.transform.position, true);
                        }
                        yield return null;
                        //if(!ReachedDestinationOrGaveUp())
                        //    Move(_tableCr.piecesGroup.transform.position, true);

                    }
                    yield return new WaitUntil(() => _tableCr.stage!=ETableState.GotTrash);
                    m_navMeshAgent.enabled = false;
                    _tableCr = null;

                    yield return null;
                    yield return null;
                    yield return null;
                    yield return new WaitForSeconds(1f);
                    EBotState = EBotState.Waiting;

                    break;
                case EBotState.Sleep:
                    Move(baseClean.position, true);

                    yield return new WaitUntil(() => ReachedDestinationOrGaveUp());
                    m_navMeshAgent.enabled = false;
                    //_fxSleep?.SetActive(true);
                    _animator?.SetBool("isRun", false);
                    //_animator?.SetBool("isSleep", true);
                    transform.DOLocalRotate(new Vector3(0, 90, 0), 0.2f);

                    yield return new WaitForSeconds(2f);
                    EBotState = EBotState.Waiting;

                    break;
                default:
                    break;
            }

            yield return null;
          
        }

        yield return null;
        //Off
        Move(baseClean.position, true);
        yield return new WaitUntil(() => ReachedDestinationOrGaveUp());
        m_navMeshAgent.enabled = false;
        _fxSleep?.SetActive(true);
        _animator?.SetBool("isRun", false);
        _animator?.SetBool("isSleep", true);
        transform.DOLocalRotate(new Vector3(0, 90, 0), 0.2f);
        objBattery?.SetActive(true);

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
        m_navMeshAgent.SetDestination(targetPos);
        _animator.SetBool("isRun", true);

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

    bool _isCountdown = false;
    protected void CountdownForNextPick()
    {
        _isCountdown = true;
        DOVirtual.DelayedCall(0.1f, () => _isCountdown = false);
    }

    public bool PickUp(ProductUnit productUnit)
    {
        if (_isCountdown)
        {
            return false;
        }
        CountdownForNextPick();
        _animator?.SetBool("isClean", true);
      

        productUnit.transform.SetParent(this.transform);
        productUnit.IsMoving = true;
        productUnit.transform.DOLocalRotate(Vector3.zero, speed);
        productUnit.transform.DOLocalJump(Vector3.zero, jumpForce, 1, speed).OnComplete(()=> { SimplePool.Despawn(productUnit.gameObject); _animator?.SetBool("isClean", false); });
        
        return true;
    }
}
