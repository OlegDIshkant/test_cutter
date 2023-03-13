using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;


public class GardenSpot : MonoBehaviour
{
    private List<int> _nearPlayerPlantIds = new List<int>();
    private List<Plant> _plants;

    public ReadOnlyCollection<int> NearPlayerPlantIds { get; private set; }


    /// <summary>
    /// Сделать все имеющиеся растения готовыми отдать урожай.
    /// </summary>
    public void GrowAllPlants(bool skipAnimations = false)
    {
        foreach (var plant in _plants)
        {
            plant.TryGrow(skipAnimations);
        }
    }


    /// <summary>
    /// Узнать расположение растения по его индексу.
    /// </summary>
    public Vector3 GetPlantPosition(int plantId) => GetPlant(plantId).transform.position;


    /// <summary>
    /// Срезать урожай с растения.
    /// </summary>
    public void RipPlant(int plantId) => GetPlant(plantId).TryRip();


    private void Awake()
    {
        SetUpPlants();
    }


    private void SetUpPlants()
    {
        NearPlayerPlantIds = new ReadOnlyCollection<int>(_nearPlayerPlantIds);
        _plants = this.transform.GetComponentsInChildren<Plant>().ToList();
        foreach (var plant in _plants)
        {
            plant.OnPlayerComeClose += Plant_OnPlayerComeClose;
            plant.OnPlayerGoAway += Plant_OnPlayerGoAway;
        }
    }



    private int PlantId(Plant plant) => _plants.IndexOf(plant);

    private Plant GetPlant(int plantId) => _plants[plantId];


    private void Plant_OnPlayerGoAway(Plant plant)
    {
        var plantId = PlantId(plant);
        _nearPlayerPlantIds.Remove(plantId);
    }


    private void Plant_OnPlayerComeClose(Plant plant)
    {
        var plantId = PlantId(plant);

        if (!_nearPlayerPlantIds.Contains(plantId))
        {
            _nearPlayerPlantIds.Add(plantId);            
        }
    }


}
