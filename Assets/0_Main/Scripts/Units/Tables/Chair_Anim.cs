using DG.Tweening;
using UnityEngine;

public class Chair_Anim : MonoBehaviour
{
    public Vector3 vtOn;
    public Vector3 vtOff;

    Tween twOn;
    Tween twOff;
    public void Action_On()
    {
        if (twOff != null)
            twOff.Kill();

        if (twOn != null)
            twOn.Kill();

        twOn = transform.DOLocalRotate(vtOn, 0.1f);
    }

    public void Action_Off()
    {
        if (twOff != null)
            twOff.Kill();

        twOff = transform.DOLocalRotate(vtOff, 0.1f);
    }
}
