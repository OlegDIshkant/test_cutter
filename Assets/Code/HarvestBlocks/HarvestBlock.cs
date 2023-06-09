using DG.Tweening;
using System;
using System.Linq;
using UnityEngine;


public class HarvestBlock : MonoBehaviour
{
    public enum States
    {
        NONE,
        APPEARING,
        IDLING,
        MOVING_TO_BACKPACK,
        IN_BACKPACK,
        DISAPEARING,
        DISAPEARED
    }

    private Behaviour _behaviour;

    public States CurrentState { get; private set; } = States.NONE;


    private void Awake()
    {
        _behaviour = new DoNothing(this, ChangeState);
    }


    private void Update()
    {
        _behaviour.Update();
    }


    public void ChangeState(States state, object args)
    {
        if (!MayChangeToState(state))
        {
            throw new System.InvalidOperationException($"New state '{state}' is not allowed due to current state '{CurrentState}'.");
        }

        _behaviour.OnStop();
        _behaviour = BehaviourForState(state, args);
        _behaviour.OnStart();

        CurrentState = state;
    }


    private bool MayChangeToState(States state)
    {
        switch (state)
        {
            case States.APPEARING: return CurrentState == States.NONE;
            case States.IDLING: return CurrentState == States.APPEARING;
            case States.MOVING_TO_BACKPACK: return CurrentState == States.IDLING;
            case States.IN_BACKPACK: return CurrentState == States.MOVING_TO_BACKPACK;
            case States.DISAPEARING: return CurrentState == States.IN_BACKPACK;
            case States.DISAPEARED: return CurrentState == States.DISAPEARING;
        }

        throw new Exception($"Dont know what to do with state '{state}'.");
    }


    private Behaviour BehaviourForState(States state, object args)
    {
        switch (state)
        {
            case States.APPEARING: return new Appear((Appear.Args)args, this, ChangeState);
            case States.IDLING: return new Idle((Idle.Args)args, this, ChangeState);
            case States.MOVING_TO_BACKPACK: return new MoveToBackpack((MoveToBackpack.Args)args, this, ChangeState);
            case States.IN_BACKPACK: return new InBackpack((InBackpack.Args)args, this, ChangeState);
            case States.DISAPEARING: return new Disapeare((Disapeare.Args)args, this, ChangeState);
            case States.DISAPEARED: return new DoNothing(this, ChangeState);
        }

        throw new Exception($"Dont know behaviour for the state '{state}'.");
    }



    public abstract class Behaviour
    {
        private readonly Action<States, object> _ToChangeState;
        private readonly HarvestBlock _masterBlock;

        protected HarvestBlock MasterBlock => _masterBlock;

        public Behaviour(HarvestBlock master, Action<States, object> ToChangeState)
        {
            _ToChangeState = ToChangeState;
            _masterBlock = master;
        }

        public abstract void Update();

        public virtual void OnStop() { }

        public virtual void OnStart() { }


        protected void ChangeState(States state, object args) => _ToChangeState(state, args);
    }



    public class DoNothing : Behaviour
    {
        public DoNothing(HarvestBlock master, Action<States, object> ToChangeState) : base(master, ToChangeState) { }

        public override void Update() { }
    }



    public class Appear : Behaviour
    {
        public struct Args
        {
            public Vector3 appearPoint;
            public Vector3 landPoint;
            public Func<Idle.Args> CreateIdleArgs;
        }

        private readonly Args _args;

        private bool _landed = false;

        public Appear(Args args, HarvestBlock master, Action<States, object> ToChangeState) : base(master, ToChangeState)
        {
            _args = args;
        }

        public override void Update()
        {
            if (_landed)
            {
                Finish();
            }
        }


        public override void OnStart()
        {
            MasterBlock.transform.position = _args.appearPoint;
            MasterBlock.transform
                .DOJump(_args.landPoint, 2f, 1, 0.35f, false)
                .OnComplete(() => _landed = true)
                .Play();
            
        }

        private void Finish()
        {
            ChangeState(States.IDLING, _args.CreateIdleArgs());
        }
    }



    public class Idle : Behaviour
    {
        public struct Args
        {
            public Func<HarvestBlock, int?> TryReservePlaceInBackpack;
            public Func<int, MoveToBackpack.Args> CreateMoveToBackpackArgs; // int-�������� ��� ������ ����� � �������
        }

        private readonly Args _args;
        private readonly TriggerCatcher _playerCatcher;

        public Idle(Args args, HarvestBlock master, Action<States, object> ToChangeState) : base(master, ToChangeState)
        {
            _args = args;
            _playerCatcher = master.GetComponentInChildren<TriggerCatcher>();
            _playerCatcher.SetUpTarget(GameConstants.GetInstance().playerTag);
        }

        public override void Update()
        {
            if (PlayerIsNearBlock() && TryReservePlaceInBackpack(out var placeIndex))
            {
                ChangeState(States.MOVING_TO_BACKPACK, _args.CreateMoveToBackpackArgs(placeIndex));
            }
        }


        private bool PlayerIsNearBlock() => _playerCatcher.CatchedObjects.Any();


        private bool TryReservePlaceInBackpack(out int placeIndex)
        {
            var reservedPlace = _args.TryReservePlaceInBackpack(MasterBlock);
            
            if (reservedPlace != null)
            {
                placeIndex = reservedPlace.Value;
                return true;
            }
            else
            {
                placeIndex = default;
                return false;
            }
        }
    }



    public class MoveToBackpack : Behaviour
    {
        public struct Args
        {
            public int placeIndex;
            public Action<Transform, int> AttachToBackpack;
            public Func<int, InBackpack.Args> CreateInBackpackArgs; // int-�������� ��� ������ ����� � �������
        }

        private readonly Args _args;


        public MoveToBackpack(Args args, HarvestBlock master, Action<States, object> ToChangeState) : base(master, ToChangeState)
        {
            _args = args;
        }


        public override void Update()
        {
            Finish();
        }


        public override void OnStart()
        {
            _args.AttachToBackpack(MasterBlock.transform, _args.placeIndex);
        }


        private void Finish()
        {
            ChangeState(States.IN_BACKPACK, _args.CreateInBackpackArgs(_args.placeIndex));
        }
    }




    public class InBackpack : Behaviour
    {
        public struct Args
        {
            public int placeIndex;
            public Func<int, Vector3> GetPositionInBackPack;
        }

        private readonly Args _args;


        public InBackpack(Args args, HarvestBlock master, Action<States, object> ToChangeState) : base(master, ToChangeState)
        {
            _args = args;
        }


        public override void Update()
        {
        }


    }




    public class Disapeare : Behaviour
    {
        public struct Args
        {
            public Action<Transform> DetachFromBackpack;
            public Vector3 vanishPoint;
            public Action<HarvestBlock> OnBlockDisapeared;
        }

        private readonly Args _args;

        private bool _canDisapear = false;

        public Disapeare(Args args, HarvestBlock master, Action<States, object> ToChangeState) : base(master, ToChangeState)
        {
            _args = args;
        }


        public override void Update()
        {
            if (_canDisapear)
            {
                Finish();
            }
        }


        public override void OnStart()
        {
            FlyToVanishPoint();
        }


        private void FlyToVanishPoint()
        {
            var constants = GameConstants.GetInstance();
            var distance = constants.blockFlyDistanceOnSell;
            var duration = constants.blockFlyDurationOnSell;
            var vanishPoint = _args.vanishPoint;

            var sequence = DOTween.Sequence();

            sequence.Append(
                MasterBlock.transform
                .DOMoveY(distance, duration)
                .SetRelative(true)
                .OnComplete(() => _args.DetachFromBackpack(MasterBlock.transform)));

            sequence.Append(
                MasterBlock.transform
                .DOMove(vanishPoint, duration)
                .OnComplete(() => _canDisapear = true));

            sequence.Play();
        }


        private void Finish()
        {
            ;
            _args.OnBlockDisapeared(MasterBlock);
            ChangeState(States.DISAPEARED, null);
        }
    }

}
