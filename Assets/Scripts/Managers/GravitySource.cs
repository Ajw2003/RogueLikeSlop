using UnityEngine;

public class GravitySource : MonoBehaviour
{
    public enum GravityType { Spherical, Directional }

    [Header("Settings")]
    public GravityType type = GravityType.Spherical;
    public float gravityStrength = 9.81f;
    public float influenceRadius = 50f;

    [Header("Directional Settings")]
    [Tooltip("The direction of gravity in local space of this object (usually 0, -1, 0)")]
    public Vector3 localGravityDirection = Vector3.down;

    public Vector3 GetGravityDirection(Vector3 position)
    {
        if (type == GravityType.Spherical)
        {
            return (transform.position - position).normalized;
        }
        else
        {
            // Transform local direction to world space
            return transform.TransformDirection(localGravityDirection).normalized;
        }
    }

    public float GetDistance(Vector3 position)
    {
        if (type == GravityType.Spherical)
        {
            return Vector3.Distance(transform.position, position);
        }
        else
        {
            // For directional, we could use distance to a plane, 
            // but for simplicity we'll use distance to the object center 
            // or just rely on the influenceRadius check in the controller.
            return Vector3.Distance(transform.position, position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, influenceRadius);
        
        if (type == GravityType.Directional)
        {
            Vector3 worldDir = transform.TransformDirection(localGravityDirection);
            Gizmos.DrawRay(transform.position, worldDir * 5f);
        }
    }
}
