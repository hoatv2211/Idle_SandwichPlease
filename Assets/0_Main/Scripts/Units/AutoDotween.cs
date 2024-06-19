using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AutoDotween : MonoBehaviour
{
    [SerializeField] private DOTweenAnimation animation;
    private void OnEnable()
    {
        animation.DOPlay();
    }

    private void OnDisable()
    {
        animation.DOPause();
    }

    [ContextMenu("Getanimation")]
    void GetAnimation()
    {
        animation = transform.GetComponent<DOTweenAnimation>();
    }
}
