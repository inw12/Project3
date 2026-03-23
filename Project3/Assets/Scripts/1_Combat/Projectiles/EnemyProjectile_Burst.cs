using UnityEngine;
public class EnemyProjectile_Burst : Projectile
{
    [SerializeField] private int burstProjectileCount;
    [SerializeField] private float burstProjectileSpeedMultiplier;
    private ProjectilePool _pool2;

    // Overloaded 'Initialize()' method to take another 'ProjectilePool' as a parameter
    public void Initialize(ProjectilePool burstPool, ProjectilePool basicPool, ProjectileStats stats, Transform spawn)
    {
        // Object Pool
        _pool = burstPool;  // burst projectile object pool
        _pool2 = basicPool; // basic projectile object pool

        // Projectile Stats
        _stats = stats;

        // Spawn Position
        transform.position = spawn.position;

        _distanceTraveled = 0f;
    }

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
            Burst();
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
                Damage = _stats.Damage,
                Speed = _stats.Speed * burstProjectileSpeedMultiplier,
                Range = _stats.Range,
                Direction = direction.normalized
            };

            _pool2.Get(stats, transform);
        }
    }
}