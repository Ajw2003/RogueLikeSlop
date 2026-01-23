using StateMachine;
using StateMachine.States;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class MonsterStateMachine : BaseStateMachine
{
    public Transform PlayerTarget { get; set; }
    public float MoveSpeed = 3f;
    public List<Vector3> PatrolPoints = new List<Vector3>();
    public int CurrentPatrolPointIndex = 0;
    public float PatrolPointReachedThreshold = 1.0f; // Increased for NavMesh precision

    [Header("Patrol Settings")]
    public float PatrolRadius = 10f;
    public int PatrolPointCount = 3;
    
    [Header("Vision Cone Settings")]
    public float VisionRange = 10f;
    public float VisionAngle = 90f;
    public LayerMask VisionBlockingLayers;

    [Header("Combat Settings")]
    public float AttackDamage = 10f;
    public float AttackRange = 1.5f;
    public float AttackRate = 1.5f; // Seconds between attacks

    private NavMeshAgent _agent;
    private float _health;
    public float maxHealth = 100;

    // States
    public MonsterPatrolState PatrolState { get; private set; }
    public MonsterPursueState PursueState { get; private set; }
    public MonsterAttackState AttackState { get; private set; }
    public MonsterIdleState IdleState { get; private set; }
    public MonsterDeadState DeadState { get; private set; }

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = MoveSpeed;

        PatrolState = new MonsterPatrolState(this);
        PursueState = new MonsterPursueState(this);
        AttackState = new MonsterAttackState(this);
        IdleState = new MonsterIdleState(this);
        DeadState = new MonsterDeadState(this); // Initialize DeadState
        _health =  maxHealth;
    }

    private bool _isActive = false;

    private void Start()
    {
        // Automatically find player if not assigned
        if (PlayerTarget == null)
        {
            PlayerStateMachine player = Object.FindFirstObjectByType<PlayerStateMachine>();
            if (player != null)
            {
                PlayerTarget = player.transform;
            }
        }
    }

    public override void Update()
    {
        if (!_isActive) return;
        base.Update();
    }

    public void Activate()
    {
        if (_isActive) return;
        _isActive = true;

        if (PatrolPoints.Count == 0)
        {
            GeneratePatrolPoints();
        }
        
        ChangeState(PatrolState);
    }

    public void Deactivate()
    {
        if (!_isActive) return;
        _isActive = false;

        StopMoving();
        ChangeState(IdleState);
    }

    private void GeneratePatrolPoints()
    {
        PatrolPoints.Clear();
        int attempts = 0;
        while (PatrolPoints.Count < PatrolPointCount && attempts < 50)
        {
            attempts++;
            Vector2 randomCircle = Random.insideUnitCircle * PatrolRadius;
            Vector3 randomPoint = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
            
            NavMeshHit hit;
            // Use a reasonable range for vertical search, or PatrolRadius if terrain varies wildly
            if (NavMesh.SamplePosition(randomPoint, out hit, 10.0f, NavMesh.AllAreas))
            {
                PatrolPoints.Add(hit.position);
            }
        }
    }

    public override void ChangeState(IState newState)
    {
        if (newState == CurrentState)
            return;

        base.ChangeState(newState);
        // Debug.Log removed for performance
    }

    public void TakeDamage(float damage)
    {
        _health -= damage;
        if(_health <= 0)
            Die();
    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }

    public void Die()
    {
        ChangeState(DeadState); // Transition to DeadState instead of destroying immediately
    }

    public bool CanSeePlayer()
    {
        if (PlayerTarget == null) return false;

        Vector3 offsetToPlayer = PlayerTarget.position - transform.position;
        
        // Optimization: Check squared distance first to avoid Sqrt and expensive Angle checks
        if (offsetToPlayer.sqrMagnitude > VisionRange * VisionRange) return false;

        // Now calculate angle
        if (Vector3.Angle(transform.forward, offsetToPlayer) < VisionAngle / 2f)
        {
            // Finally perform Raycast
            RaycastHit hit;
            Vector3 rayOrigin = transform.position + Vector3.up * 1.5f; 
            Vector3 rayDirection = (PlayerTarget.position + Vector3.up * 1.5f) - rayOrigin;

            if (Physics.Raycast(rayOrigin, rayDirection.normalized, out hit, VisionRange, VisionBlockingLayers))
            {
                return hit.collider.transform == PlayerTarget || hit.collider.transform.IsChildOf(PlayerTarget);
            }
            else
            {
                // If raycast didn't hit anything (e.g. inside trigger), assume visible or blocked? 
                // Original logic returned true here. Sticking to original logic.
                return true;
            }
        }
        return false;
    }

    [Header("Debug")]
    public bool ShowDebugGizmos = false;

    private void OnDrawGizmosSelected()
    {
        if (!ShowDebugGizmos) return;

        // Visualize Attack Range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);

        // Visualize Vision Cone
        Gizmos.color = Color.yellow;
        Vector3 forward = transform.forward * VisionRange;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-VisionAngle / 2, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(VisionAngle / 2, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * forward;
        Vector3 rightRayDirection = rightRayRotation * forward;

        Gizmos.DrawRay(transform.position + Vector3.up, leftRayDirection);
        Gizmos.DrawRay(transform.position + Vector3.up, rightRayDirection);
        Gizmos.DrawLine(transform.position + Vector3.up + leftRayDirection, transform.position + Vector3.up + rightRayDirection);

        // Visualize Patrol Path
        if (PatrolPoints != null && PatrolPoints.Count > 0)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < PatrolPoints.Count; i++)
            {
                // Use WireCube instead of Sphere (WireCube is much cheaper to render)
                Gizmos.DrawWireCube(PatrolPoints[i], Vector3.one * 0.5f);
                if (i < PatrolPoints.Count - 1)
                    Gizmos.DrawLine(PatrolPoints[i], PatrolPoints[i + 1]);
                else
                    Gizmos.DrawLine(PatrolPoints[i], PatrolPoints[0]);
            }
        }
    }

    public void MoveTo(Vector3 targetPosition)
    {
        if (_agent != null && _agent.isOnNavMesh)
        {
            _agent.SetDestination(targetPosition);
        }
    }

    public void StopMoving()
    {
        if (_agent != null && _agent.isOnNavMesh)
        {
            _agent.ResetPath();
        }
    }

    public bool HasReachedDestination()
    {
        if (_agent == null || !_agent.isOnNavMesh) return false;
        
        // Check if the agent is close enough to the target and has no path pending
        return !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance + PatrolPointReachedThreshold;
    }
}
