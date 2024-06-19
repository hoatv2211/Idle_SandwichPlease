using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SeatPoint : CustomerPoint
{
    public TableCreateArea tableArea;
    public CustomerUnit customerUnit;
    public bool isEating = false;

    public bool isBlocked => tableArea.isBlocked;
    public bool isSlotFree => !tableArea.piecesGroup.isGotTrash() && !IsFilled && customerUnit == null;
    public void SetUnitBase(TableCreateArea _unit)
    {
        tableArea = _unit;
    }

    public void SetCustomer(CustomerUnit _customerUnit)
    {
        customerUnit = _customerUnit;
        IsFilled = true;
    }

    public bool isCheckFilled()
    {
        if (customerUnit != null)
            return true;

        return false;
    }

    public void Clean()
    {
        IsFilled = false;
        isEating = false;
        customerUnit = null;
    }

    public bool CheckClean()
    {
        if (transform.childCount == 1)
        {
            return true;
        }

        return false;
    }

    private void OnEnable()
    {
        DOVirtual.DelayedCall(0.1f, () => MapController.Instance.seatPoints.Add(this));
    }

    private void OnDisable()
    {
        MapController.Instance.seatPoints.Remove(this);
    }
}
