using UnityEngine;

namespace StateMachine.States
{
    public class MonsterDeadState : MonsterState
    {
        public MonsterDeadState(MonsterStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            // Play death animation, disable components, etc.
            // For now, we'll just disable the NavMeshAgent and collider to prevent further interaction.
            var agent = _stateMachine.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null) agent.enabled = false;
            var collider = _stateMachine.GetComponent<Collider>();
            if (collider != null) collider.enabled = false;
            // Optionally, make the enemy ragdoll or play a death animation here
            Debug.Log("Monster entered DeadState.");
        }

        public override void Update()
        {
            // In the dead state, nothing should happen.
        }

        public override void Exit()
        {
            // Cleanup if necessary, though usually entities in dead state are destroyed or pooled.
        }
    }
}