using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cash : MonoBehaviour
{
    public int value = 1;

    public void SetData(int _value)
    {
        
        value = (int)(_value*MapController.Instance.profit);

        //Debug.LogError(_value + "--" + value);
    }
}
