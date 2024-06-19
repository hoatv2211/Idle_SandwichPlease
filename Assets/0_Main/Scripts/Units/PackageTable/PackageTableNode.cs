using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageTableNode : UnitBase, ITaskWorker
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

    [Header("Fxz")]
    [SerializeField] private ParticleSystem fxUnlock;
    [SerializeField] private ParticleSystem fxUpgrade;
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject objMax;
    [SerializeField] private GameObject objEmpty;

    [Header("Products")]
    public PackagePickup m_Pickup;
    public PackageStorage m_Storage;

    [SerializeField] private PackageProduct packageProduct;
    [SerializeField] private Transform contentPackageProduct;
    PackageProduct crPackageProduct;

    [SerializeField] private BoardNode boardNode;


    #region Base
    public override void CallStart()
    {
        if (m_UnitData.isUnlocked)
        {
            Action_Show(true);
        }


        DOVirtual.DelayedCall(0.5f, () => {

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
        });

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
            StartCoroutine(IeOnWorkPackage());
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

        MapController.Instance.booterCtrl.ShowBaseBooter();

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
        MapController.Instance.mapData.GetUnlockUnitData(IdUnit).crLevel= m_UnitData.crLevel;
    }

    #endregion

    #region Working
    bool isCanWorking()
    {
        objEmpty?.SetActive(m_Pickup.ProductUnits.Count == 0);
        return (0 <= m_Pickup.ProductUnits.Count) && (m_Storage.ProductUnits.Count < 4);

    }


    IEnumerator IeOnWorkPackage()
    {
        yield return new WaitForSeconds(0.5f);
        InitTask();

      
        while (true)
        {
            while (isCanWorking())
            {
                yield return null;
                //Debug.Log("IeOnWorkPackage");
                if (crPackageProduct == null)
                {
                    crPackageProduct = SimplePool.Spawn(packageProduct, Vector3.zero, Quaternion.identity);
                    crPackageProduct.transform.SetParent(contentPackageProduct);
                    crPackageProduct.transform.localPosition = Vector3.zero;
                    crPackageProduct.transform.localScale = Vector3.one;
                    //crPackageProduct.transform.localRotation = Quaternion.Euler(new Vector3(0,45,0));
                }
              
                yield return new WaitUntil(() => m_Pickup.ProductUnits.Count > 0);
                objEmpty?.SetActive(m_Pickup.ProductUnits.Count == 0);
                yield return new WaitForSeconds(unitDataBase.Speed(m_UnitData.crLevel) / 4);
               
                ProductUnit _product = m_Pickup.ProductUnits[0];

                crPackageProduct.Action_AddToPackage(_product);
                m_Pickup.RemoveProduct(_product);
              

                if (crPackageProduct.isMax)
                {
                    m_Storage.AddProduct(crPackageProduct);
                    crPackageProduct = null;
                    crPackageProduct = SimplePool.Spawn(packageProduct, Vector3.zero, Quaternion.identity);
                    crPackageProduct.transform.SetParent(contentPackageProduct);
                    crPackageProduct.transform.localPosition = Vector3.zero;
                    crPackageProduct.transform.localScale = Vector3.one;

                    yield return new WaitForSeconds(0.5f);
                }
             

            }

            yield return null;
            yield return new WaitForSeconds(1f);
        }
    }


    #region Interface
    //WorkerTask
    public void InitTask()
    {
        MapController.Instance._ITaskWorkers.Add(this);

        if (!m_UnitData.isUnlocked)
            return;
        //Creat task
        WorkerTask task = new WorkerTask();
        task.unitBase = this;
        task.taskWorker = this;
        task.typeWorkerTask = EWorkerTask.Fill_PackageTable;
        task.productType = EProductType.sandwich;
        task.inputSlot = MapController.Instance.GetWorkerSlot_Product(EProductType.sandwich);
        task.outputSlot = _inputWorkerSlot;
        

        //Debug.LogError("Fill_PackageTable");
        workerTask = task;
    }

    bool isDoing = false;
    public void ResetTask()
    {
        Debug.Log(gameObject.name);
        isDoing = false;

        //MapController.Instance.TaskAddOn(GetWorkerTask());
    }

    public WorkerTask workerTask;
    public WorkerTask GetWorkerTask()
    {
        if(m_UnitData.isUnlocked==false)
            return null;

        if (m_Pickup.ProductUnits.Count < (int)unitDataBase.Capacity(m_UnitData.crLevel))
        {
            isDoing = true;
            workerTask.inputSlot = MapController.Instance.GetWorkerSlot_Product(EProductType.sandwich);
            return workerTask;

        }

        return null;
    }

    public bool IsDoing()
    {
        return isDoing;
    }

    #endregion

    #endregion
}
