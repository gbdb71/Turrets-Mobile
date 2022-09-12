
using Assets.Scripts.StateMachine;
using System;

public class HelperStateMachine : StateMachine
{

    public readonly Helper Owner;
    public IdleState IdleState { get; private set; }
    public ChargeTurretState ChargeTurretState { get; private set; }

    public static explicit operator HelperStateMachine(HelperBaseState v)
    {
        throw new NotImplementedException();
    }

    public TakeAmmoState TakeAmmoState { get; private set; }


    public HelperStateMachine(Helper owner) : base()
    {
        Owner = owner;

        ChargeTurretState = new ChargeTurretState(this);
        TakeAmmoState = new TakeAmmoState(this);
        IdleState = (IdleState)_currentState;
    }


    protected override State GetInitialState()
    {
        return new IdleState(this);
    } 
}

