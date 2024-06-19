using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class DriveCarCounterNode : UnitBase, ICashier, ITaskWorker
{
    [Header("Machine")]
    public UnitDataBase unitDataBase;

    [Header("BUILDING")]
    [Tooltip("Unlock")]
    [SerializeField] private List<GameObject> listLocked;
    [SerializeField] private List<GameObject> listUnlocked;

    [Space, Tooltip("Upgrade")]
    [SerializeField] private List<GameObject> OnUpgraded;
    [SerializeField] private List<GameObject> OffUpgraded;
    [SerializeField] private BoardNode boardNode;

    [SerializeField] private GameObject staff;
    [SerializeField] private GameObject adsUnlockStaff;

    [Header("Fxz")]
    [SerializeField] private ParticleSystem fxUnlock;
    [SerializeField] private ParticleSystem fxUpgrade;
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject objMax;
    [SerializeField] private GameObject objEmpty;

    [Header("Products")]
    public CounterPickup m_Pickup;
    public CounterStorage m_Storage;
    [SerializeField] private Transform _moneyArea;

    [SerializeField] private CarPointCtrl m_CarPointCtrl;

    int isUnlockStaff
    {
        get { return PlayerPrefs.GetInt("staff_drive" + MapController.Instance._idMap, 0); }
        set { PlayerPrefs.SetInt("staff_drive" + MapController.Instance._idMap, value); }
    }
    public override void CallStart()
    {
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

        unitDataBase = GameManager.Instance.mapModel.GetUnitDataBase(IdUnit);

    }

    public void UnlockStaff()
    {
        isUnlockStaff = 1;
        adsUnlockStaff.gameObject.SetActive(false);
        staff.gameObject.SetActive(true);

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
        base.Save_Unlock();
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
            DOVirtual.DelayedCall(0.5f, () =>
            {
                foreach (var k in m_CarPointCtrl._unitPoints)
                {
                    MapController.Instance.carUnitPoints.Add(k);

                }


                if (m_UnitData.crLevel > 1)
                {
                    foreach (var k in OnUpgraded)
                    {
                        k.SetActive(true);
                    }

                    foreach (var k in OffUpgraded)
                    {
                        k.SetActive(false);
                    }
                }

                InitTask();
            });

            if (isUnlockStaff == 0 )
            {
                if (MapController.Instance._idMap == 1)
                {
                    if (MapController.Instance.IsCompleteTut("tut_21") || UserModel.Instance.level >= 5)
                    {
                        adsUnlockStaff.gameObject.SetActive(true);
                        staff.gameObject.SetActive(false);
                    }
                } else
                {
                    if (MapController.Instance.IsCompleteTut("tut_21") || UserModel.Instance.level >=12 )
                    {
                        adsUnlockStaff.gameObject.SetActive(true);
                        staff.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                adsUnlockStaff.gameObject.SetActive(false);
                staff.gameObject.SetActive(true);
            }
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


        DOVirtual.DelayedCall(120, () => { MapController.Instance.ShowUnit_UpgradeAds(); }).SetLoops(-1);

    }
    public override void Action_UpgradeUnit()
    {
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
        MapController.Instance.mapData.GetUnlockUnitData(IdUnit).crLevel = m_UnitData.crLevel;
    }

    #region Interface
    public bool isPay()
    {
        if (m_UnitData.crLevel > 1 || boardNode?.isCanPay == true || isUnlockStaff == 1 )
        {
            return true;
        }

        return false;
    }

    public Transform moneyArea()
    {
        return _moneyArea;
    }

    public void Action_Pay()
    {
        throw new System.NotImplementedException();
    }

    //WorkerTask
    public void InitTask()
    {
        MapController.Instance._ITaskWorkers.Add(this);

        if (!m_UnitData.isUnlocked)
            return;
        //Creat task
        WorkerTask task = new WorkerTask();
        task.unitBase = this;
        task.typeWorkerTask = EWorkerTask.Fill_DriveCar;
        task.productType = EProductType.sandwich_pakage;
        task.inputSlot = MapController.Instance.GetWorkerSlot_Product(EProductType.sandwich_pakage);
        task.outputSlot = _inputWorkerSlot;

        //Debug.LogError("Fill_DriveCar");
        workerTask = task;
    }

    bool isDoing = false;
    public void ResetTask()
    {
        //Debug.LogError(gameObject.name);
        isDoing = false;
        //MapController.Instance.TaskAddOn(GetWorkerTask());
    }

    public WorkerTask workerTask;
    public WorkerTask GetWorkerTask()
    {
        if (m_UnitData.isUnlocked == false)
            return null;

        if (MapController.Instance.packageStorage?.ProductUnits.Count > 0 &&
            m_Pickup.ProductUnits.Count < (int)unitDataBase.Capacity(m_UnitData.crLevel))
        {
            isDoing = true;

            return workerTask;

        }

        return null;
    }

    public bool IsDoing()
    {
        return isDoing;
    }

    #endregion
}
