using System;
using System.Linq;
using UnityEngine;


public class Plant : MonoBehaviour
{
    public event Action<Plant> OnPlayerComeClose;
    public event Action<Plant> OnPlayerGoAway;

    private TriggerCatcher _playerCatcher;

    public bool MayBeGrownNow { get; private set; }


    /// <summary>
    /// Сделать растение сново готовым отдать урожай.
    /// </summary>
    public void TryGrow(bool skipAnimations = false)
    {

    }


    /// <summary>
    /// Забрать урожай.
    /// </summary>
    public void TryRip()
    {

    }


    private void Start()
    {
        SetUpPlayerCatcher();
    }


    private void Update()
    {
        CheckForPlayerNearPresence();
    }


    private void SetUpPlayerCatcher()
    {
        _playerCatcher = GetComponent<TriggerCatcher>();
        _playerCatcher.SetUpTarget(GameConstants.GetInstance().playerTag);
    }


    private void CheckForPlayerNearPresence()
    {
#if DEBUG
        if (_playerCatcher.CatchedObjects.Count > 1)
        {
            throw new Exception("Unormal amount of objects in the player catcher: {}");
        }
#endif

        if (_playerCatcher.HasChanged)
        {
            if (_playerCatcher.CatchedObjects.Any())
            {
                OnPlayerComeClose?.Invoke(this);
            }
            else
            {
                OnPlayerGoAway?.Invoke(this);
            }
        }
    }


}
