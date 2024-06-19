using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ProductButton : MonoBehaviour
{
    public string productID; //Ten trung voi ten co trong Name list product IAP
    [SerializeField] private TextMeshProUGUI price;
    [SerializeField] UIButton btnBuy;

    public bool IsInit = true;
    public Action actionCallBack;

    private void Awake()
    {
        if(IsInit) ReloadCost();
    }

    public void CallStart(string _productID)
    {
        productID = _productID;
        ReloadCost();
    }

    public void SetActionCallBack(Action _callback)
    {
        actionCallBack = _callback;
    }

    private void OnEnable()
    {
        btnBuy.SetUpEvent(OnClick);
    }

    public bool UpdateData(string _productID)
    {
        bool sucsses = false;
        if (PurchaserManager.Instance == null) return false;
        foreach (var item in PurchaserManager.Instance.productIAP)
        {
            if (item.id == _productID)
            {
                if (PurchaserManager.Instance.IsInitialized())
                {
                    if (price != null)
                    {
                        string priceStr = PurchaserManager.m_StoreController.products.WithID(item.id).metadata.localizedPriceString.ToString();
                        if (priceStr != "")
                        {
                            price.text = priceStr;
                            sucsses = true;
                        }
                    }
                }
                productID = item.id;
                break;
            }
        }
        return sucsses;
    }

    private void ReloadCost()
    {
        if (!UpdateData(productID))
            Invoke("ReloadCost", 1);
    }

    public void OnClick()
    {
        Debug.Log(gameObject.name);
        if (PurchaserManager.Instance != null)
        {
            if (productID != "")
            {
                PurchaserManager.Instance.BuyProductID(productID,actionCallBack);
            }
        }
    }
}
