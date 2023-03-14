using UnityEngine;


public class StorehouseController : IStorehouseInfoProvider
{
    private readonly Storehouse _storehouse;

    public bool PlayerInStorehouseNow { get; private set; }
    public Vector3 BlocksVanishPoint => _storehouse.BlocksVanishPoint;


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
    public Vector3 BlocksVanishPoint { get; }
}