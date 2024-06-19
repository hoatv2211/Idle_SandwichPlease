using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class TutNode : MonoBehaviour
{
    public string idTut = "None";
    public string descTut;
    public bool isTrigger = false;
    public bool isCameraFocus = false;
    public TutNode nextTut;
    public List<GameObject> listUnlock;

    

    private void Start()
    {
        CallStart();
    }

    public void CallStart()
    {
        if (!string.IsNullOrEmpty(descTut))
        {
            StartCoroutine(ICallStart());
        }
          
    }

    IEnumerator ICallStart()
    {
        yield return null;
        yield return new WaitUntil(() => UIMainGame.Instance!=null);
        string tutHide ="tut_" + (int.Parse(idTut.Substring(4, 2))-1).ToString("00");
        //Debug.LogError(tutHide);
        MapController.Instance.HideTut(tutHide);

        if (isCameraFocus)
        {
            MapController.Instance.cameraCtrl.SetCamTargetFocusNoAction(this.transform);
            MapController.Instance.objDirection.SetTarget(this.transform);
        }
           
        UIMainGame.Instance.Show_TextTutHelper(descTut);

        if(!MapController.Instance.mapData.crTut.Contains(idTut))
            MapController.Instance.mapData.crTut.Add(idTut);
    }

  

    public void Action_CompleteTut()
    {
        //Debug.LogError("comeplete " +idTut);
        UIMainGame.Instance.Hide_TutHelper();
        FirebaseManager.Instance.LogEvent_StepTut(idTut);
        AdjustTracking.Instance.Event_step_tutorial(idTut);

        MapController.Instance.mapData.crTut.Clear();
        MapController.Instance.mapData.AddTut(idTut);

        if (nextTut != null)
            nextTut.gameObject.SetActive(true);

        if (listUnlock.Count > 0)
            foreach (var k in listUnlock)
                k.gameObject.SetActive(true);

        gameObject.SetActive(false);

        if (idTut == "tut_01")
            MapController.Instance.mapData.isMoneyBonus = false;

        if(idTut == "tut_21")
            MapController.Instance.ShowDirection(false);
    }

    public void Hide_Tut()
    {
        if (gameObject.activeInHierarchy)
        {
            MapController.Instance.mapData.AddTut(idTut);
            gameObject.SetActive(false);
        }
           
    }

    private void OnBecameInvisible()
    {
        if(gameObject.activeInHierarchy)
            MapController.Instance.ShowDirection(true);
    }

    private void OnBecameVisible()
    {
        if (gameObject.activeInHierarchy)
            MapController.Instance.ShowDirection(false);
    }
}
