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
            _stateMachine.DestroySelf();
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