using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerPoint : MonoBehaviour
{
    [SerializeField] private Transform _lookAtTransform;
    public bool IsPayPoint =false;

    [SerializeField] protected bool _isFilled;
    protected string _productType;

    public bool IsFilled { get => _isFilled; set => _isFilled = value; }
    public string ProductType { get => _productType; set => _productType = value; }
    public Transform LookAtTransform => _lookAtTransform;


    [ContextMenu("Generate")]
    public void Generate()
    {
        if (!_lookAtTransform&&transform.childCount>0)
            _lookAtTransform = transform.GetChild(0);
    }
}
