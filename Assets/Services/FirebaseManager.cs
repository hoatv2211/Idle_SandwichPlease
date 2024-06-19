using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using Firebase.Messaging;
using Firebase.RemoteConfig;
using System;
using Firebase.Crashlytics;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;

public class FirebaseManager : Singleton<FirebaseManager>
{
    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
    protected bool firebaseInitialized = false;
    public bool remoteInitialized = false;

    private string id_map => "map_" + MapController.Instance._idMap.ToString("00");
    private string level_user => "level_" + UserModel.Instance.level;
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError(
                  "Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });


    }

    #region Inits
    void InitializeFirebase()
    {
        Debug.Log("Enabling data collection.");
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

        Debug.Log("Set user properties.");
        // Set the user's sign up method.
        FirebaseAnalytics.SetUserProperty(
          FirebaseAnalytics.UserPropertySignUpMethod, "Google");
        
        // Set the user ID.
        FirebaseAnalytics.SetUserId(Module.idUser);
        // Set default session duration values.
        FirebaseAnalytics.SetSessionTimeoutDuration(new TimeSpan(0, 30, 0));
        firebaseInitialized = true;
        AnalyticsLogin();
        InitsMessenge();
        InitsFirebaseRemote();
    }


    public void AnalyticsLogin()
    {
        // Log an event with no parameters.
        Debug.Log("Logging a login event.");
        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLogin);
        FirebaseAnalytics.LogEvent("fbs_login");


        LogEvent_Session_Start();
    }

    #endregion

    #region Removes
    /// <summary>
    /// Đếm số resources (tiền, ticket, vật phẩm) user spend
    /// </summary>
    /// <param name="resource_id">ID loại resource user spend</param>
    /// <param name="resource_name">Name loại resources user spend</param>
    /// <param name="change_num">Số resource tiêu</param>
    /// <param name="before_num">Số resource trước khi tiêu</param>
    /// <param name="after_num">Số resource sau khi tiêu</param>
    /// <param name="item_id">Item mà user spend vào- Optional</param>
    /// <param name="item_name">Tên item user spend- Optional</param>
    /// <param name="item_price">Giá của item- Optional</param>
    public void LogEvent_spend(string resource_id, string resource_name,
        int change_num, int before_num, int after_num,
        string item_id, string item_name, int item_price)
    {
        //if (Module.isCheater != 0)
        //    return;

        //if (firebaseInitialized)
        //{
        //    string str = "spend";

        //    FirebaseAnalytics.LogEvent(str,
        //        new Parameter("ts",             Module.TimestampNow()),
        //        new Parameter("resource_id",    resource_id),
        //        new Parameter("resource_name",  resource_name),
        //        new Parameter("change_num",     change_num.ToString()),
        //        new Parameter("before_num",     before_num.ToString()),
        //        new Parameter("after_num",      after_num.ToString()),
        //        new Parameter("item_id",        item_id.ToString()),
        //        new Parameter("item_name",      item_name.ToString()),
        //        new Parameter("item_price",     item_price.ToString()),
        //        new Parameter("id_map",         id_map),
        //        new Parameter("time_play ",     Module.time_first_open.ToString()));

        //    Debug.Log(str + ": " + change_num +"/" + item_price);
        //}
    }


    /// <summary>
    /// Đếm số resources (tiền, ticket, vật phẩm) nhận được
    /// </summary>
    /// <param name="resource_id">ID loại resource user nhận</param>
    /// <param name="resource_name">Name loại resources user nhận</param>
    /// <param name="change_num">Số resource nhận</param>
    /// <param name="before_num">Số resource trước khi nhận</param>
    /// <param name="after_num">Số resource sau khi nhận</param>
    /// <param name="item_id">Item mà user tương tác để nhận resources- Optional</param>
    /// <param name="item_name">Tên item mà user tương tác để nhận resources- Optional</param>
    /// <param name="item_price">Giá của item- Optional</param>
    public void LogEvent_receive(string resource_id, string resource_name,
        int change_num, int before_num, int after_num,
        string item_id, string item_name, int item_price)
    {
        //if (Module.isCheater != 0)
        //    return;

        //if (firebaseInitialized)
        //{
        //    string str = "receive";

        //    FirebaseAnalytics.LogEvent(str,
        //        new Parameter("ts",             Module.TimestampNow()),
        //        new Parameter("resource_id",    resource_id),
        //        new Parameter("resource_name",  resource_name),
        //        new Parameter("change_num",     change_num),
        //        new Parameter("before_num",     before_num),
        //        new Parameter("after_num",      after_num),
        //        new Parameter("item_id",        item_id.ToString()),
        //        new Parameter("item_name",      item_name.ToString()),
        //        new Parameter("item_price",     item_price.ToString()),
        //        new Parameter("id_map",         id_map),
        //        new Parameter("time_play ",     Module.time_first_open.ToString()));

        //    Debug.Log(str + ": " + change_num + "/" + item_price);
        //}
    }

    public void LogEvent_AppExit()
    {
        //if (firebaseInitialized)
        //{
        //    string str = "app_exit";
        //    FirebaseAnalytics.LogEvent(str);
        //}
    }

    #endregion

    #region Event Game Update


    /// <summary>
    /// process của map -> khi progress thay đổi
    /// </summary>
    /// <param name="progress">điểm cộng</param>
    /// <param name="current_progress">điểm hiện tại sau khi cộng</param>
    public void LogEvent_map_progress(int progress,int current_progress,string idUnit)
    {
        //if (firebaseInitialized)
        //{
        //    string str = "map_progress";

        //    FirebaseAnalytics.LogEvent(str,
        //        new Parameter("id_map", id_map),
        //        new Parameter("id_unlock",          idUnit),
        //        new Parameter("progress",           progress),
        //        new Parameter("current_progress ",  current_progress));

        //    Debug.Log(str);
        //}
    }


    /// <summary>
    /// Chỉ số liên quan tới staff
    /// </summary>
    /// <param name="event_type"> Dạng type của event update staff (employ, speed, capacity)</param>
    /// <param name="level">Level của event update staff</param>
    /// <param name="unlock_type">Loại action để unlock staff</param>
    public void LogEvent_staff(string event_type, int level, string unlock_type)
    {
        if (firebaseInitialized)
        {
            string str = "staff";

            FirebaseAnalytics.LogEvent(str,
                new Parameter("event_type", event_type),
                new Parameter("level", level),
                new Parameter("id_map", id_map),
                new Parameter("unlock_type ", unlock_type));

            Debug.Log(str);
        }
    }


    /// <summary>
    /// Chỉ số liên quan tới player
    /// </summary>
    /// <param name="event_type"> Dạng type của event update player (profit, speed, capacity)</param>
    /// <param name="level">Level của event update staff</param>
    /// <param name="unlock_type">Loại action để unlock staff</param>
    public void LogEvent_player(string event_type, int level, string unlock_type)
    {
        //if (firebaseInitialized)
        //{
        //    string str = "player";

        //    FirebaseAnalytics.LogEvent(str,
        //        new Parameter("event_type", event_type),
        //        new Parameter("level", level),
        //        new Parameter("id_map", id_map),
        //        new Parameter("unlock_type ", unlock_type));

        //    Debug.Log(str + "--"+ level);
        //}
    }


    /// <summary>
    /// Chỉ số liên quan tới table - Gọi khi mở create table và sau khi created table
    /// </summary>
    /// <param name="id">Item ID</param>
    /// <param name="name">Item name</param>
    /// <param name="state">Trạng thái của event (unlock, unlocked)</param>
    public void LogEvent_table(string id, string name, string state)
    {
        //if (firebaseInitialized)
        //{
        //    string str = "table";

        //    FirebaseAnalytics.LogEvent(str,
        //        new Parameter("id", id),
        //        new Parameter("name", name),
        //        new Parameter("id_map", id_map),
        //        new Parameter("state ", state));

        //    Debug.Log(str);
        //}
    }


    /// <summary>
    /// Chỉ số liên quan tới sandwich_machine
    /// </summary>
    /// <param name="id"> Item ID</param>
    /// <param name="name"> Item name</param>
    /// <param name="state">Trạng thái của event (unlock, unlocked, upgrade)</param>
    /// <param name="level"> Level của item ở event -sau khi thực hiện event thành công</param>
    public void LogEvent_sandwich_machine(string id, string name, string state, int level)
    {
        //if (firebaseInitialized)
        //{
        //    string str = "sandwich_machine";

        //    FirebaseAnalytics.LogEvent(str,
        //        new Parameter("id", id),
        //        new Parameter("id_map", id_map),
        //        new Parameter("name", name),
        //        new Parameter("state ", state),
        //        new Parameter("level ", level));

        //    Debug.Log(str);
        //}
    }


    /// <summary>
    /// Chỉ số liên quan tới drive_thru
    /// </summary>
    /// <param name="id"> Item ID</param>
    /// <param name="name"> Item name</param>
    /// <param name="state">Trạng thái của event (unlock, unlocked, upgrade)</param>
    /// <param name="level"> Level của item ở event -sau khi thực hiện event thành công</param>
    public void LogEvent_drive_thru(string id, string name, string state, int level)
    {
        //if (firebaseInitialized)
        //{
        //    string str = "drive_thru";

        //    FirebaseAnalytics.LogEvent(str,
        //        new Parameter("id_map", id_map),
        //        new Parameter("id", id),
        //        new Parameter("name", name),
        //        new Parameter("state ", state),
        //        new Parameter("level ", level));

        //    Debug.Log(str);
        //}
    }


    /// <summary>
    /// Chỉ số liên quan tới package_table
    /// </summary>
    /// <param name="id"> Item ID</param>
    /// <param name="name"> Item name</param>
    /// <param name="state">Trạng thái của event (unlock, unlocked, upgrade)</param>
    /// <param name="level"> Level của item ở event -sau khi thực hiện event thành công</param>
    public void LogEvent_package_table(string id, string name, string state, int level)
    {
        //if (firebaseInitialized)
        //{
        //    string str = "package_table";

        //    FirebaseAnalytics.LogEvent(str,
        //        new Parameter("id_map", id_map),
        //        new Parameter("id", id),
        //        new Parameter("name", name),
        //        new Parameter("state ", state),
        //        new Parameter("level ", level));

        //    Debug.Log(str);
        //}
    }


    /// <summary>
    /// Chỉ số liên quan tới counter
    /// </summary>
    /// <param name="id"> Item ID</param>
    /// <param name="name"> Item name</param>
    /// <param name="state">Trạng thái của event (unlock, unlocked, upgrade)</param>
    /// <param name="level"> Level của item ở event -sau khi thực hiện event thành công</param>
    public void LogEvent_counter(string id, string name, string state, int level)
    {
        //if (firebaseInitialized)
        //{
        //    string str = "counter";

        //    FirebaseAnalytics.LogEvent(str,
        //        new Parameter("id_map", id_map),
        //        new Parameter("id", id),
        //        new Parameter("name", name),
        //        new Parameter("state ", state),
        //        new Parameter("level ", level));

        //    Debug.Log(str);
        //}
    }


    /// <summary>
    /// Khi nào bấm nút unlock trên map UI
    /// </summary>
    /// <param name="id">MapID</param>
    /// <param name="stage_id">ID Stage của map unlock</param>
    public void LogEvent_map_unlock(int idMap)
    {
        if (firebaseInitialized)
        {
            string str = "map_unlock";

            FirebaseAnalytics.LogEvent(str,
                   new Parameter("id_map", id_map));

            Debug.Log(str);
        }
    }

    #endregion

    #region Event Game Analytic
    public void LogEvent_firebase_purchase(Product product, string section, string pack_name)
    {
        if (firebaseInitialized)
        {
            string str = "firebase_purchase_" + section + "_" + pack_name;

            FirebaseAnalytics.LogEvent(str,
                    new Parameter("fbs_product", product.metadata.localizedTitle),
                    new Parameter("fbs_product_id", product.definition.id),
                    new Parameter("fbs_price", product.metadata.localizedPriceString),
                    new Parameter("fbs_revenue", GetAppsflyerRevenue(product.metadata.localizedPrice)),
                    new Parameter("fbs_currency", "USD"),
                    new Parameter("section", section),
                    new Parameter("id_map", id_map),
                    new Parameter("pack_name", pack_name)
                    );
            Debug.Log(str);
        }
    }

    public static string GetAppsflyerRevenue(decimal amount)
    {
        decimal val = decimal.Multiply(amount, 0.63m);
        return val.ToString();
    }

    // Reset analytics data for this app instance.
    public void ResetAnalyticsData()
    {
        Debug.Log("Reset analytics data.");
        FirebaseAnalytics.ResetAnalyticsData();
    }

  
    //check loading scene
    public void LogEvent_LoadingSceneComplete()
    {
        if (firebaseInitialized)
        {
            string str = "a_loading_scene_complete";
            FirebaseAnalytics.LogEvent(str);
        }
    }

    public void LogEvent_StepTut(string _tut)
    {
        //Debug.LogError(_tut);
        if (firebaseInitialized)
        {
            string str = "a_step_tutorial";
            FirebaseAnalytics.LogEvent(str,
                new Parameter("id_map", id_map),
                new Parameter("step_tutorial", _tut.ToString())
                );
        }
    }

    public void LogEvent_Table(string _table)
    {
        if (firebaseInitialized)
        {
            string str = "a_table_unlock";
            FirebaseAnalytics.LogEvent(str,
                new Parameter("id_map", id_map),
                new Parameter("table_unlock", _table.ToString())
                );
        }
    }

    public void LogEvent_InternetClickRetry()
    {
        if (firebaseInitialized)
        {
            string str = "internet_popup";
            FirebaseAnalytics.LogEvent(str, new Parameter("tut_complete", MapController.Instance.mapData.tutlated));
        }
    }

    public void LogEvent_RemoveAdsSuccess()
    {
        if (firebaseInitialized)
        {
            string str = "removeads_success";
            FirebaseAnalytics.LogEvent(str, new Parameter("id_map", id_map));
        }
    }


    public void LogEvent_Quest(string _type,string _state)
    {
        if (firebaseInitialized)
        {
            string str = "quest";
            FirebaseAnalytics.LogEvent(str,
                new Parameter("id_map", id_map),
                new Parameter("type", _type),
                new Parameter("state", _state));
        }
    }

    public void LogEvent_Booster(string _type, string _state)
    {
        if (firebaseInitialized)
        {
            string str = "booster";
            FirebaseAnalytics.LogEvent(str,
                new Parameter("id_map", id_map),
                new Parameter("type", _type),
                new Parameter("state", _state));
        }
    }

    public void LogEvent_PlayTime(string _time)
    {
        if (firebaseInitialized)
        {
            string str = "playtime_" +_time;
            FirebaseAnalytics.LogEvent(str,
                new Parameter("id_map", id_map),
                new Parameter("process", MapController.Instance.mapData.crProcess));
        }
    }

    #region Request marketing
    private int ads_complete { get { return PlayerPrefs.GetInt("ads_complete", 0); } set { PlayerPrefs.SetInt("ads_complete", value); } }
    private int ads_inter_complete { get { return PlayerPrefs.GetInt("ads_inter_complete", 0); } set { PlayerPrefs.SetInt("ads_inter_complete", value); } }
    private int ads_reward_complete { get { return PlayerPrefs.GetInt("ads_reward_complete", 0); } set { PlayerPrefs.SetInt("ads_reward_complete", value); } }
    private int session_start { get { return PlayerPrefs.GetInt("session_start", 0); } set { PlayerPrefs.SetInt("session_start", value); } }
    public void LogEvent_Ads_Complete()
    {     
        if (ads_complete > 5)
            return;
        ads_complete++;

        if (firebaseInitialized)
        {
            string str = "ads_complete_" + ads_complete;
            FirebaseAnalytics.LogEvent(str);
        }
    }

    public void LogEvent_Ads_Inter_Complete()
    {
        if (ads_inter_complete > 3)
            return;
        ads_inter_complete++;

        if (firebaseInitialized)
        {
            string str = "ads_inter_complete_" + ads_complete;
            FirebaseAnalytics.LogEvent(str);
        }
    }

    public void LogEvent_Ads_Reward_Complete()
    {
        if (ads_reward_complete > 3)
            return;
        ads_reward_complete++;

        if (firebaseInitialized)
        {
            string str = "ads_reward_complete_" + ads_complete;
            FirebaseAnalytics.LogEvent(str);
        }
    }

    public void LogEvent_Session_Start()
    {
        session_start++;
        if (session_start % 2 == 0)
        {
            if (firebaseInitialized)
            {
                string str = "session_start_2" + ads_complete;
                FirebaseAnalytics.LogEvent(str);
            }
        }

        if (session_start % 3 == 0)
        {
            if (firebaseInitialized)
            {
                string str = "session_start_3" + ads_complete;
                FirebaseAnalytics.LogEvent(str);
            }
        }
    }

    public void LogEvent_User_level(int _exp, string cr_exp, string idUnit)
    {
        if (firebaseInitialized)
        {
            string str = "user_level";

            FirebaseAnalytics.LogEvent(str,
                new Parameter("id_map", id_map),
                new Parameter("id_unlock", idUnit),
                new Parameter("level", level_user),
                new Parameter("exp_add", _exp),
                new Parameter("exp_current", cr_exp));

            Debug.Log(str);
        }
    }

    #endregion

    #endregion

    #region Event Ads Analytic
    //AD value
    public void Event_af_ad_impression(MaxSdk.AdInfo adInfo)
    {
        if (firebaseInitialized)
        {
            //string str = "a_start_first_session";
            double revenue = adInfo.Revenue;
            var impressionParameters = new[] {
            new Parameter("ad_platform", "AppLovin"),
            new Parameter("adFormat", adInfo.AdFormat),

            new Parameter("networkName", adInfo.NetworkName),
            //new Parameter("ad_unit_name", adInfo.AdUnitIdentifier),
            new Parameter("networkPlacement", adInfo.Placement), // Please check this - as we couldn't find format refereced in your unity docs https://dash.applovin.com/documentation/mediation/unity/getting-started/advanced-settings#impression-level-user-revenue - api
            new Parameter("value", revenue),
            new Parameter("revenuePrecision", adInfo.RevenuePrecision),
            new Parameter("currency", "USD"), // All Applovin revenue is sent in USD
            };
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventAdImpression, impressionParameters);
        }
    }
    public void Event_af_ad_impression_gb(MaxSdk.AdInfo adInfo)
    {
        //if (firebaseInitialized)
        //{
        //    //string str = "a_start_first_session";
        //    double revenue = adInfo.Revenue;
        //    var impressionParameters = new[] {
        //    new Parameter("ad_platform", "AppLovin"),
        //    new Parameter("adFormat", adInfo.AdFormat),
        //    new Parameter("networkName", adInfo.NetworkName),
        //    //new Parameter("ad_unit_name", adInfo.AdUnitIdentifier),
        //    new Parameter("networkPlacement", adInfo.Placement), // Please check this - as we couldn't find format refereced in your unity docs https://dash.applovin.com/documentation/mediation/unity/getting-started/advanced-settings#impression-level-user-revenue - api
        //    new Parameter("value", revenue),
        //    new Parameter("gb_ads_value",revenue),
        //    new Parameter("revenuePrecision", adInfo.RevenuePrecision),
        //    new Parameter("currency", "USD"), // All Applovin revenue is sent in USD
        //    };
        //    FirebaseAnalytics.LogEvent("fbs_ad_impression", impressionParameters);
        //}
    }
    //Banner
    public void LogEvent_ads_banner_click()
    {
        if (firebaseInitialized)
        {
            string str = "ads_banner_click";

            FirebaseAnalytics.LogEvent(str);
            Debug.Log(str);
        }
    }
    public void LogEvent_ads_banner_request()
    {
        if (firebaseInitialized)
        {
            string str = "ads_banner_request";
            FirebaseAnalytics.LogEvent(str);
            Debug.Log(str);
        }
    }
    //Inter
    public void LogEvent_ads_inter_request()
    {
        //if (firebaseInitialized)
        //{
        //    string str = "ads_inter_request";
        //    FirebaseAnalytics.LogEvent(str);
        //    Debug.Log(str);
        //}
    }
    public void LogEvent_ads_inter_request_complete()
    {
        if (firebaseInitialized)
        {
            string str = "ads_inter_request_complete";
            FirebaseAnalytics.LogEvent(str);
            Debug.Log(str);
        }
    }
    public void LogEvent_ads_inter_failed(MaxSdk.ErrorInfo errorInfo)
    {
        if (firebaseInitialized)
        {
            string str = "ads_inter_failed";
            FirebaseAnalytics.LogEvent(str, new Parameter("error_info", errorInfo.ToString()));

            Debug.Log(str);
        }
    }
    public void LogEvent_ads_inter_show_failed()
    {
        if (firebaseInitialized)
        {
            string str = "ads_inter_show_failed";
            FirebaseAnalytics.LogEvent(str);
            Debug.Log(str);
        }
    }
    public void LogEvent_ads_inter_impression()
    {
        if (firebaseInitialized)
        {
            string str = "ads_inter_click_impression";
            FirebaseAnalytics.LogEvent(str);
            Debug.Log(str);
        }
    }
    public void LogEvent_ads_inter_show()
    {
        if (firebaseInitialized)
        {
            string str = "ads_inter_show";
            FirebaseAnalytics.LogEvent(str);
            Debug.Log(str);
        }
    }
    public void LogEvent_ads_inter_complete(MaxSdk.AdInfo _info = null)
    {
        if (firebaseInitialized)
        {
            string str = "ads_inter_complete";

            FirebaseAnalytics.LogEvent(str,
                new Parameter("network", _info.NetworkName)
                );
            Debug.Log(str);
        }
    }
    //reward
    public void LogEvent_ads_reward_request()
    {
        //if (firebaseInitialized)
        //{
        //    string str = "ads_reward_request";
        //    FirebaseAnalytics.LogEvent(str);
        //    Debug.Log(str);
        //}
    }
    public void LogEvent_ads_reward_request_complete()
    {
        if (firebaseInitialized)
        {
            string str = "ads_reward_request_complete";
            FirebaseAnalytics.LogEvent(str);
            Debug.Log(str);
        }
    }
    public void LogEvent_ads_reward_failed()
    {
        if (firebaseInitialized)
        {
            string str = "ads_reward_failed";
            FirebaseAnalytics.LogEvent(str);
            Debug.Log(str);
        }
    }

    public void LogEvent_ads_reward_show()
    {
        if (firebaseInitialized)
        {
            string str = "ads_reward_show";

            FirebaseAnalytics.LogEvent(str);
            Debug.Log(str);
        }
    }

    public void LogEvent_ads_reward_complete(MaxSdk.AdInfo _info = null,string _rewardType="",string _rewardValue="")
    {
        if (firebaseInitialized)
        {
            string str = "ads_reward_complete";

            FirebaseAnalytics.LogEvent(str,
                 new Parameter("id_map", id_map),
                 new Parameter("reward_type", _rewardType),
                 new Parameter("reward_value", _rewardValue),
                 new Parameter("network", _info.NetworkName));
            Debug.Log(str);
        }
    }
    //Open app
    public void LogEvent_ads_open_request()
    {
        //if (firebaseInitialized)
        //{
        //    string str = "ads_open_request";
        //    FirebaseAnalytics.LogEvent(str);
        //    Debug.Log(str);
        //}
    }
    public void LogEvent_ads_open_request_complete()
    {
        if (firebaseInitialized)
        {
            string str = "ads_open_request_complete";
            FirebaseAnalytics.LogEvent(str);
            Debug.Log(str);
        }
    }
    public void LogEvent_ads_open_show()
    {
        if (firebaseInitialized)
        {
            string str = "ads_open_show";
            FirebaseAnalytics.LogEvent(str);
            Debug.Log(str);
        }
    }

    public void LogEvent_ads_open_failed(MaxSdk.ErrorInfo _info = null)
    {
        if (firebaseInitialized)
        {
            string str = "ads_open_failed";
            FirebaseAnalytics.LogEvent(str, new Parameter("error_info", _info.ToString()));
            Debug.Log(str);
        }
    }

    public void LogEvent_ads_open_complete(MaxSdk.AdInfo _info = null)
    {
        if (firebaseInitialized)
        {
            string str = "ads_open_complete";
            FirebaseAnalytics.LogEvent(str, new Parameter("network", _info.NetworkName));
            Debug.Log(str);
        }
    }
    //Native Ads
    public void LogEvent_ads_native_request()
    {
        if (firebaseInitialized)
        {
            string str = "ads_native_request";
            FirebaseAnalytics.LogEvent(str);
            Debug.Log(str);
        }
    }
    public void LogEvent_ads_native_impression()
    {
        if (firebaseInitialized)
        {
            string str = "ads_native_request_impression";
            FirebaseAnalytics.LogEvent(str);
            Debug.Log(str);
        }
    }

    public void LogEvent_click_button(string _button)
    {
        //if (firebaseInitialized)
        //{
        //    string str = "click_button_" + _button;
        //    FirebaseAnalytics.LogEvent(str, new Parameter("id_map", id_map));
        //    Debug.Log(str);
        //}
    }
    #endregion

    #region Remote Config
    const string _keyRemoteConfigs= "gab012_configs";
    public void InitsFirebaseRemote()
    {
        FirebaseRemoteConfig.DefaultInstance.OnConfigUpdateListener += ConfigUpdateListenerEventHandler;

        // [START set_defaults]
        Dictionary<string, object> defaults = new Dictionary<string, object>();

        // These are the values that are used if we haven't fetched data from the
        // server
        // yet, or if we ask for values that the server doesn't have:
        defaults.Add(_keyRemoteConfigs, "default local string");

        FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults)
          .ContinueWithOnMainThread(task =>
          {
              // [END set_defaults]
              Debug.Log("RemoteConfig configured and ready!");
              remoteInitialized = true;
          });

        FetchDataAsync();
    }

    private void ConfigUpdateListenerEventHandler(
       object sender, ConfigUpdateEventArgs args)
    {
        if (args.Error != RemoteConfigError.None)
        {
            Debug.Log(String.Format("Error occurred while listening: {0}", args.Error));
            return;
        }
        Debug.Log(String.Format("Auto-fetch has received a new config. Updated keys: {0}",
            string.Join(", ", args.UpdatedKeys)));
        var info = FirebaseRemoteConfig.DefaultInstance.Info;
        FirebaseRemoteConfig.DefaultInstance.ActivateAsync()
          .ContinueWithOnMainThread(task =>
          {
              Debug.Log(String.Format("Remote data loaded and ready (last fetch time {0}).",
                                  info.FetchTime));


          });


    }


    // Start a fetch request.
    // FetchAsync only fetches new data if the current data is older than the provided
    // timespan.  Otherwise it assumes the data is "recent enough", and does nothing.
    // By default the timespan is 12 hours, and for production apps, this is a good
    // number. For this example though, it's set to a timespan of zero, so that
    // changes in the console will always show up immediately.
    public Task FetchDataAsync()
    {
        Debug.Log("Fetching data...");
        Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(
             TimeSpan.Zero);
        return fetchTask.ContinueWithOnMainThread(FetchComplete);
    }

    private void FetchComplete(Task fetchTask)
    {
        if (!fetchTask.IsCompleted)
        {
            Debug.LogError("Retrieval hasn't finished.");
            return;
        }

        var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
        var info = remoteConfig.Info;
        if (info.LastFetchStatus != LastFetchStatus.Success)
        {
            Debug.LogError($"{nameof(FetchComplete)} was unsuccessful\n{nameof(info.LastFetchStatus)}: {info.LastFetchStatus}");
            return;
        }

        // Fetch successful. Parameter values must be activated to use.
        remoteConfig.ActivateAsync()
          .ContinueWithOnMainThread(
            task =>
            {
                Debug.Log($"Remote data loaded and ready for use. Last fetch time {info.FetchTime}.");
            });

        LogFetchedData();
        string strConfig = FirebaseRemoteConfig.DefaultInstance.GetValue(_keyRemoteConfigs).StringValue;
        Debug.Log("config_test_string: " + strConfig);
        Module.Action_Event_RemoteConfigs(_keyRemoteConfigs, strConfig);

    }


    /// <summary>
    /// Get the currently loaded data. If fetch has been called, this will be the data fetched from the server. Otherwise, it will be the defaults.
    /// Note: Firebase will cache this between sessions, so even if you haven't called fetch yet, if it was called on a previous run of the program, you will still have data from the last time it was run.
    /// </summary>
    public double GetNumberValue(string pKey, double pDefault = 0)
    {

        try
        {
            return FirebaseRemoteConfig.DefaultInstance.GetValue(pKey).DoubleValue;
        }
        catch (Exception ex)
        {

            return pDefault;
        }

    }

    public string GetStringValue(string pKey, string pDefault = "")
    {
        try
        {
            return FirebaseRemoteConfig.DefaultInstance.GetValue(pKey).StringValue;
        }
        catch (Exception ex)
        {

            return pDefault;
        }

    }

    public bool GetBoolValue(string pKey, bool pDefault = false)
    {
        try
        {
            return FirebaseRemoteConfig.DefaultInstance.GetValue(pKey).BooleanValue;
        }
        catch (Exception ex)
        {

            return pDefault;
        }
    }

    // Display the currently loaded data.  If fetch has been called, this will be
    // the data fetched from the server.  Otherwise, it will be the defaults.
    // Note:  Firebase will cache this between sessions, so even if you haven't
    // called fetch yet, if it was called on a previous run of the program, you
    //  will still have data from the last time it was run.
    public void DisplayData()
    {
        Debug.Log("Current Data:");
        Debug.Log("config_test_string: " +
                 Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
                 .GetValue("config_test_string").StringValue);
        Debug.Log("config_test_int: " +
                 Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
                 .GetValue("config_test_int").LongValue);
        Debug.Log("config_test_float: " +
                 Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
                 .GetValue("config_test_float").DoubleValue);
        Debug.Log("config_test_bool: " +
                 Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
                 .GetValue("config_test_bool").BooleanValue);
    }



    public void LogFetchedData()
    {
        string log = "";
        var result = new Dictionary<string, ConfigValue>();
        var keys = FirebaseRemoteConfig.DefaultInstance.Keys;
        foreach (string key in keys)
        {
            var value = FirebaseRemoteConfig.DefaultInstance.GetValue(key);
            result.Add(key, value);
            log += $"Key:{key} Value:{value}\n";
        }
        Debug.Log(log);
    }

    #endregion


    #region Messaging
    public void InitsMessenge()
    {
        FirebaseMessaging.TokenReceived += OnTokenReceived;
        FirebaseMessaging.MessageReceived += OnMessageReceived;
    }

    public void OnTokenReceived(object sender, TokenReceivedEventArgs token)
    {
        Debug.Log("Received Registration Token: " + token.Token);
//#if UNITY_ANDROID
//        AppsFlyer.updateServerUninstallToken(token.Token);
//#endif
        //LogEvent_delete_app();
        //AdjustTracking.Instance?.Event_delete_app();
    }

    public void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        Debug.Log("Received a new message from: " + e.Message.From);
    }
    #endregion
}
