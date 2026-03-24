using UnityEngine;
public class ScatterShot : EnemyRangedAttack
{
    [Space]
    [SerializeField] protected float duration;
    private float _durationTimer;
    [Header("Num of Projectiles per Attack")]
    [SerializeField] private int min = 1;
    [SerializeField] private int max = 3;
    [Header("AoE Burst")]
    [SerializeField] protected float burstProjectileSpeed;
    [SerializeField] protected float burstRange;
    [Space]
    [SerializeField] private ProjectilePool extraProjectilePool;
    [SerializeField] private int burstProjectileCount = 25;
    [SerializeField] private int shotsToBurst = 10;
    private int _shotCount;

    // buffer variables to change stats for burst attack
    private float _tempSpeed;
    private float _tempRange;

    public override void Attack(Transform target)
    {
        _durationTimer += Time.deltaTime;

        _fireTimer += Time.deltaTime;
        if (_fireTimer >= fireRate)
        {
            var amount = Random.Range(min, max + 1);
            for (int i = 0; i < amount; i++)
            {
                // Get Random Direction
                Vector2 randomCircle = Random.insideUnitCircle;
                Vector3 randomPoint = new(randomCircle.x, 0f, randomCircle.y);
                randomPoint = randomPoint.normalized;

                var stats = new ProjectileStats
                {
                    Damage = damage,
                    Speed = projectileSpeed,
                    Range = range,
                    Direction = randomPoint
                };

                // Spawn bullet
                projectilePool.Get(stats, projectileSpawn);
            }

            _shotCount++;

            // Trigger burst shot
            if (_shotCount >= shotsToBurst)
            {
                CircleBurst();
                _shotCount = 0;
            }

            _fireTimer = 0f;
        }

        if (_durationTimer >= duration)
        {
            if (Enemy.Instance) Enemy.Instance.SetToIdle();
            _durationTimer = 0f;
        }
    }

    private void CircleBurst()
    {
        _tempSpeed = projectileSpeed;
        _tempRange = range;

        projectileSpeed = burstProjectileSpeed;
        range = burstRange;

        var angleStep = 360f / burstProjectileCount;
        for (int i = 0; i < burstProjectileCount; i++)
        {
            var angle = i * angleStep;
            var rad = angle * Mathf.Deg2Rad;
            var direction = new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad));

            var stats = new ProjectileStats
            {
                Damage = damage,
                Speed = projectileSpeed,
                Range = range,
                Direction = direction.normalized
            };

            extraProjectilePool.Get(stats, projectileSpawn);
        }

        projectileSpeed = _tempSpeed;
        range = _tempRange;
    }
}