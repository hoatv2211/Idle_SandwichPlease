using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EDefaultAnim
{
    Typing,
    Worker
}
public class AnimCashies : MonoBehaviour
{
    public EDefaultAnim defaultAnim;
    public Animator animator;

    private void OnEnable()
    {
        switch (defaultAnim)
        {
            case EDefaultAnim.Typing:
                animator.SetBool("isTyping",true);
                break;
            case EDefaultAnim.Worker:
                animator.SetBool("isWorking", true);
                break;
            default:
                break;
        }
    }


}
