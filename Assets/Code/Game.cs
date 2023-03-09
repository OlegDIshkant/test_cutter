using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Game : MonoBehaviour
{

    private readonly StorehouseController _storehouse = new StorehouseController();
    private readonly GardenSpotsController _gardenSpots = new GardenSpotsController();
    private readonly PlayerController _player = new PlayerController();
    private readonly HarvestBlocksController _harvestBlocks = new HarvestBlocksController();
    private readonly UiController _ui = new UiController();



    void Update()
    {
        // Порядок обновления контроллеров важен!!!
        _storehouse.Update();
        _gardenSpots.Update(_player);
        _player.Update(_storehouse, _harvestBlocks);
        _harvestBlocks.Update(_player, _gardenSpots);
        _ui.Update(_harvestBlocks);
    }
}
