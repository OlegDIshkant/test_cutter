using UnityEngine;

public class UiController 
{
    private readonly BlocksCounter _blocksCounter;


    public UiController()
    {
        _blocksCounter = GameObject.FindObjectOfType<BlocksCounter>();
    }


    public void Update(IHarvestBlocksInfoProvider harvestBlocks)
    {
        UpdateBlocksCounter(harvestBlocks);
    }


    private void UpdateBlocksCounter(IHarvestBlocksInfoProvider harvestBlocks)
    {
        _blocksCounter.SetMax(harvestBlocks.BackpackCapacity);
        _blocksCounter.SetCurrent(harvestBlocks.BlocksInBackpackAmount);
    }



}
