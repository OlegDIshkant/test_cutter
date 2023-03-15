using UnityEngine;


[CreateAssetMenu(fileName = "GameConstants", menuName = "ScriptableObjects /GameConstants", order = 1)]
public class GameConstants : ScriptableObject
{
    private static GameConstants _instance;

    public static GameConstants GetInstance()
    {
        if (_instance == null)
        {
            _instance = (GameConstants)Resources.Load("Settings/GameConstants");
        }
        return _instance;
    }


    public string playerTag;

    /// <summary>
    /// Максимально разрешенный угол (в градусах) между вектором направлением игрока и вектором от игрока к растению,
    /// чтобы оастение можно было срезать.
    /// </summary>
    public float maxAngleToRipPlant;
    /// <summary>
    /// В градусах.
    /// </summary>
    public float turningToPlantAngleSpeed;

    public float playerSpeed;

    public string pathToHarvestBlockPrefab;
    public int backpackCapacity;

    public int secToRegeneratePlant;

    public float blockAppearDistance;

    public int backpackWidth;
    public int backpackHeight;
    
    public float heightDistance;
    public float widthDistance;
    public float lenghtDistance;

    public float blockInBackpackShrinkFactor;

    //public float backpackOffset;
    //public float backpackAltitude;

    public float blockFlyDistanceOnSell;
    public float blockFlyDurationOnSell;


    public string pathToCoinPrefab;
    public float coinDeliveryDuration;
    public float coinDeliveryDelay;

    public Vector3 coinStartScale;
    public Vector3 coinMidScale;
    public Vector3 coinEndScale;

    public int moneyPerCoin;

    public float moneyGainAnimDuration;


    public int plantsPerGardenSpot;
    public string pathToPlantPrefab;

}
