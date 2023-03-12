using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;


/// <summary>
/// Инкапсулирует часто используемую процедуру ловли определенного игрового предмета внутрь триггера.
/// </summary>
public class TriggerCatcher : MonoBehaviour
{
    private string _targetTag;
    private List<GameObject> _objsInsideTrigger = new List<GameObject>();

    public bool MaySetUpTarget => !_objsInsideTrigger.Any();
    public ReadOnlyCollection<GameObject> CatchedObjects { get; private set; }
    /// <summary>
    /// Модифицировался ли <see cref="_objsInsideTrigger"/>.
    /// </summary>
    public bool HasChanged { get; private set; }


    private void Awake()
    {
        CatchedObjects = new ReadOnlyCollection<GameObject>(_objsInsideTrigger);
    }


    public void SetUpTarget (string tag)
    {
        if (!MaySetUpTarget)
        {
            throw new System.InvalidOperationException("Inappropriate time to set up a trogger catcher.");
        }
        _targetTag = tag;
    }


    /// <summary>
    /// Устонавливает <see cref="HasChanged"/> в false, чтобы можно было отследить новые изменения.
    /// </summary>
    public void CallItCheckpoint() => HasChanged = false;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == _targetTag)
        {
#if DEBUG
            if (_objsInsideTrigger.Contains(other.gameObject))
            {
                throw new Exception($"The object {other.gameObject} is supposed to already be inside the trigger.");
            }
#endif

            _objsInsideTrigger.Add(other.gameObject);
            HasChanged = true;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == _targetTag)
        {
#if DEBUG
            if (!_objsInsideTrigger.Contains(other.gameObject))
            {
                throw new Exception($"The object {other.gameObject} is supposed to be inside the trigger, but it is not.");
            }
#endif

            _objsInsideTrigger.Remove(other.gameObject);
            HasChanged = true;
        }
    }

}
