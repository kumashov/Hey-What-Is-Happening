using UnityEngine;

public static class ColliderExtensions
{
    public static bool ContainsRaycast(this Collider @this, Vector3 point)
    {
        Vector3 direction = @this.bounds.center - point;
        RaycastHit hitInfo;
        return !@this.Raycast(new Ray(point, direction), out hitInfo, direction.magnitude);
    }
}
