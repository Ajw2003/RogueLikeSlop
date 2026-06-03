using UnityEngine;

public class StrategyPlayerController : MonoBehaviour
{
    private Vector3 target;
    private Camera cam;
    private Rigidbody rb;

    [SerializeField] private LayerMask walkableLayers;
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float slopeCheckDistance = 1f;
    [SerializeField] private float movementAcceleration = 50f; 
    
    // The distance from the target where the player will start smoothly slowing down
    [SerializeField] private float brakingDistance = 1.5f;

    private void Start()
    {
        cam = FindFirstObjectByType<Camera>();
        target = transform.position;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; 
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, walkableLayers))
            {
                target = hit.point;
            }
        }
    }

    private void FixedUpdate()
    {
        Vector3 direction = target - transform.position;
        direction.y = 0; // Work entirely in 2D space for horizontal travel distance

        float distance = direction.magnitude;
        Vector3 desiredVelocity = Vector3.zero;

        // Only calculate movement if we aren't microscopically close to the target
        if (distance > 0.02f)
        {
            direction.Normalize();
            Vector3 moveDirection = AdjustDirectionForSlope(direction);
            
            // Calculate a smooth braking factor based on how close we are to the center
            float speedFactor = Mathf.Clamp01(distance / brakingDistance);
            
            desiredVelocity = moveDirection * (moveSpeed * speedFactor);
        }

        // Calculate velocity differences (ignoring vertical gravity)
        Vector3 currentHorizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        Vector3 desiredHorizontalVelocity = new Vector3(desiredVelocity.x, 0, desiredVelocity.z);
        
        Vector3 velocityChange = desiredHorizontalVelocity - currentHorizontalVelocity;
        Vector3 movementForce = velocityChange * movementAcceleration;
        
        // Apply climbing forces smoothly if moving up a slope
        if (desiredVelocity.y > 0)
        {
            movementForce.y = desiredVelocity.y * movementAcceleration;
        }

        rb.AddForce(movementForce, ForceMode.Force);
    }

    private Vector3 AdjustDirectionForSlope(Vector3 horizontalDirection)
    {
        Vector3 rayStart = transform.position + Vector3.up * 0.5f;
        Vector3 rayDir = horizontalDirection + Vector3.down * 0.5f;

        if (Physics.Raycast(rayStart, rayDir.normalized, out RaycastHit slopeHit, slopeCheckDistance, walkableLayers))
        {
            return Vector3.ProjectOnPlane(horizontalDirection, slopeHit.normal).normalized;
        }

        return horizontalDirection; 
    }
}