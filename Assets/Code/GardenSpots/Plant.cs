using System;
using System.Linq;
using UnityEngine;


public class Plant : MonoBehaviour
{
    public event Action<Plant> OnPlayerComeClose;
    public event Action<Plant> OnPlayerGoAway;

    [SerializeField]
    private GameObject _grownAppearence;
    [SerializeField]
    private GameObject _rippedAppearence;

    private TriggerCatcher _playerCatcher;

    public bool IsGrown { get; private set; }




    /// <summary>
    /// Сделать растение сново готовым отдать урожай.
    /// </summary>
    public bool TryGrow(bool skipAnimations = false)
    {
        if (IsGrown)
        {
            return false;
        }

        SetAppearence(asGrown: true);
        IsGrown = true;

        return true;
    }


    /// <summary>
    /// Забрать урожай.
    /// </summary>
    public bool TryRip()
    {
        if (!IsGrown)
        {
            return false;
        }

        SetAppearence(asGrown: false);
        IsGrown = false;

        return true;
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


    private void SetAppearence(bool asGrown)
    {
        _grownAppearence.SetActive(asGrown);
        _rippedAppearence.SetActive(!asGrown);
    }


}
