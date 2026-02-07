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

        public float FallMultiplier = 2.5f;

        [Range(0, 1)]
        public float AirControl = 0.7f;

        public float DodgeForce;

        public float respawnSpeed;
        
        public float MouseSensitivity = 100f;
        public Transform CameraTransform;
        
        [Header("Combat Settings")]
        public Transform AttackPoint;
        public float AttackRange = 0.5f;
        public float AttackDamage = 10f;
        public LayerMask EnemyLayers;

        [Header("Ground Check Settings")]
        public Transform GroundCheckPosition;
        public float GroundCheckRadius = 0.2f;
        public LayerMask GroundLayer;
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
            if (AttackPoint != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(AttackPoint.position, AttackRange);
            }

            if (GroundCheckPosition != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(GroundCheckPosition.position, GroundCheckRadius);
            }
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

            if (GroundCheckPosition == null)
            {
                Debug.LogWarning("GroundCheckPosition is not assigned in PlayerStateMachine!");
            }
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

        public override void FixedUpdate()
        {
            GroundCheck();
            base.FixedUpdate();

            // Apply extra gravity force if not grounded to make falling feel weightier
            if (!IsGrounded && _rb != null)
            {
                // We apply (Multiplier - 1) because the physics engine is already applying 1x gravity
                Vector3 extraGravity = Vector3.up * Physics.gravity.y * (FallMultiplier - 1);
                _rb.AddForce(extraGravity, ForceMode.Acceleration);
            }
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

        public void GroundCheck()
        {
            if (GroundCheckPosition != null)
            {
                IsGrounded = false;
                Collider[] colliders = Physics.OverlapSphere(GroundCheckPosition.position, GroundCheckRadius, GroundLayer);
                
                foreach (var collider in colliders)
                {
                    if (collider.transform.root != transform.root)
                    {
                        IsGrounded = true;
                        break;
                    }
                }
            }
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
