using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardNode : MonoBehaviour
{
    public bool isCanPay=false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            isCanPay = true;
            GetComponent<SpriteRenderer>().color = Color.green;
          
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            isCanPay = false;
            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}
