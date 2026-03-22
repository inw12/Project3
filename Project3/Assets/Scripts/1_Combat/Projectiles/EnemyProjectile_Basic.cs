/// * A straight-flying projectile owned by Enemy *
using UnityEngine;
public class EnemyProjectile_Basic : Projectile
{
    protected override void Move()
    {
        // Update distance to travel this frame
        _distanceThisFrame = _stats.Speed * Time.fixedDeltaTime;

        // Travel forward
        transform.position += _stats.Direction * _distanceThisFrame;
        
        // Return to object pool after travelling a certain distance;
        _distanceTraveled += _distanceThisFrame;
        if (_distanceTraveled >= _stats.Range)
        {
            _pool.Release(gameObject);
        }
    }
}
