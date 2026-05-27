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
    private Camera playerCamera;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCamera = GetComponentInChildren<Camera>();

        // Locks the mouse cursor to the center of the screen and hides it
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Get horizontal and vertical mouse movement multiplied by time and sensitivity
        mouseX += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY += Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        
        // Rotate the entire Player GameObject around the Y-axis (Yaw)
        transform.localRotation = Quaternion.Euler(0f, mouseX, 0f);
        //clamp the y value between 90 and -90 so you cannot break your players neck
        var ClampedY = Mathf.Clamp(mouseY, -90f, 90f);
        //rotate the camera up and down independently from player movement
        playerCamera.transform.localRotation = Quaternion.Euler(ClampedY, 0f, 0f);
        
        float moveX = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right
        float moveZ = Input.GetAxisRaw("Vertical");   // W/S or Up/Down

        // Calculate direction relative to where the player is currently facing
        Vector3 moveDirection = (transform.right * moveX + transform.forward * moveZ).normalized;

        // Apply velocity, keeping the existing Y velocity for gravity/jumping
        rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);
        
    }
}