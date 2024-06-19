using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCreateArea : UnitBase
{
    public List<GameObject> listLocked;
    public List<GameObject> listUnlocked;
    public ParticleSystem fxUnlock;

    public override void CallStart()
    {
        Action_Show(m_UnitData.isUnlocked);
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

        MapController.Instance.mapData.isMoneyBonus = false;
       
    }
}
