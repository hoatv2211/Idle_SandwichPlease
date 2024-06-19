using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class BoosterCD : MonoBehaviour
{
    public ETypeBooster typeBooster;
    [SerializeField] private Image imgIcon;
    [SerializeField] private Image imgProcess;
    [SerializeField] private TextMeshProUGUI txtTimeProcess;

    [SerializeField] private Sprite[] sprIcons;
    [SerializeField] private int timeCD = 180;

    /// <summary>
    /// 0. Speed
    /// 1. Stack
    /// 3. SpeedEat
    /// </summary>
    /// <param name="_typeBooster"></param>
    public void CallStart(ETypeBooster _typeBooster, int _timeCD = 180)
    {
        typeBooster = _typeBooster;
        timeCD = _timeCD;

        switch (_typeBooster)
        {
            case ETypeBooster.Capacity:
                imgIcon.sprite = sprIcons[1];
                MapController.Instance.stackBooster = true;
                break;
            case ETypeBooster.MoveSpeed:
                imgIcon.sprite = sprIcons[0];
                MapController.Instance.speedBooster = true;
                break;
            case ETypeBooster.EatSpeed:
                imgIcon.sprite = sprIcons[2];
                MapController.Instance.eatBooster = true;
                break;
            default:
                imgIcon.sprite = sprIcons[0];
                break;
        }

        if (crProcess != null)
            StopCoroutine(crProcess);
        crProcess= StartCoroutine(IProcess());
        transform.localScale = Vector3.one;

        FirebaseManager.Instance.LogEvent_Booster(typeBooster.ToString(), "complete");
    }

    public void AddTime()
    {
        timeCD += 180;
      
        if (crProcess != null)
            StopCoroutine(crProcess);
        crProcess = StartCoroutine(IProcess());

        FirebaseManager.Instance.LogEvent_Booster(typeBooster.ToString(), "complete");
    }

    Coroutine crProcess;
    IEnumerator IProcess()
    {
        while (timeCD > 0)
        {
            txtTimeProcess.text = Module.SecondCustomToTime(timeCD);
            yield return new WaitForSeconds(1f);
            timeCD--;
        }

        yield return null;
        switch (typeBooster)
        {
            case ETypeBooster.Capacity:
                MapController.Instance.stackBooster = false;
                break;
            case ETypeBooster.MoveSpeed:
                MapController.Instance.speedBooster = false;
                break;
            case ETypeBooster.EatSpeed:
                MapController.Instance.eatBooster = false;
                break;
            default:
                break;
        }

        UIMainGame.Instance.listBooster.Remove(this);
        SimplePool.Despawn(this.gameObject);
    }
}
