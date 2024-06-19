using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;

public class PurchaserManager : Singleton<PurchaserManager>, IStoreListener
{
    public static IStoreController m_StoreController;
    private static IExtensionProvider m_StoreExtensionProvider;
    public string AndroidKey = "";
    public List<ProductIAP> productIAP;
    private Action onSuccess;

    void Start()
    {
        if (m_StoreController == null)
        {
            InitializePurchasing();
        }
    }

    public bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            return;
        }
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        //builder.Configure<IGooglePlayConfiguration>().SetPublicKey(AndroidKeyVIP);

        //KHOI TAO CAC TEN MAT HANG TREN CAC STORE
        foreach (var item in productIAP)
        {
            builder.AddProduct(item.id, item.product_type, new IDs(){
                {item.id, AppleAppStore.Name.ToString()},//dinh nghia them cho ro rang cac store, cung khong can thiet + IOS
                {item.id, GooglePlay.Name.ToString()},
                //{item.ID, FacebookStore.Name.ToString()},
                {item.id, WindowsStore.Name.ToString()},
                {item.id, AmazonApps.Name.ToString()},
                //{item.ID, TizenStore.Name.ToString()},
                //{item.ID, XiaomiMiPay.Name.ToString()},
                //{item.ID, MoolahAppStore.Name.ToString()},
                {item.id, MacAppStore.Name.ToString()}
            });
        }
        UnityPurchasing.Initialize(this, builder);
    }


    public void BuyProductID(string productId, Action onSuccess = null)
    {
        this.onSuccess = onSuccess;
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(productId);

            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                m_StoreController.InitiatePurchase(product);
                //FirebaseManager.Instance.LogEvent_trackIAP_CLICK(product.definition.storeSpecificId);
            }
            else
            {
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {
            Debug.Log("BuyProductID FAIL. Not initialized.");
            InitializePurchasing();
        }
    }
    public string GetPriceProduct(string productId)
    {
        Product product = m_StoreController.products.WithID(productId);
        if (product != null && product.availableToPurchase)
        {
            Debug.Log("price product " + product.metadata.localizedPriceString);
            return product.metadata.localizedPriceString;
        }
        return null;
    }
    public void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) =>
            {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
            RestoreIAP();
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("OnInitialized: PASS");
        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
    }

    bool allowRetore = false;
    public void RestoreIAP()
    {
        if (!allowRetore) return;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnInitializeFailed");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        Debug.Log(args.purchasedProduct.definition.id);
        foreach (var item in productIAP)
        {
            if (String.Equals(args.purchasedProduct.definition.id, item.id, StringComparison.Ordinal))
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                GameManager.Instance.userModel.Add_IAP(item.id);
                //
                switch (item.type)
                {
                    case ModificationType.remove_ads:
                        Module.isRemoveAds = 1;
                        UIMainGame.Instance.Action_RemoveAds();
                        FirebaseManager.Instance.LogEvent_RemoveAdsSuccess();
                        break;
                    case ModificationType.vip_subscription:
                        Module.isVip_subscription = 1;
                        Module.isRemoveAds = 1;
                        UIMainGame.Instance.Action_RemoveAds();
                        Module.ticket_currency += 3;
                        GameManager.Instance.skinModel.UnlockSkin(ETypeSkin.chef);
                        break;
                    case ModificationType.sub_manager:
                        Module.Action_Event_RefreshBattlePass();
                        break;
                    case ModificationType.sub_ceo:
                        Module.Action_Event_RefreshBattlePass();
                        break;

                    case ModificationType.premium_pro:
                        Module.isRemoveAds = 1;
                        UIMainGame.Instance.Action_RemoveAds();
                        Module.ticket_currency += 20;

                        break;
                    case ModificationType.ads_free_pack:
                        Module.isRemoveAds = 1;
                        UIMainGame.Instance.Action_RemoveAds();
                        Module.ticket_currency += 20;

                        break;
                    case ModificationType.growth_offer:
                        Module.isRemoveAds = 1;
                        UIMainGame.Instance.Action_RemoveAds();

                        break;
                    case ModificationType.ticket_pack_1:
                        Module.ticket_currency += 20;
                        break;
                    case ModificationType.ticket_pack_2:
                        Module.ticket_currency += 60;
                        break;
                    case ModificationType.ticket_pack_3:
                        Module.ticket_currency += 95;
                        break;
                    case ModificationType.ticket_pack_4:
                        Module.ticket_currency += 150;
                        break;
                    case ModificationType.ticket_pack_5:
                        Module.ticket_currency += 330;
                        break;
                    case ModificationType.ticket_pack_6:
                        Module.ticket_currency += 550;
                        break;
                    default:
                        break;
                }
                onSuccess?.Invoke();
                break;
            }
        }



        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        throw new NotImplementedException();
    }

    [ContextMenu("Generate")]
    public void Generate()
    {
        IAPModel model = Resources.Load<IAPModel>("IAP");
        productIAP = model.productIAP.ToList();
    }

}
[System.Serializable]
public class ProductIAP
{
    public string id;
    public string name;
    public ProductType product_type;
    public ModificationType type;
    public int count = 1;
}

public enum ModificationType
{
    remove_ads,
    vip_subscription,
    premium_pro,
    ads_free_pack,
    growth_offer,
    money_pack_1,
    money_pack_2,
    money_pack_3,
    money_pack_4,
    money_pack_5,
    money_pack_6,
    ticket_pack_1,
    ticket_pack_2,
    ticket_pack_3,
    ticket_pack_4,
    ticket_pack_5,
    ticket_pack_6,
    sub_manager,
    sub_ceo
}
