using UnityEngine;
using System.Collections.Generic;

public class RawMathPlayerController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Assign the Main Camera child object here.")]
    public Transform playerCamera; 

    [Header("Settings")]
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;
    public float gravityStrength = 9.81f;
    public float jumpForce = 5f;
    public float alignmentSpeed = 10f;

    [Header("Gravity")]
    public GravitySource currentGravitySource;
    private List<GravitySource> allGravitySources = new List<GravitySource>();

    // Track our exact viewing angles manually
    private float pitch = 0f; // Vertical rotation (Up/Down)
    private float verticalVelocity = 0f;
    private bool isGrounded = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        RefreshGravitySources();
    }

    void RefreshGravitySources()
    {
        allGravitySources.Clear();
        allGravitySources.AddRange(Object.FindObjectsByType<GravitySource>(FindObjectsSortMode.None));
    }

    void Update()
    {
        UpdateGravity();
        HandleRotation();
        HandleMovement();
        HandleTransport();
    }

    void HandleTransport()
    {
        // Transport to nearest planet on 'T' press
        if (Input.GetKeyDown(KeyCode.T))
        {
            RefreshGravitySources();
            float closestDist = float.MaxValue;
            GravitySource target = null;

            foreach (var source in allGravitySources)
            {
                if (source == currentGravitySource) continue;
                float dist = Vector3.Distance(transform.position, source.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    target = source;
                }
            }

            if (target != null)
            {
                // Move towards the target planet slightly outside its surface
                // We'll just set it as current and let gravity do the rest, 
                // or teleport closer.
                Vector3 targetDir = (target.transform.position - transform.position).normalized;
                transform.position = target.transform.position - targetDir * (target.influenceRadius * 0.8f);
                currentGravitySource = target;
                verticalVelocity = 0;
            }
        }
    }

    void UpdateGravity()
    {
        // 1. Find the nearest gravity source for force direction
        float closestDistance = float.MaxValue;
        GravitySource closestSource = null;

        foreach (var source in allGravitySources)
        {
            if (source == null) continue;
            float dist = Vector3.Distance(transform.position, source.transform.position);
            if (dist < source.influenceRadius && dist < closestDistance)
            {
                closestDistance = dist;
                closestSource = source;
            }
        }

        if (closestSource != null)
        {
            currentGravitySource = closestSource;
        }

        if (currentGravitySource != null)
        {
            // The 'down' force direction comes from the gravity source
            Vector3 gravityForceDir = currentGravitySource.GetGravityDirection(transform.position);
            Vector3 upDir = -gravityForceDir;

            // 2. Determine alignment (Up direction)
            // Use a higher start point and a longer ray for smoother detection
            RaycastHit hit;
            Vector3 targetUp = upDir;
            Vector3 alignmentRayStart = transform.position + transform.up * 0.5f;
            
            if (Physics.Raycast(alignmentRayStart, -transform.up, out hit, 2.0f))
            {
                targetUp = hit.normal;
            }

            // Smoothly rotate the player to align with the surface normal
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, targetUp) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, alignmentSpeed * Time.deltaTime);

            // 3. Jump and Gravity
            bool jumpPressed = Input.GetButtonDown("Jump");

            if (isGrounded && jumpPressed)
            {
                verticalVelocity = jumpForce;
                isGrounded = false;
            }

            if (!isGrounded)
            {
                verticalVelocity -= currentGravitySource.gravityStrength * Time.deltaTime;
                // Move along the local vertical axis only when in air or jumping
                transform.position += transform.up * verticalVelocity * Time.deltaTime;
            }
            else
            {
                verticalVelocity = 0;
            }

            // 4. Grounding check and Smooth Snapping
            // We use a safe offset to avoid starting inside the mesh
            Vector3 groundRayStart = transform.position + transform.up * 0.5f;
            if (verticalVelocity <= 0 && Physics.Raycast(groundRayStart, -transform.up, out hit, 1.6f))
            {
                isGrounded = true;
                
                // Calculate the "height error" (how far we are from the target 1.0 distance)
                float currentHeight = hit.distance - 0.5f;
                float targetHeight = 1.0f;
                float error = currentHeight - targetHeight;

                // Smoothly correct the height error instead of hard snapping
                // This eliminates the high-frequency jitter (camera shake)
                if (Mathf.Abs(error) > 0.001f)
                {
                    float correction = error * 15f * Time.deltaTime;
                    // Clamp correction to avoid overshooting
                    if (Mathf.Abs(correction) > Mathf.Abs(error)) correction = error;
                    transform.position -= transform.up * correction;
                }
            }
            else
            {
                isGrounded = false;
            }
        }
        else
        {
            // Space/Floating fallback
            verticalVelocity *= 0.99f;
            transform.position += transform.up * verticalVelocity * Time.deltaTime;
            isGrounded = false;
        }
    }

    void HandleRotation()
    {
        // Mouse Input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Yaw: Rotate the player around their local UP axis
        transform.Rotate(Vector3.up, mouseX);

        // Pitch: Rotate the camera around its local RIGHT axis
        pitch = Mathf.Clamp(pitch - mouseY, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void HandleMovement()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        // Movement relative to current orientation
        Vector3 moveDir = (transform.forward * inputY + transform.right * inputX).normalized;
        transform.position += moveDir * (moveSpeed * Time.deltaTime);
    }
}