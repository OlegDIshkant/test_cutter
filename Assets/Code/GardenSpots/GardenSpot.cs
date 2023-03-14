using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;


public class GardenSpot : MonoBehaviour
{
    private List<int> _harvestablePlantIds = new List<int>();
    private List<Plant> _plants;

    public ReadOnlyCollection<int> HarvestablePlantIds { get; private set; }


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
        HarvestablePlantIds = new ReadOnlyCollection<int>(_harvestablePlantIds);

        _plants = SpawnPlants();
        foreach (var plant in _plants)
        {
            PlacePlant(plant);
            plant.OnCanBeHarvestedNow += Plant_OnCanBeHarvestedNow;
            plant.OnCanNotBeHarvestedNow += Plant_OnCanNotBeHarvestedNow;
        }
    }


    private List<Plant> SpawnPlants()
    {
        var prefab = Resources.Load(GameConstants.GetInstance().pathToPlantPrefab);
        var plantsAmount = GameConstants.GetInstance().plantsPerGardenSpot;

        return Enumerable.Range(0, plantsAmount)
            .Select(_ => (GameObject)GameObject.Instantiate(prefab))
            .Select(go => go.GetComponent<Plant>())
            .ToList();
    }


    private void PlacePlant(Plant plant)
    {
        var side = 0.8f;

        var newPosition = new Vector3(
            UnityEngine.Random.Range(-side, side),
            0,
            UnityEngine.Random.Range(-side, side));

        plant.transform.SetParent(this.transform, worldPositionStays: false);
        plant.transform.localPosition = newPosition;
    }



    private int PlantId(Plant plant) => _plants.IndexOf(plant);

    private Plant GetPlant(int plantId) => _plants[plantId];


    private void Plant_OnCanNotBeHarvestedNow(Plant plant)
    {
        _harvestablePlantIds.Remove(PlantId(plant));
    }


    private void Plant_OnCanBeHarvestedNow(Plant plant)
    {
        var plantId = PlantId(plant);

        if (!_harvestablePlantIds.Contains(plantId))
        {
            _harvestablePlantIds.Add(plantId);            
        }
    }


}
