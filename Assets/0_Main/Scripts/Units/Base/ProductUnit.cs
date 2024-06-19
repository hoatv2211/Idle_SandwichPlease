using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ProductData:AbstractMasterData
{
    public string id;
    public EProductType type;
    public int cost;
}
public class ProductUnit : UnitBase
{
    [Space, Header("PRODUCT UNIT - DATA")]
    public ProductData productData;
    public EProductType typeProduct;

    private bool _isMoving = false;
    private BoxCollider _boxCollider;

    public bool IsMoving { get => _isMoving; set => _isMoving = value; }

    private BoxCollider BoxCollider
    {
        get
        {
            if (!_boxCollider)
            {
                _boxCollider = GetComponentInChildren<BoxCollider>();
            }
            return _boxCollider;
        }
    }

    public Vector3 BoxSize => BoxCollider == null ? Vector3.one : _boxCollider.size;

}
