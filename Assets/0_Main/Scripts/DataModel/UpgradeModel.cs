using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MAD/UpgradeModel", fileName = "UpgradeModel")]
public class UpgradeModel : ScriptableObject
{
    public UpgradeData[] PU_Speed;
    public UpgradeData[] PU_Capacity;
    public UpgradeData[] PU_Profit;
    public UpgradeData[] Staff_Speed;
    public UpgradeData[] Staff_Capacity;
    public UpgradeData[] Staff_Employ;

    public LevelProcess[] levelProcesses;
}
