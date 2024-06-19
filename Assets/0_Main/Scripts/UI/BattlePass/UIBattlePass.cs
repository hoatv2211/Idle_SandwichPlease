using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBattlePass : MonoBehaviour
{
    [SerializeField] private UIButton btnExit;

    public SViewBattlePass SViewBattlePass;

    public UIButton btnSubManager;
    public UIButton btnSubCEO;
    public void CallStart()
    {
        btnExit.SetUpEvent(Action_btnExit);
        SViewBattlePass.CallStart(BattlePassModel.Instance.level_Users);

        btnSubManager.gameObject.SetActive(!UserModel.Instance.isManagerSub);
        btnSubCEO.gameObject.SetActive(!UserModel.Instance.isCEOSub);

        Module.Event_RefreshBattlePass += Module_Event_RefreshBattlePass;
    }

    private void Module_Event_RefreshBattlePass()
    {
        btnSubManager.gameObject.SetActive(!UserModel.Instance.isManagerSub);
        btnSubCEO.gameObject.SetActive(!UserModel.Instance.isCEOSub);
        SViewBattlePass.CallStart(BattlePassModel.Instance.level_Users);
    }

    public void Action_btnExit()
    {
        Module.Event_RefreshBattlePass -= Module_Event_RefreshBattlePass;
        gameObject.SetActive(false);

        Module.Action_Event_RefreshNotice();
    }
}
