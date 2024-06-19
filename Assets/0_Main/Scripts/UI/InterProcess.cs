using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

public class InterProcess : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtProcess;
    [SerializeField] private Image imgProcess;

    Coroutine coroutine;
    Action callback;

 
    public void CallStart(Action _callback)
    {
        gameObject.SetActive(true);
        callback = _callback;
        if (coroutine != null)
            StopCoroutine(IProcess());

        coroutine = StartCoroutine(IProcess());
    }

    IEnumerator IProcess()
    {
        int _value = 5;
        imgProcess.fillAmount = 1;
        yield return null;
        imgProcess.DOFillAmount(0, _value).SetEase(Ease.Linear);
        txtProcess.text = _value.ToString();
        while (_value > 0)
        {
            yield return new WaitForSeconds(1);
            _value--;
            txtProcess.text = _value.ToString();
        }

        yield return null;
        gameObject.SetActive(false);
        callback?.Invoke();
        
    }
}
