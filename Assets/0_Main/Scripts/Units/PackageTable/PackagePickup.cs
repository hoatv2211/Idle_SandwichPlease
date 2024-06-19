using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class PackagePickup : BasePickAndDrop
{
    [SerializeField] private List<Transform> _slots;
    [SerializeField] private List<EProductType> input_ProductType;
    public int Capacity => _slots.Count;

    int numProduct = 0;
    public override bool PickUp(ProductUnit productUnit, int maxQuantity)
    {
        if (_isCountdown)
        {
            return false;
        }

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
            productUnit.transform.DOLocalRotate(_slots[index].localEulerAngles, _speed)
                .OnComplete(() =>
                {
                    productUnit.transform.localRotation = _slots[index].localRotation;
                });
            productUnit.transform.DOLocalJump(targetPos, _jumpForce, 1, _speed)
                .OnComplete(() =>
                {
                    productUnit.IsMoving = false;
                    productUnit.transform.localPosition = targetPos;
                    productUnit.transform.localRotation = _slots[index].localRotation;
                    // productUnit.transform.localRotation = _slots[index].localRotation;
                    OnChangedProducts(productUnit.m_UnitData.idUnit);
                });
            return true;
        }
        return false;
    }

    public override void AddProduct(ProductUnit productUnit)
    {
        _productUnits.Add(productUnit);
        numProduct++;
        if (numProduct >= 4)
            MapController.Instance.CompleteTut("tut_18");

    }
    public override void RemoveProduct(ProductUnit productUnit)
    {
        _productUnits.Remove(productUnit);
        OnChangedProducts(productUnit.m_UnitData.idUnit);

    }
    protected override void OnChangedProducts(string productName)
    {
        //fix bugs
        if (_productUnits.Count == 0)
        {
            ProductUnit[] _list = transform.GetComponentsInChildren<ProductUnit>();

            if (_list.Length > 0)
            {
                foreach (var k in _list)
                {
                    _productUnits.Add(k);
                }
            }
        }

        for (int i = 0; i < _productUnits.Count; i++)
        {
            Vector3 targetPos = _slots[i].localPosition;
            _productUnits[i].transform.DOLocalMove(targetPos, 0.1f);
            _productUnits[i].transform.DOLocalRotate(_slots[i].localEulerAngles, 0.1f)
                .OnComplete(() =>
                {
                    _productUnits[i].transform.localRotation = _slots[i].localRotation;
                    _productUnits[i].transform.localScale = Vector3.one;
                });
        }

        
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
        Debug.Log(addOn_Y);
        for (int i = 1; i < _slots.Count; i++)
        {
            Transform g = _slots[i];
            g.position = new Vector3(vtBase.x, vtBase.y + addOn_Y * (i - 1), vtBase.z);

        }
    }
}
