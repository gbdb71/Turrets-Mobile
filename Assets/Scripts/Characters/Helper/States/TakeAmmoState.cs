using UnityEngine;

public class TakeAmmoState : HelperBaseState
{
    private Factory _targetFactory;
    private FactoryPlate _targetPlate;

    public TakeAmmoState(HelperStateMachine stateMachine) : base(stateMachine)
    {
    }


    public void SetTarget(Factory target)
    {
        _targetFactory = target;
    }


    public override void Enter()
    {

    }
    public override void Update()
    {
        if(_stateMachine.Owner.InventoryFull)
        {
            _stateMachine.ChangeState(_stateMachine.ChargeTurretState);
            return;
        }

        if (_targetFactory == null)
        {
            _stateMachine.ChangeState(_stateMachine.IdleState);
        }
        else
        {
            if(_targetPlate != null)
            {
                if (_targetPlate.CanPlace())
                    _targetPlate = null;
            }

            if(_targetPlate == null)
            {
                _targetPlate = GetTargetPlate();

                if(_targetPlate == null)
                {
                    _targetFactory = null;
                    return;
                }
            }

            _stateMachine.Owner.Agent.SetDestination(_targetPlate.transform.position);
        }
    }
    public override void Exit()
    {

    }


    private FactoryPlate GetTargetPlate()
    {
        if (_targetFactory == null)
            return null;

        for (int i = 0; i < _targetFactory.Plates.Count; i++)
        {
            FactoryPlate plate = _targetFactory.Plates[i];

            if (plate.gameObject.activeSelf && plate.CanPlace() == false)
            {
                return plate;
            }
        }

        return null;
    }

}

