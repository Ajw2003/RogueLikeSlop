using UnityEngine;

public class GravitySource : MonoBehaviour
{
    public float gravityStrength = 9.81f;
    public float influenceRadius = 50f;

    public Vector3 GetGravityDirection(Vector3 position)
    {
        return (transform.position - position).normalized;
    }

    public float GetDistance(Vector3 position)
    {
        return Vector3.Distance(transform.position, position);
    }
}
