using UnityEngine;

namespace Characters.Drone.Attack
{
    public class IdleState : AttackBaseState
    {
        public IdleState(AttackStateMachine stateMachine) : base(stateMachine)
        {
        }

        private Vector3 _targetPos;
        private Vector2 _circlePos;
        private float _reposTimer = 0;

        public override void Update()
        {
            AttackDrone owner = _stateMachine.Owner;

            _reposTimer += Time.deltaTime;

            if (_reposTimer >= 2.5f)
            {
                _circlePos = Random.insideUnitCircle * 2f;
                _reposTimer = 0;
            }

            _targetPos = owner.Player.transform.position;
            _targetPos.x += _circlePos.x;
            _targetPos.z += _circlePos.y;

            owner.Agent.SetDestination(_targetPos);
        }

        public override void Enter()
        {

        }

        public override void Exit()
        {

        }
    }
}
