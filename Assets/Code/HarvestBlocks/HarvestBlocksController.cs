using System.Collections.Generic;
using UnityEngine;


public class HarvestBlocksController : IHarvestBlocksInfoProvider
{
    private HashSet<HarvestBlock> _blocks = new HashSet<HarvestBlock>();
    private HashSet<HarvestBlock> _selledBlocks = new HashSet<HarvestBlock>();
    private Object _harvestBlockPrefab;
    private Stack<HarvestBlock> _blocksInBackpack;

    public int BackpackCapacity { get; }
    public int BlocksInBackpackAmount => _blocksInBackpack.Count;


    public HarvestBlocksController()
    {
        _harvestBlockPrefab = Resources.Load(GameConstants.GetInstance().pathToHarvestBlockPrefab);
        BackpackCapacity = GameConstants.GetInstance().backpackCapacity;
        _blocksInBackpack = new Stack<HarvestBlock>(BackpackCapacity);
    }


    public void Update(IPlayerInfoProvider player, IGardenSpotsInfoProvider gardenSpots)
    {
        SpawnBlocksForRippedPlants(gardenSpots);
        SellingBlocksInBackpack(player);
        DeleteSelledBlocks();
    }


    private void SpawnBlocksForRippedPlants(IGardenSpotsInfoProvider gardenSpots)
    {
        foreach (var ripInfo in gardenSpots.RecentRippings)
        {
            var block = SpawnBlock();
            _blocks.Add(block);
            block.ChangeState(HarvestBlock.States.APPEARING, PrepareArgsForNewBlock(block, ripInfo));
        }
    }


    private void SellingBlocksInBackpack(IPlayerInfoProvider player) =>
        StartSelling(FindBlockToSell(player));


    private void DeleteSelledBlocks()
    {
        foreach (var block in _selledBlocks)
        {
            _blocks.Remove(block);
        }
        _selledBlocks.Clear();
    }


    private HarvestBlock SpawnBlock() =>
        ((GameObject)GameObject.Instantiate(_harvestBlockPrefab)).GetComponent<HarvestBlock>();


    private int? TryReservePlaceInBackPack(HarvestBlock block)
    {
        if (_blocksInBackpack.Count < BackpackCapacity)
        {
            _blocksInBackpack.Push(block);
            return _blocksInBackpack.Count - 1;
        }
        return null;
    }


    private void OnBlockDisapeared(HarvestBlock block)
    {
        if (block == _blocksInBackpack.Peek())
        {
            _blocksInBackpack.Pop();
        }
        _selledBlocks.Add(block);
    }


    private HarvestBlock FindBlockToSell(IPlayerInfoProvider player)
    {
        if (player.IsInStorehouse && _blocksInBackpack.Count > 0)
        {
            var topBlock = _blocksInBackpack.Peek();
            if (topBlock.CurrentState == HarvestBlock.States.IN_BACKPACK)
            {
                return topBlock;
            }
        }
        return null;
    }


    private void StartSelling(HarvestBlock block) =>
        block?.ChangeState(HarvestBlock.States.DISAPEARING, DisapearArgs());


    private HarvestBlock.Appear.Args PrepareArgsForNewBlock(HarvestBlock block, PlantRipInfo ripInfo) =>
        new HarvestBlock.Appear.Args
        {
            appearPoint = ripInfo.RipPosition,
            landPoint = ripInfo.RipPosition + ripInfo.RipDirection,
            CreateIdleArgs = IdleArgs(block)
        };



    private System.Func<HarvestBlock.Idle.Args> IdleArgs(HarvestBlock block) =>
        () => new HarvestBlock.Idle.Args()
        {
            playerCatcher = block.GetComponent<TriggerCatcher>(),
            TryReservePlaceInBackpack = TryReservePlaceInBackPack,
            CreateMoveToBackpackArgs = MoveToBackpackArgs
        };



    private HarvestBlock.MoveToBackpack.Args MoveToBackpackArgs(int placeIndex) =>
        new HarvestBlock.MoveToBackpack.Args()
        {
            placeIndex = placeIndex,
            CreateInBackpackArgs = InBackpackArgs
        };



    private HarvestBlock.InBackpack.Args InBackpackArgs(int placeIndex) =>
        new HarvestBlock.InBackpack.Args()
        {
            placeIndex = placeIndex
        };



    private HarvestBlock.Disapeare.Args DisapearArgs() =>
        new HarvestBlock.Disapeare.Args()
        {
            OnBlockDisapeared = OnBlockDisapeared
        };

}


public interface IHarvestBlocksInfoProvider
{
    int BackpackCapacity { get; }
    int BlocksInBackpackAmount { get; }
}
