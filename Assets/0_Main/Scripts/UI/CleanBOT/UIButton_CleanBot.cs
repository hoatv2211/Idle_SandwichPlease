using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIButton_CleanBot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtTime;
    [SerializeField] private int timeCD = 0;

    private void OnEnable()
    {
        Module.Event_CleanBot += Module_Event_CleanBot;
    }

    Coroutine corCountdown;
    private void Module_Event_CleanBot(int _timeLeft)
    {
        timeCD = Module.timeCleanBot;

        if (corCountdown != null)
            StopCoroutine(corCountdown);

         corCountdown= StartCoroutine(IECountdownTime());
    }

    private void OnDisable()
    {
        Module.Event_CleanBot -= Module_Event_CleanBot;
    }

    IEnumerator IECountdownTime()
    {
        while (timeCD > 0)
        {
            txtTime.text = Module.SecondCustomToTime(timeCD);
            yield return new WaitForSeconds(1f);
            timeCD--;
            Module.timeCleanBot--;
        }

        txtTime.text = "00m00s";
        Module.timeCleanBot = 0;
    }

}
