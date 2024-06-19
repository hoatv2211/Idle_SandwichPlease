using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CounterPickup : BasePickAndDrop
{
    [SerializeField] private List<Transform> _slots;
    [SerializeField] private CounterStorage counterStorage;
    [SerializeField] private List<EProductType> input_ProductType;
    [SerializeField] private GameObject objEmpty;
    public int Capacity => _slots.Count;
    public bool isFull => _productUnits.Count >= Capacity;
    public override bool PickUp(ProductUnit productUnit, int maxQuantity)
    {
        if (_isCountdown)
        {
            return false;
        }

       // Debug.LogError(productUnit.typeProduct);

        if (!input_ProductType.Contains(productUnit.typeProduct))
        {
            return false;
        }

        if (_productUnits.Count >= Capacity)
        {
            return false;
        }
        //CounterData counterData = _unitBase.m_UnitData as CounterData;
        bool check = true/* productUnit.ObjectType == counterData.inputType*/ /*&& counterData.inputNames.Contains(productUnit.m_UnitData.idUnit)*/;
        if (check)
        {
            CountdownForNextPick();

            int index = _productUnits.Count;

            productUnit.transform.SetParent(_productParent);
            AddProduct(productUnit);

            Vector3 targetPos = _slots[index].localPosition;
            productUnit.IsMoving = true;
            productUnit.transform.localRotation = _slots[index].localRotation;
            productUnit.transform.DOLocalJump(targetPos, _jumpForce, 1, _speed)
                .OnComplete(() =>
                {
                    productUnit.IsMoving = false;
                    productUnit.transform.localPosition = targetPos;
                        // productUnit.transform.localRotation = _slots[index].localRotation;
                    OnChangedProducts("");
                });
            return true;
        }

      

        return false;
    }

    public override void AddProduct(ProductUnit productUnit)
    {
        _productUnits.Add(productUnit);
        counterStorage.AddProduct(productUnit);

        objEmpty?.SetActive(_productUnits.Count == 0);
    }
    public override void RemoveProduct(ProductUnit productUnit)
    {
        _productUnits.Remove(productUnit);
        objEmpty?.SetActive(_productUnits.Count == 0);
        //OnChangedProducts(productUnit.ObjectNameString);
    }

    CounterNode counterNode => (CounterNode)_unitBase;
    protected override void OnChangedProducts(string productName)
    {
        for (int i = 0; i < _productUnits.Count; i++)
        {
            Vector3 targetPos = _slots[i].localPosition;
            _productUnits[i].transform.localPosition=targetPos;
            _productUnits[i].transform.DOLocalRotate(_slots[i].localEulerAngles, 0.1f)
                .OnComplete(() =>
                {
                    _productUnits[i].transform.localRotation = _slots[i].localRotation;
                
                });
        }

      
    }
}


[System.Serializable]
public class CounterData : UnitData
{
    [Space]
    public EObjectType inputType;
    public List<string> inputNames;

    [Space]
    public EObjectType outputType;
    public string outputName;
    public int outputValue;

}