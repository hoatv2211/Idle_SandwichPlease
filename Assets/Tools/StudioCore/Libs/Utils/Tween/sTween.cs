using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sTween
{
    public static void ScaleTo(GameObject gamemObject, Vector3 to, float duration)
    {
        var script = gamemObject.GetComponent<TweenScale>();
        if (script ==null){
            script = gamemObject.AddComponent<TweenScale>();
        }
        script.ApplyEffect(to, duration);
    }
}
