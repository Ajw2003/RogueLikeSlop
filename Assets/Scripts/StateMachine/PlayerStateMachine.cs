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
        
        public PlayerJumpState JumpState { get; set; }
        
        public Vector2 MovementDirection { get; set; }

        public Rigidbody _rb;

        public float walkSpeed;

        public float JumpForce;

        public float DodgeForce;

        public float respawnSpeed;
        
        public float MouseSensitivity = 100f;
        public Transform CameraTransform;
        
        [Header("Combat Settings")]
        public Transform AttackPoint;
        public float AttackRange = 0.5f;
        public float AttackDamage = 10f;
        public LayerMask EnemyLayers;

        public bool IsGrounded;

        private float _xRotation = 0f;
        private float _health;
        private float _maxHealth = 100;
        public bool dead;

        public override void ChangeState(IState newState)
        {
            if (newState == CurrentState)
                return;

            PreviousState = CurrentState as PlayerState;
            Debug.Log($"Previous State: {PreviousState}");
            base.ChangeState(newState);
            Debug.Log($"State Changed to: {CurrentState}");
        }

        public void Look(Vector2 lookDelta)
        {
            if (CameraTransform == null)
            {
                Debug.LogWarning("CameraTransform is not assigned in PlayerStateMachine.");
                return;
            }

            float mouseX = lookDelta.x * MouseSensitivity * Time.deltaTime;
            float mouseY = lookDelta.y * MouseSensitivity * Time.deltaTime;

            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

            CameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }

        private void OnDrawGizmosSelected()
        {
            if (AttackPoint == null)
                return;

            Gizmos.DrawWireSphere(AttackPoint.position, AttackRange);
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
            JumpState = new PlayerJumpState(this);
            _rb = GetComponent<Rigidbody>();
            _health = _maxHealth;

        }

        private void Start()
        {
            ChangeState(IdleState);
            
        }

        public void Die()
        {
            ChangeState(DeadState);
            dead = true;
        }

        public void Walk()
        {
            ChangeState(WalkState);
        }

        public void TakeDamage(float damage)
        {
            if(dead) return;
            _health -= damage;
            if (_health <= 0)
            {
                Die();
            }
        }
        public void Move(Vector2 movement)
        {
            MovementDirection = movement;
        }
        
        public void Run()
        {
            ChangeState(RunState);
        }

        public void Dodge()
        {
            if (CurrentState != DodgeState)
            {
                ChangeState(DodgeState);
            }
        }

        public void Respawn()
        {
            ChangeState(RespawnState);
        }

        public void Dead()
        {
            ChangeState(DeadState);
        }
        
        public void Idle()
        {
            ChangeState(IdleState);
        }

        public void Jump()
        {
            // Only allow jumping if grounded and not already in the jump state
            if (IsGrounded && CurrentState != JumpState)
            {
                ChangeState(JumpState);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            IsGrounded = true;
        }

        private void OnTriggerStay(Collider other)
        {
            IsGrounded = true;
        }

        private void OnTriggerExit(Collider other)
        {
            IsGrounded = false;
        }

        public void Attack()
        {
            ChangeState(AttackState);
        }

        public void Invunerable()
        {
            ChangeState(InvunerableState);
        }
    }
}
