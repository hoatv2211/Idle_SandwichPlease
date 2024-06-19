using System.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using System;

public class BooterCtrl : MonoBehaviour
{
    public List<BoosterUnit> boosterUnits;
    public List<Transform> trSpawns;
    public BoosterUnit prefabSpawn;
    public Transform baseBooster; //show 1st booster
    [SerializeField] private List<GameObject> crBooter;
    [SerializeField] private List<GameObject> zoneBooter;
    

    private void OnEnable()
    {
        DOVirtual.DelayedCall(1, () => {
            if (UserModel.Instance.level>=3 )
            {
                StartCoroutine(ShowBooster());
            }
        
        });
    }

    IEnumerator ShowBooster()
    {
        yield return null;
        yield return new WaitForSeconds(Module.EasyRandom(40, 60));
        yield return new WaitUntil(() => MapController.Instance.mapData.IsUnlock("package_table_1_1"));
        //Debug.LogError("Can show booster");

        while (true)
        {
            yield return null;
            ShowRandomBooter();
            yield return new WaitForSeconds(Module.EasyRandom(60, 80));
        }
    }

    public void ShowRandomBooter()
    {
        if (!AdsAppLovinController.Instance.isRewardedAdReady)
        {
            AdsAppLovinController.Instance.LoadRewardedAd();
            return;
        }
          

        if (crBooter.Count > 1)
        {
            GameObject g = crBooter[0];
            crBooter.Remove(g);

            FirebaseManager.Instance.LogEvent_Booster(g.GetComponent<BoosterUnit>()?.m_ETypeBooster.ToString(), "fail");

            if (g != UIMainGame.Instance.btnEatSpeed.gameObject)
                SimplePool.Despawn(g);
            else
                g.SetActive(false);
        }

        try
        {
            Transform tr = null;
            List<Transform> trs = trSpawns.FindAll(x => Vector3.Distance(x.position, MapController.Instance.playerCtrl.transform.position) < 6f 
            &&(x.GetComponentInChildren<BoosterUnit>()==null|| x.GetComponentInChildren<BoosterUnit>().isActiveAndEnabled==false));
            
            if (trs.Count > 0)
            {
                tr = trs[Module.EasyRandom(trs.Count)];
            }

            List<ETypeBooster> eTypes = new List<ETypeBooster> { ETypeBooster.CoinBag, ETypeBooster.Capacity, ETypeBooster.MoveSpeed,ETypeBooster.EatSpeed };

            if (crBooter.Count > 0)
            {
                foreach (var k in crBooter)
                {
                    ETypeBooster _type = k.GetComponent<BoosterUnit>().m_ETypeBooster;
                    if (eTypes.Contains(_type))
                        eTypes.Remove(_type);
                }
            }

            ETypeBooster _typeSpawn = eTypes[Module.EasyRandom(0, eTypes.Count)];
            //Debug.LogError(_typeSpawn);

            if (_typeSpawn == ETypeBooster.EatSpeed)
            {
                UIMainGame.Instance.btnEatSpeed.gameObject.SetActive(true);
                GameObject g = UIMainGame.Instance.btnEatSpeed.gameObject;
                if (!crBooter.Contains(g))
                    crBooter.Add(g);

                return;
            }
           

            if (tr!=null)
            {
                GameObject g = SimplePool.Spawn(prefabSpawn.gameObject, tr.position, Quaternion.identity);
                g.transform.SetParent(tr);
                g.GetComponent<BoosterUnit>().CallStart(_typeSpawn);

                if(!crBooter.Contains(g))
                    crBooter.Add(g);
            }
        }
        catch(Exception e)
        {
            Debug.LogError(e.Message);
        }
     
     
    }

    public void ShowBaseBooter()
    {
        GameObject g = SimplePool.Spawn(prefabSpawn.gameObject, baseBooster.position, Quaternion.identity);

        g.GetComponent<BoosterUnit>().CallStart(ETypeBooster.CoinBag);
        if (!crBooter.Contains(g))
            crBooter.Add(g);

        StartCoroutine(ShowBooster());
    }


}
