using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class GardenSpotsController : IGardenSpotsInfoProvider
{
    private List<GardenSpot> _gardenSpots;
    private List<PlantRipInfo> _recentRippings;
    private GameConstants _gameConstants;

    public Vector3? NearPlayerPlantPosition => TryFindNearPlayerPlantPosition();
    public ReadOnlyCollection<PlantRipInfo> RecentRippings { get; private set; }


    public GardenSpotsController()
    {
        SetUpConsts();
        SetUpRipInfoContainers();
        SetUpGardenSpots();
    }


    public void Update(IPlayerInfoProvider player)
    {
        ForgetPreviousRippings(); // информация о скосе растения не должна жить дольше одной итерации главного цикла
        CheckIfPlantToBeRippedBy(player);
    }


    private void SetUpGardenSpots()
    {
        _gardenSpots = GameObject.FindObjectsOfType<GardenSpot>().ToList();
        foreach (var spot in _gardenSpots)
        {
            spot.GrowAllPlants(skipAnimations: true);
        }
    }


    private void SetUpRipInfoContainers()
    {
        _recentRippings = new List<PlantRipInfo>();
        RecentRippings = new ReadOnlyCollection<PlantRipInfo>(_recentRippings);
    }


    private void SetUpConsts() =>
        _gameConstants = GameConstants.GetInstance();


    private void ForgetPreviousRippings() => _recentRippings.Clear();


    private void CheckIfPlantToBeRippedBy(IPlayerInfoProvider player)
    {
        if (player.IsMomentToRip)
        {
            if (TryRipOnePlant(player.Position, player.Direction, out var ripInfo))
            {
                _recentRippings.Add(ripInfo);
            }
            else
            {
                Debug.LogError("Player has failed to rip a plant.");
            }
        }
    }


    private bool TryRipOnePlant(Vector3 playerPosition, Vector3 playerDirection, out PlantRipInfo ripInfo)
    {
        foreach (var spot in _gardenSpots)
        {
            foreach (var position in PlantPositions(spot))
            {
                if (PlantCanBeRipped(position, playerPosition, playerDirection, out ripInfo))
                {
                    return true;
                }
            }
        }

        ripInfo = default;
        return false;
    }


    private IEnumerable<Vector3> PlantPositions(GardenSpot spot) =>
        spot.NearPlayerPlantIds.Select(id => spot.GetPlantPosition(id));


    private bool PlantCanBeRipped(Vector3 plantPosition, Vector3 playerPosition, Vector3 playerDirection, out PlantRipInfo ripInfo)
    {
        plantPosition.y = playerPosition.y = playerDirection.y = 0;
        var playerToPlant = playerPosition - plantPosition;

        if (Vector3.Angle(playerToPlant, playerDirection) <= _gameConstants.maxAngleToRipPlant)
        {
            ripInfo = new PlantRipInfo(ripPosition: plantPosition, ripDirection: playerDirection);
            return true;
        }
        else
        {
            ripInfo = default;
            return false;
        }
    }


    private Vector3? TryFindNearPlayerPlantPosition()
    {
        foreach (var spot in _gardenSpots)
        {
            if (spot.NearPlayerPlantIds.Any())
            {
                return spot.GetPlantPosition(spot.NearPlayerPlantIds.First());
            }
        }

        return null;
    }

    
}


/// <summary>
/// Информация о пожатии урожая.
/// </summary>
public struct PlantRipInfo
{
    public Vector3 RipPosition { get; set; }
    public Vector3 RipDirection { get; set; }


    public PlantRipInfo(Vector3 ripPosition, Vector3 ripDirection)
    {
        RipPosition = ripPosition;
        RipDirection = ripDirection;
    }
}


public interface IGardenSpotsInfoProvider
{

    public Vector3? NearPlayerPlantPosition { get; }

    public ReadOnlyCollection<PlantRipInfo> RecentRippings { get; }
}