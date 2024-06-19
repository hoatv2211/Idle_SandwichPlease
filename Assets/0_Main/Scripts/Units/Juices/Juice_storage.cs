using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Juice_storage : StorageBase
{
    public void PickUpCup(Transform _tray)
    {

    }

    public override void AddProduct(ProductUnit productUnit)
    {
        base.AddProduct(productUnit);
    }

    protected override void RemoveProduct(ProductUnit productUnit)
    {
        base.RemoveProduct(productUnit);
    }

    protected override void OnChangedProducts()
    {
        base.OnChangedProducts();
    }
}
