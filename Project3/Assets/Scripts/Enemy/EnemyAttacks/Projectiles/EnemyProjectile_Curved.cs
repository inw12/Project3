///
/// * Projectile Behavior:
///     - Fires in a given direction
///     - Curves as it travels over time
///     - Returns to object pool when...
///         - Range distance reached
///         - Collides with specified collision layer
///
using UnityEngine;
public class EnemyProjectile_Curved : EnemyProjectile
{
    [SerializeField] private float curveAngle;

    protected override void Move()
    {
        // Calculate curve offset
        _direction = Quaternion.AngleAxis
        (
            curveAngle * Time.fixedDeltaTime,
            Vector3.up
        ) * _direction;

        transform.position += _direction * _distanceThisFrame;
        transform.rotation = Quaternion.LookRotation(_direction);
    }
}