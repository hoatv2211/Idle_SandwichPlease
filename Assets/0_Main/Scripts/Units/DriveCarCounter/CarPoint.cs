using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPoint : MonoBehaviour
{
    [SerializeField] private Transform _lookAtTransform;
    public bool IsPayPoint = false;
    protected bool _isFilled;
    protected string _productType;

    public bool IsFilled { get => _isFilled; set => _isFilled = value; }
    public string ProductType { get => _productType; set => _productType = value; }
    public Transform LookAtTransform => _lookAtTransform;


    [ContextMenu("Generate")]
    public void Generate()
    {
        if (!_lookAtTransform)
            _lookAtTransform = this.transform;
    }
}
