using System.Linq;
using UnityEngine;


public class Storehouse : MonoBehaviour
{
    [SerializeField]
    private Transform _blocksVanishPoint;
    
    private TriggerCatcher _triggerCatcher;

    public bool PlayerIsInside() => _triggerCatcher.CatchedObjects.Any();
    public Vector3 BlocksVanishPoint => _blocksVanishPoint.position;


    private void Awake()
    {
        SetUpPlayerCatcher();
    }

    private void SetUpPlayerCatcher()
    {
        _triggerCatcher = transform.GetComponentInChildren<TriggerCatcher>();
        _triggerCatcher.SetUpTarget(GameConstants.GetInstance().playerTag);
    }

}
