using UnityEngine;
public class EnemyProjectile_Burst : Projectile
{
    [SerializeField] private int burstProjectileCount;
    [SerializeField] private float burstProjectileSpeedMultiplier;

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
            Burst();
            _pool.Release(gameObject);
        }
    }

    public override void OnHit(Collider other)
    {
        base.OnHit(other);
        Burst();
    }

    private void Burst()
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
                Range = _stats.Range / 2f,
                Direction = direction.normalized
            };

            _pool.Get(stats, transform);
        }
    }
}