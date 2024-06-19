using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbstractMasterData { }

[System.Serializable]
public class ObjectBaseData : AbstractMasterData
{
    public EObjectType objectType;
    public string objectName;
    public float speed;

}

[System.Serializable]
public class StatsAndUpgradeData : AbstractMasterData
{
    public int mapID;
    public EObjectType objectType;
    public string objectName;
    public int id;
    public EStatsType statsType;
    public int level;
    public float statsValue;
    public int cost;

}

[System.Serializable]
public class InputAndOutputData : AbstractMasterData
{
    public int mapID;
    public EObjectType objectType;
    public string objectName;
    public int id;
    public EObjectType inputType;
    public Dictionary<string, int> inputNames;
    public EObjectType outputType;
    public string outputName;
    public int outputValue;

}

[System.Serializable]
public class CustomerMData : AbstractMasterData
{
    public int mapID;
    public int customerID;
    public List<string> productList;
    public int numberOfProduct;
    public int numberOfCounter;

}

[System.Serializable]
public class CustomerSpawnLogicData : AbstractMasterData
{
    public int mapID;
    public List<int> customerIDList;
    public Dictionary<int, int> maxCustomerDefineMap;
}

[System.Serializable]
public class UnlockCondition
{
    public EObjectType conditionType;
    public string conditionName;
    public int conditionID;
}

[System.Serializable]
public class UnlockConditionData : AbstractMasterData
{
    public int mapID;
    public EObjectType objectType;
    public string objectName;
    public int id;
    public List<UnlockCondition> conditions;

}


[System.Serializable]
public class AssetCostData : AbstractMasterData
{
    public int mapID;
    public EObjectType objectType;
    public string objectName;
    public int id;
    public int cost;

}

[System.Serializable]
public class ProductCostData : AbstractMasterData
{
    public int mapID;
    public EProductType objectName;
    public int cost;

}
