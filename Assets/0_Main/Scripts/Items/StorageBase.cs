using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


/// <summary>
/// Storage products 
/// </summary>
public class StorageBase : MonoBehaviour
{
    [Space, Header("BASE")]
    [SerializeField] protected Transform _productParent;
    [SerializeField] protected List<Transform> _slots;

    [SerializeField] protected UnitBase _unitBase;
    [SerializeField] protected List<ProductUnit> _productUnits = new List<ProductUnit>();
    public UnitBase unitBase => _unitBase;
    public virtual List<ProductUnit> ProductUnits => _productUnits;

    private PlayerCtrl _playerUnit;

    public virtual bool CheckBeforePickup(Collider other) => _playerUnit != null /*&& !_playerUnit.IsMax && !other.GetComponent<PlayerUnit>()*/;

    protected virtual void OnEnable()
    {
        DOVirtual.DelayedCall(0.1f,()=> MapController.Instance.m_Storages.Add(this));
       
    }

    protected virtual void OnDisable()
    {
        MapController.Instance.m_Storages.Remove(this);
    }

    public virtual void AddProduct(ProductUnit productUnit)
    {
        int index = _productUnits.Count;
        _productUnits.Add(productUnit);

        productUnit.transform.SetParent(_productParent);
        Vector3 targetPos = _slots[index].localPosition;
        productUnit.transform.localPosition = targetPos;
        productUnit.transform.localRotation = Quaternion.identity;
    }
    protected virtual void OnChangedProducts()
    {
        for (int i = 0; i < _productUnits.Count; i++)
        {
            Vector3 targetPos = _slots[i].localPosition;
            _productUnits[i].transform.localPosition = targetPos;
            _productUnits[i].transform.localScale = Vector3.one;
        }

    }

    protected virtual void CheckPickup(Collider other)
    {
        PlayerCtrl player = other.gameObject.GetComponent<PlayerCtrl>();
        if (player)
        {
            if (!MapController.Instance.mapData.IsUnlock("counter_1_1"))
                return;

            PlayerPickup pickUp = other.gameObject.GetComponent<PlayerPickup>();
            if (player)
            {
                for (int i = _productUnits.Count - 1; i >= 0; i--)
                {
                    if (_productUnits[i].IsMoving)
                    {
                        continue;
                    }
                    int maxQuantity = (int)player.m_PlayerData.CarryStack;
                    bool check = pickUp.PickUp(_productUnits[i], maxQuantity);
                    if (check)
                    {
                        RemoveProduct(_productUnits[i]);
                        return;
                    }
                }
            }
        }
        WorkerUnit worker = other.gameObject.GetComponent<WorkerUnit>();
        if (worker /*&& worker.ReachedDestinationOrGaveUp()*/)
        {
            if (worker.workerTask == null)
                return;

            if (_unitBase == null)
                Debug.LogError("___NULL_____" + gameObject.name);

            if (worker.workerTask.inputSlot!=null&& worker.workerTask.inputSlot._unitBase == _unitBase)
            {
                for (int i = _productUnits.Count - 1; i >= 0; i--)
                {
                    if (_productUnits[i].IsMoving)
                    {
                        continue;
                    }

                    int maxQuantity = worker.m_WorkerData.CarryStack;
                    bool check = worker.Pickup(_productUnits[i], maxQuantity);
                    if (check)
                    {
                        RemoveProduct(_productUnits[i]);
                        if (!worker.IsDoneTask)
                        {
                            //Debug.LogError("Done");
                        }
                        return;
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {

    }
    protected virtual void OnTriggerStay(Collider other)
    {
        CheckPickup(other);
    }
    private void OnTriggerExit(Collider other)
    {

    }
    protected virtual void RemoveProduct(ProductUnit productUnit)
    {
        _productUnits.Remove(productUnit);
        OnChangedProducts();
    }

    public void SetUnitBase(UnitBase unit)
    {
        _unitBase = unit;
    }


    [ContextMenu("Generate")]
    private void GenerateSlot()
    {
        _slots.Clear();
        for (int i = 0; i < _productParent.childCount; i++)
        {
            _slots.Add(_productParent.GetChild(i));
        }

      
        Vector3 vtBase = _slots[0].position;
        float addOn_Y = _slots[1].position.y - _slots[0].position.y;
        for (int i =2;i< _slots.Count; i++)
        {
            Transform g = _slots[i];
            g.position = new Vector3(vtBase.x, g.position.y + addOn_Y*(i-1), vtBase.z);
           
        }
    }
}

