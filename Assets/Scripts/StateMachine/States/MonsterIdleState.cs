using UnityEngine;
using UnityEngine.AI;

namespace StateMachine.States
{
    public class MonsterIdleState : MonsterState
    {
        private float _visionTimer;
        private NavMeshAgent _agent;
        private Rigidbody _rb;

        public MonsterIdleState(MonsterStateMachine stateMachine) : base(stateMachine)
        {
            _agent = stateMachine.GetComponent<NavMeshAgent>();
            _rb = stateMachine.GetComponent<Rigidbody>();
        }

        public override void Enter()
        {
            _stateMachine.StopMoving();
            _visionTimer = 0f;
        }

        public override void Update()
        {
            // If the agent is disabled (e.g., from PickedUp state), check if we can re-enable it
            if (_agent != null && !_agent.enabled)
            {
                // Wait until the monster is grounded and moving slowly
                if (_rb != null && _rb.linearVelocity.magnitude < 0.2f)
                {
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(_rb.position, out hit, 1.0f, NavMesh.AllAreas))
                    {
                        _agent.enabled = true;
                        _stateMachine.Activate(); // Re-activate the monster's logic
                    }
                }
                
                // If the agent is still disabled, don't perform other logic
                return;
            }

            // Normal Idle Logic
            _visionTimer += Time.deltaTime;
            if (_visionTimer < 0.2f) return;
            _visionTimer = 0f;

            // Even when idling, check if the player can be seen
            if (_stateMachine.CanSeePlayer())
            {
                _stateMachine.ChangeState(_stateMachine.PursueState);
                return;
            }

            // If patrol points are added or becomes available, go back to patrol
            if (_stateMachine.PatrolPoints.Count > 0)
            {
                _stateMachine.ChangeState(_stateMachine.PatrolState);
                return;
            }
        }

        public override void Exit()
        {
        }
    }
}
