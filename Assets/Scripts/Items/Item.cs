using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour
{
    private Rigidbody _rb;
    private bool _isDragging = false;
    private Vector3 _targetPosition;
    private Quaternion _targetRotation = Quaternion.identity;
    
    // References for enemy handling
    private MonsterStateMachine _monsterAI;
    private NavMeshAgent _agent;

    [Header("Physics Settings")]
    [SerializeField] private float _followSpeed = 20f;
    [SerializeField] private float _rotationSpeed = 10f;

    [Header("Damage Settings")]
    [SerializeField] private float _damageMultiplier = 2f;
    [SerializeField] private float _damageCooldown = 0.5f;

    private float _lastDamageTime;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _monsterAI = GetComponent<MonsterStateMachine>();
        _agent = GetComponent<NavMeshAgent>();

        // Ensure the rigidbody is set up for physical dragging
        _rb.useGravity = true;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        _targetRotation = transform.rotation;
    }

    private void FixedUpdate()
    {
        if (_isDragging)
        {
            // Smoothly move towards target position using MovePosition
            // This is better than linearVelocity for following a point exactly
            Vector3 newPosition = Vector3.Lerp(_rb.position, _targetPosition, Time.fixedDeltaTime * _followSpeed);
            _rb.MovePosition(newPosition);
            
            // Smoothly rotate towards target rotation
            Quaternion newRotation = Quaternion.Slerp(_rb.rotation, _targetRotation, Time.fixedDeltaTime * _rotationSpeed);
            _rb.MoveRotation(newRotation);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 1. Safety Checks
        if (_isDragging) return;
        if (Time.time < _lastDamageTime + _damageCooldown) return;

        // 2. Velocity Calculation
        float impactVelocity = collision.relativeVelocity.magnitude;
        float myVelocity = _rb.linearVelocity.magnitude;

        // 3. Damage Application
        int damage = Mathf.RoundToInt(impactVelocity * _damageMultiplier);
        bool dealtDamage = false;

        // Damage target
        if (collision.gameObject.TryGetComponent(out MonsterStateMachine targetMonster))
        {
            float currentHealthBefore = targetMonster.CurrentHealth;
            targetMonster.TakeDamage(damage, impactVelocity);
            if (targetMonster.CurrentHealth < currentHealthBefore) dealtDamage = true;
        }
        else if (collision.gameObject.TryGetComponent(out StateMachine.PlayerStateMachine player))
        {
            float currentHealthBefore = player.CurrentHealth;
            player.TakeDamage(damage, impactVelocity);
            if (player.CurrentHealth < currentHealthBefore) dealtDamage = true;
        }

        // Damage ourselves (if we are an enemy item being thrown)
        if (_monsterAI != null)
        {
            // Only take impact damage if we are NOT grounded/active
            if (!_agent.enabled || myVelocity > 1f) 
            {
                float currentHealthBefore = _monsterAI.CurrentHealth;
                _monsterAI.TakeDamage(damage, impactVelocity);
                if (_monsterAI.CurrentHealth < currentHealthBefore) dealtDamage = true;
            }
        }

        if (dealtDamage)
        {
            _lastDamageTime = Time.time;
            Debug.Log($"Impact Damage Dealt: {damage} (Impact: {impactVelocity:F1})");
        }
    }

    public void StartDragging()
    {
        _isDragging = true;
        
        // Disable gravity and make kinematic to prevent shaking/fighting
        // But keep MovePosition functional
        _rb.useGravity = false;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.isKinematic = true; 
        
        
        _targetRotation = transform.rotation;

        // Transition to PickedUp state if we are an enemy
        if (_monsterAI != null)
        {
            _monsterAI.PickUp();
        }
    }

    public void StopDragging()
    {
        _isDragging = false;
        
        // Re-enable physics
        _rb.isKinematic = false;
        _rb.useGravity = true;

        // Release call removed - Monster handles its own recovery via struggle routine
    }

    public void Throw(Vector3 direction, float force)
    {
        _isDragging = false;
        
        // Re-enable physics BEFORE applying force
        _rb.isKinematic = false;
        _rb.useGravity = true;
        
        _rb.AddForce(direction * force, ForceMode.Impulse);

        // Release call removed - Monster handles its own recovery via struggle routine
    }

    public void UpdateTargetPosition(Vector3 position)
    {
        _targetPosition = position;
    }

    public void UpdateRotation(Quaternion rotation)
    {
        _targetRotation = rotation;
    }

    public Quaternion TargetRotation => _targetRotation;
    public bool IsDragging => _isDragging;
}