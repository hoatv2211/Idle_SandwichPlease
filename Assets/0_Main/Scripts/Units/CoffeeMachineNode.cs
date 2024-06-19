using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoffeeMachineNode : UnitBase
{
    [Header("Machine")]
    public UnitDataBase unitDataBase;

    public List<GameObject> listLocked;
    public List<GameObject> listUnlocked;

    public ParticleSystem fxUnlock;
    public ParticleSystem fxUpgrade;
    public ParticleSystem fxWork;
    public Animator _animator;

    [Header("Products")]
    public StorageBase storageBase;
    public GameObject objMaxItem;
    public GameObject objProduct;
    public Transform trProducts;
    public List<Transform> listTrSpawns;

    [Header("UseLate")]
    [SerializeField] private List<UnlockLevel> unlockLevels;


    public float speed => unitDataBase.Speed(m_UnitData.crLevel)>0? unitDataBase.Speed(m_UnitData.crLevel):1;
    public int capacity => (int)(unitDataBase.Capacity(m_UnitData.crLevel) > 0 ? unitDataBase.Capacity(m_UnitData.crLevel) : 6);
    public override void CallStart()
    {
        if (m_UnitData.isUnlocked)
        {
            Action_Show(true);
        }

        unitDataBase = GameManager.Instance.mapModel.GetUnitDataBase(IdUnit);

        ////Show building
        //if (!m_UnitData.isUnlocked)
        //    return;

        //int index = m_UnitData.crLevel;
        //if (index >= unlockLevels.Count)
        //    index = unlockLevels.Count - 1;

        ////Debug.LogError(index);
        //UnlockLevel unlockLevel = unlockLevels[index];
        //foreach (var k in unlockLevel.listOffs)
        //    k.SetActive(false);
        //foreach (var k in unlockLevel.listOns)
        //    k.SetActive(true);

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

        if (_isUnlock)
        {
            StartCoroutine(IAction_Work());
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

        m_UnitData.isUnlocked = true;
    }

    public override void Action_UpgradeUnit()
    {
        if (fxUpgrade != null)
        {
            fxUpgrade.gameObject.SetActive(true);
            fxUpgrade.Play(true);
        }

        //if (_animator)
        //{
        //    _animator.SetBool("isSpawn", true);

        //}

        m_UnitData.crLevel++;
        MapController.Instance.mapData.GetUnlockUnitData(IdUnit).crLevel= m_UnitData.crLevel;
    }

    private IEnumerator IAction_Work()
    {
        yield return null;
        fxWork.gameObject.SetActive(true);
        fxWork.Play();

        while (true)
        {
            if (m_UnitData == null)
            {
                yield return null;
            }
            bool check = CheckProduce;
            if (_animator)
            {
                //_animator.SetBool("isSpawn", check);
                
            }
            if (check)
            {
                yield return new WaitForSeconds(speed);
                Produce();
            }
           
            yield return null;
            yield return null;
            yield return null;
            objMaxItem.SetActive(storageBase.ProductUnits.Count >= capacity);
        }
    }

    public void Produce()
    {
        //Debug.Log("Produce");

        GameObject g = SimplePool.Spawn(objProduct,Vector3.zero,Quaternion.identity);
        storageBase.AddProduct(g.GetComponent<ProductUnit>());
    }
    public bool CheckProduce
    {
        get
        {
            if (storageBase.ProductUnits.Count >= capacity)
                return false;

            return true;
        }
    }
}
