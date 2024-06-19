using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DoorAnim : MonoBehaviour
{
    public Animator animator;
    public bool isGoIn = false;
    

    List<GameObject> listObject = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="Player"|| other.gameObject.tag == "Worker")
        {
            listObject.Add(other.gameObject);

            if (isGoIn)
            {
                animator.SetBool("isOut", true);

            }
            else
            {
                animator.SetBool("isIn", true);
            }
 
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Worker")
        {
            listObject.Remove(other.gameObject);
            if (listObject.Count == 0)
            {
                animator.SetBool("isIn", false);
                animator.SetBool("isOut", false);

            }
        }

    }
}
