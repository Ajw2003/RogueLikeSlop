using System;
using StateMachine.States;
using UnityEngine;

namespace StateMachine
{
    public class PlayerStateMachine : BaseStateMachine
    {
        
        public PlayerState PreviousState { get; set; }
        
        public PlayerRunState RunState { get; set; }
        public PlayerInvunerableState InvunerableState { get; set; }
        public PlayerWalkState WalkState { get; set; }
        public PlayerAttackState AttackState { get; set; }
        public PlayerDeadState DeadState { get; set; }
        public PlayerDodgeState DodgeState { get; set; }
        public PlayerRespawnState RespawnState { get; set; }
        public PlayerIdleState IdleState { get; set; }

        public override void ChangeState(IState newState)
        {
            if (newState == CurrentState)
                return;

            PreviousState = CurrentState as PlayerState;
            Debug.Log($"Previous State: {PreviousState}");
            base.ChangeState(newState);
            Debug.Log($"State Changed to: {CurrentState}");
        }

        public void Awake()
        {
            RunState = new PlayerRunState(this);
            InvunerableState = new PlayerInvunerableState(this);
            WalkState = new PlayerWalkState(this);
            AttackState = new PlayerAttackState(this);
            DeadState = new PlayerDeadState(this);
            DodgeState = new PlayerDodgeState(this);
            RespawnState = new PlayerRespawnState(this);
            IdleState = new PlayerIdleState(this);
            
        }

        private void Start()
        {
            ChangeState(IdleState);
        }

        private void Walk()
        {
            ChangeState(WalkState);
        }
        
        private void Run()
        {
            ChangeState(RunState);
        }

        private void Dodge()
        {
            ChangeState(DodgeState);
        }

        private void Respawn()
        {
            ChangeState(RespawnState);
        }

        private void Dead()
        {
            ChangeState(DeadState);
        }
        
        private void Idle()
        {
            ChangeState(IdleState);
        }

        private void Attack()
        {
            ChangeState(AttackState);
        }

        private void Invunerable()
        {
            ChangeState(InvunerableState);
        }
    }
}
