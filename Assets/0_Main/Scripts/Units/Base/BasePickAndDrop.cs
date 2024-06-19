using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class SlotDefine
{
    public List<Transform> slots;
    public Transform parent;
}
public class BasePickAndDrop : MonoBehaviour
{
    [Space, Header("BASE")]
    [SerializeField] protected Transform _productParent;
   
    [Space, Header("ANIM DATA")]
    [SerializeField] protected float _speed;
    [SerializeField] protected float _jumpForce;

    protected UnitBase _unitBase;
    protected bool _isCountdown = false;
    [SerializeField] protected List<ProductUnit> _productUnits = new List<ProductUnit>();
    public UnitBase UnitBase { get => _unitBase; set => _unitBase = value; }

    public List<ProductUnit> ProductUnits => _productUnits;
    public bool IsCountdown => _isCountdown;

    protected virtual void Awake()
    {
        _isCountdown = false;
    }

    protected virtual void OnEnable()
    {
        _productUnits.Clear();
    }

    public virtual bool PickUp(ProductUnit productUnit, int maxQuantity)
    {
        return false;
    }


    public virtual void AddProduct(ProductUnit productUnit)
    {
        //OnChangedProducts(productUnit.ObjectNameString);
    }
    public virtual void RemoveProduct(ProductUnit productUnit)
    {
        //OnChangedProducts(productUnit.ObjectNameString);
    }
    protected virtual void OnChangedProducts(string productName)
    {

    }

    protected virtual void CountdownForNextPick()
    {
        _isCountdown = true;
        DOVirtual.DelayedCall(_speed, () => _isCountdown = false);
    }

    private void OnTriggerStay(Collider other)
    {
        CheckPickup(other);
    }
    protected virtual void CheckPickup(Collider other)
    {

    }

    public void SetUnitBase(UnitBase unit)
    {
        _unitBase = unit;
    }
}
