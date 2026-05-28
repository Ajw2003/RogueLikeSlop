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
        // Find nearest gravity source if we don't have one or if we are far from current
        float closestDistance = float.MaxValue;
        GravitySource closestSource = null;

        // Use cached list for better performance
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
            Vector3 gravityDir = (currentGravitySource.transform.position - transform.position).normalized;
            Vector3 upDir = -gravityDir;

            // Align player up with surface normal
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, upDir) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, alignmentSpeed * Time.deltaTime);

            // Apply gravity
            if (!isGrounded)
            {
                verticalVelocity -= gravityStrength * Time.deltaTime;
            }
            else if (verticalVelocity < 0)
            {
                verticalVelocity = -0.1f; // Small downward force to stay grounded
            }

            // Jump
            if (isGrounded && Input.GetButtonDown("Jump"))
            {
                verticalVelocity = jumpForce;
                isGrounded = false;
            }

            // Move along gravity/jump axis
            transform.position += upDir * verticalVelocity * Time.deltaTime;

            // Simple grounding check
            RaycastHit hit;
            if (Physics.Raycast(transform.position, gravityDir, out hit, 1.1f))
            {
                isGrounded = true;
                // Snap to surface if very close
                if (hit.distance < 1.0f)
                {
                    transform.position += gravityDir * (hit.distance - 1.0f);
                }
            }
            else
            {
                isGrounded = false;
            }
        }
        else
        {
            // Floating in space logic
            verticalVelocity *= 0.99f; // Air resistance
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