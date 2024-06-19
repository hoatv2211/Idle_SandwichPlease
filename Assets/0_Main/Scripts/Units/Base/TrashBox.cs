using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TrashBox : BasePickAndDrop
{
    public Transform workerSlot;

    public override bool PickUp(ProductUnit productUnit, int maxQuantity)
    {
        if (_isCountdown)
        {
            return false;
        }

        CountdownForNextPick();
        productUnit.transform.SetParent(_productParent);
        Vector3 targetPos = _productParent.localPosition;

        productUnit.IsMoving = true;
        productUnit.transform.DOLocalJump(targetPos, _jumpForce, 1, _speed)
            .OnComplete(() =>
            {
                productUnit.IsMoving = false;
                productUnit.transform.localPosition = targetPos;
                productUnit.transform.localRotation = Quaternion.identity;
                if (productUnit)
                {
                    SimplePool.Despawn(productUnit.gameObject);
                }
            });


        MapController.Instance.CompleteTut("tut_10");
        return true;
    }

}
