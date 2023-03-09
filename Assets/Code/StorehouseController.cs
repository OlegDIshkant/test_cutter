

using UnityEngine;


public class StorehouseController : IStorehouseInfoProvider
{
    public bool PlayerInStorehouseNow { get; private set; }

    public void Update()
    {

    }

}


public class IStorehouseInfoProvider
{

    bool PlayerInStorehouseNow { get; }
}