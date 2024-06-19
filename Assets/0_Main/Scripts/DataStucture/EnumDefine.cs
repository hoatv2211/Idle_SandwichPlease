
#region Unit
public enum EObjectType
{
    None = -1,
    Player,
    Worker,
    Customer,
    Counter,
    Environment,
    Item,
    Machine,
    Plant,
    Product,
    NewArea,
    Market,
    Enemy,
    Cashies
}
public enum EObjectName
{
    None = -1,
    new_area,                           // NewArea
    next_map,                           // Market
    cashier,                            // Worker idle
    staff,                              // Worker move
    customer,                           // Customer
    coffee_machine,                     // Machine      
    package_table,
    drive_car_counter,
    table_area,
    counter_node,
    product,
}

public enum EWorkerType
{
    None = -1,
    cashier = 0,
    staff
}

public enum EProductType
{
    None = -1,
    sandwich = 0,
    sandwich_pakage=1,
    coffee=2,
    trash
}
public enum EStatsType
{
    Stack = 0,
    Speed,
}

#endregion




#region Skin
public enum ESkinBuyingType
{
    iap,
    in_game_money,
    ads
}
#endregion