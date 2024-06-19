using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class OutlineAnim : MonoBehaviour
{
    public float _scaleRate = 1.2f;
    private Vector3 baseScale;

    private void OnEnable()
    {
        baseScale = this.transform.localScale;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerCtrl player = other.gameObject.GetComponent<PlayerCtrl>();
        if (player)
        {
            transform.DOScale(baseScale*_scaleRate, 0.2f);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerCtrl player = other.gameObject.GetComponent<PlayerCtrl>();
        if (player)
        {
            transform.DOScale(baseScale, 0.2f);
        }
    }
}
