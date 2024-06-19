using AssetKits.ParticleImage;
using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMainGame : Singleton<UIMainGame>
{
    [Header("Money Effect")]
    public ParticleImage particleImage;
    public ParticleImage moneyBooter;
    public Transform baseMoneyFx;
    public Transform targetMoneyFx;
    public Transform trMoney;

    [Header("HUD")]
    [SerializeField] private GameObject objProcess;
    [SerializeField] private Slider processMap;
    [SerializeField] private TextMeshProUGUI txtLevel;
    [SerializeField] private TextMeshProUGUI txtProccess;
    [SerializeField] private InterProcess interProcess;


    [Space]
    [Tooltip("Process UI")]
    public UIButton btnSetting;
    public UIButton btnShop;
    public UIButton btnMap;
    public UIButton btnRemoveAds;
    public UIButton btnDaily;
    public UIButton btnSkin;
    public UIButton btnCleanBot;
    public UIButton btnEatSpeed;
    public UIButton btnBattlePass;

    public UIButton btnUpgradeStaff;
    public UIButton btnUpgradePlayer;
    public UIButton btnCompass;
    public GameObject labelUpgrade;


    [Header("Tutorial")]
    [SerializeField] private GameObject tutHelper;
    [SerializeField] private TextMeshProUGUI txtHelper;


    [Header("Popups")]
    public UIUpgradePlayer m_UIUpgradePlayer;
    public UIUpgradeStaff m_UIUpgradeStaff;
    public UISetting m_UISetting;
    public UIBooster m_UIBooster;
    public UIMap m_UIMap;
    public UISpecialOrder m_UISpecialOrder;
    public UIRemoveAds m_UIAdsRemove;
    public UIDailySpin m_UIDaily;
    public UINoInternet m_UINoInternet;
    public UICleanBot m_UICleanBot;
    public UIRewardOffline m_UIRewardOffline;
    public UISkinChange m_UISkinChange;
    public UIShop m_UIShop;
    public UICheater m_UICheater;
    public UILevelUp m_UILevelUp;
    public UIBattlePass m_UIBattlePass;
    public PopupStoreUnlock popupStoreUnlock;


    [Header("Booster")]
    [SerializeField] private BoosterCD boosterCD;
    [SerializeField] private Transform contentBooster;
    public List<BoosterCD> listBooster = new List<BoosterCD>();

    [Header("Notice")]
    public GameObject notice_TimeFree;
    public GameObject notice_UpgradeStaff;
    public GameObject notice_UpgradePlayer;
    public GameObject notice_BattleClaim;

    public GameObject tut_hand;

    private bool isShowDaily => UserModel.Instance.level >= 2;
    private bool isShowCompass => UserModel.Instance.level >= 2;
    private bool isShowBtnAds => Module.isRemoveAds == 0 && (MapController.Instance.mapData.mapID != 1 || MapController.Instance.mapData.IsUnlockByName(GameManager.Instance.m_DataConfigRemote.idUnlock_ads_inter));
    private bool isShowBtnNextMap => MapController.Instance.mapData.mapID != 1 || UserModel.Instance.level >= 9;
    private bool isShowBtnCleanBot => UserModel.Instance.level >= 5;
    private bool isShowBtnSkin => MapController.Instance.mapData.mapID != 1 || UserModel.Instance.level >= 5;
    private bool isShowBtnShop => MapController.Instance.mapData.mapID != 1 || UserModel.Instance.level >= 4;
    private bool isShowLabelUpgrade => MapController.Instance.mapData.IsUnlockByName("hr_office") && !MapController.Instance.mapData.upgradeStaff.isMaxAll && !MapController.Instance.mapData.upgradePlayer.isMaxAll;
    private bool isShowUpgradePlayer => MapController.Instance.mapData.IsUnlockByName("upgrade_office");

    private bool isFreeTimeGet
    {
        get
        {
            bool isCanGet = false;
            if (!string.IsNullOrEmpty(Module.datetime_reward_free))
            {
                int timeCD = 20 * 60 - (int)Convert.ToDateTime(DateTime.Now).Subtract(Convert.ToDateTime(Module.datetime_reward_free)).TotalSeconds;
                if (timeCD <= 0)
                    isCanGet = true;
            }
            else
            {
                isCanGet = true;
            }


            return isCanGet;
        }
    }


    private void Start()
    {
        btnSetting.SetUpEvent(Show_UISetting);
        btnMap.SetUpEvent(Show_UIMap);
        btnCleanBot.SetUpEvent(() =>
        {
            Show_UICleanBOT();
        });

        btnDaily.SetUpEvent(Show_UIDaily);

        btnRemoveAds.SetUpEvent(() =>
        {
            Show_UIAdsRemove();
            FirebaseManager.Instance.LogEvent_click_button("open_removeads");
        });
        btnShop.SetUpEvent(Show_UIShop);
        btnSkin.SetUpEvent(Show_UISkin);

        btnRemoveAds.gameObject.SetActive(isShowBtnAds);
        btnEatSpeed.SetUpEvent(() => Show_UIBooster(ETypeBooster.EatSpeed, () => { btnEatSpeed.gameObject.SetActive(false); }));



        btnUpgradePlayer.SetUpEvent(() =>
        {
            Show_UIUpgradePlayer();
            FirebaseManager.Instance.LogEvent_click_button("upgrade_player");
        });
        btnUpgradeStaff.SetUpEvent(() =>
        {
            Show_UIUpgradeStaff();
            FirebaseManager.Instance.LogEvent_click_button("upgrade_staff");
        });

        btnBattlePass.SetUpEvent(() =>
        {
            Show_UIBattlePass();
            FirebaseManager.Instance.LogEvent_click_button("open_battlepass");
        });

        btnCompass.SetUpEvent(Action_btnCompass);


        //Event
        Module.Event_MapPoint += Module_Event_MapPoint;
        Module.Event_Change_Money += Module_Event_Change_Money;
        Module.Event_RefreshNotice += Module_Event_RefreshNotice;

        //procress map
        DOVirtual.DelayedCall(0.3f, () =>
        {
            ShowInfo_Process();

            //Delay show
            btnMap.gameObject.SetActive(isShowBtnNextMap);
            btnCleanBot.gameObject.SetActive(isShowBtnCleanBot);
            btnSkin.gameObject.SetActive(isShowBtnSkin);
            btnShop.gameObject.SetActive(isShowBtnShop);
            notice_TimeFree.gameObject.SetActive(isFreeTimeGet);
            labelUpgrade.SetActive(isShowLabelUpgrade);
            btnUpgradePlayer.gameObject.SetActive(isShowUpgradePlayer);
            btnCompass.gameObject.SetActive(isShowCompass);
            btnDaily.gameObject.SetActive(isShowDaily);

            CheckNoticeUpgrade();

            notice_BattleClaim.SetActive(BattlePassModel.Instance.isNoticeReward());
            Show_UIRewardOffline();
            ShowSecond();
        });


        //Check Internet loop
        DOVirtual.DelayedCall(5, () =>
        {
            if (GameManager.Instance.m_DataConfigRemote.isShowInternet)
                m_UINoInternet?.gameObject.SetActive(!Module.isNetworking());

        }).SetLoops(-1);

        if (tut_hand != null)
        {
            tut_hand.SetActive(Module.isFirstHand == 0);
        }
    }

    private void Module_Event_RefreshNotice()
    {
        notice_BattleClaim.SetActive(BattlePassModel.Instance.isNoticeReward());
    }

    private void OnDisable()
    {
        Module.Event_Change_Money -= Module_Event_Change_Money;
        Module.Event_MapPoint -= Module_Event_MapPoint;
        Module.Event_RefreshNotice -= Module_Event_RefreshNotice;
    }
    private void Module_Event_Change_Money()
    {
        CheckNoticeUpgrade();
    }

    private void Module_Event_MapPoint(string _idUnlock, int _point)
    {
        UserModel.Instance.exp_current += _point;

        if (UserModel.Instance.isCheckLevelUp() == true)
        {
            Show_UILevelUP();
        }

        ShowInfo_Process();

        //FirebaseManager.Instance.LogEvent_map_progress(_point, MapController.Instance.mapData.crProcess, _idUnlock);
        FirebaseManager.Instance.LogEvent_User_level(_point, MapController.Instance.mapData.crProcess.ToString(), _idUnlock);
    }

    public void ShowInfo_Process()
    {
        processMap.maxValue = BattlePassModel.Instance.cr_level.exp_req;
        processMap.value = UserModel.Instance.exp_current;
        txtProccess.text = processMap.value + "/" + processMap.maxValue;
        txtLevel.text = UserModel.Instance.level.ToString();
    }

    public void Show_PaidEffect(Transform _target, Transform _base = null)
    {
        targetMoneyFx = _target;
        particleImage.gameObject.SetActive(true);
        particleImage.attractorTarget = targetMoneyFx;

        if (_base == null)
            particleImage.emitterConstraintTransform = baseMoneyFx;
        else
            particleImage.emitterConstraintTransform = _base;
    }

    public void Show_Effect_MoneyBooter(Transform _base = null, int _value = 20)
    {
        if (_base != null)
            moneyBooter.emitterConstraintTransform = _base;
        moneyBooter.rateOverTime = _value;
        moneyBooter.gameObject.SetActive(true);

        DOVirtual.DelayedCall(2, () => { moneyBooter.gameObject.SetActive(false); });

    }

    public void Hide_MoneyEffect()
    {
        particleImage.gameObject.SetActive(false);
    }

    public void Show_UIUpgradePlayer()
    {
        m_UIUpgradePlayer.gameObject.SetActive(true);
        m_UIUpgradePlayer.CallStart();
    }

    public void Show_UIDaily()
    {
        m_UIDaily.gameObject.SetActive(true);
        m_UIDaily.CallStart();
    }

    public void Show_UIUpgradeStaff()
    {
        m_UIUpgradeStaff.gameObject.SetActive(true);
        m_UIUpgradeStaff.CallStart();
    }

    public void Show_UISetting()
    {
        m_UISetting.gameObject.SetActive(true);
        m_UISetting.CallStart();
        AdjustTracking.Instance.Event_click_setting();
        FirebaseManager.Instance.LogEvent_click_button("open_setting");
    }

    public void Show_UIMap()
    {
        m_UIMap.gameObject.SetActive(true);
        m_UIMap.CallStart();

        FirebaseManager.Instance.LogEvent_click_button("open_map");
    }

    public void Show_UILevelUP()
    {
        m_UILevelUp.gameObject.SetActive(true);
        m_UILevelUp.CallStart();
    }

    public void Show_UIBattlePass()
    {
        m_UIBattlePass.gameObject.SetActive(true);
        m_UIBattlePass.CallStart();
    }

    public void Show_UIBooster(ETypeBooster _type, Action _callback)
    {
        m_UIBooster.gameObject.SetActive(true);
        m_UIBooster.CallStart(_type, _callback);
    }

    public void Show_UISpecial(SpecialData _data, int _time)
    {
        m_UISpecialOrder.gameObject.SetActive(true);
        m_UISpecialOrder.CallStart(_data, _time);
    }

    public void Show_TextTutHelper(string _desc)
    {
        //Debug.LogError("Show Tut: " + _desc);
        tutHelper.SetActive(true);
        txtHelper.text = _desc;
    }

    public void Hide_TutHelper()
    {
        tutHelper.SetActive(false);
    }

    public void ShowAdsInter()
    {
        interProcess.CallStart(() =>
        {
            AdsAppLovinController.Instance.ShowInterstitialAd();
            GameManager.Instance.ResetInter();
        });
    }

    public void Show_UIAdsRemove()
    {
        m_UIAdsRemove.gameObject.SetActive(true);

    }

    public void Show_UICleanBOT()
    {
        m_UICleanBot.gameObject.SetActive(true);

        FirebaseManager.Instance.LogEvent_click_button("open_cleanbot");
    }

    public void Show_UIShop()
    {
        m_UIShop.gameObject.SetActive(true);
        m_UIShop.CallStart();

        FirebaseManager.Instance.LogEvent_click_button("open_shop");
    }

    public void Show_UISkin()
    {
        m_UISkinChange.gameObject.SetActive(true);
        m_UISkinChange.CallStart();

        FirebaseManager.Instance.LogEvent_click_button("open_skin");
    }

    public void Action_RemoveAds()
    {
        Module.isRemoveAds = 1;
        btnRemoveAds.gameObject.SetActive(false);
        m_UIAdsRemove.gameObject.SetActive(false);
    }

    public void Show_HUDBooster(ETypeBooster eType)
    {
        if (listBooster.Count > 0)
        {
            BoosterCD bot = listBooster.Find(x => x.typeBooster == eType);
            if (bot != null)
            {
                bot.AddTime();
                return;
            }
        }

        BoosterCD _booster = SimplePool.Spawn(boosterCD, Vector3.zero, Quaternion.identity);
        _booster.transform.SetParent(contentBooster);
        _booster.CallStart(eType, 180);
        listBooster.Add(_booster);
    }

    Tween twProcessBar;
    public void ShowProcessBar()
    {
        objProcess.gameObject.SetActive(true);
        if (twProcessBar != null)
            twProcessBar.Kill();
        twProcessBar = DOVirtual.DelayedCall(10, () => objProcess.gameObject.SetActive(false));
    }

    public void Show_UIRewardOffline()
    {
        if (timeOffline() >= 60)
        {
            int _hour = (int)timeOffline() / 60;

            if (_hour >= 8)
                _hour = 8;

            m_UIRewardOffline.gameObject.SetActive(true);
            m_UIRewardOffline.CallStart(_hour);

        }
    }

    public void ShowSecond()
    {
        if (timeOffline() >= 10)
        {
            if (isShowDaily)
                DOVirtual.DelayedCall(10, () => Show_UIDaily());
                
            if (isShowBtnShop)
                DOVirtual.DelayedCall(30,() => Show_UIShop());

        }
    }

    public double timeOffline()
    {
        if (string.IsNullOrEmpty(Module.time_offline))
            return 0;

        double time = Convert.ToDateTime(DateTime.Now).Subtract(Convert.ToDateTime(Module.time_offline)).TotalMinutes;

        //Debug.LogError(time);
        return time;
    }

    public void CheckNoticeUpgrade()
    {
        notice_UpgradePlayer.SetActive(MapController.Instance.mapData.upgradePlayer.isNotice());
        notice_UpgradeStaff.SetActive(MapController.Instance.mapData.upgradeStaff.isNotice());
    }

    public void Action_btnCompass()
    {
        if (MapController.Instance.mapData.crUnlocks.Count <= 0)
            return;

        int index = Module.EasyRandom(MapController.Instance.mapData.crUnlocks.Count);
        UnlockUnit unlockUnit = MapController.Instance._unlockPoints.Find(x => x.IdUnit == MapController.Instance.mapData.crUnlocks[index].idUnit);

        if(unlockUnit != null)
            MapController.Instance.cameraCtrl.SetCamTargetFocusNoAction(unlockUnit.transform);
    }
}
