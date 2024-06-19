using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;

public class MapController : Singleton<MapController>
{
    
    public int _idMap;


    #region Base Available

    [Space] [SerializeField] private bool togData = true;
    [HideIfGroup("togData")] [BoxGroup("togData/Map")] public MapModel mapModel;
    [HideIfGroup("togData")] [BoxGroup("togData/Map")] public MapData mapData => mapModel.mapData;

    [HideIfGroup("togData")] [BoxGroup("togData/Ctrl")] public ShipperCtrl shipperCtrl;
    [HideIfGroup("togData")] [BoxGroup("togData/Ctrl")] public BooterCtrl booterCtrl;
    [HideIfGroup("togData")] [BoxGroup("togData/Ctrl")] public Juice_Machine juice_Machine;


    [Space] [SerializeField] private bool togStatic = true;
    [HideIfGroup("togStatic")] [BoxGroup("togStatic/Booster")] public bool speedBooster = false;
    [HideIfGroup("togStatic")] [BoxGroup("togStatic/Booster")] public bool stackBooster = false;
    [HideIfGroup("togStatic")] [BoxGroup("togStatic/Booster")] public bool eatBooster = false;


    [Header("Availables")]
    [SerializeField] private bool togAvailable = true;
    [HideIfGroup("togAvailable")] [BoxGroup("togAvailable/Control")] public PlayerCtrl playerCtrl;
    [HideIfGroup("togAvailable")] [BoxGroup("togAvailable/Control")] public JoyStickHandle joyStick;
    [HideIfGroup("togAvailable")] [BoxGroup("togAvailable/Control")] public CameraCtrl cameraCtrl;
    [HideIfGroup("togAvailable")] [BoxGroup("togAvailable/Control")] public Action<Vector2> OnChangedInput;
  
    [HideIfGroup("togAvailable")] [BoxGroup("togAvailable/Units")] public List<UnitBase>       m_listUnits;
    [HideIfGroup("togAvailable")] [BoxGroup("togAvailable/Units")] public List<TrashBox>       m_trashBinUnits;
    [HideIfGroup("togAvailable")] [BoxGroup("togAvailable/Units")] public List<WorkerUnit>     m_workerUnits;
    [HideIfGroup("togAvailable")] [BoxGroup("togAvailable/Units")] public List<StorageBase>    m_Storages;
    [HideIfGroup("togAvailable")] [BoxGroup("togAvailable/PROGRESSION")] public List<UnlockUnit> _unlockPoints;
    [HideIfGroup("togAvailable")] [BoxGroup("togAvailable/Tutorial")] public List<TutNode> tutNodes;

    [Space] [SerializeField] private bool togUnits = true;
    [HideIfGroup("togUnits")] [BoxGroup("togUnits/Customer Units")] public CounterNode counterNode;                 // couternode
    [HideIfGroup("togUnits")] [BoxGroup("togUnits/Customer Units")] public CustomerPathCtrl customerPath;          // path
    [HideIfGroup("togUnits")] [BoxGroup("togUnits/Customer Units")] public List<CustomerUnit> m_CustomerUnits;    // tổng số unit customer
    [HideIfGroup("togUnits")] [BoxGroup("togUnits/Customer Units")] public List<CustomerPoint> customerPoints;   // vị trí xếp hàng
    [HideIfGroup("togUnits")] [BoxGroup("togUnits/Customer Units")] public List<SeatPoint> seatPoints;          //ghế ngồi
    [HideIfGroup("togUnits")] [BoxGroup("togUnits/Customer Units")] public List<WorkerSlot> workerSlots;
  
    [HideIfGroup("togUnits")] [BoxGroup("togUnits/Car Units")] public DriveCarCounterNode driveCarCounterNode;
    [HideIfGroup("togUnits")] [BoxGroup("togUnits/Car Units")] public DrivePathCtrl drivePath;
    [HideIfGroup("togUnits")] [BoxGroup("togUnits/Car Units")] public List<CarUnit> m_CarUnits;
    [HideIfGroup("togUnits")] [BoxGroup("togUnits/Car Units")] public List<CarPoint> carUnitPoints;

    [HideIfGroup("togUnits")] [BoxGroup("togUnits/Worker Units")] public Transform contentWorkers;
    [HideIfGroup("togUnits")] [BoxGroup("togUnits/Worker Units")] public List<WorkerTask> workerTasks;
    [HideIfGroup("togUnits")] [BoxGroup("togUnits/Worker Units")] public PackageTableNode packageTableNode;
    [HideIfGroup("togUnits")] [BoxGroup("togUnits/Worker Units")] public List<TableCreateArea> tableCreateAreas;
    [HideIfGroup("togUnits")] [BoxGroup("togUnits/Worker Units")] public List<ITaskWorker> _ITaskWorkers = new List<ITaskWorker>();

    //Other
    [HideIfGroup("togUnits")] [BoxGroup("togUnits/Others")] public OrderBalloons orderBalloons;
    [HideIfGroup("togUnits")] [BoxGroup("togUnits/Others")] public GameObject moneyAreaBonus;
    [HideIfGroup("togUnits")] [BoxGroup("togUnits/Others")] public PathArrow objDirection;
    [HideIfGroup("togUnits")] [BoxGroup("togUnits/Others")] public GameObject objCleanBot;

    #endregion

    #region Ref
    public LevelProcess levelProcess => mapModel.GetLevelProcess(mapData.crProcess);
    public float profit => (mapData.upgradePlayer.crProfitup().value / 100f) > 1 ? (mapData.upgradePlayer.crProfitup().value / 100f) : 1;

    public bool isHaveTakeAway => juice_Machine != null && juice_Machine.gameObject.activeInHierarchy;

    #endregion

    #region Base
    private void Start()
    {
        Initialize();
    }

    protected override void Awake()
    {
     
        base.Awake();
        //Data map
        StartCoroutine(ILoadMap());
    }

    #region Editor generate
    [ContextMenu("SetDataEditor")][Button("SetDataEditor")]
    public void SetDataEditor()
    {

        foreach (var k in m_listUnits)
        {
            DataMapProcess _data = mapModel.mapObjects.ToList().Find(x => x.id == k.IdUnit);

            if (_data != null)
            {
                k.m_UnitData.dataMapProcess = _data;
                k.m_UnitData.mapID = _idMap.ToString();
                k.m_UnitData.idUnit = _data.id;
                k.m_UnitData.name = _data.name;
                k.m_UnitData.costUnlock = _data.cost;
                k.m_UnitData.residualPrice = _data.cost;
      
            }

            for(int i =0;i< k.gameObject.transform.childCount; i++)
            {
                k.transform.GetChild(i).gameObject.SetActive(false);
            }


#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(k);
#endif
        }

        foreach (var k in _unlockPoints)
        {
            DataMapProcess _data = mapModel.mapObjects.ToList().Find(x => x.id == k.IdUnit);

            if (_data != null)
            {
                k.m_UnitData.dataMapProcess = _data;
                k.m_UnitData.idUnit = _data.id;
                k.m_UnitData.name = _data.name;
                k.m_UnitData.costUnlock = _data.cost;
                k.m_UnitData.residualPrice = _data.cost;
                //k.listUnlock.Clear();

                try
                {
                    foreach (var x in k.listUnlock)
                    {
                        if (x == null)
                        {
                            k.listUnlock.Remove(x);
                            continue;
                        }

                        if (x.gameObject.GetComponent<UnlockUnit>() != null)
                        {
                            k.listUnlock.Remove(x);
                        }

                    }
                }
                catch(Exception e)
                {
                    Debug.LogError(e.Message);
                }
                

                if (_data.unlocknext.Length > 0)
                {
                    if(_data.id!= "counter_1_1")
                    {
                        foreach (var u in _data.unlocknext)
                        {
                            //Debug.LogError(u);
                            GameObject g = _unlockPoints.Find(x => x.IdUnit == u)?.gameObject;
                            if (g != null)
                                k.AddOnListUnlock(g);
                        }
                    }

                }

#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(k);
#endif
                continue;
            }

            
        }
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.SaveAssets();
#endif
        Debug.LogError("Done SetDataEditor");
    }


    [ContextMenu("ShowAll")][Button("ShowAll")]
    private void ShowAll()
    {
        foreach (var k in m_listUnits)
        {
            for (int i = 0; i < k.gameObject.transform.childCount; i++)
            {
                k.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    
    }


    [ContextMenu("ShowOff")][Button("ShowOff")]
    public void ShowOff()
    {
        foreach (var k in m_listUnits)
        {
            k.m_UnitData.isUnlocked = false;
            k.m_UnitData.crLevel = 1;
            for (int i = 0; i < k.gameObject.transform.childCount; i++)
            {
                k.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    #endregion

    #endregion

    #region Inits
    public void Initialize()
    {
      
        joyStick.RegisterListenInput(ChangeInput);

        
        //InitCustomer
        DOVirtual.DelayedCall(2, () =>
        {
            for (int i = 0; i < mapData.upgradeStaff.crlvEmploy().value + GameManager.Instance.skinModel.infoSkin.staff_employ; i++)
            {
                //Debug.LogError(mapData.upgradeStaff.crlvEmploy().value);
                SpawnWorker();
            }
        });

        moneyAreaBonus.SetActive(mapData.isMoneyBonus);
        foreach (var k in m_listUnits)
        {
            k.Load_Unlock();
        }
        //Action
        StartCoroutine(IeSpawnCustomer());
        StartCoroutine(IeSpawnCar());

        //if(mapData.IsUnlock("door_01"))
        //    playerCtrl.LoadPos();

        if(isShowUpgradeAds)
            DOVirtual.DelayedCall(120,()=> ShowUnit_UpgradeAds()).SetLoops(-1);

        AdsAppLovinController.Instance.ShowBanner();
    }

    IEnumerator ILoadMap()
    {
        
        mapData.LoadData();
        mapModel.LoadRefToMapData();
        yield return null;


        //LoadData
        foreach (var k in _unlockPoints)
        {
            var _data = mapData.crUnlocks.Find(x => x.idUnit == k.IdUnit);
            if (_data != null)
            {
                //Debug.LogError(k.name + " - " + k.IdUnit);
                k._residualPrice = _data.residualPrice;
                k.gameObject.SetActive(true);
                k.CallStart();
            }
            else
                k.gameObject.SetActive(false);

        }

        foreach(var k in tutNodes)
        {
            if (mapData.crTut.Contains(k.idTut))
                k.gameObject.SetActive(true);
        }

        yield return null;
        moneyAreaBonus.SetActive(mapData.isMoneyBonus);

        yield return null;
        objCleanBot.SetActive(mapData.IsUnlockByName("table_03"));

        yield return null;
        ETypeSkin eTypeSkin = GameManager.Instance.skinModel.typeSkin;
        Module.Action_Event_SkinChange(eTypeSkin);

    }

    public void Reload()
    {
  
        StopCoroutine(IeSpawnCustomer());
        StopCoroutine(IeSpawnCar());
        foreach (var k in m_listUnits)
        {
 
            k.m_UnitData.isUnlocked = false;
            k.m_UnitData.crLevel = 1;
            for (int i = 0; i < k.gameObject.transform.childCount; i++)
            {
                k.transform.GetChild(i).gameObject.SetActive(false);
        
            }
            k.Load_Unlock();
        }
 
        foreach (var k in m_CustomerUnits)
            SimplePool.Despawn(k.gameObject);
        m_CustomerUnits.Clear();

        foreach (var k in m_CarUnits)
            SimplePool.Despawn(k.gameObject);
        m_CarUnits.Clear();

        foreach (var k in m_workerUnits)
            SimplePool.Despawn(k.gameObject);
        m_workerUnits.Clear();

        StartCoroutine(IeSpawnCustomer());
        StartCoroutine(IeSpawnCar());

        for (int i = 0; i < mapData.upgradeStaff.crlvEmploy().value + GameManager.Instance.skinModel.infoSkin.staff_employ; i++)
        {
            SpawnWorker();
        }

        Debug.LogError("Reload");
    }

    private void OnDisable()
    {
        foreach (var k in m_listUnits)
        {
            k.Save_Unlock();
        }
        mapData.SaveData();
       
    }

    #endregion

    #region Application
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            foreach (var k in m_listUnits)
            {
                k.Save_Unlock();
            }
            mapData.SaveData();
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            foreach (var k in m_listUnits)
            {
                k.Save_Unlock();
            }
            mapData.SaveData();

        }
        else
        {
            Module.time_offline = timeNow().ToString();
        }

    }

    private void OnApplicationQuit()
    {
        foreach (var k in m_listUnits)
        {
            k.Save_Unlock();
        }
        mapData.SaveData();

        Module.time_offline = timeNow().ToString();
    }

    #endregion

    #region Time Manager
    public double timeOffline()
    {
        if (string.IsNullOrEmpty(Module.time_offline))
            return 0;

        double time = Convert.ToDateTime(DateTime.Now).Subtract(Convert.ToDateTime(Module.time_offline)).TotalMinutes;

        return time;
    }


    public DateTime timeNow()
    {
        return DateTime.Now;
    }

    #endregion

    #region Events
    public void ChangeInput(Vector2 inputDir)
    {
        if (cameraCtrl.Target == playerCtrl.transform)
        {
            OnChangedInput?.Invoke(inputDir);
        }
        else
        {
            OnChangedInput?.Invoke(Vector2.zero);
        }
    }

    #endregion

    #region Customer

    public CounterStorage counterStorage =>(CounterStorage)m_Storages.Find(x => x.unitBase is CounterNode);

    int idxTable = 0;
    public TableCreateArea GetTableIndex()
    {
        TableCreateArea _table = tableCreateAreas[idxTable];
        idxTable++;
        if (idxTable >= tableCreateAreas.Count)
            idxTable = 0;

        return _table;
    }

    public TableCreateArea crTable;
    public SeatPoint GetSeatPointFree()
    {
        //if (seatPoints.Count == 0)
        //    return null;

        //SeatPoint _seatPoint = null;
        //_seatPoint = seatPoints.Find(p => p.isSlotFree && !p.isBlocked);
        if (tableCreateAreas.Count == 0)
            return null;

        if (crTable == null
            || crTable.isBlocked
            || crTable.GetSeatPoint() == null)
        {
            crTable = GetTableIndex();
            return null;
        }
            
        SeatPoint _seatPoint = null;
         _seatPoint = crTable.GetSeatPoint();

        return _seatPoint;
    }

    public CustomerPoint GetCustomerPointFree()
    {
        CustomerPoint _cusPoint = null;
        //_cusPoint = customerPoints.Find(p => !p.IsFilled);
        try
        {
            _cusPoint = customerPoints[m_CustomerUnits.Count];
        }
        catch
        {
            _cusPoint = customerPoints.Find(p => !p.IsFilled);
        }
        

        return _cusPoint;
    }

    public IEnumerator IeSpawnCustomer()
    {
        yield return new WaitUntil(() => customerPoints.Count>0);

        while (true)
        {
            yield return null;
            yield return new WaitUntil(() => GetCustomerPointFree() != null);

            if (Module.isFirstCustomer == 0)
            {
                Module.isFirstCustomer = 1;
                SpawnBaseCustomer();
                yield return null;
            }

            SpawnCustomer();

            yield return null;
            yield return null;
            yield return new WaitForSeconds(Module.EasyRandom(0.5f,3f));
        }
    }

    public void SpawnBaseCustomer()
    {
        Debug.Log("SpawnCustomer");
        if (m_CustomerUnits.Count >= counterNode._customerPointCtrl._customerPoints.Count)
        {
            return;
        }

        bool isVip = false;
        numCustomerSpawn++;
        CustomerUnit _customerPrefab = AssetConfigs.Instance.GetCustomerUnit("");


        if (numCustomerSpawn >= Module.EasyRandom(10, 15))
        {
            numCustomerSpawn = 0;
            //Debug.LogError("Viper");
            isVip = true;
            _customerPrefab = AssetConfigs.Instance.GetCustomerUnit("vip");
        }


        CustomerUnit customer = SimplePool.Spawn(_customerPrefab, customerPath.trPath1.position, Quaternion.identity);
        customer.transform.SetParent(customerPath.trContent);


        CustomerPoint _customerPoint = GetCustomerPointFree();
        customer.InitTargetPoint(_customerPoint);

        m_CustomerUnits.Add(customer);

        if (!isVip)
            customer.CallStart();
        else
            customer.CallVip();

       
    }

    int numCustomerSpawn = 0;
    public void SpawnCustomer()
    {
        Debug.Log("SpawnCustomer");
        if (m_CustomerUnits.Count>= counterNode._customerPointCtrl._customerPoints.Count)
        {
            return;
        }

        bool isVip = false;
        numCustomerSpawn++;
        CustomerUnit _customerPrefab = AssetConfigs.Instance.GetCustomerUnit("");


        if (numCustomerSpawn >= Module.EasyRandom(10,15))
        {
            numCustomerSpawn = 0;
            //Debug.LogError("Viper");
            isVip = true;
            _customerPrefab = AssetConfigs.Instance.GetCustomerUnit("vip");
        }

        Transform trSpawn = customerPath.trPathStart;
        if (!IsVisibleFromCamera(customerPath.trPath1.gameObject))
        {
            trSpawn = customerPath.trPath1;
        }
        else
        {
            trSpawn = customerPath.trPathStart;
        }

        CustomerUnit customer = SimplePool.Spawn(_customerPrefab, trSpawn.position, Quaternion.identity);
        customer.transform.SetParent(customerPath.trContent);
      

        CustomerPoint _customerPoint = GetCustomerPointFree();
        customer.InitTargetPoint(_customerPoint);

        m_CustomerUnits.Add(customer);

        if (!isVip)
            customer.CallStart();
        else
            customer.CallVip();


    }

    public void RemoveCustomer(CustomerUnit customer)
    {
        m_CustomerUnits.Remove(customer);
    }

    public float CheckDistance(Transform cus)
    {
        return Vector3.Distance(customerPath.trPath2.position, cus.position);
    }

    public void SortLineupCustomer()
    {
        //m_CustomerUnits.Sort((x, y) => CheckDistance(y.transform).CompareTo(CheckDistance(x.transform)));

        for (int i =0;i< m_CustomerUnits.Count; i++)
        {
            m_CustomerUnits[i].SetNewTarget(customerPoints[i]);
        }
    }


    private bool IsVisibleFromCamera(GameObject obj)
    {
        if (cameraCtrl == null || obj == null)
            return false;

        // Get the object's bounds.
        Renderer renderer = obj.GetComponent<Renderer>();
        Bounds bounds = renderer != null ? renderer.bounds : new Bounds(obj.transform.position, Vector3.zero);

        // Test the object's bounds against the camera frustum planes.
        return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(Camera.main), bounds);
    }
    #endregion

    #region Cars
    public CounterStorage driveCarStorage => (CounterStorage)m_Storages.Find(x => x.unitBase is DriveCarCounterNode);
    public CarPoint GetCarPointFree()
    {
        int foundedIndex = carUnitPoints.FindIndex(p => !p.IsFilled);
        return carUnitPoints[foundedIndex];
    }

    public void SortLineupCar()
    {
        for (int i = 0; i < m_CarUnits.Count; i++)
        {
            m_CarUnits[i].SetNewTarget(carUnitPoints[i]);
        }
    }

    public IEnumerator IeSpawnCar()
    {
        yield return new WaitUntil(() => carUnitPoints.Count > 0);

        while (true)
        {
            yield return null;
            yield return new WaitUntil(() => GetCarPointFree() != null);

            SpawnCar();
            yield return null;
            yield return null;
            yield return new WaitForSeconds(Module.EasyRandom(2f, 5f));
        }
    }

    bool isCanSpawnCar()
    {
        return m_CarUnits.Count < 8;
    }
    public void SpawnCar()
    {   
        if (!isCanSpawnCar())
        {
            return;
        }
        Debug.Log("SpawnCar");
        CarUnit _prefab = AssetConfigs.Instance.GetCarUnit("");

        CarUnit _unit = SimplePool.Spawn(_prefab, drivePath.trPathStart.position, Quaternion.identity);
        _unit.transform.SetParent(drivePath.transform);
   
        //CarPoint _unitPoint = GetCarPointFree();
        CarPoint _unitPoint = carUnitPoints[m_CarUnits.Count];
        _unit.InitTargetPoint(_unitPoint);


        _unit.CallStart();
        m_CarUnits.Add(_unit);


    }

    public void RemoveCar(CarUnit car)
    {
        m_CarUnits.Remove(car);
    }
    #endregion

    #region Worker
    public void SpawnWorker()
    {
        if (m_workerUnits.Count >= mapData.upgradeStaff.crlvEmploy().value + GameManager.Instance.skinModel.infoSkin.staff_employ)
            return;

        Debug.Log("Spawn Worker");
        WorkerUnit _prefab = AssetConfigs.Instance.GetWorkerUnit("");

        WorkerUnit _worker = SimplePool.Spawn(_prefab, contentWorkers.GetChild(Module.EasyRandom(5)).position, Quaternion.identity);
        _worker.transform.SetParent(contentWorkers);
        _worker.CallStart();
        m_workerUnits.Add(_worker);


    }

    public void SpawnPieceTrash(TrashPiecesGroup _trashGroup)
    {
        TrashPiece _prefab = AssetConfigs.Instance.GetTrashPiece("");
        TrashPiece _trashPiece = SimplePool.Spawn(_prefab, _trashGroup.transform.GetChild(Module.EasyRandom(_trashGroup.countTrash)).position, Quaternion.identity);
        _trashPiece.transform.SetParent(_trashGroup._parent);
    }

    public void CheckWorker()
    {
        int count = m_workerUnits.Count - ((int)mapData.upgradeStaff.crlvEmploy().value + (int)GameManager.Instance.skinModel.infoSkin.staff_employ);
        //Debug.LogError(count);
        if (count<0)
        {
            for(int i = 0; i < Math.Abs(count); i++)
            {
                WorkerUnit _prefab = AssetConfigs.Instance.GetWorkerUnit("");

                WorkerUnit _worker = SimplePool.Spawn(_prefab, contentWorkers.GetChild(Module.EasyRandom(5)).position, Quaternion.identity);
                _worker.transform.SetParent(contentWorkers);
                _worker.CallStart();
                m_workerUnits.Add(_worker);
            }
        }

        if (count > 0)
        {
            for(int i = 0; i < Math.Abs(count); i++)
            {
                var cus = m_workerUnits[0];
                m_workerUnits.Remove(cus);
                SimplePool.Despawn(cus.gameObject);
            }
        }
    }

    #endregion

    #region TaskWorker
    public WorkerTask GetTaskWorker()
    {
        //filter type task
        List<EWorkerTask> eWorkerTasks = new List<EWorkerTask>();
        List<WorkerTask> _workerTasks = new List<WorkerTask>(); //Get all task

        foreach(var k in _ITaskWorkers)
        {
            WorkerTask _task = k.GetWorkerTask();
          
            if (_task != null)
            {
                //Debug.LogError(_task.typeWorkerTask);
                _workerTasks.Add(_task);
                eWorkerTasks.Add(_task.typeWorkerTask);
            }
               
        }
        //Debug.LogError("worker task count : "+ _workerTasks.Count);
        
        foreach(var k in m_workerUnits)
        {
            if(k.workerTask!=null && k.workerTask.typeWorkerTask != EWorkerTask.None)
            {
                if(eWorkerTasks.Contains(k.workerTask.typeWorkerTask))
                    eWorkerTasks.Remove(k.workerTask.typeWorkerTask);
            }
        }
        //Debug.LogError("Type worker task count : " + eWorkerTasks.Count);

        if (eWorkerTasks.Count > 0)
        {
            EWorkerTask taskPick = eWorkerTasks[Module.EasyRandom(0, eWorkerTasks.Count - 1)];
            WorkerTask _GetTask = _workerTasks.Find(x => x.typeWorkerTask == taskPick);
            //Debug.LogError("worker task get : " + _GetTask.typeWorkerTask);

            return _GetTask;
        }
        else
        {
            WorkerTask _GetTask = workerTasks.FirstOrDefault(x => x.workerUnit == null);

            if (_GetTask == null)
            {
                if (counterNode?._counterStorage.ProductUnits.Count < 5)
                {
                    _GetTask = _workerTasks.Find(x => x.typeWorkerTask == EWorkerTask.Fill_Counter);
                    return _GetTask;
                }
                else if (packageTableNode?.m_Pickup.ProductUnits.Count < 5)
                {
                    _GetTask = _workerTasks.Find(x => x.typeWorkerTask == EWorkerTask.Fill_PackageTable);
                    return _GetTask;
                }
                else 
                {
                    _GetTask = _workerTasks.FirstOrDefault((x) => (x.unitBase is TableCreateArea) && (x.workerUnit == null));
                }


            }

            //Debug.LogError(_GetTask?.typeWorkerTask);
            return _GetTask;
        }

    }

    public PackageStorage packageStorage => (PackageStorage)m_Storages.Find(x => x.unitBase is PackageTableNode);

    public void TaskAddOn(WorkerTask _task)
    {
        if (_task == null || _task.typeWorkerTask == EWorkerTask.None)
            return;

        if (workerTasks.FindAll(x => x.typeWorkerTask == _task.typeWorkerTask).Count >= 2)
            return;

        workerTasks.Add(_task);

        workerTasks.Sort((x, y) => x.typeWorkerTask.CompareTo(y.typeWorkerTask));
    }

  

    public WorkerSlot GetWorkerSlot_Product(EProductType _type)
    {
        WorkerSlot _slot = null;
        switch (_type)
        {
            case EProductType.None:
                break;
            case EProductType.sandwich:
                List<WorkerSlot> _list = new List<WorkerSlot>();
                foreach (var k in workerSlots)
                {
                    if (k.productType != EProductType.trash 
                        && k._unitBase.m_UnitData.isUnlocked 
                        && k._unitBase is CoffeeMachineNode && k.productType == _type
                        && !k.isInput)
                    {
                        if(!_list.Contains(k))
                            _list.Add(k);
                    }
                }

                //Debug.LogError(_list.Count);
                switch (_list.Count)
                {
                    case 0:
                        //Debug.LogError("Can't Find CoffeeMachineNode");
                        break;
                    case 1:
                        _slot = _list[0];
                        break;
                    default:
                        int index = Module.EasyRandom(_list.Count);
                        _slot = _list[index];
                        //Debug.LogError(index);
                        break;
                }

                break;
            case EProductType.sandwich_pakage:
                foreach (var k in workerSlots)
                {
                    if (k.productType != EProductType.trash && k._unitBase.m_UnitData.isUnlocked && k._unitBase is PackageTableNode && k.productType == _type)
                    {
                        _slot = k;
                        break;
                    }
                }
                break;
            case EProductType.coffee:
                break;

            case EProductType.trash:
                foreach (var k in workerSlots)
                {
                    if (k.productType == _type)
                    {
                        _slot = k;
                        break;
                    }
                }
                break;

            default:
                break;
        }

        return _slot;
    }

    public WorkerSlot GetWorkerSlot_Ouput(EProductType _type)
    {
        WorkerSlot _slot = null;
        switch (_type)
        {
            case EProductType.None:
                _slot = m_trashBinUnits[0].workerSlot.GetComponent<WorkerSlot>();
                break;
            case EProductType.sandwich:
                if (!counterNode._counterPickup.isFull)
                    _slot = counterNode._inputWorkerSlot;
                else
                    _slot = packageStorage.unitBase._inputWorkerSlot;

                break;
            case EProductType.sandwich_pakage:
                _slot = driveCarCounterNode._inputWorkerSlot;
                break;
            case EProductType.trash:
                _slot = m_trashBinUnits[0].workerSlot.GetComponent<WorkerSlot>();
                break;

            default:
                _slot = m_trashBinUnits[0].workerSlot.GetComponent<WorkerSlot>();
                break;
        }

        return _slot;
    }

    public EWorkerTask GetWorkerTaskType(EProductType _type)
    {
        EWorkerTask _slot = EWorkerTask.None;
        switch (_type)
        {
            case EProductType.sandwich:
                _slot = EWorkerTask.Fill_Counter;
                break;
            case EProductType.sandwich_pakage:
                _slot = EWorkerTask.Fill_DriveCar;
                break;
            case EProductType.trash:
                _slot = EWorkerTask.Trash;
                break;

            default:
                _slot = EWorkerTask.Trash;
                break;
        }
        return _slot;
    }
    #endregion

    #region Tuts
    public void ShowDirection(bool _isOn=true)
    {
        
        objDirection.gameObject.SetActive(_isOn);
    }


    public void CompleteTut(string _id)
    {
        if (mapData.crTut.Count == 0)
            return;

        if (mapData.tutCleans.Contains(_id)||mapData.crTut[0]!=_id)
            return;

        tutNodes.Find(x => x.idTut == _id).Action_CompleteTut();

    }

    public void HideTut(string _id)
    {
        TutNode tutNode = tutNodes.Find(x => x.idTut == _id);
        if(tutNode!=null)
            tutNode.Hide_Tut();
    }

    public bool IsCompleteTut(string _id)
    {
        return mapData.tutCleans.Contains(_id);
    }

    #endregion

    #region Others
    private bool isShowUpgradeAds => UserModel.Instance.level>=3;
    public void ShowUnit_UpgradeAds()
    {
        if (mapData.crUnlocks.Count == 0 || !Module.isNetworking())
            return;

        if (!isShowUpgradeAds)
            return;

        int index = Module.EasyRandom(mapData.crUnlocks.Count);
        UnlockUnit unlockUnit = _unlockPoints.Find(x => x.IdUnit == mapData.crUnlocks[index].idUnit);
        unlockUnit.SetTypeUnlock(EUnlockCost.ads);

        DOVirtual.DelayedCall(60,()=> {
            if (!unlockUnit._isUnlocked&&unlockUnit._isReqToUnlock)
                unlockUnit.SetTypeUnlock(EUnlockCost.money);
        });

    }

    #endregion
}
