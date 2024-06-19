using com.adjust.sdk;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustTracking : Singleton<AdjustTracking>
{
    
    public const string str_ad_impression       = "jfvz7n";
    public const string str_fbs_ad_impression   = "oz6n1i";
    public const string str_ads_banner_click    = "gjg5kg";
    public const string str_ads_inter_request   = "sl8w7b";
    public const string str_ads_inter_failed    = "799iba";
    public const string str_ads_inter_show      = "kqyyk8";
    public const string str_ads_inter_complete  = "ix9brn";
    public const string str_ads_reward_request  = "b0rfp2";
    public const string str_ads_reward_failed   = "kl1jje";
    public const string str_ads_reward_show     = "hnusck";
    public const string str_ads_reward_complete = "d9x4ke";
    public const string str_ads_open_request    = "60q3sk";
    public const string str_ads_open_show       = "f3hvso";
    public const string str_ads_open_failed     = "24jkpv";
    public const string str_ads_open_complete   = "8dah1n";
    public const string str_click_setting       = "mfew11";
    public const string str_click_remove_ads    = "jhq1sy";
    public const string str_remove_ads_success  = "ahi8vx";
    public const string str_delete_app          = "ad5big";
    public const string str_rate_star           = "s7ulsm";
    public const string str_click_open_shop     = "suqgqo";
    public const string str_step_tutorial       = "ovnity";
    public const string str_table_unlock        = "4bb3br";
    public const string str_click_game_update   = "of6vzz";
    public const string str_click_cancel_game_update = "c4ysxb";


    public override void InitAwake()
    {
        AdjustConfig config = new AdjustConfig("s29rzrzqm6tc", AdjustEnvironment.Production, true);
        config.setLogLevel(AdjustLogLevel.Verbose);
        Adjust.start(config);

       
    }

    public void Event_ad_impression(MaxSdk.AdInfo adInfo)
    {
        AdjustAdRevenue adjustEvent = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceAppLovinMAX);

        adjustEvent.setRevenue(adInfo.Revenue, "USD");
        adjustEvent.setAdImpressionsCount(10);
        adjustEvent.setAdRevenueNetwork(adInfo.NetworkName);
        adjustEvent.setAdRevenuePlacement(adInfo.Placement);

        adjustEvent.addCallbackParameter("ad_platform", "AppLovin");
        adjustEvent.addPartnerParameter("adFormat", adInfo.AdFormat);
        adjustEvent.addPartnerParameter("network_name", adInfo.NetworkName);
        adjustEvent.addCallbackParameter("value", adInfo.Revenue.ToString());
    
      
        Adjust.trackAdRevenue(adjustEvent);

   
    }

    public void Event_sub_appstore()
    {
        AdjustAppStoreSubscription subscription = new AdjustAppStoreSubscription( "price", "currency", "transactionId",  "receipt");
        subscription.setTransactionDate("transactionDate");
        subscription.setSalesRegion("salesRegion");

        // add callback parameters
        subscription.addCallbackParameter("key", "value");
        subscription.addCallbackParameter("foo", "bar");

        // add partner parameters
        subscription.addPartnerParameter("key", "value");
        subscription.addPartnerParameter("foo", "bar");

        Adjust.trackAppStoreSubscription(subscription);
    }

    public void Event_ads_banner_click()
    {
        AdjustEvent adjustEvent = new AdjustEvent(str_ads_banner_click);

        Adjust.trackEvent(adjustEvent);
    }

    public void Event_ads_inter_request()
    {
        AdjustEvent adjustEvent = new AdjustEvent(str_ads_inter_request);

        Adjust.trackEvent(adjustEvent);
    }

    public void Event_ads_inter_failed()
    {
        AdjustEvent adjustEvent = new AdjustEvent(str_ads_inter_failed);

        Adjust.trackEvent(adjustEvent);
    }

    public void Event_ads_inter_show()
    {
        AdjustEvent adjustEvent = new AdjustEvent(str_ads_inter_show);

        Adjust.trackEvent(adjustEvent);
    }

    public void Event_ads_inter_complete()
    {
        AdjustEvent adjustEvent = new AdjustEvent(str_ads_inter_complete);

        Adjust.trackEvent(adjustEvent);
    }  

    public void Event_ads_reward_request()
    {
        AdjustEvent adjustEvent = new AdjustEvent(str_ads_reward_request);

        Adjust.trackEvent(adjustEvent);
    }

    public void Event_ads_reward_failed()
    {
        AdjustEvent adjustEvent = new AdjustEvent(str_ads_reward_failed);

        Adjust.trackEvent(adjustEvent);
    }

    public void Event_ads_reward_show()
    {
        AdjustEvent adjustEvent = new AdjustEvent(str_ads_reward_show);

        Adjust.trackEvent(adjustEvent);
    }
    public void Event_ads_reward_complete(MaxSdkBase.AdInfo adInfo)
    {
        AdjustEvent adjustEvent = new AdjustEvent(str_ads_reward_complete);
        adjustEvent.addCallbackParameter("reward", adInfo.AdUnitIdentifier);
        adjustEvent.addCallbackParameter("network_name", adInfo.NetworkName);
        Adjust.trackEvent(adjustEvent);
    }
    public void Event_ads_open_request()
    {
        AdjustEvent adjustEvent = new AdjustEvent(str_ads_open_request);

        Adjust.trackEvent(adjustEvent);
    }
    public void Event_ads_open_show()
    {
        AdjustEvent adjustEvent = new AdjustEvent(str_ads_open_show);

        Adjust.trackEvent(adjustEvent);
    }
    public void Event_ads_open_failed()
    {
        AdjustEvent adjustEvent = new AdjustEvent(str_ads_open_failed);

        Adjust.trackEvent(adjustEvent);
    }
    public void Event_ads_open_complete(MaxSdkBase.AdInfo adInfo)
    {
        AdjustEvent adjustEvent = new AdjustEvent(str_ads_open_complete);
        adjustEvent.addCallbackParameter("network_name", adInfo.NetworkName);
        Adjust.trackEvent(adjustEvent);
    }
    public void Event_click_setting()
    {
        AdjustEvent adjustEvent = new AdjustEvent(str_click_setting);

        Adjust.trackEvent(adjustEvent);
    }
    public void Event_click_remove_ads()
    {
        AdjustEvent adjustEvent = new AdjustEvent(str_click_remove_ads);

        Adjust.trackEvent(adjustEvent);
    }
    public void Event_remove_ads_success()
    {
        AdjustEvent adjustEvent = new AdjustEvent(str_remove_ads_success);

        Adjust.trackEvent(adjustEvent);
    }
    public void Event_delete_app()
    {
        AdjustEvent adjustEvent = new AdjustEvent(str_delete_app);

        Adjust.trackEvent(adjustEvent);
    }

    public void Event_click_game_update()
    {
        AdjustEvent adjustEvent = new AdjustEvent(str_click_game_update);

        Adjust.trackEvent(adjustEvent);
    }

    public void Event_click_cancel_game_update()
    {
        AdjustEvent adjustEvent = new AdjustEvent(str_click_cancel_game_update);

        Adjust.trackEvent(adjustEvent);
    }

    public void Event_click_open_shop()
    {
        AdjustEvent adjustEvent = new AdjustEvent(str_click_open_shop);

        Adjust.trackEvent(adjustEvent);
    }

    public void Event_step_tutorial(string _idTut)
    {
        AdjustEvent adjustEvent = new AdjustEvent(str_step_tutorial);
        adjustEvent.addCallbackParameter("tutorial", _idTut);
        Adjust.trackEvent(adjustEvent);
    }

    public void Event_table_unlock(string _idtable)
    {
        AdjustEvent adjustEvent = new AdjustEvent(str_table_unlock);
        adjustEvent.addCallbackParameter("table", _idtable);
        Adjust.trackEvent(adjustEvent);
    }

    public void Log_Event(string _strEvent)
    {

        AdjustEvent adjustEvent = new AdjustEvent("abc123");
        adjustEvent.addCallbackParameter("key", "value");
        adjustEvent.addCallbackParameter("foo", "bar");
        Adjust.trackEvent(adjustEvent);
    }

  
}
