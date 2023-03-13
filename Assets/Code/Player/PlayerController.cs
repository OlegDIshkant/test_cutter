using System;
using UnityEngine;
using MoveInfo = PlayerMovement.Result;
using RipInfo = PlayerRipping.Result;


public class PlayerController : IPlayerInfoProvider
{

    private readonly PlayerMovement _movement;
    private readonly PlayerRipping _ripping;
    private readonly Rigidbody _rigidbody;
    private readonly Transform _rotatableBody;
    private readonly PlayerAnimator _animator;

    private bool _isRipMoment = false;

    public Vector3 Position { get; private set; }
    public Vector3 Direction { get; private set; }
    public Vector3 BackpackPosition { get; private set; }
    public bool IsMomentToRip { get; private set; }
    public bool IsInStorehouse { get; private set; }


    public PlayerController()
    {
        var playerGo = GameObject.FindGameObjectWithTag(GameConstants.GetInstance().playerTag);
        _rigidbody = playerGo.GetComponent<Rigidbody>();
        _rotatableBody = playerGo.transform.GetChild(0);
        _animator = _rotatableBody.GetComponent<PlayerAnimator>();
        if (_animator == null)
        {
            _animator = _rotatableBody.GetComponentInChildren<PlayerAnimator>();
        }
        _animator.OnRipMoment += _animator_OnRipMoment;

        _movement = new PlayerMovement();
        _ripping = new PlayerRipping(
            GetPlayerPosition: () => _rigidbody.transform.position,
            GetPlayerRotation: () => _rotatableBody.rotation);
    }

    public void Update(IStorehouseInfoProvider storehouse, IGardenSpotsInfoProvider gardenSpots)
    {
        var playerInfo = UpdatePlayer(gardenSpots);
        UpdateOutput(storehouse, playerInfo);
    }


    private void UpdateOutput(IStorehouseInfoProvider storehouse, PlayerInfo playerInfo)
    {
        IsInStorehouse = storehouse.PlayerInStorehouseNow;
        Position = playerInfo.position;
        Direction = playerInfo.direction;
        BackpackPosition = playerInfo.position;
        IsMomentToRip = _isRipMoment;

        _isRipMoment = false; // сразу обнуляем флаг, чтобы мочь отловить следующее подобное сообщение
    }


    private PlayerInfo UpdatePlayer(IGardenSpotsInfoProvider gardenSpots)
    {
        var input = new InputArgs()
        { 
            rippingInProgress = _animator.IsRipAnimationPlaying(),
            rippablePlantPosition = gardenSpots.NearPlayerPlantPosition
        };

        var moveInfo = _movement.MakeDecision(input);
        var ripInfo = _ripping.MakeDecision(moveInfo, input);

        return ApplyDecisions(moveInfo, ripInfo);
    }


    private PlayerInfo ApplyDecisions(MoveInfo moveInfo, RipInfo ripInfo)
    {
        var (position, direction) = HandlePhysicalPlacement(moveInfo, ripInfo);
        HandleAnimations(moveInfo, ripInfo);

        return new PlayerInfo()
        {
            position = position,
            direction = direction
        };
    }


    private (Vector3 position, Vector3 direction) HandlePhysicalPlacement(MoveInfo moveInfo, RipInfo ripInfo)
    {
        ApplyVelocity(moveInfo.velocity);

        if (ripInfo.newRotation != null)
        {
            ApplyRotatation(ripInfo.newRotation.Value);
        }
        else 
        {
            ApplyRotatation(lookDirection: moveInfo.velocity);
        }

        return (_rigidbody.position, _rotatableBody.forward);
    }


    private void HandleAnimations(MoveInfo moveInfo, RipInfo ripInfo)
    {
        if (moveInfo.isMovingNow)
        {
            _animator.PlayRunAnim();
        }
        else
        {
            if (ShouldStartRipAnim(ripInfo))
            {
                _animator.PlayRipAnim();
            }
            else if (!_animator.IsRipAnimationPlaying())
            {
                _animator.PlayIdleAnim();
            }
        }
    }


    private bool ShouldStartRipAnim(RipInfo ripInfo) => ripInfo.ripTrigger && _animator.CanPlayRipAnim();


    private void ApplyVelocity(Vector3 velocity) => _rigidbody.velocity = velocity;

    private void ApplyRotatation(Vector3 lookDirection)
    {
        if (lookDirection == Vector3.zero) return;
        _rotatableBody.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
    }

    private void ApplyRotatation(Quaternion rotation) => _rotatableBody.rotation = rotation;

    private void _animator_OnRipMoment()
    {
        _isRipMoment = true;
    }


    private struct PlayerInfo
    {
        public Vector3 position;
        public Vector3 direction;
    }




}





public interface IPlayerInfoProvider
{
    Vector3 Position { get; }
    Vector3 Direction { get; }
    Vector3 BackpackPosition { get; }

    /// <summary>
    /// Флаг означающий, что игрок выполняет движение скоса урожая и его инструмент как раз находится в позиции для среза растения.
    /// </summary>
    bool IsMomentToRip { get; }

    bool IsInStorehouse { get; }
}
