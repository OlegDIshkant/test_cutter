using UnityEngine;


public class StorehouseController : IStorehouseInfoProvider
{
    private readonly Storehouse _storehouse;

    public bool PlayerInStorehouseNow { get; private set; }


    public StorehouseController()
    {
        _storehouse = GameObject.FindObjectOfType<Storehouse>();
    }


    public void Update()
    {
        PlayerInStorehouseNow = _storehouse.PlayerIsInside();
    }

}


public interface IStorehouseInfoProvider
{
    bool PlayerInStorehouseNow { get; }
}