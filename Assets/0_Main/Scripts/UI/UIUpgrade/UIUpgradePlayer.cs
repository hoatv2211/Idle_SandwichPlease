using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUpgradePlayer : MonoBehaviour
{
    [SerializeField] private UIButton btnClose;
    [SerializeField] private TaskUpgrade taskMoveSpeed; 
    [SerializeField] private TaskUpgrade taskCapacity; 
    [SerializeField] private TaskUpgrade taskProfit;

    private UpgradePlayer upgradePlayer => MapController.Instance.mapData.upgradePlayer;
    public void CallStart()
    {
        btnClose.SetUpEvent(Action_btnClose);

        if (upgradePlayer.isMaxSpeed)
            taskMoveSpeed.MaxLevel();
        else
            taskMoveSpeed.CallStart(Action_UpgradeMoveSpeed,upgradePlayer.nextSpeed());

        if (upgradePlayer.isMaxCapacity)
            taskCapacity.MaxLevel();
        else
            taskCapacity.CallStart(Action_UpgradeCapacity,upgradePlayer.nextCapacity());

        if (upgradePlayer.isMaxProfit)
            taskProfit.MaxLevel();
        else
            taskProfit.CallStart(Action_UpgradeProfit, upgradePlayer.nextProfitup());
    }

    public void Action_UpgradeMoveSpeed()
    {
        upgradePlayer.Upgrade_Speed();

        CallStart();
    }

    public void Action_UpgradeCapacity()
    {
        upgradePlayer.Upgrade_Capacity();

        CallStart();
    }

    public void Action_UpgradeProfit()
    {
        upgradePlayer.Upgrade_Profit();

        CallStart();
    }

    private void Action_btnClose()
    {
        gameObject.SetActive(false);
    }
}
