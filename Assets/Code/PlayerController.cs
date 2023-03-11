using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : IPlayerInfoProvider
{
    public Vector3 Position { get; private set; }
    public Vector3 Direction { get; private set; }
    public Vector3 BackpackPosition { get; private set; }
    public bool IsMomentToRip { get; private set; }
    public bool IsInStorehouse { get; private set; }


    public void Update(IStorehouseInfoProvider storehouse, IHarvestBlocksInfoProvider harvestBlocks, IGardenSpotsInfoProvider gardenSpots)
    {
        
    }


}



public interface IPlayerInfoProvider
{
    Vector3 Position { get; }
    Vector3 Direction { get; }
    Vector3 BackpackPosition { get; }

    /// <summary>
    /// Флаг означающий, что игрок выполняет движение скоса урожая и его инструмент как раз находится в позиции для среза растения.
    /// </summary>
    bool IsMomentToRip { get; }

    bool IsInStorehouse { get; }
}
