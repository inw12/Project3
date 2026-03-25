using UnityEngine;
public class PlayerAttackRanged : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float damage = 1f;
    [SerializeField] private float fireRate = 0.24f;
    [SerializeField] private float projectileSpeed = 25f;
    [SerializeField] private float projectileRange = 50f;
    private float _fireTimer;
    private Vector3 _projectileDirection;

    [Header("Components")]
    [SerializeField] private ProjectilePool projectilePool;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform projectileSpawn;

    public void Initialize()
    {
        _fireTimer = fireRate;
    }

    public void Attack(ref CombatState state, float deltaTime)
    {
        // Increment Fire Rate Timer
        _fireTimer += deltaTime;

        // Calculate Projectile Direction
        var source = Vector3.ProjectOnPlane(projectileSpawn.position, Vector3.up);
        _projectileDirection = (state.Target - source).normalized;

        // Fire Projectile
        if (_fireTimer >= fireRate)
        {
            var stats = new ProjectileStats
            {
                Damage = damage,
                Speed = projectileSpeed,
                Range = projectileRange,
                Direction = _projectileDirection
            };
            projectilePool.Get(stats, projectileSpawn);

            _fireTimer = 0f;
        }
    }
}