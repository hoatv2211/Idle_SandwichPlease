using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

interface ICashier
{
    Transform moneyArea();
    bool isPay();
    void Action_Pay();
}

/// <summary>
/// là nơi bán Sandwich cho khách ăn tại chỗ
/// </summary>
public class CounterNode : UnitBase,ICashier,ITaskWorker
{
    [Header("Machine")]
    public UnitDataBase unitDataBase;

    [Header("BUILDING")]
    [Tooltip("Unlock")]
    [SerializeField] private List<GameObject> listLocked;
    [SerializeField] private List<GameObject> listUnlocked;

    [Space,Tooltip("Upgrade")]
    [SerializeField] private List<GameObject> OnUpgraded;
    [SerializeField] private List<GameObject> OffUpgraded;
    [SerializeField] private BoardNode boardNode;

    [Header("UseLate")]
    [SerializeField] private List<UnlockLevel> unlockLevels;

    [Header("Fxz")]
    [SerializeField] private ParticleSystem fxUnlock;
    [SerializeField] private ParticleSystem fxUpgrade;
    [SerializeField] private Animator _animator;
    public GameObject objMax;
    public GameObject objEmpty;


    [Header("Products")]
    public CounterPickup _counterPickup;
    public CounterStorage _counterStorage;
    [SerializeField] private Transform _moneyArea;

    [Space, Header("COUNTER UNIT - CUSTOMER")]
    public CustomerPointCtrl _customerPointCtrl;

    public float speed => unitDataBase.Speed(m_UnitData.crLevel);

    public override void CallStart()
    {
        //Debug.LogError("CounterNode \n" + JsonUtility.ToJson(m_UnitData));

        if (m_UnitData.isUnlocked)
        {
            Action_Show(true);

            if (m_UnitData.moneySave > 0)
            {
                GameObject g = Instantiate(AssetConfigs.Instance.cashBox.gameObject, _moneyArea.position, Quaternion.identity);
                g.transform.SetParent(_moneyArea);

                CashBox _cashbox = g.GetComponent<CashBox>();
                _cashbox.AddOn(m_UnitData.moneySave);
            }
        }

        if (m_UnitData.crLevel > 1)
        {
            foreach (var k in unlockLevels[2].listOns)
            {
                k.SetActive(true);
            }

            foreach (var k in unlockLevels[2].listOffs)
            {
                k.SetActive(false);
            }
        }

        unitDataBase = GameManager.Instance.mapModel.GetUnitDataBase(IdUnit);

       
    }

    private void Action_Show(bool _isUnlock = false)
    {
        foreach (var k in listLocked)
        {
            k.SetActive(!_isUnlock);
        }

        foreach (var k in listUnlocked)
        {
            k.SetActive(_isUnlock);
        }

        if (_isUnlock)
        {
            m_UnitData.isUnlocked = true;
           
            DOVirtual.DelayedCall(0.1f, () => 
            {
                MapController.Instance.customerPoints.Clear();
                foreach (var k in _customerPointCtrl._customerPoints)
                {
                    if (!MapController.Instance.customerPoints.Contains(k))
                         MapController.Instance.customerPoints.Add(k);
                }

                InitTask();
            });
           
           
        }
    }


    public override void Action_UnlockUnit()
    {
        base.Action_UnlockUnit();
        Action_Show(true);

        if (fxUnlock != null)
        {
            fxUnlock.gameObject.SetActive(true);
            fxUnlock.Play(true);
        }
    }

    public override void Action_UpgradeUnit()
    {
        //m_UnitData.crLevel++;
        if (fxUpgrade != null)
        {
            fxUpgrade.gameObject.SetActive(true);
            fxUpgrade.Play(true);
        }

        foreach (var k in OnUpgraded)
        {
            k.SetActive(true);
        }

        foreach (var k in OffUpgraded)
        {
            k.SetActive(false);
        }

        m_UnitData.crLevel++;
        UnitData unitData = MapController.Instance.mapData.GetUnlockUnitData(IdUnit);
        if(unitData!=null)
            unitData.crLevel= m_UnitData.crLevel;
     
    }

    public override void Save_Unlock()
    {
        int _money = 0;
        if (_moneyArea.transform.childCount > 0)
        {
            foreach (CashBox k in _moneyArea.GetComponentsInChildren<CashBox>())
            {
                _money += k.cashValue;
            }
        }
        m_UnitData.moneySave = _money;

        //Debug.LogError("CounterNode \n" + JsonUtility.ToJson(m_UnitData));
        base.Save_Unlock();
    }


    #region Interface
    public bool isPay()
    {
        if (m_UnitData.crLevel > 1 || boardNode?.isCanPay == true)
        {
            return true;
        }

        return false;
    }


    public void Action_Pay()
    {
        
    }

    public Transform moneyArea()
    {
        return _moneyArea;
    }

    //WorkerTask
    public void InitTask()
    {
        //Debug.LogError("InitTask");
        MapController.Instance._ITaskWorkers.Add(this);

        if (!m_UnitData.isUnlocked)
            return;
        //Set task base
        WorkerTask task = new WorkerTask();
        task.unitBase = this;
        task.taskWorker = this;

        task.typeWorkerTask = EWorkerTask.Fill_Counter;
        task.productType = EProductType.sandwich;
        //task.inputSlot = MapController.Instance.GetWorkerSlot_Product(EProductType.sandwich);
        task.outputSlot = _outputWorkerSlot;

        //Debug.LogError("Fill_Counter");

        workerTask = task;
    }

    bool isDoing = false;
    public void ResetTask()
    {
        isDoing = false;
        //MapController.Instance.TaskAddOn(GetWorkerTask());
    }

    public WorkerTask workerTask;
    public WorkerTask GetWorkerTask()
    {
        if (!m_UnitData.isUnlocked)
            return null;

        if (_counterPickup.ProductUnits.Count < (int)unitDataBase.Capacity(m_UnitData.crLevel))
        {
            //Creat task
            isDoing = true;
            workerTask.inputSlot = MapController.Instance.GetWorkerSlot_Product(EProductType.sandwich);
            return workerTask;
        }

       
        return workerTask;
    }

    public bool IsDoing()
    {
        return isDoing;
    }



    #endregion
}
