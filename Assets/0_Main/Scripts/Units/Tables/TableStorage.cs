using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TableStorage : StorageBase
{
    public TableCreateArea tableCreate => unitBase as TableCreateArea;
    public void Action_GetProduct(CustomerPickup customer)
    {
        //Debug.Log(this.gameObject.name+"---"+ customer.name);
        for (int i = ProductUnits.Count - 1; i >= 0; i--)
        {
            if (ProductUnits[i].IsMoving)
            {
                continue;
            }
            bool check = customer.PickUp(ProductUnits[i], 0);
            if (check)
            {
                RemoveProduct(ProductUnits[i]);
            }
        }
    }

    protected override void OnTriggerStay(Collider other)
    {

    }

    public override void AddProduct(ProductUnit productUnit)
    {
        int index = _productUnits.Count;
        _productUnits.Add(productUnit);

        productUnit.transform.SetParent(_productParent);
        Vector3 targetPos = _slots[index].localPosition;
        productUnit.transform.localPosition = targetPos;
        productUnit.transform.localRotation = Quaternion.identity;
        OnChangedProducts();
    }

    bool isEating;
    public bool isDoneEat;
    public void Eating()
    {
        if (!isEating)
        {
            StartCoroutine(IEating());
        }
       
    }

    IEnumerator IEating()
    {
        isEating = true;
        isDoneEat = false;
        yield return null;
        while (_productUnits.Count > 0)
        {
            float speedEat = tableCreate.speedEat + GameManager.Instance.m_DataConfigRemote.speedBonus_Eat;
            if (MapController.Instance.eatBooster)
                speedEat /= 1.5f;

            //Debug.LogError(speedEat);
            yield return new WaitForSeconds(speedEat);
            ProductUnit _product = _productUnits[0];
            RemoveProduct(_product);

            yield return null;
            MapController.Instance.SpawnPieceTrash(tableCreate.piecesGroup);
            Transform _trMoneyArea = tableCreate.moneyArea;
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
                        GameObject g = Instantiate(AssetConfigs.Instance.cashBox.gameObject, _trMoneyArea.position, Quaternion.identity);
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
                GameObject g = Instantiate(AssetConfigs.Instance.cashBox.gameObject, _trMoneyArea.position, Quaternion.identity);
                g.transform.SetParent(_trMoneyArea);

                CashBox _cashbox = g.GetComponent<CashBox>();
                _cashbox.AddOn(3);
            }
        }

        isEating = false;
        isDoneEat = true;
        tableCreate.SetStage(ETableState.GotTrash);
    }

    protected override void RemoveProduct(ProductUnit productUnit)
    {
        base.RemoveProduct(productUnit);
        SimplePool.Despawn(productUnit.gameObject);
    }

    protected override void OnChangedProducts()
    {
        _productParent.gameObject.SetActive(ProductUnits.Count > 0);
        for (int i = 0; i < _productUnits.Count; i++)
        {
            //Vector3 productOffset = new Vector3(0, _productUnits[i].BoxSize.y, 0);
            // Vector3 targetPos = _originalSlot.localPosition + productOffset * i;
            Vector3 targetPos = GetLastPosition(i);
            _productUnits[i].transform.localPosition = targetPos;
            _productUnits[i].transform.localScale = Vector3.one;
        }

    }

    private Vector3 GetLastPosition(int index)
    {
        Vector3 result = Vector3.zero;
        if (index >= 0)
        {
            for (int i = 0; i < index; i++)
            {
                Vector3 productOffset = new Vector3(0, _productUnits[i].BoxSize.y, 0);
                result += productOffset;
            }
        }
        return result;
    }
}
