using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using System;

[System.Serializable]
public class CustomerData : UnitData
{
    public bool isVip = false;
    public float moveSpeed  = 3;
    public int stackProduct = 3;
    public List<EProductType> productList;
    

}
public class CustomerPickup : BasePickAndDrop
{
    [SerializeField] private List<Transform> _slots;
    [SerializeField] private Vector3 _offset;

    public GameObject tray_Customer;

    [SerializeField] private CustomerData m_customerData;

    private Vector3 basePosTray;
    private Transform baseParentTray;
    public void CallStart(CustomerData _data)
    {
        for(int i =0;i< _productParent.childCount; i++)
        {
            if (_productParent.GetChild(i).GetComponent<ProductUnit>() != null)
            {
                SimplePool.Despawn(_productParent.GetChild(i).gameObject);
            }
        }

        _productUnits.Clear();
        tray_Customer.gameObject.SetActive(false);
        m_customerData = _data;
        basePosTray = tray_Customer.transform.localPosition;
        baseParentTray = tray_Customer.transform.parent;
    }

    public void ResetTray()
    {
        tray_Customer.transform.SetParent(baseParentTray);
        tray_Customer.transform.localPosition = basePosTray;
    }

    public bool IsMaxItem()
    {
        if (ProductUnits.Count >= m_customerData.stackProduct)
            return true;
  
        return false;
    }

    private int BaseCapacity => _slots.Count;
    public int productLeft => m_customerData.stackProduct - ProductUnits.Count;

    public override bool PickUp(ProductUnit productUnit, int maxQuantity)
    {
        if (_isCountdown||IsMaxItem())
        {
            return false;
        }

        bool check = true;
        if (check)
        {
            CountdownForNextPick();

            int index = _productUnits.Count;
            AddProduct(productUnit);
            //CustomerUnit.AddBuyingProduct(currentBuyingProduct);

            productUnit.transform.SetParent(_productParent);

            int slotIndex = index % BaseCapacity;
            int row = index / BaseCapacity;

            Vector3 targetPos = _slots[slotIndex].localPosition + _offset * row;
            productUnit.IsMoving = true;
            productUnit.transform.DOLocalRotate(_slots[slotIndex].localEulerAngles, _speed)
            .OnComplete(() =>
            {
                productUnit.transform.localRotation = _slots[slotIndex].localRotation;
            });
            productUnit.transform.DOLocalJump(targetPos, _jumpForce, 1, _speed)
            .OnComplete(() =>
            {
                productUnit.IsMoving = false;
                productUnit.transform.localPosition = targetPos;
                OnChangedProducts(productUnit.productData.id);
            });
            return true;
        }
        return false;
    }

    public void Action_TakeAway(Transform _trSpawn)
    {
        GameObject _prefab = AssetConfigs.Instance.unitBases.Find(x => x.IdUnit == "juice_cup").gameObject;
        GameObject _spawn = SimplePool.Spawn(_prefab, _trSpawn.position,Quaternion.identity);
        ProductUnit productUnit = _spawn.GetComponent<ProductUnit>();
        int index = _productUnits.Count;
        AddProduct(productUnit);

        productUnit.transform.SetParent(_productParent);
        int slotIndex = index % BaseCapacity;
        int row = index / BaseCapacity;

        Vector3 targetPos = _slots[slotIndex].localPosition + _offset * row;
        productUnit.IsMoving = true;
        productUnit.transform.DOLocalRotate(_slots[slotIndex].localEulerAngles, _speed)
        .OnComplete(() =>
        {
            productUnit.transform.localRotation = _slots[slotIndex].localRotation;
        });
        productUnit.transform.DOLocalJump(targetPos, _jumpForce, 1, _speed)
        .OnComplete(() =>
        {
            productUnit.IsMoving = false;
            productUnit.transform.localPosition = targetPos;
            OnChangedProducts(productUnit.productData.id);
        });
    }

    public override void AddProduct(ProductUnit productUnit)
    {
        _productUnits.Add(productUnit);
        tray_Customer?.SetActive(ProductUnits.Count > 0);
    }
    public override void RemoveProduct(ProductUnit productUnit)
    {
        _productUnits.Remove(productUnit);
        OnChangedProducts(productUnit.productData.id) ;
    }
    protected override void OnChangedProducts(string productName)
    {
        tray_Customer?.SetActive(ProductUnits.Count > 0);
        for (int i = 0; i < _productUnits.Count; i++)
        {
            int slotIndex = i % BaseCapacity;
            int row = i / BaseCapacity;
            Vector3 targetPos = _slots[slotIndex].localPosition + _offset * row;
            _productUnits[i].transform.localPosition = targetPos;
            _productUnits[i].transform.localScale = Vector3.one;
        }
    }

    public bool isDoneEating = false;
    public IEnumerator IEatting()
    {
        isDoneEating = false;
        CustomerUnit customerUnit = GetComponent<CustomerUnit>();
        for (int i = _productUnits.Count-1; i>=0; i--)
        {
            yield return new WaitForSeconds(3f);
            SimplePool.Despawn(_productUnits[i].gameObject);
            MapController.Instance.SpawnPieceTrash(customerUnit.seatPoint.tableArea.piecesGroup);
            Transform _trMoneyArea = customerUnit.seatPoint.tableArea.moneyArea;
            if (_trMoneyArea.childCount > 0)
            {

                try
                {
                    CashBox _cashbox = _trMoneyArea.GetComponentsInChildren<CashBox>().First(x => !x.isPickingCash);
                    if (_cashbox != null)
                    {
                        _cashbox.AddOn(3);
                    }
                    else
                    {
                        GameObject g = Instantiate(customerUnit.cashBox.gameObject, _trMoneyArea.position, Quaternion.identity);
                        g.transform.SetParent(_trMoneyArea);

                        _cashbox = g.GetComponent<CashBox>();
                        _cashbox.AddOn(3);
                    }
                }
                catch (Exception e)
                {
                    continue;
                }

            }
            else
            {
                GameObject g = Instantiate(customerUnit.cashBox.gameObject, _trMoneyArea.position, Quaternion.identity);
                g.transform.SetParent(_trMoneyArea);

                CashBox _cashbox = g.GetComponent<CashBox>();
                _cashbox.AddOn(3);
            }
        }
        yield return null;
        tray_Customer?.SetActive(false);
        yield return null;
        isDoneEating = true;
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
        for (int i = 1; i < _slots.Count; i++)
        {
            Transform g = _slots[i];
            g.position = new Vector3(vtBase.x, g.position.y + addOn_Y * (i - 1), vtBase.z);

        }
    }
}
