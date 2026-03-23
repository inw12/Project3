/// * Projectile Behavior:
///     - Lightly tracks target location
///     - Spawns basic projectiles in a circle shooting outward
using UnityEngine;
public class EnemyProjectile_Burst : Projectile
{
    [Header("Burst Effect")]
    [SerializeField] private int burstProjectileCount;
    [SerializeField] [Range(0f, 2f)] private float burstProjectileSpeedMultiplier;
    [Header("Target Tracking")]
    [SerializeField] [Range(0f, 1f)] private float trackingStrength;

    protected override void Move()
    {
        // Update distance to travel this frame
        _distanceThisFrame = _stats.Speed * Time.fixedDeltaTime;

        // "Where is our target?"
        var targetDirection = (Vector3.ProjectOnPlane(_target.position, Vector3.up) - Vector3.ProjectOnPlane(transform.position, Vector3.up)).normalized;

        // "What direction should I steer towards?"
        var steerDirection = Vector3.Slerp(_stats.Direction, targetDirection, trackingStrength);

        // Calculate steer force
        _stats.Direction = Vector3.Slerp(_stats.Direction, steerDirection, 1f - Mathf.Exp(-(trackingStrength * 2f) * Time.fixedDeltaTime));

        // Apply Movement
        transform.position += _stats.Direction * _distanceThisFrame;
        
        // Return to object pool after travelling a certain distance;
        _distanceTraveled += _distanceThisFrame;
        if (_distanceTraveled >= _stats.Range)
        {
            Burst();
            _pool.Release(gameObject);
        }
    }

    private void Burst()
    {
        if (_poolSecondary)
        {
            var angleStep = 360f / burstProjectileCount;
            for (int i = 0; i < burstProjectileCount; i++)
            {
                var angle = i * angleStep;
                var rad = angle * Mathf.Deg2Rad;
                var direction = new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad));

                var stats = new ProjectileStats
                {
                    Damage = _stats.Damage / 2f,
                    Speed = _stats.Speed * burstProjectileSpeedMultiplier,
                    Range = _stats.Range * 2f,
                    Direction = direction.normalized
                };

                _poolSecondary.Get(stats, transform);
            }
        }
    }
}