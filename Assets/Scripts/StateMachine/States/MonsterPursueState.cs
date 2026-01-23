using UnityEngine;

namespace StateMachine.States
{
    public class MonsterPursueState : MonsterState
    {
        private float _repathTimer;

        public MonsterPursueState(MonsterStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            _repathTimer = 0f;
        }

        public override void Update()
        {
            // Check for attack range (responsive check every frame)
            if (_stateMachine.PlayerTarget != null)
            {
                float sqrDistance = (_stateMachine.transform.position - _stateMachine.PlayerTarget.position).sqrMagnitude;
                if (sqrDistance <= _stateMachine.AttackRange * _stateMachine.AttackRange)
                {
                    _stateMachine.ChangeState(_stateMachine.AttackState);
                    return;
                }
            }
            else
            {
                // Lost target
                _stateMachine.ChangeState(_stateMachine.PatrolState);
                return;
            }

            // Throttle pathfinding and vision checks to improve performance
            _repathTimer += Time.deltaTime;
            if (_repathTimer < 0.2f) return;
            _repathTimer = 0f;

            // Check vision
            if (!_stateMachine.CanSeePlayer())
            {
                _stateMachine.ChangeState(_stateMachine.PatrolState);
                return;
            }

            // Update path
            _stateMachine.MoveTo(_stateMachine.PlayerTarget.position);
        }

        public override void Exit()
        {
            // Clean up if necessary
        }
    }
}