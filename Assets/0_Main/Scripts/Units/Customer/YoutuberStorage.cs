using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using System;

public class YoutuberStorage : BasePickAndDrop
{
    public YoutuberUnit youtuberUnit;
    public float _scaleRate = 1.2f;
    private Vector3 baseScale;
    [SerializeField] private Transform _originalSlot;
    public SpecialData specialData => youtuberUnit.specialData;
    protected override void OnEnable()
    {
        base.OnEnable();
        baseScale = this.transform.localScale;

        _productUnits.Clear();
        for (int i = 0; i < _productParent.childCount; i++)
        {
            SimplePool.Despawn(_productParent.GetChild(i).gameObject);
        }
    }

    public void Clean()
    {
        _productUnits.Clear();
        for (int i = 0; i < _productParent.childCount; i++)
        {
            SimplePool.Despawn(_productParent.GetChild(i).gameObject);
        }

        _originalSlot.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (youtuberUnit.CurrentState != ECustomerState.WaitingCheckOut)
            return;

        PlayerCtrl player = other.gameObject.GetComponent<PlayerCtrl>();
        if (player)
        {
            transform.DOScale(baseScale * _scaleRate, 0.2f);
            transform.GetComponent<SpriteRenderer>().color = Color.green;

            UIMainGame.Instance.Show_UISpecial(specialData, (int)youtuberUnit.waitingTime);
        }
    }

    private void OnTriggerStay(Collider other)
    {

    }

    private void OnTriggerExit(Collider other)
    {
        PlayerCtrl player = other.gameObject.GetComponent<PlayerCtrl>();
        if (player)
        {
            transform.DOScale(baseScale, 0.2f);
            transform.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }


    public override bool PickUp(ProductUnit productUnit, int maxQuantity)
    {
        if (_isCountdown)
        {
            return false;
        }

        if (productUnit.typeProduct != EProductType.sandwich)
            return false;

        if (specialData.specialProduct.isFull)
        {
            return false;
        }

        bool check = true;
        if (check)
        {
            CountdownForNextPick();
            specialData.specialProduct.numberfill++;

            int offsetMult = _productUnits.Count;

            productUnit.transform.SetParent(_originalSlot);
            Vector3 productOffset = new Vector3(0, productUnit.BoxSize.y, 0);
            Vector3 targetPos = GetLastPosition(offsetMult);
            AddProduct(productUnit);

            productUnit.IsMoving = true;
            productUnit.transform.DOLocalRotate(Vector3.zero, _speed);
            productUnit.transform.DOLocalJump(targetPos, _jumpForce, 1, _speed)
                .OnComplete(() =>
                {
                    productUnit.IsMoving = false;
                    productUnit.transform.localPosition = targetPos;
                    OnChangedProducts(productUnit.productData.id);

                    Module.Action_Event_Special_Quest(specialData);
                });
            return true;
        }
        return false;
    }


    public override void AddProduct(ProductUnit productUnit)
    {
        _productUnits.Add(productUnit);

    }

    public override void RemoveProduct(ProductUnit productUnit)
    {
        _productUnits.Remove(productUnit);
        OnChangedProducts("");
    }

    public void Eating()
    {
        if (!isEating)
        {
            StartCoroutine(IEating());
        }

    }

    bool isEating;
    public bool isDoneEat;
    IEnumerator IEating()
    {
        isEating = true;
        isDoneEat = false;
        yield return null;
        while (_productUnits.Count > 0)
        {
            float speedEat = 1f;
            if (MapController.Instance.eatBooster)
                speedEat /= 1.5f;

            //Debug.LogError(speedEat);
            yield return new WaitForSeconds(speedEat);
            ProductUnit _product = _productUnits[0];
            RemoveProduct(_product);

            yield return null;
            MapController.Instance.SpawnPieceTrash(youtuberUnit.table.piecesGroup);
            Transform _trMoneyArea = youtuberUnit.table.moneyArea;
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
        
    }

    protected override void OnChangedProducts(string productName)
    {
        _productParent.gameObject.SetActive(ProductUnits.Count > 0);
        for (int i = 0; i < _productUnits.Count; i++)
        {
            Vector3 productOffset = new Vector3(0, _productUnits[i].BoxSize.y, 0);
            Vector3 targetPos = GetLastPosition(i);
            _productUnits[i].transform.DOLocalMove(targetPos, 0.1f);
            _productUnits[i].transform.localScale = new Vector3(-1, 1, 1);
        }

       // youtuberUnit.CarryState(ProductUnits.Count);
    }

    private Vector3 GetLastPosition(int index)
    {
        Vector3 result = _originalSlot.localPosition;
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
