using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerPickup : BasePickAndDrop
{
    [Space, Header("SLOT DATA")]
    [SerializeField] private Transform _originalSlot;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private EProductType productType;
    [SerializeField] private PlayerCtrl playerCtrl;
    [SerializeField] private GameObject objMax;
    [SerializeField] private float moneySpeed=0.25f;

    private void OnTriggerStay(Collider other)
    {
        BasePickAndDrop pickupAndDropBase = other.GetComponent<BasePickAndDrop>();

        if (pickupAndDropBase is CounterPickup||
            pickupAndDropBase is PackagePickup)
        {
            if (ProductUnits.Count > 0)
                switch (ProductUnits[0].typeProduct)
                {
                    case EProductType.sandwich:
                        MapController.Instance.CompleteTut("tut_07");
                        break;
                    default:
                        break;
                }

            for (int i = ProductUnits.Count - 1; i >= 0; i--)
            {
                if (ProductUnits[i].IsMoving)
                {
                    continue;
                }
                bool check = pickupAndDropBase.PickUp(ProductUnits[i], 0);

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

  
        if (pickupAndDropBase is ShipperStorage)
        {
            ShipperStorage s = pickupAndDropBase as ShipperStorage;
            if (s.shipperUnit.CurrentState != ECustomerState.WaitingCheckOut)
                return;

            for (int i = ProductUnits.Count - 1; i >= 0; i--)
            {
                if (ProductUnits[i].IsMoving)
                {
                    continue;
                }
                bool check = pickupAndDropBase.PickUp(ProductUnits[i], 0);

                if (check)
                {
                    RemoveProduct(ProductUnits[i]);
                }
            }

            return;
        }

        if(pickupAndDropBase is YoutuberStorage)
        {
            YoutuberStorage s = pickupAndDropBase as YoutuberStorage;
            if (s.youtuberUnit.CurrentState != ECustomerState.WaitingCheckOut)
                return;

            for (int i = ProductUnits.Count - 1; i >= 0; i--)
            {
                if (ProductUnits[i].IsMoving)
                {
                    continue;
                }
                bool check = pickupAndDropBase.PickUp(ProductUnits[i], 0);

                if (check)
                {
                    RemoveProduct(ProductUnits[i]);
                }
            }

            return;
        }


        if (pickupAndDropBase is TrashBox)
        {
            //Debug.LogError("Drop trash box");
            if (ProductUnits.Count > 0)
            {
                for (int i =ProductUnits.Count - 1; i >= 0; i--)
                {
                    if (ProductUnits[i].IsMoving)
                    {
                        continue;
                    }
                    bool check = pickupAndDropBase.PickUp(ProductUnits[i], 0);
                    if (check)
                    {
                        RemoveProduct(ProductUnits[i]);
                    }
                }
            }
        }
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

    public override bool PickUp(ProductUnit productUnit, int maxQuantity)
    {
        if (_isCountdown&& productType != EProductType.trash)
        {
            return false;
        }

        if (productType == EProductType.trash)
            MapController.Instance.CompleteTut("tut_09");


        if ((productType != EProductType.None)&&(productUnit.typeProduct != productType))
        {
            if (_productUnits.Count <= 0)
                productType = EProductType.None;

            return false;
        }

        if (productType != EProductType.trash&&_productUnits.Count >= this.GetComponent<PlayerCtrl>().m_PlayerData.CarryStack)
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
        SoundManager.Instance.PlayOnCamera();
        productUnit.transform.DOLocalRotate(Vector3.zero, _speed);
        productUnit.transform.DOLocalJump(targetPos, _jumpForce, 1, _speed)
            .OnComplete(() =>
            {
                productUnit.IsMoving = false;
                productUnit.transform.localPosition = targetPos;
                OnChangedProducts(productUnit.productData.id);
            });

        switch (productType)
        {
            case EProductType.sandwich:
                MapController.Instance.CompleteTut("tut_06");
                MapController.Instance.CompleteTut("tut_17");
                break;
            case EProductType.sandwich_pakage:
                MapController.Instance.CompleteTut("tut_19");
                break;
            default:
                break;
        }

        return true;
    }

    public Ease ease;
    public bool PickUpCash(Cash cash)
    {
        cash.transform.SetParent(transform);
        cash.transform.DOScaleY(4f, moneySpeed/2);
        SoundManager.Instance.PlayOnCamera(1);

        cash.transform.DOLocalRotate(Vector3.zero, moneySpeed).SetEase(ease);
        //Vector3 targetPos = _originalSlot.localPosition + _offset;
        cash.transform.DOLocalJump(Vector3.up, _jumpForce, 1, moneySpeed).SetEase(ease)
            .OnComplete(() =>
            {
                Destroy(cash.gameObject);
                if (cash.value < 50)
                    Module.LowVibrate();
                else if (50 < cash.value && cash.value < 100)
                    Module.MediumVibrate();
                else
                    Module.HardVibrate();

                cash.transform.position = Vector3.zero;
                cash.transform.rotation = Quaternion.identity;
                Module.money_currency += cash.value;

               
               
            });

        return true;
    }

    public bool PickUpTrash(TrashPiece _trash)
    {
      
        return PickUp(_trash, playerCtrl.m_PlayerData.CarryStack);
    }

    public override void AddProduct(ProductUnit productUnit)
    {
        _productUnits.Add(productUnit);
        _productParent.gameObject.SetActive(ProductUnits.Count > 0);

        if(_productUnits.Count>0)
            productType = productUnit.typeProduct;
    }
    public override void RemoveProduct(ProductUnit productUnit)
    {
        _productUnits.Remove(productUnit);
        SoundManager.Instance.PlayOnCamera();
        OnChangedProducts(productUnit.productData.id);

        if (_productUnits.Count <= 0)
            productType = EProductType.None;

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

        if (productType != EProductType.trash)
        {
            objMax?.SetActive(ProductUnits.Count >= playerCtrl.m_PlayerData.CarryStack);
        }
        else
            objMax?.SetActive(false);

        playerCtrl.CarryState(ProductUnits.Count);
    }
}
