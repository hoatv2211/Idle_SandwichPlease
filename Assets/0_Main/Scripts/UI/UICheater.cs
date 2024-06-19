using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UICheater : MonoBehaviour
{
    [SerializeField] private UIButton btnExit;
    [SerializeField] private UIButton btnAddCoin;
    [SerializeField] private UIButton btnSubCoin;
    [SerializeField] private UIButton btnRemoveAds;
    [SerializeField] private UIButton btnLogConsole;
    [SerializeField] private UIButton btnOffConsole;
    [SerializeField] private UIButton btnFullStack;

    [Header("Map Process")]
    [SerializeField] private UIButton btnUnlockMap;
    [SerializeField] private TMP_Dropdown map_Dropdown;
    [SerializeField] private TextMeshProUGUI txtValue;
    // Start is called before the first frame update
    private void OnEnable()
    {
        btnExit.SetUpEvent(Action_btnExit);
        btnAddCoin.SetUpEvent(Action_btnAddCoin);
        btnSubCoin.SetUpEvent(Action_btnSubCoin);
        btnRemoveAds.SetUpEvent(Action_btnRemoveAds);
        btnLogConsole.SetUpEvent(Action_btnLogConsole);
        btnFullStack.SetUpEvent(Action_btnFullStack);
        btnOffConsole.SetUpEvent(Action_btnOffConsole);

        btnUnlockMap.SetUpEvent(Action_btnUnlockMap);
        Module.isCheater = 1;

        SetDataUnlockMap();

    }

    private void Action_btnExit()
    {
        gameObject.SetActive(false);
    }

    private void Action_btnAddCoin()
    {
        
        Module.money_currency += 1000;
    }

    private void Action_btnSubCoin()
    {
        Module.money_currency -= 1000;
    }

    private void Action_btnRemoveAds()
    {
        Module.isRemoveAds = 1;
    }

    private void Action_btnFullStack()
    {
        MapController.Instance.mapData.upgradePlayer.lvSpeed = 5;
        MapController.Instance.mapData.upgradePlayer.lvCapacity = 5;
    }

    private void Action_btnLogConsole()
    {
        DontDestroyThisGameobject.Instance.GetComponent<FPSDisplay>().enabled = true;
        GameManager.Instance.ingameDebug.SetActive(true);

    }

    private void Action_btnOffConsole()
    {
        DontDestroyThisGameobject.Instance.GetComponent<FPSDisplay>().enabled = false;
        GameManager.Instance.ingameDebug.SetActive(false);

    }


    private void SetDataUnlockMap()
    {
        List<string> listUnits = new List<string>();
        foreach(var k in MapController.Instance.m_listUnits)
        {
            listUnits.Add(k.IdUnit);
        }

        map_Dropdown.ClearOptions();
        map_Dropdown.AddOptions(listUnits);
    }

    private void Action_btnUnlockMap()
    {
        StartCoroutine(IUnlockMap());
    }

    public List<string> listCRs;
    public List<string> listNexts;
    public List<string> listReqs;
    MapController mapCtrl => MapController.Instance;
    IEnumerator IUnlockMap()
    {
        Module.isCheatProcess = true;
        //PlayerPrefs.DeleteAll();
        mapCtrl.mapData.crProcess = 0;
        mapCtrl.ShowOff();
        mapCtrl.mapData.tutCleans.Clear();
        mapCtrl.mapData.crTut.Clear();
        mapCtrl.mapData.unlocked_UnitDatas.Clear();
        mapCtrl.mapData.crUnlocks.Clear();

        listCRs     = new List<string>();
        listNexts   = new List<string>();
        listReqs    = new List<string>();
        string unlockUnit = txtValue.text;
        //Debug.LogError(unlockUnit);
        yield return null;
        yield return null;
        yield return null;
        yield return new WaitForSeconds(1f);

        string crUnlock = unlockUnit;
        List<GameObject> listobjsOff = new List<GameObject>();
        while (crUnlock != "none")
        {
            yield return null;
            DataMapProcess _unit = mapCtrl.mapModel.GetDataMapProcess(crUnlock);
            UnlockUnit unitBase = mapCtrl._unlockPoints.Find(x => x.m_UnitData.idUnit == crUnlock);
            UnitData unitData = unitBase.m_UnitData;

            listNexts.AddRange(_unit.unlocknext);
            listReqs.AddRange(_unit.unlockreq);
            listCRs.Add(_unit.id);

            mapCtrl.mapData.crProcess += _unit.exp;
            unitBase.CheatUnlock(listobjsOff);
            listobjsOff.Add(unitBase.gameObject);
            yield return null;
           
           
            crUnlock = _unit.unlockreq[0];
            //Debug.LogError("setoff - "+ unitBase.name +" : "+ unitBase.m_UnitData.idUnit);
            
        }
        

        UnlockUnit unitBasecr = mapCtrl._unlockPoints.Find(x => x.m_UnitData.idUnit == unlockUnit);

        foreach (var k in unitBasecr.listUnlock)
        {
            k.gameObject.SetActive(true);
            if(k.GetComponent<UnitBase>()!=null)
                k.GetComponent<UnitBase>().m_UnitData.isUnlocked = false;
        }
        unitBasecr.gameObject.SetActive(false);

      
        Module.isCheatProcess = false;
        yield return null;

        foreach (var k in mapCtrl.mapData.crUnlocks)
        {
            k.isUnlocked = false;
        }
        mapCtrl.mapData.SaveData();
       
        yield return null;
        //yield return new WaitForSeconds(1f);
        //mapCtrl.Reload();
        
        yield return null;
        yield return null;
        
        //yield return new WaitForSeconds(1f);
        //SceneManager.LoadScene(0,LoadSceneMode.Single);

        Debug.LogError("Done");
    }
}
