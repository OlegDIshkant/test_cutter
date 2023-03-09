using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : IPlayerInfoProvider
{
    public Vector3 BackpackPosition { get; private set; }


    public void Update(IStorehouseInfoProvider storehouse, IHarvestBlocksInfoProvider harvestBlocks)
    {
        
    }
}



public interface IPlayerInfoProvider
{
    Vector3 BackpackPosition { get; }
}
