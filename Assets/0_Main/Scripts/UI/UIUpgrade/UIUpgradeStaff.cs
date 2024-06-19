using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUpgradeStaff : MonoBehaviour
{
    [SerializeField] private UIButton btnClose;
    [SerializeField] private TaskUpgrade taskMoveSpeed;
    [SerializeField] private TaskUpgrade taskCapacity;
    [SerializeField] private TaskUpgrade taskEmploy;
    [SerializeField] private GameObject[] tutHands;
    private UpgradeStaff upgradeStaff => MapController.Instance.mapData.upgradeStaff;
    public void CallStart()
    {
        btnClose.SetUpEvent(Action_btnClose);

        if (upgradeStaff.isMaxSpeed)
            taskMoveSpeed.MaxLevel();
        else
            taskMoveSpeed.CallStart(Action_UpgradeMoveSpeed, upgradeStaff.nextSpeed());

        if (upgradeStaff.isMaxCapacity)
            taskCapacity.MaxLevel();
        else
            taskCapacity.CallStart(Action_UpgradeCapacity, upgradeStaff.nextCapacity());

        if (upgradeStaff.isMaxEmploy)
            taskEmploy.MaxLevel();
        else
        {
            taskEmploy.CallStart(Action_UpgradeEmploy, upgradeStaff.nextlvEmploy());
            if (upgradeStaff.lvEmploy == 0)
            {
                taskMoveSpeed.ShowLocked();
                taskCapacity.ShowLocked();
            }
        }
           
        foreach(var k in tutHands)
        {
            k.SetActive(upgradeStaff.lvEmploy == 0);
        }
    }

    public void Action_UpgradeMoveSpeed()
    {
        upgradeStaff.Upgrade_Speed();

        CallStart();
    }

    public void Action_UpgradeCapacity()
    {
        upgradeStaff.Upgrade_Capacity();

        CallStart();
    }

    public void Action_UpgradeEmploy()
    {
        upgradeStaff.Upgrade_Employ();
        MapController.Instance.SpawnWorker();
        CallStart();

        taskMoveSpeed.ShowUnlocked();
        taskCapacity.ShowUnlocked();
    }

    private void Action_btnClose()
    {
        gameObject.SetActive(false);
    }
}
