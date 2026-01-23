using StateMachine;
using StateMachine.States;
using UnityEngine;
using System.Collections.Generic;

public class MonsterStateMachine : BaseStateMachine
{
    public Transform PlayerTarget { get; set; }
    public float MoveSpeed = 3f;
    public List<Vector3> PatrolPoints = new List<Vector3>();
    public int CurrentPatrolPointIndex = 0;
    public float PatrolPointReachedThreshold = 0.1f;
    
    [Header("Vision Cone Settings")]
    public float VisionRange = 10f;
    public float VisionAngle = 90f; // Total angle, e.g., 90 means 45 degrees left and right of forward
    public LayerMask VisionBlockingLayers; // Layers that can block line of sight

    // States
    public MonsterPatrolState PatrolState { get; private set; }
    public MonsterPursueState PursueState { get; private set; }
    public MonsterIdleState IdleState { get; private set; }

    private void Awake()
    {
        PatrolState = new MonsterPatrolState(this);
        PursueState = new MonsterPursueState(this);
        IdleState = new MonsterIdleState(this);
        // Additional initialization if needed
    }

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

        ChangeState(PatrolState);
    }

    public override void ChangeState(IState newState)
    {
        if (newState == CurrentState)
            return;

        base.ChangeState(newState);
        Debug.Log($"Monster State Changed to: {CurrentState.GetType().Name}");
    }

    public bool CanSeePlayer()
    {
        if (PlayerTarget == null) return false;

        Vector3 directionToPlayer = (PlayerTarget.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (angleToPlayer < VisionAngle / 2f)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, PlayerTarget.position);
            if (distanceToPlayer < VisionRange)
            {
                // Check for line of sight obstruction
                RaycastHit hit;
                // Offset raycast origin slightly so it doesn't hit the monster's own feet/ground
                Vector3 rayOrigin = transform.position + Vector3.up * 1.5f; 
                Vector3 rayDirection = (PlayerTarget.position + Vector3.up * 1.5f) - rayOrigin;

                if (Physics.Raycast(rayOrigin, rayDirection.normalized, out hit, VisionRange, VisionBlockingLayers))
                {
                    return hit.collider.transform == PlayerTarget || hit.collider.transform.IsChildOf(PlayerTarget);
                }
                else
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
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
                Gizmos.DrawSphere(PatrolPoints[i], 0.3f);
                if (i < PatrolPoints.Count - 1)
                    Gizmos.DrawLine(PatrolPoints[i], PatrolPoints[i + 1]);
                else
                    Gizmos.DrawLine(PatrolPoints[i], PatrolPoints[0]);
            }
        }
    }

    public void MoveTo(Vector3 targetPosition)
    {
        // Simple movement for now, can be replaced by NavMeshAgent later
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * MoveSpeed * Time.deltaTime;
        // Rotate to face the target
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f); // Smooth rotation
    }
}
