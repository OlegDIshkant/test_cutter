using System;
using UnityEngine;


public class PlayerAnimator : MonoBehaviour
{
    private static readonly string RIP_ANIM_ID = "Rip";
    private static readonly string RUN_ANIM_ID = "Run";

    public event Action OnRipMoment;

    private Animator _animator;

    private bool _ripAnimWasLaunched = false;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }


    private void AnimEvent_OnRipMoment()
    {
        OnRipMoment?.Invoke();
    }


    public bool CanPlayRipAnim() => true;


    public void PlayRunAnim()
    {
        MakeSureRipWasCanceled();

        if (!_animator.GetBool(RUN_ANIM_ID))
        {
            _animator.SetBool(RUN_ANIM_ID, true);
        }
    }

    public void PlayRipAnim()
    {
        _ripAnimWasLaunched = true;

        _animator.ResetTrigger(RIP_ANIM_ID);
        _animator.SetTrigger(RIP_ANIM_ID);
    }

    public void PlayIdleAnim()
    {
        MakeSureRipWasCanceled();

        if (_animator.GetBool(RUN_ANIM_ID))
        {
            _animator.SetBool(RUN_ANIM_ID, false);
        }
    }


    private void MakeSureRipWasCanceled()
    {
        if (_ripAnimWasLaunched)
        {
            _animator.ResetTrigger(RIP_ANIM_ID);
        }
    }


    public bool IsRipAnimationPlaying() =>
        _animator.GetCurrentAnimatorStateInfo(0).IsName(RIP_ANIM_ID);




}
