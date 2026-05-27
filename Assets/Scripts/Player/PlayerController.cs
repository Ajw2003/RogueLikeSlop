using UnityEngine;

public class RawMathPlayerController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Assign the Main Camera child object here.")]
    public Transform playerCamera; 

    [Header("Settings")]
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;

    // Track our exact viewing angles manually
    private float pitch = 0f; // Vertical rotation (Up/Down)
    private float yaw = 0f;   // Horizontal rotation (Left/Right)

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // --- 1. MOUSE INPUT & TRACKING ---
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Update raw viewing angles
        yaw += mouseX; 
        pitch = Mathf.Clamp(pitch - mouseY, -90f, 90f);// clamp up and down rotation so you can't snap your neck

        // Apply both Pitch and Yaw directly to the camera object so you can look around.
        playerCamera.localRotation = Quaternion.Euler(pitch, yaw, 0f);


        // --- 2. RAW MATH MOVEMENT ---
        // Get raw WASD/Joystick input
        float  inputX = Input.GetAxis("Horizontal"); // Left/Right (X)
        float inputY = Input.GetAxis("Vertical");   // Forward/Back (Z)

        // Convert the yaw / rotation into radian form to be used in our vector2 calculations
        float yawRad = yaw * Mathf.Deg2Rad;
        //sin and cos calculations to translate 
        float cos = Mathf.Cos(yawRad);
        float sin = Mathf.Sin(yawRad);

        // Apply the 2D Rotation Matrix adapted for Unity's coordinate system
        // mixes the players WASD inputs with the yaw input 
        float rotatedX = inputX * cos + inputY * sin;
        float rotatedZ = -inputX * sin + inputY * cos;

        // Create the final 3D directional vector
        Vector3 moveDirection = new Vector3(rotatedX, 0f, rotatedZ);

        // Move the player manually by altering their absolute world position
        transform.position += moveDirection * (moveSpeed * Time.deltaTime);
    }
}