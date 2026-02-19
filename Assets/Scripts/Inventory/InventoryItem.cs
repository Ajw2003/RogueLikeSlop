using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class InventoryItem : MonoBehaviour
{
    private Rigidbody _rb;
    private bool _isDragging = false;
    private Vector3 _targetPosition;
    
    [Header("Physics Settings")]
    [SerializeField] private float _followSpeed = 20f;
    [SerializeField] private float _rotationSpeed = 10f;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        // Ensure the rigidbody is set up for physical dragging
        _rb.useGravity = true;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    private void FixedUpdate()
    {
        if (_isDragging)
        {
            // Move towards target position using physics
            Vector3 direction = _targetPosition - _rb.position;
            _rb.linearVelocity = direction * _followSpeed;
            
            // Optional: keep rotation upright or follow a certain orientation
            Quaternion targetRotation = Quaternion.identity; 
            _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRotation, Time.fixedDeltaTime * _rotationSpeed));
        }
    }

    public void StartDragging()
    {
        _isDragging = true;
        _rb.useGravity = false;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }

    public void StopDragging()
    {
        _isDragging = false;
        _rb.useGravity = true;
    }

    public void UpdateTargetPosition(Vector3 position)
    {
        _targetPosition = position;
    }

    public bool IsDragging => _isDragging;
}