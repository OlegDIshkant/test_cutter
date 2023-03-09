using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GardenSpotsController : IGardenSpotsInfoProvider
{
    public bool PlayerIsNearbyNow { get; private set; }


    public void Update(IPlayerInfoProvider player)
    {

    }
}


public interface IGardenSpotsInfoProvider
{

    bool PlayerIsNearbyNow { get; }
}