using UnityEngine;

public class Simple3DPlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 200f;
    
    private Rigidbody rb;
    private float mouseX;
    private float mouseY;
    public ThirdPersonCameraController thirdPersonCameraController;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        thirdPersonCameraController = FindFirstObjectByType<ThirdPersonCameraController>();
    }

    void Update()
    {
        var currentRotation = thirdPersonCameraController.currentRotation;
        
        // Rotate the entire Player GameObject around the Y-axis (Yaw)
        transform.localRotation = Quaternion.Euler(0f, currentRotation.y, 0f);
        
        float moveX = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right
        float moveZ = Input.GetAxisRaw("Vertical");   // W/S or Up/Down

        // Calculate direction relative to where the player is currently facing
        Vector3 moveDirection = (transform.right * moveX + transform.forward * moveZ).normalized;

        // Apply velocity, keeping the existing Y velocity for gravity/jumping
        rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);
        
    }
}