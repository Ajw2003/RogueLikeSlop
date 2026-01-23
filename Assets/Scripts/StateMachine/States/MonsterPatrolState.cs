using UnityEngine;

namespace StateMachine.States
{
    public class MonsterPatrolState : MonsterState
    {
        public MonsterPatrolState(MonsterStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            if (_stateMachine.PatrolPoints.Count == 0)
            {
                _stateMachine.ChangeState(_stateMachine.IdleState);
                return;
            }

            MoveToNextPoint();
        }

        public override void Update()
        {
            // Check for player vision
            if (_stateMachine.CanSeePlayer())
            {
                _stateMachine.ChangeState(_stateMachine.PursueState);
                return;
            }

            // Check if reached current patrol point using the NavMesh optimized check
            if (_stateMachine.HasReachedDestination())
            {
                _stateMachine.CurrentPatrolPointIndex = (_stateMachine.CurrentPatrolPointIndex + 1) % _stateMachine.PatrolPoints.Count;
                MoveToNextPoint();
            }
        }

        private void MoveToNextPoint()
        {
            if (_stateMachine.PatrolPoints.Count == 0) return;
            Vector3 targetPoint = _stateMachine.PatrolPoints[_stateMachine.CurrentPatrolPointIndex];
            _stateMachine.MoveTo(targetPoint);
        }

        public override void Exit()
        {
            // Clean up if necessary
        }
    }
}