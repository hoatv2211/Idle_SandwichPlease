using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneUnit : UnitBase
{
    [Header("BUILDING")]
    [Tooltip("Unlock")]
    [SerializeField] private List<GameObject> listLocked;
    [SerializeField] private List<GameObject> listUnlocked;
    [SerializeField] private ParticleSystem fxUnlock;


    public override void CallStart()
    {
        if (m_UnitData.isUnlocked)
        {
            Action_Show(true);
        }
    }

    private void Action_Show(bool _isUnlock = false)
    {
        foreach (var k in listLocked)
        {
            k.SetActive(!_isUnlock);
        }

        foreach (var k in listUnlocked)
        {
            k.SetActive(_isUnlock);
        }

    }


    public override void Action_UnlockUnit()
    {
        base.Action_UnlockUnit();
        Action_Show(true);

        if (fxUnlock != null)
        {
            fxUnlock.gameObject.SetActive(true);
            fxUnlock.Play(true);
        }

    }
}
