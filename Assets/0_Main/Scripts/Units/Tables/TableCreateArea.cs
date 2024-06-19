using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum ETableState
{
    None,
    Clean,
    Seating,
    Eating,
    GotTrash,
    Blocked
}
public class TableCreateArea : UnitBase, ITaskWorker
{
    [Header("Machine")]
    public UnitDataBase unitDataBase;
    public float speedEat => unitDataBase.Speed(m_UnitData.crLevel);

    public ETableState stage;
    public List<GameObject> listLocked;
    public List<GameObject> listUnlocked;
    public ParticleSystem fxUnlock;
    public ParticleSystem fxUpgrade;

    [Header("Customers")]
    public Transform moneyArea;
    public TableStorage tableStorage;

    public TrashPiecesGroup piecesGroup;
    public List<SeatPoint> seatPoints;
    public List<Chair_Anim> chair_Anims;
    [HideInInspector] public bool isBlocked = false;

    [Header("UseLate")]
    public List<GameObject> listOffs;
    public List<UnlockLevel> unlockLevels;

  
    public void SetStage(ETableState _stage)
    {
        stage = _stage;
        
    }

    public SeatPoint GetSeatPoint()
    {
        foreach(var k in seatPoints)
        {
            if (k.isSlotFree)
                return k;
        }

        return null;
    }

    public override void CallStart()
    {
        if (m_UnitData.isUnlocked)
        {
            Action_Show(true);
            if (m_UnitData.moneySave > 0)
            {
                GameObject g = Instantiate(AssetConfigs.Instance.cashBox.gameObject, moneyArea.position, Quaternion.identity);
                g.transform.SetParent(moneyArea);

                CashBox _cashbox = g.GetComponent<CashBox>();
                _cashbox.AddOn(m_UnitData.moneySave);
            }

            stage = ETableState.Clean;
        }

        UnitDataBase _unit = GameManager.Instance.mapModel.GetUnitDataBase(IdUnit);

        if(_unit!=null)
            unitDataBase = _unit;


        Action_ShowLevel(m_UnitData.crLevel);
    }

    public override void Save_Unlock()
    {
        int _money = 0;
        if (moneyArea.transform.childCount > 0)
        {
            foreach (CashBox k in moneyArea.GetComponentsInChildren<CashBox>())
            {
                _money += k.cashValue;
            }
        }
        m_UnitData.moneySave = _money;
        base.Save_Unlock();
    }

    public bool isEmpty()
    {
        foreach (var k in seatPoints)
        {
            if (!k.isSlotFree)
            {
                return false;
            }
        }

        return true;
    }

    public bool isFullSeatEating()
    {
        foreach(var k in seatPoints)
        {
            if (!k.isEating)
            {
                return false;
            }
        }

        stage = ETableState.Eating;
        return true;
    }

    public bool isDoneEat()
    {

        foreach (var k in seatPoints)
        {
            if (k.isEating||k.isBlocked)
            {
                return false;
            }
        }
        stage = ETableState.GotTrash;
        return true;
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

        if(_isUnlock)
        {
            //Init
            foreach (var k in seatPoints)
                k.SetUnitBase(this);

            piecesGroup.SetInit(this);
            InitTask();
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

        if (IdUnit == "table_3_1")
        {
            MapController.Instance.objCleanBot.SetActive(true);
            UIMainGame.Instance.btnCleanBot.gameObject.SetActive(true);
        }

        if(IdUnit == "table_4_1")
        {
            MapController.Instance.shipperCtrl.StartSpecialShiper();
        }
        FirebaseManager.Instance.LogEvent_Table(IdUnit);
        AdjustTracking.Instance.Event_table_unlock(IdUnit);

        Action_ShowLevel(m_UnitData.crLevel);
    }

    public override void Action_UpgradeUnit()
    {
        if (fxUpgrade != null)
        {
            fxUpgrade.gameObject.SetActive(true);
            fxUpgrade.Play(true);
        }

        m_UnitData.crLevel++;
        MapController.Instance.mapData.GetUnlockUnitData(IdUnit).crLevel = m_UnitData.crLevel;

        Action_ShowLevel(m_UnitData.crLevel);
    }

    public void CleanTable()
    {
        //Debug.LogError("Clean " + gameObject.name) ;
        foreach (var k in seatPoints)
            k.Clean();

        foreach (var k in chair_Anims)
            k.Action_On();

        piecesGroup.Clean();
        stage = ETableState.Clean;
    }

    public void CheckClean()
    {
        bool _isClean = true;
        foreach (var k in seatPoints)
        {
            if (!k.CheckClean())
                _isClean = false;
        }

        if (_isClean)
        {
            CleanTable();
        }
         
    }

    public void Action_ShowLevel(int _lv)
    {
        foreach (var k in listOffs)
            k.gameObject.SetActive(false);

        try
        {
            switch (_lv)
            {
                case 1:
                    foreach (var k in unlockLevels[0].listOns)
                    {
                        k.gameObject.SetActive(true);
                    }
                    break;
                case 2:
                    foreach (var k in unlockLevels[1].listOns)
                    {
                        k.gameObject.SetActive(true);
                    }
                    break;
                case 3:
                    foreach (var k in unlockLevels[2].listOns)
                    {
                        k.gameObject.SetActive(true);
                    }
                    break;
                default:
                    break;
            }
        }
        catch
        {
            Debug.LogError(gameObject.name);
        }
      
    }

    #region Interface
    //WorkerTask
    public void InitTask()
    {
        MapController.Instance._ITaskWorkers.Add(this);
        MapController.Instance.tableCreateAreas.Add(this);
        if (!m_UnitData.isUnlocked)
            return;
        //Set taskbase
        WorkerTask task = new WorkerTask();
        task.unitBase = this;
        task.taskWorker = this;
        task.typeWorkerTask = EWorkerTask.Clean_Table;
        task.productType = EProductType.trash;

        task.inputSlot = _outputWorkerSlot;
        task.outputSlot = MapController.Instance.GetWorkerSlot_Product(EProductType.trash);

        workerTask = task;
    }

    bool isDoing = false;
    public void ResetTask()
    {
        isDoing = false;
        //foreach (var k in seatPoints)
        //    k.Clean();
    }

    public void SetAnimChairOff()
    {
        foreach (var k in chair_Anims)
            k.Action_Off();
    }

    public WorkerTask workerTask;
    public WorkerTask GetWorkerTask()
    {
        if (!m_UnitData.isUnlocked)
            return null;

        //Debug.LogError(gameObject.name);
        if (piecesGroup.isGotTrash()&& isDoneEat())
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
