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
    /// ����������� ����������� ���� (� ��������) ����� �������� ������������ ������ � �������� �� ������ � ��������,
    /// ����� �������� ����� ���� �������.
    /// </summary>
    public float maxAngleToRipPlant;
    /// <summary>
    /// � ��������.
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

}
