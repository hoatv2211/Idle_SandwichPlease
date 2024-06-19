using System;
using System.Collections.Generic;
using UnityEngine;

public class CounterStorage : StorageBase
{

    [SerializeField] private CounterPickup counterPickup;

    public void Action_GetProduct(CustomerPickup customer)
    {
        //Debug.Log(this.gameObject.name+"---"+ customer.name);
        for (int i = ProductUnits.Count - 1; i >= 0; i--)
        {
            if (ProductUnits[i].IsMoving)
            {
                continue;
            }
            bool check = customer.PickUp(ProductUnits[i], 0);
            if (check)
            {
                RemoveProduct(ProductUnits[i]);
            }
        }
    }

    protected override void OnTriggerStay(Collider other)
    {
     
    }

    public override void AddProduct(ProductUnit productUnit)
    {
        _productUnits.Add(productUnit);

        switch (productUnit.typeProduct)
        {
            case EProductType.sandwich_pakage:
                MapController.Instance.CompleteTut("tut_20");
                break;
        }
        OnChangedProducts();
    }

    protected override void RemoveProduct(ProductUnit productUnit)
    {
        base.RemoveProduct(productUnit);
        counterPickup.RemoveProduct(productUnit);
    }
}
