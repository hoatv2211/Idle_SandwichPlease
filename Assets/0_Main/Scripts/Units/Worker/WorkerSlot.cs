using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WorkerSlot : MonoBehaviour
{
    public UnitBase _unitBase;
    public bool isInput = true; //Spawn product?
    public EProductType productType;

    private bool _isFilled = false;
    public bool IsFilled { get => _isFilled; set => _isFilled = value; }

    private void OnEnable()
    {
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        yield return new WaitForSeconds(1f);
        MapController.Instance.workerSlots.Add(this);
    }

    private void OnTriggerStay(Collider other)
    {
        OnCheckWorker(other);
    }

    protected virtual void OnCheckWorker(Collider other)
    {

    }
}
