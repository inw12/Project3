///
/// * Projectile Behavior:
///     - Moves straight towards given direction
///     - Returns to object pool when...
///         - Range distance reached
///         - Collides with specified collision layer
/// 
public class EnemyProjectile_Basic : EnemyProjectile
{
    protected override void Move()
    {
        // Move straight towards target position
        transform.position += _direction * _distanceThisFrame;
    }
}
