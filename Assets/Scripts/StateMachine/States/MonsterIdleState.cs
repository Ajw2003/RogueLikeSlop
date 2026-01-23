using UnityEngine;

namespace StateMachine.States
{
    public class MonsterIdleState : MonsterState
    {
        private float _visionTimer;

        public MonsterIdleState(MonsterStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            _stateMachine.StopMoving();
            _visionTimer = 0f;
        }

        public override void Update()
        {
            // If patrol points are added or becomes available, go back to patrol
            if (_stateMachine.PatrolPoints.Count > 0)
            {
                _stateMachine.ChangeState(_stateMachine.PatrolState);
                return;
            }

            _visionTimer += Time.deltaTime;
            if (_visionTimer < 0.2f) return;
            _visionTimer = 0f;

            // Even when idling, check if the player can be seen
            if (_stateMachine.CanSeePlayer())
            {
                _stateMachine.ChangeState(_stateMachine.PursueState);
                return;
            }
        }

        public override void Exit()
        {
        }
    }
}