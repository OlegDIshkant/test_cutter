using UnityEngine;

public class GameConstants : ScriptableObject
{
    public static GameConstants GetInstance()
    {
        var instances = FindObjectsOfType<GameConstants>();
        if (instances.Length > 1)
        {
            throw new System.Exception("Too many instances.");
        }
        else if (instances.Length < 0)
        {
            throw new System.Exception("No instances.");
        }
        return instances[0];
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

}
