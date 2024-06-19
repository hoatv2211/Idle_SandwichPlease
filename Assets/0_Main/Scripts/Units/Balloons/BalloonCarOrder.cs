using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BalloonCarOrder : Balloon
{
    [SerializeField] private TextMeshPro txtOrder;

    int numberOrder=0;
    public void ShowBalloon(int _numOrder,Transform _tr)
    {
        if (numberOrder == _numOrder)
            return;

        numberOrder = _numOrder;
        gameObject.SetActive(true);
        transform.position = _tr.position;
        txtOrder.text = "<sprite=5>" +_numOrder.ToString();
    }

    public void HideBalloon()
    {
        gameObject.SetActive(false);
    }

}
