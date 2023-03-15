using System;
using UnityEngine;
using MovementInfo = PlayerMovement.Result;

public class PlayerRipping
{
    public struct Result
    {
        public Quaternion? newRotation;
        public bool ripTrigger;
    }


    private readonly Func<Quaternion> _GetPlayerRotation;
    private readonly Func<Vector3> _GetPlayerPosition;
    private readonly float _turningSpeed;

    private Func<MovementInfo, InputArgs, Result> UpdateAction;



    public PlayerRipping(
        Func<Vector3> GetPlayerPosition, 
        Func<Quaternion> GetPlayerRotation)
    {
        _GetPlayerPosition = GetPlayerPosition;
        _GetPlayerRotation = GetPlayerRotation;

        _turningSpeed = GameConstants.GetInstance().turningToPlantAngleSpeed;
        UpdateAction = DoNothing;
    }


    public Result MakeDecision(MovementInfo movementInfo, InputArgs input)
    {
        return UpdateAction(movementInfo, input);
    }
    

    private Result DoNothing(MovementInfo movementInfo, InputArgs input)
    {
        if (MayStartNewRip(movementInfo.isMovingNow, input, out var targetRotation))
        {
            if (targetRotation != null)
            {
                UpdateAction = TurningToPlantAndRipping;
            }
        }

        return AskNothing();
    }


    private Result TurningToPlantAndRipping(MovementInfo movementInfo, InputArgs input)
    {
        if (MayStartNewRip(movementInfo.isMovingNow, input, out var targetRotation))
        {
            if (targetRotation == null)
            {
                return AskStartRipAnimation();
            }
            else
            {
                return AskChangeRotation(CalcTurnStep(targetRotation.Value));
            }
        }
        else
        {
            UpdateAction = DoNothing;
            return AskNothing();
        }
    }



    private bool MayStartNewRip(bool isMovingNow, InputArgs input, out Quaternion? targetRotation)
    {
        targetRotation = null;

        if (isMovingNow) return false;
        if (input.rippingInProgress) return false;

        var plantPosition = input.rippablePlantPosition;
        if (plantPosition == null) return false;

        var rotation = CalcTargetRotation(plantPosition.Value);
        targetRotation = AlreadyHave(rotation) ? (Quaternion?)null : (Quaternion?)rotation;
        return true;
    }


    private Quaternion CalcTargetRotation(Vector3 plantPosition)
    {
        var playerToPlant = plantPosition - _GetPlayerPosition();
        playerToPlant.y = 0;

        return Quaternion.LookRotation(playerToPlant, Vector3.up);
    }


    private bool AlreadyHave(Quaternion rotation) => _GetPlayerRotation() == rotation;


    private Quaternion CalcTurnStep(Quaternion targetRotation)
    {
        return Quaternion.RotateTowards(_GetPlayerRotation(), targetRotation, _turningSpeed);
    }


    private Result AskNothing()
    {
        return new Result()
        {
            newRotation = null,
            ripTrigger = false
        };
    }


    private Result AskChangeRotation(Quaternion rotation)
    {
        return new Result()
        {
            newRotation = rotation,
            ripTrigger = false
        };
    }


    private Result AskStartRipAnimation()
    {
        return new Result()
        {
            newRotation = null,
            ripTrigger = true
        };
    }
}
