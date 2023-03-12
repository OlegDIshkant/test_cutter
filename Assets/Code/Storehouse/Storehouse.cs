using System.Linq;
using UnityEngine;


public class Storehouse : MonoBehaviour
{
    private TriggerCatcher _triggerCatcher;

    public bool PlayerIsInside() => _triggerCatcher.CatchedObjects.Any();


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
