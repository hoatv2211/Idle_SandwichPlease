using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public enum ETypeUnlock
{
    unlock,
    upgrade
}


//TrackingHelper
public enum ETypeUnitUnlock
{
    none,
    table,
    sandwich_machine,
    drive_thru,
    package_table,
    counter
}

public enum EUnlockCost
{
    money,
    ads
}

public class UnlockUnit : UnitBase
{
    public ETypeUnitUnlock typeUnit;
    public ETypeUnlock typeUnlock;
    public EUnlockCost typeUnlockCost;

    public List<GameObject> listLocked;
    public List<GameObject> listUnlock;

    [Space, Header("UNLOCK POINT UNIT - UI")]
    [SerializeField] private TextMeshPro _priceText;
    [SerializeField] private float _duration = 2f;
    [SerializeField] private Transform process;
    [SerializeField] private TutNode tutNode;

    [SerializeField] private GameObject objCost;
    [SerializeField] private GameObject objAds;
    [SerializeField] private GameObject objPayAds;

    [SerializeField] private GameObject objBase;
    [SerializeField] private GameObject objLocked;
    [SerializeField] private TextMeshPro txtLevelRq;

    public int _residualPrice { get { return m_UnitData.residualPrice; } set { m_UnitData.residualPrice = value; } }
    public bool _isUnlocked { get { return m_UnitData.isUnlocked; } set { m_UnitData.isUnlocked = value; } }

    public bool _isReqToUnlock { get {
            if (m_UnitData == null)
                return false;

            return m_UnitData.dataMapProcess.levelreq <= UserModel.Instance.level;
        } }
    private void Awake()
    {
        ShowPriceAmount(_residualPrice);
        if (objPayAds != null)
        {
            SetAdsOnly();
        }
    }


    BoxCollider boxCollider => GetComponent<BoxCollider>();
    private void OnEnable()
    {

        DOVirtual.DelayedCall(0.1f, () =>
        {
            if (MapController.Instance.mapData.GetCurrentUnlock(IdUnit) != null)
            {
                m_UnitData = MapController.Instance.mapData.GetCurrentUnlock(IdUnit);
                ShowPriceAmount(_residualPrice);

                Module_Event_RefreshArea();

                boxCollider.enabled = false;
                DOVirtual.DelayedCall(0.5f, () => boxCollider.enabled = _isReqToUnlock);
            }
            else
                MapController.Instance.mapData.SetCurrentUnlock(m_UnitData);


        });

        MapController.Instance.cameraCtrl.SetCamTargetFocusNoAction(this.transform);


        //Tracking
        if (typeUnlock == ETypeUnlock.unlock)
        {
            switch (typeUnit)
            {
                case ETypeUnitUnlock.none:
                    break;
                case ETypeUnitUnlock.table:
                    FirebaseManager.Instance.LogEvent_table(IdUnit, "table", "unlock");
                    break;
                case ETypeUnitUnlock.sandwich_machine:
                    FirebaseManager.Instance.LogEvent_sandwich_machine(IdUnit, "sandwich_machine", "unlock", 0);
                    break;
                case ETypeUnitUnlock.drive_thru:
                    FirebaseManager.Instance.LogEvent_drive_thru(IdUnit, "drive_thru", "unlock", 0);
                    break;
                case ETypeUnitUnlock.package_table:
                    FirebaseManager.Instance.LogEvent_package_table(IdUnit, "package_table", "unlock", 0);
                    break;
                case ETypeUnitUnlock.counter:
                    FirebaseManager.Instance.LogEvent_counter(IdUnit, "counter", "unlock", 0);
                    break;
                default:
                    break;
            }
        }


        Module.Event_RefreshArea += Module_Event_RefreshArea;
        DOVirtual.DelayedCall(0.5f, Module_Event_RefreshArea);

    }

    private void Module_Event_RefreshArea()
    {
        try
        {
            objBase?.SetActive(_isReqToUnlock);
            objLocked?.SetActive(!_isReqToUnlock);

            if (txtLevelRq != null)
                txtLevelRq.text = m_UnitData.dataMapProcess.levelreq.ToString();

            boxCollider.enabled = _isReqToUnlock;
        }
        catch
        {

        }

    }

    private void OnDisable()
    {
        Module.Event_RefreshArea -= Module_Event_RefreshArea;
    }

    public override void CallStart()
    {
        ShowPriceAmount(m_UnitData.residualPrice);
        if (process != null)
        {
            Vector3 vtScale = process.transform.localScale;
            process.transform.localScale = new Vector3(vtScale.x, 3.5f * (1 - (float)(_residualPrice) / (float)m_UnitData.costUnlock), vtScale.z);
        }

        if (m_UnitData.residualPrice <= 0)
            gameObject.SetActive(false);

        Module_Event_RefreshArea();
    }


    public void SetTypeUnlock(EUnlockCost _type)
    {

        if (_type == EUnlockCost.ads)
        {
            if (_residualPrice < m_UnitData.costUnlock)
                return;
        }

        typeUnlockCost = _type;
        switch (typeUnlockCost)
        {
            case EUnlockCost.money:
                ShowPriceAmount(_residualPrice);
                break;
            case EUnlockCost.ads:
                Debug.Log(gameObject.name);
                break;
            default:
                break;
        }

        objAds.SetActive(typeUnlockCost == EUnlockCost.ads);
        objCost.SetActive(typeUnlockCost == EUnlockCost.money);

        Module_Event_RefreshArea();
    }


    public void SetAdsOnly()
    {
        objPayAds.SetActive(typeUnlockCost == EUnlockCost.ads);
    }

    public void AddOnListUnlock(GameObject _unlock)
    {
        if (!listUnlock.Contains(_unlock))
            listUnlock.Add(_unlock);

    }


    public void ShowPriceAmount(int residualPrice)
    {
        if (residualPrice <= 0 && objPayAds == null)
        {
            gameObject.SetActive(false);
            Action_UnlockUnit();
        }

        if (_priceText != null)
            _priceText.text = residualPrice.ToString();
    }

    float timeDelay = 0;
    private Tweener _tweener;
    private int costSpend;

    bool isCanShow = false;
    private void OnTriggerEnter(Collider other)
    {
        PlayerCtrl player = other.gameObject.GetComponent<PlayerCtrl>();
        if (player)
        {
            timeDelay = 0;
            costSpend = 0;
            isCanShow = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        PlayerCtrl player = other.gameObject.GetComponent<PlayerCtrl>();
        if (player)
        {


            timeDelay += Time.deltaTime;
            if (timeDelay >= 0.3f)
            {

                if (!player.IsMoving())
                {
                    if (typeUnlockCost == EUnlockCost.ads && isCanShow)
                    {
                        isCanShow = false;
                        AdsAppLovinController.Instance.ShowRewardedAd(() =>
                        {
                            Action_UnlockUnit();
                        }, "upgrade_unit", m_UnitData.idUnit);
                        return;
                    }

                    if (typeUnlockCost == EUnlockCost.ads)
                    {
                        return;
                    }


                    if (Module.money_currency > 0 && _residualPrice > 0)
                    {

                        UIMainGame.Instance.Show_PaidEffect(this.transform);
                        player.PayState(true);
                        if (_tweener == null)
                        {
                            float _timeUnlock = 2 * (float)((float)_residualPrice / (float)m_UnitData.costUnlock);
                            if (_timeUnlock <= 0)
                                _timeUnlock = 1;
                            Module.LowVibrate();
                            SoundManager.Instance.PlayOnCamera(1);
                            Debug.Log(_timeUnlock);
                            _tweener = DOTween.To(() => _residualPrice, x =>
                            {
                                Module.money_currency -= (_residualPrice - x);
                                costSpend += (_residualPrice - x);
                                _residualPrice = x;
                                ShowPriceAmount(_residualPrice);
                                if (Module.money_currency <= 0)
                                {
                                    Module.money_currency = 0;
                                    _tweener.Kill();

                                    //Tracking
                                    int money = Module.money_currency;
                                    switch (typeUnlock)
                                    {
                                        case ETypeUnlock.unlock:
                                            FirebaseManager.Instance.LogEvent_spend("money", "money",
                                                     costSpend, money + costSpend, money,
                                                     IdUnit, "unlock unit", m_UnitData.costUnlock);
                                            break;
                                        case ETypeUnlock.upgrade:
                                            FirebaseManager.Instance.LogEvent_spend("money", "money",
                                                     costSpend, money + costSpend, money,
                                                     IdUnit, "upgrade unit", m_UnitData.costUnlock);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                if (process != null)
                                {
                                    Vector3 vtScale = process.transform.localScale;
                                    process.transform.localScale = new Vector3(vtScale.x, 3.5f * (1 - (float)(_residualPrice) / (float)m_UnitData.costUnlock), vtScale.z);
                                }

                            }, 0, _timeUnlock)
                            .OnComplete(() => { player.PayState(false); Action_UnlockUnit(); })
                            .OnKill(() => player.PayState(false));
                        }
                    }

                    if (_residualPrice <= 0)
                        Action_UnlockUnit();
                }
                else
                {
                    //animator.SetBool("isPay", false);
                }
            }

        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (_isUnlocked) return;
        PlayerCtrl player = other.gameObject.GetComponent<PlayerCtrl>();
        if (player)
        {
            isCanShow = false;
            if (typeUnlockCost == EUnlockCost.ads)
            {

                return;
            }

            //_onSaveProgress?.Invoke(this);
            if (_tweener != null)
            {
                _tweener.Kill();
                _tweener = null;
            }

            UIMainGame.Instance.Hide_MoneyEffect();

            if (costSpend <= 0)
                return;

            //Tracking
            int money = Module.money_currency;
            switch (typeUnlock)
            {
                case ETypeUnlock.unlock:
                    FirebaseManager.Instance.LogEvent_spend("money", "money",
                             costSpend, money + costSpend, money,
                             IdUnit, "unlock unit", m_UnitData.costUnlock);
                    break;
                case ETypeUnlock.upgrade:
                    FirebaseManager.Instance.LogEvent_spend("money", "money",
                             costSpend, money + costSpend, money,
                             IdUnit, "upgrade unit", m_UnitData.costUnlock);
                    break;
                default:
                    break;
            }
        }
    }

    public override void Action_UnlockUnit()
    {
        if (_isUnlocked)
            return;
        _isUnlocked = true;

        Module.Action_Event_MapPoint(IdUnit, m_UnitData.dataMapProcess.exp);
        SoundManager.Instance.PlayOnCamera(2);
        int money = Module.money_currency;
        switch (typeUnlock)
        {
            case ETypeUnlock.unlock:
                transform.parent.GetComponent<UnitBase>()?.Action_UnlockUnit();
                FirebaseManager.Instance.LogEvent_spend("money", "money",
                         costSpend, money + costSpend, money,
                         IdUnit, "unlock unit", m_UnitData.costUnlock);
                break;
            case ETypeUnlock.upgrade:
                transform.parent.GetComponent<UnitBase>()?.Action_UpgradeUnit();
                FirebaseManager.Instance.LogEvent_spend("money", "money",
                         costSpend, money + costSpend, money,
                         IdUnit, "upgrade unit", m_UnitData.costUnlock);
                break;
            default:
                break;
        }

        if(IdUnit== "door_1")
        {
            if (GameManager.Instance.m_DataConfigRemote.isShowWelcome)
            {
                MapController.Instance.cameraCtrl.ActionShowCamDoor(() =>
                {
                    foreach (var k in listUnlock)
                    {
                        k.SetActive(true);
                    }
                });
            }
            else
            {
                foreach (var k in listUnlock)
                {
                    k.SetActive(true);
                }
            }
          

        }
        else
        {
            foreach (var k in listUnlock)
            {
                k.SetActive(true);
            }

            foreach (var k in listLocked)
            {
                k.SetActive(false);
            }
        }


        if (tutNode != null)
        {
            tutNode.Action_CompleteTut();
        }
        MapController.Instance.mapData.Action_UnlockUnit(m_UnitData);
        MapController.Instance.mapData.RemoveCrUnitUnlock(m_UnitData);
        gameObject.SetActive(false);
       

        //Tracking
        if (typeUnlock == ETypeUnlock.unlock)
        {
            switch (typeUnit)
            {
                case ETypeUnitUnlock.none:
                    break;
                case ETypeUnitUnlock.table:
                    FirebaseManager.Instance.LogEvent_table(IdUnit, "table", "unlocked");
                    break;
                case ETypeUnitUnlock.sandwich_machine:
                    FirebaseManager.Instance.LogEvent_sandwich_machine(IdUnit, "sandwich_machine", "unlocked", 1);
                    break;
                case ETypeUnitUnlock.drive_thru:
                    FirebaseManager.Instance.LogEvent_drive_thru(IdUnit, "drive_thru", "unlocked", 1);
                    break;
                case ETypeUnitUnlock.package_table:
                    FirebaseManager.Instance.LogEvent_package_table(IdUnit, "package_table", "unlocked", 1);
                    break;
                case ETypeUnitUnlock.counter:
                    FirebaseManager.Instance.LogEvent_counter(IdUnit, "counter", "unlocked", 1);
                    break;
                default:
                    break;
            }
        }

        if (typeUnlock == ETypeUnlock.upgrade)
        {
            switch (typeUnit)
            {
                case ETypeUnitUnlock.sandwich_machine:
                    FirebaseManager.Instance.LogEvent_sandwich_machine(IdUnit, "sandwich_machine", "upgrade", m_UnitData.crLevel);
                    break;
                case ETypeUnitUnlock.drive_thru:
                    FirebaseManager.Instance.LogEvent_drive_thru(IdUnit, "drive_thru", "upgrade", m_UnitData.crLevel);
                    break;
                case ETypeUnitUnlock.package_table:
                    FirebaseManager.Instance.LogEvent_package_table(IdUnit, "package_table", "upgrade", m_UnitData.crLevel);
                    break;
                case ETypeUnitUnlock.counter:
                    FirebaseManager.Instance.LogEvent_counter(IdUnit, "counter", "upgrade", m_UnitData.crLevel);
                    break;
                default:
                    break;
            }
        }

        Module.Action_Event_RefreshArea();
    }


    public void CheatUnlock(List<GameObject> list_Offs)
    {
        //_isUnlocked = true;
        switch (typeUnlock)
        {
            case ETypeUnlock.unlock:
                transform.parent.GetComponent<UnitBase>()?.Action_UnlockUnit();
              
                break;
            case ETypeUnlock.upgrade:
                transform.parent.GetComponent<UnitBase>()?.Action_UpgradeUnit();
               
                break;
            default:
                break;
        }

        foreach (var k in listUnlock)
        {
            if(!listUnlock.Contains(k))
                k.SetActive(true);
        }

        if (tutNode != null)
        {
            tutNode.Action_CompleteTut();
        }
        MapController.Instance.mapData.Action_UnlockUnit(m_UnitData);
        MapController.Instance.mapData.RemoveCrUnitUnlock(m_UnitData);
        gameObject.SetActive(false);
    }
}
