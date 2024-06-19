using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WorkerPickUp : BasePickAndDrop
{
    [Space, Header("SLOT DATA")]
    [SerializeField] private Transform _originalSlot;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private GameObject objMax;
    public EProductType productType { get { return workerUnit.workerTask!=null? workerUnit.workerTask.productType : EProductType.None; } set { } }
    WorkerUnit workerUnit => GetComponent<WorkerUnit>();
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

    private void OnTriggerStay(Collider other)
    {
        BasePickAndDrop pickupAndDropBase = other.GetComponent<BasePickAndDrop>();

        if (pickupAndDropBase is CounterPickup ||
            pickupAndDropBase is PackagePickup)
        {

            for (int i = ProductUnits.Count - 1; i >= 0; i--)
            {
                if (ProductUnits[i].IsMoving)
                {
                    continue;
                }
                bool check = pickupAndDropBase.PickUp(ProductUnits[i], carryStack);

                if (check)
                {
                    RemoveProduct(ProductUnits[i]);
                }
                if (i == 0 && check)
                {
                    //Debug.LogError("Play sound ");
                    //SoundManager.Instance.PlaySFX(SoundDefine.DROP_ITEM);
                }
            }
            return;
        }

        if (pickupAndDropBase is TrashBox)
        {
            //Debug.LogError("Drop trash box");
            if (ProductUnits.Count > 0)
            {
                for (int i = ProductUnits.Count - 1; i >= 0; i--)
                {
                    if (ProductUnits[i].IsMoving)
                    {
                        continue;
                    }
                    bool check = pickupAndDropBase.PickUp(ProductUnits[i], 100);
                    if (check)
                    {
                        RemoveProduct(ProductUnits[i]);
                    }
                    if (i == 0 && check)
                    {
                        //Debug.LogError("Play sound ");
                        //SoundManager.Instance.PlaySFX(SoundDefine.DROP_ITEM);
                    }
                }
            }
        }
    }

    public int carryStack => this.GetComponent<WorkerUnit>().m_WorkerData.CarryStack;
    public override bool PickUp(ProductUnit productUnit, int maxQuantity)
    {
        if (_isCountdown&& productType != EProductType.trash)
        {
            return false;
        }

        if ((productType != EProductType.None) && (productUnit.typeProduct != productType))
        {
            return false;
        }

        if (productType != EProductType.trash && _productUnits.Count > maxQuantity)
        {
            return false;
        }

        if (_productUnits.Contains(productUnit))
        {
            return false;
        }


        CountdownForNextPick();

        int offsetMult = _productUnits.Count;

        productUnit.transform.SetParent(_originalSlot);
        Vector3 productOffset = new Vector3(0, productUnit.BoxSize.y, 0);
        // Vector3 targetPos = _originalSlot.localPosition + productOffset * offsetMult;
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
            });

        return true;
    }

    public bool PickUpTrash(TrashPiece _trash)
    {
        return PickUp(_trash, 100);
    }

    public override void AddProduct(ProductUnit productUnit)
    {
        _productUnits.Add(productUnit);
        _productParent.gameObject.SetActive(ProductUnits.Count > 0);

        //if (_productUnits.Count > 0)
        //    productType = productUnit.typeProduct;
    }
    public override void RemoveProduct(ProductUnit productUnit)
    {
        _productUnits.Remove(productUnit);
        OnChangedProducts(productUnit.productData.id);

        //if (_productUnits.Count <= 0)
        //    productType = EProductType.None;

    }
    protected override void OnChangedProducts(string productName)
    {
        _productParent.gameObject.SetActive(ProductUnits.Count > 0);
        for (int i = 0; i < _productUnits.Count; i++)
        {
            Vector3 productOffset = new Vector3(0, _productUnits[i].BoxSize.y, 0);
            // Vector3 targetPos = _originalSlot.localPosition + productOffset * i;
            Vector3 targetPos = GetLastPosition(i);
            _productUnits[i].transform.localPosition = targetPos;
            _productUnits[i].transform.localScale = Vector3.one;
        }

        workerUnit.CarryState(ProductUnits.Count);
        //objMax?.gameObject.SetActive(ProductUnits.Count >= workerUnit.m_WorkerData.CarryStack);
    }
}
