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
    /// ������� ��� ��������� �������� �������� ������ ������.
    /// </summary>
    public void GrowAllPlants(bool skipAnimations = false)
    {
        foreach (var plant in _plants)
        {
            plant.TryGrow(skipAnimations);
        }
    }


    /// <summary>
    /// ������ ������������ �������� �� ��� �������.
    /// </summary>
    public Vector3 GetPlantPosition(int plantId) => GetPlant(plantId).transform.position;


    /// <summary>
    /// ������� ������ � ��������.
    /// </summary>
    public void RipPlant(int plantId) => GetPlant(plantId).TryRip();


    private void Awake()
    {
        SetUpPlants();
    }


    private void SetUpPlants()
    {
        HarvestablePlantIds = new ReadOnlyCollection<int>(_harvestablePlantIds);
        _plants = this.transform.GetComponentsInChildren<Plant>().ToList();
        foreach (var plant in _plants)
        {
            plant.OnCanBeHarvestedNow += Plant_OnCanBeHarvestedNow;
            plant.OnCanNotBeHarvestedNow += Plant_OnCanNotBeHarvestedNow;
        }
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
