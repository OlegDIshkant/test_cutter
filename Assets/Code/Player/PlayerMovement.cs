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
            velocity = CalcVelocity(input.inputMoveVector)
        };
    }


    private Vector3 CalcVelocity(Vector3 moveVector)
    {
        if (moveVector.magnitude < 0.3f)
            return Vector3.zero;

        return new Vector3(
            moveVector.x,
            0,
            moveVector.y) * _speed;
    }


}
