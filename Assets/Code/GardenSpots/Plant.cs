using System;
using System.Linq;
using UnityEngine;


public class Plant : MonoBehaviour
{
    public event Action<Plant> OnCanBeHarvestedNow;
    public event Action<Plant> OnCanNotBeHarvestedNow;

    [SerializeField]
    private GameObject _grownAppearence;
    [SerializeField]
    private GameObject _rippedAppearence;

    private TriggerCatcher _playerCatcher;

    private bool _isGrown = false;
    private bool _playerIsClose = false;




    /// <summary>
    /// Сделать растение сново готовым отдать урожай.
    /// </summary>
    public bool TryGrow(bool skipAnimations = false)
    {
        if (_isGrown)
        {
            return false;
        }

        SetAppearence(asGrown: true);
        _isGrown = true;
        DecideIfCanBeHarvestedNow();

        return true;
    }


    /// <summary>
    /// Забрать урожай.
    /// </summary>
    public bool TryRip()
    {
        if (!_isGrown)
        {
            return false;
        }

        SetAppearence(asGrown: false);
        _isGrown = false;
        DecideIfCanBeHarvestedNow();

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
            _playerIsClose = _playerCatcher.CatchedObjects.Any();
            DecideIfCanBeHarvestedNow();
        }
    }


    private void DecideIfCanBeHarvestedNow()
    {
        if (_playerIsClose && _isGrown)
        {
            OnCanBeHarvestedNow?.Invoke(this);
        }
        else
        {
            OnCanNotBeHarvestedNow?.Invoke(this);
        }
    }


    private void SetAppearence(bool asGrown)
    {
        _grownAppearence.SetActive(asGrown);
        _rippedAppearence.SetActive(!asGrown);
    }


}
