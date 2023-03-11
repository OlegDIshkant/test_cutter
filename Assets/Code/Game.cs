using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Game : MonoBehaviour
{
    private StorehouseController _storehouse;
    private GardenSpotsController _gardenSpots;
    private PlayerController _player;
    private HarvestBlocksController _harvestBlocks;
    private UiController _ui;


    private void Start()
    {
        _storehouse = new StorehouseController();
        _player = new PlayerController();
        _gardenSpots = new GardenSpotsController();
        _harvestBlocks = new HarvestBlocksController();
        _ui = new UiController();
    }


    void Update()
    {
        // Порядок обновления контроллеров важен!!!
        _storehouse.Update();
        _player.Update(_storehouse, _harvestBlocks, _gardenSpots);
        _gardenSpots.Update(_player);
        _harvestBlocks.Update(_player, _gardenSpots);
        _ui.Update(_harvestBlocks);
    }
}
