using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BalloonCustomerOrder : MonoBehaviour
{
    [SerializeField] private TextMeshPro txtOrder;

    int numberOrder = 0;
    public void ShowBalloon(int _numOrder, Transform _tr)
    {
        if (numberOrder == _numOrder)
            return;

        numberOrder = _numOrder;
        gameObject.SetActive(true);
        transform.position = _tr.position;
        txtOrder.text = "<sprite=0>" + _numOrder.ToString();
    }

    public void HideBalloon()
    {
        numberOrder = 0;
        gameObject.SetActive(false);
    }
}
