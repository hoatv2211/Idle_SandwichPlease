using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PackageStorage : StorageBase
{
    public override void AddProduct(ProductUnit productUnit)
    {
        int index = _productUnits.Count;
        _productUnits.Add(productUnit);

        productUnit.transform.SetParent(_productParent);
        Vector3 targetPos = _slots[index].localPosition;
        //productUnit.transform.localPosition = targetPos;
        //productUnit.transform.localRotation = Quaternion.identity;

        productUnit.IsMoving = true;
        productUnit.transform.DOLocalJump(targetPos, 1.5f, 1, 0.2f)
            .OnComplete(() =>
            {
                productUnit.IsMoving = false;
                productUnit.transform.localPosition = targetPos;
                productUnit.transform.localRotation = Quaternion.identity;
            });


         MapController.Instance.mapData.ShowTut("tut_19");
    }

}
