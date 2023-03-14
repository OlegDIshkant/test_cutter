using System.Data;
using UnityEngine;

public class UiController 
{
    private readonly BlocksCounter _blocksCounter;
    private readonly MoneyDelivery _moneyDelivery;
    private readonly MoneyCounter _moneyCounter;
    private readonly Canvas _canvas;


    public UiController()
    {
        _canvas = GameObject.FindObjectOfType<Canvas>();
        _blocksCounter = GameObject.FindObjectOfType<BlocksCounter>();
        _moneyCounter = GameObject.FindObjectOfType<MoneyCounter>();
        _moneyDelivery = new MoneyDelivery(SpawnPrefabOnCanvas, _moneyCounter.CanvasPosition);
    }


    public void Update(IHarvestBlocksInfoProvider harvestBlocks, IStorehouseInfoProvider storehouse)
    {
        UpdateBlocksCounter(harvestBlocks);
        CoinDelivery(harvestBlocks, storehouse);
    }


    private void UpdateBlocksCounter(IHarvestBlocksInfoProvider harvestBlocks)
    {
        _blocksCounter.SetMax(harvestBlocks.BackpackCapacity);
        _blocksCounter.SetCurrent(harvestBlocks.BlocksInBackpackAmount);
    }

    private void CoinDelivery(IHarvestBlocksInfoProvider harvestBlocks, IStorehouseInfoProvider storehouse)
    {
        _moneyDelivery.Update(
            new InputInfo() 
            { 
                blocksVanishPoint = storehouse.BlocksVanishPoint,
                blockSoldThisFrame = harvestBlocks.BlocksSoldThisFrame 
            });
    }


    private RectTransform SpawnPrefabOnCanvas(string pathToPrefab, Vector2 position)
    {
        var prefab = Resources.Load(pathToPrefab);
        var result = ((GameObject)GameObject.Instantiate(prefab)) .GetComponent<RectTransform>();
        result.SetParent(_canvas.transform);
        result.position = position;
        return result;
    }

}
