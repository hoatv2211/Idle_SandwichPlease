using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenScale : MonoBehaviour
{
    private Vector3 finalValue;
    private Vector3 initialValue;
    // Update is called once per frame
    private void Start()
    {
        initialValue = transform.localScale;
    }

    public void ApplyEffect(Vector3 to, float _duration)
    {
        this.initialValue = transform.localScale;
        this.finalValue = to;
        time = 0;
        rate = 1 / _duration;
        this.enabled = true;
    }

    private float time, rate;
    void Update()
    {
        if (time >1){
            this.enabled = false;
            return;
        }

        time += rate * Time.deltaTime;
        transform.localScale = Vector3.Lerp(initialValue, finalValue, time);
    }
}