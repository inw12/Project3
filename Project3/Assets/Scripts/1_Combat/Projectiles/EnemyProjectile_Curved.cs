/// * Projectile travels along a curve with given angle *
using UnityEngine;
public class EnemyProjectile_Curved : Projectile
{
    [SerializeField] private float curveAngle;

    protected override void Move()
    {
        // Update distance to travel this frame
        _distanceThisFrame = _stats.Speed * Time.fixedDeltaTime;

        // Calculate curve offset
        _stats.Direction = Quaternion.AngleAxis
        (
            curveAngle * Time.fixedDeltaTime,
            Vector3.up
        ) * _stats.Direction;

        // Move in curved direction
        transform.position += _stats.Direction * _distanceThisFrame;
        transform.rotation = Quaternion.LookRotation(_stats.Direction);

        // Return to object pool after travelling a certain distance;
        _distanceTraveled += _distanceThisFrame;
        if (_distanceTraveled >= _stats.Range)
        {
            _pool.Release(gameObject);
        }
    }
}