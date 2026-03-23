/// * Shoots 2 'BurstProjectiles' to the left & right of the enemy *
using UnityEngine;
public class BurstShot : EnemyRangedAttack
{
    [Space]
    [SerializeField] private Transform secondProjectileSpawn;
    [SerializeField] private ProjectilePool basicProjectilePool;
    [SerializeField] private float duration;
    private float _durationTimer;

    void Start() => _fireTimer = fireRate;

    public override void Attack(Transform target)
    {
        _durationTimer += Time.deltaTime;
        _fireTimer += Time.deltaTime;

        if (_fireTimer >= fireRate)
        {
            // Projectile #1
            var spawn = projectileSpawn.position;
            spawn.y = 1f;
            projectileSpawn.position = spawn;
            var stats = new ProjectileStats
            {
                Damage = damage,
                Speed = projectileSpeed,
                Range = range,
                Direction = transform.right
            };
            projectilePool.Get(stats, projectileSpawn, basicProjectilePool, target);    // Projectile "D"

            // Projectile #2
            spawn = secondProjectileSpawn.position;
            spawn.y = 1f;
            secondProjectileSpawn.position = spawn;
            stats.Direction *= -1;
            projectilePool.Get(stats, secondProjectileSpawn, basicProjectilePool, target);

            // Reset fire rate timer
            _fireTimer = 0f;
        }

        if (_durationTimer >= duration)
        {
            if (Enemy.Instance) Enemy.Instance.SetToIdle();
            _durationTimer = 0f;
        }
    }
}