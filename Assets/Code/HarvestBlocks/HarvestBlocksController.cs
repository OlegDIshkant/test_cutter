using System.Collections.Generic;
using UnityEngine;


public class HarvestBlocksController : IHarvestBlocksInfoProvider
{
    private HashSet<HarvestBlock> _blocks = new HashSet<HarvestBlock>();
    private HashSet<HarvestBlock> _selledBlocks = new HashSet<HarvestBlock>();
    private Object _harvestBlockPrefab;
    private Stack<HarvestBlock> _blocksInBackpack;
    private Transform _backpack;

    public int BackpackCapacity { get; }
    public int BlocksInBackpackAmount => _blocksInBackpack.Count;


    public HarvestBlocksController()
    {
        _harvestBlockPrefab = Resources.Load(GameConstants.GetInstance().pathToHarvestBlockPrefab);
        BackpackCapacity = GameConstants.GetInstance().backpackCapacity;
        _blocksInBackpack = new Stack<HarvestBlock>(BackpackCapacity);
        _backpack = new GameObject("Backpack").transform;
    }


    public void Update(IPlayerInfoProvider player, IGardenSpotsInfoProvider gardenSpots, IStorehouseInfoProvider storehouse)
    {
        SpawnBlocksForRippedPlants(gardenSpots);
        UpdateBackpack(player);
        SellingBlocksInBackpack(player, storehouse);
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


    private void UpdateBackpack(IPlayerInfoProvider player)
    {
        _backpack.position = player.BackpackPosition; //player.Position - player.Direction * consts.backpackOffset + Vector3.up * consts.backpackAltitude;
        _backpack.rotation = player.BackpackRotation;
    }


    private void SellingBlocksInBackpack(IPlayerInfoProvider player, IStorehouseInfoProvider storehouse) =>
        StartSelling(FindBlockToSell(player), storehouse.BlocksVanishPoint);


    private void DeleteSelledBlocks()
    {
        foreach (var block in _selledBlocks)
        {
            _blocks.Remove(block);
            GameObject.Destroy(block.gameObject);
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


    private void AttachToBackPack(Transform blockTransform, int placeIndex)
    {
        blockTransform.transform.SetParent(_backpack);
        blockTransform.localPosition = CalcPositionInBackpack(placeIndex);
        blockTransform.localRotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        blockTransform.localScale = CalcScale();
    }


    private void DetachFromBackpack(Transform blockTransform)
    {
        blockTransform.transform.SetParent(null);
    }


    private Vector3 CalcPositionInBackpack(int placeIndex)
    {
        var constants = GameConstants.GetInstance();
        
        var column = (int)(placeIndex / constants.backpackWidth);
        var wid = placeIndex % constants.backpackWidth;
        var len = (int)(column / constants.backpackHeight);
        var hei = column % constants.backpackHeight;

        float widStart = -constants.widthDistance * (constants.backpackWidth - 1) / 2;

        return new Vector3(
            widStart + wid * constants.widthDistance,
            hei * constants.heightDistance,
            -len * constants.lenghtDistance);
    }


    private Vector3 CalcScale()
    {
        var scale = 1f / GameConstants.GetInstance().blockInBackpackShrinkFactor;
        return new Vector3(scale, scale, scale);
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


    private void StartSelling(HarvestBlock block, Vector3 blockVanishPoint) =>
        block?.ChangeState(HarvestBlock.States.DISAPEARING, DisapearArgs(blockVanishPoint));


    private HarvestBlock.Appear.Args PrepareArgsForNewBlock(HarvestBlock block, PlantRipInfo ripInfo) =>
        new HarvestBlock.Appear.Args
        {
            appearPoint = ripInfo.RipPosition,
            landPoint = ripInfo.RipPosition + ripInfo.RipDirection * GameConstants.GetInstance().blockAppearDistance,
            CreateIdleArgs = IdleArgs(block)
        };



    private System.Func<HarvestBlock.Idle.Args> IdleArgs(HarvestBlock block) =>
        () => new HarvestBlock.Idle.Args()
        {
            TryReservePlaceInBackpack = TryReservePlaceInBackPack,
            CreateMoveToBackpackArgs = MoveToBackpackArgs
        };



    private HarvestBlock.MoveToBackpack.Args MoveToBackpackArgs(int placeIndex) =>
        new HarvestBlock.MoveToBackpack.Args()
        {
            placeIndex = placeIndex,
            AttachToBackpack = AttachToBackPack,
            CreateInBackpackArgs = InBackpackArgs
        };



    private HarvestBlock.InBackpack.Args InBackpackArgs(int placeIndex) =>
        new HarvestBlock.InBackpack.Args()
        {
            placeIndex = placeIndex
        };



    private HarvestBlock.Disapeare.Args DisapearArgs(Vector3 blockVanishPoint) =>
        new HarvestBlock.Disapeare.Args()
        {
            vanishPoint = blockVanishPoint,
            DetachFromBackpack = DetachFromBackpack,
            OnBlockDisapeared = OnBlockDisapeared
        };

}


public interface IHarvestBlocksInfoProvider
{
    int BackpackCapacity { get; }
    int BlocksInBackpackAmount { get; }
}
