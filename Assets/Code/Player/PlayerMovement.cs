using System;
using UnityEngine;


public class PlayerMovement
{
    public struct Result
    {
        public bool isMovingNow => velocity != Vector3.zero;
        public Vector3 velocity;
    }

    private readonly float _speed;

    public PlayerMovement()
    {
        _speed = GameConstants.GetInstance().playerSpeed;
    }


    public Result MakeDecision(InputArgs input)
    {
        return new Result()
        {
            //velocity = input.rippingInProgress ? Vector3.zero : CalcVelocity(CalcMoveVector())
            velocity = CalcVelocity(CalcMoveVector())
        };
    }


    private Vector3? CalcMoveVector()
    {
        // Временное управление
        if (Input.GetKey(KeyCode.W))        return Vector3.forward;
        else if (Input.GetKey(KeyCode.S))   return Vector3.back;
        else if (Input.GetKey(KeyCode.A))   return Vector3.left;
        else if (Input.GetKey(KeyCode.D))   return Vector3.right;

        return null;
        //
    }


    private Vector3 CalcVelocity(Vector3? moveVector)
    {
        return moveVector == null ? 
            Vector3.zero : 
            moveVector.Value * _speed;
    }


}
