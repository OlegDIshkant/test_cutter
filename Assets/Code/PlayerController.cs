using System;
using UnityEngine;
using MoveInfo = PlayerMovement.Result;
using RipInfo = PlayerRipping.Result;


public class PlayerController : IPlayerInfoProvider
{
    private static readonly string RIP_ANIM_ID = "Rip";
    private static readonly string RUN_ANIM_ID = "Run";

    private readonly PlayerMovement _movement;
    private readonly PlayerRipping _ripping;
    private readonly Rigidbody _rigidbody;
    private readonly Transform _rotatableBody;
    private readonly Animator _animator;
    private readonly RipMomentTrigger _ripMomentTrigger = new RipMomentTrigger();

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
        _animator = playerGo.GetComponent<Animator>();

        _movement = new PlayerMovement();
        _ripping = new PlayerRipping(
            GetPlayerPosition: () => _rigidbody.transform.position,
            GetPlayerRotation: () => _rotatableBody.rotation);
    }


    public void AnimEvent_OnRipMoment()
    {
        _ripMomentTrigger.Fire();
    }


    public void Update(IStorehouseInfoProvider storehouse, IGardenSpotsInfoProvider gardenSpots)
    {
        var playerInfo = UpdatePlayer(gardenSpots);
        UpdateOutput(storehouse, playerInfo);
    }


    private void UpdateOutput(IStorehouseInfoProvider storehouse, PlayerInfo playerInfo)
    {
        _ripMomentTrigger.CheckAndRelease(out var isRipMoment);

        IsInStorehouse = storehouse.PlayerInStorehouseNow;
        Position = playerInfo.position;
        Direction = playerInfo.direction;
        BackpackPosition = playerInfo.position;
        IsMomentToRip = isRipMoment;
    }


    private PlayerInfo UpdatePlayer(IGardenSpotsInfoProvider gardenSpots)
    {
        var moveInfo = _movement.MakeDecision();
        var ripInfo = _ripping.MakeDecision(moveInfo, gardenSpots.NearPlayerPlantPosition);

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
            PlayRunAnim();
        }
        else
        {
            if (ShouldStartRipAnim(ripInfo))
            {
                PlayRipAnim();
            }
            else if (!IsRipAnimationPlaying())
            {
                PlayIdleAnim();
            }
        }
    }


    private bool ShouldStartRipAnim(RipInfo ripInfo) => ripInfo.ripTrigger && _ripMomentTrigger.IsReleased;


    private void ApplyVelocity(Vector3 velocity) => _rigidbody.velocity = velocity;

    private void ApplyRotatation(Vector3 lookDirection) => new NotImplementedException();

    private void ApplyRotatation(Quaternion rotation) => new NotImplementedException();

    private void PlayRunAnim()
    {
        _animator.SetBool(RUN_ANIM_ID, true);
    }

    private void PlayRipAnim()
    {
        _ripMomentTrigger.StartWaitingForFire();

        _animator.SetTrigger(RIP_ANIM_ID);
        _animator.ResetTrigger(RIP_ANIM_ID);
    }

    private void PlayIdleAnim()
    {
        _animator.SetBool(RUN_ANIM_ID, false);        
    }


    private bool IsRipAnimationPlaying() =>
        _animator.GetCurrentAnimatorStateInfo(0).IsName(RIP_ANIM_ID);


    private struct PlayerInfo
    {
        public Vector3 position;
        public Vector3 direction;
    }



    private class RipMomentTrigger
    {
        private enum States { NONE, WAITING, FIRE }
        private States _state = States.NONE;

        public bool IsReleased => _state == States.NONE;
        public bool IsWaitingForFire => _state == States.WAITING;


        public void CheckAndRelease(out bool wasFired)
        {
            if (_state == States.NONE)
            {
                throw new InvalidOperationException();
            }

            wasFired = (_state == States.FIRE);
            if (wasFired)
            {
                _state = States.NONE;
            }
        }


        public void Fire()
        {
            if (_state != States.WAITING)
            {
                throw new InvalidOperationException();
            }
            _state = States.FIRE;
        }


        public void StartWaitingForFire()
        {
            if (_state != States.NONE)
            {
                throw new InvalidOperationException();
            }
            _state = States.WAITING;
        }

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
