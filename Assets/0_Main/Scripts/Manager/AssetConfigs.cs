using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetConfigs : Singleton<AssetConfigs>
{
    [Header("Sprites")]
    public List<Sprite> sprIcon_Stores;
    public Sprite GetIconStore(string _id)
    {
        Sprite _sprt = sprIcon_Stores.Find(x => x.name == "icon_store_"+_id);
        if (_sprt == null)
            _sprt = sprIcon_Stores[0];

        return _sprt;
    }

    public List<UnitBase> unitBases;
    public UnitBase GetUnitBase(string _idUnit)
    {
        UnitBase _unit = unitBases.Find(x => x.m_UnitData.idUnit == _idUnit);
        if (_unit == null)
            _unit = unitBases[0];

        return _unit;
    }

    [Header("Customer")]
    public List<CustomerUnit> customerUnits;
    public CustomerUnit GetCustomerUnit(string _id = "")
    {
        CustomerUnit _unit = customerUnits.Find(x => x.IdUnit == _id);
        if (_unit == null)
            _unit = customerUnits[0];

        return _unit;
    }

    [Header("Car")]
    public List<CarUnit> carUnits;
    public CarUnit GetCarUnit(string _id = "")
    {
        CarUnit _unit = carUnits.Find(x => x.m_UnitData.idUnit == _id);
        if (_unit == null)
            _unit = carUnits[0];

        return _unit;
    }

    [Header("Worker")]
    public List<WorkerUnit> workerUnits;
    public WorkerUnit GetWorkerUnit(string _id = "")
    {
        WorkerUnit _unit = workerUnits.Find(x => x.m_UnitData.idUnit == _id);
        if (_unit == null)
            _unit = workerUnits[0];

        return _unit;
    }

    [Header("Peace trash")]
    public List<TrashPiece> trashPieces;
    public TrashPiece GetTrashPiece(string _id = "")
    {
        TrashPiece _unit = trashPieces.Find(x => x.name == _id);
        if (_unit == null)
            _unit = trashPieces[0];

        return _unit;
    }

    [Header("Cash")]
    public CashBox cashBox;
}
