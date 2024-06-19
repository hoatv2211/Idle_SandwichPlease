using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public static class ModalControllerAnimation
{
    public static readonly Color Opaque = new Color(0, 0, 0, 0);

    public static IEnumerator ShowModalAnim(RectTransform r)
    {
        ModalAnims modalAnims = r.GetComponent<ModalAnims>();
        if(!modalAnims)
        {
            yield return null;
            yield break;
        }
        List<DOTweenAnimation> animations = modalAnims.Animations;
        float time = 0;
        animations.ForEach(animation =>
        {
            float animTime = animation.duration * animation.loops;
            if (animTime > time)
            {
                time = animTime;
            }
            animation.DORestart();
        });
        yield return new WaitForSeconds(time);
    }
    public static IEnumerator HideModalAnim(RectTransform r)
    {
        ModalAnims modalAnims = r.GetComponent<ModalAnims>();
        if (!modalAnims)
        {
            yield return null;
            yield break;
        }
        List<DOTweenAnimation> animations = modalAnims.Animations;
        float time = 0;
        animations.ForEach(animation =>
        {
            float animTime = animation.duration * animation.loops;
            if (animTime > time)
            {
                time = animTime;
            }
            animation.DOPlayBackwards();
        });
        yield return new WaitForSeconds(time);
    }

    public static IEnumerator ModalAnimFadeIn(RectTransform r)
    {
        Graphic[] panel = r.GetComponentsInChildren<Graphic>();

        foreach (Graphic p in panel)
        {
            //p.DOColor(Opaque, 0.3f).From().OnComplete(() => { });
        }

        yield return null;
    }

    public static IEnumerator ModalAnimFadeInUnlock(RectTransform r)
    {
        Graphic[] panel = r.GetComponentsInChildren<Graphic>();

        foreach (Graphic p in panel)
        {
            //p.DOColor(Opaque, 0.3f).From().OnComplete(() => { });
        }

        yield return null;
    }

    public static IEnumerator ModalAnimFadeOut(RectTransform r)
    {
        Graphic[] panel = r.GetComponentsInChildren<Graphic>();
        Animator[] animator = r.GetComponentsInChildren<Animator>();

        foreach (Graphic p in panel)
        {
           // p.DOColor(Opaque, 0.3f);
        }

        foreach (var a in animator)
        {
            a.enabled = false;
        }

        yield return new WaitForSeconds(0.3f);
    }
}
