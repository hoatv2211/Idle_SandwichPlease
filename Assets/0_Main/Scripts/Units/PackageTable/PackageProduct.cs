using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PackageProduct : ProductUnit
{
    public List<Transform> slotProducts;
    public List<ProductUnit> productUnits;

    public bool isMax => productUnits.Count >= slotProducts.Count;

    public bool Action_AddToPackage(ProductUnit _product)
    {
        bool _isMax = productUnits.Count >= slotProducts.Count;
        
        if (!_isMax)
        {
            int _index = productUnits.Count;
            _product.transform.SetParent(slotProducts[_index]);

            productUnits.Add(_product);
            Vector3 targetPos = slotProducts[_index].localPosition;
            _product.IsMoving = true;
            _product.transform.DOLocalRotate(slotProducts[_index].localEulerAngles, 0.2f)
                .OnComplete(() =>
                {
                    _product.transform.localRotation = slotProducts[_index].localRotation;
                });
            _product.transform.DOLocalJump(targetPos, 1.5f, 1, 0.2f)
                .OnComplete(() =>
                {
                    _product.IsMoving = false;
                    _product.transform.localPosition = targetPos;
                    _product.transform.localRotation = Quaternion.Euler(Vector3.zero);
                    _product.transform.localPosition = Vector3.zero;
                    //OnChangedProducts(productUnit.ObjectNameString);
                });
        }
        

        return _isMax;
    }
}
