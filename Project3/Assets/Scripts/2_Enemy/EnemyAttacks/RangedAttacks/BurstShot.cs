/// * Shoots a 'BurstProjectile' at a target direction *
using UnityEngine;
public class BurstShot : EnemyRangedAttack
{
    [Space]
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
            // Calculate Target Direction
            var direction = (Vector3.ProjectOnPlane(target.position, Vector3.up) - Vector3.ProjectOnPlane(projectileSpawn.position, Vector3.up)).normalized;

            // Initialize Stats
            var stats = new ProjectileStats
            {
                Damage = damage,
                Speed = projectileSpeed,
                Range = range,
                Direction = direction
            };

            // Spawn bullet
            projectilePool.Get(stats, projectileSpawn, basicProjectilePool);

            // Reset fire rate timer
            _fireTimer = 0f;
        }

        if (_durationTimer >= duration)
        {
            if (Enemy.Instance) Enemy.Instance.SetToIdle();
        }
    }
}