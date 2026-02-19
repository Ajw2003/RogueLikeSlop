using UnityEngine;
using UnityEngine.AI;

namespace StateMachine.States
{
    public class MonsterPickedUpState : MonsterState
    {
        private NavMeshAgent _agent;
        private Rigidbody _rb;

        public MonsterPickedUpState(MonsterStateMachine stateMachine) : base(stateMachine)
        {
            _agent = stateMachine.GetComponent<NavMeshAgent>();
            _rb = stateMachine.GetComponent<Rigidbody>();
        }

        public override void Enter()
        {
            base.Enter();
            
            // Disable navigation
            if (_agent != null)
            {
                _agent.enabled = false;
            }

            // Ensure physics settings are correct for being held
            if (_rb != null)
            {
                _rb.useGravity = false;
                _rb.linearVelocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
            }
        }

        public override void Exit()
        {
            base.Exit();
            
            // Note: We don't re-activate the AI immediately here because 
            // the monster might still be flying through the air.
            // Re-activation logic usually happens in the MonsterStateMachine 
            // or after a landing check.
        }

        public override void Update()
        {
            // Do nothing while picked up, Item.cs handles the movement
        }
    }
}
