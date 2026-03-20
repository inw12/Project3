using System.Linq;
using UnityEngine;
public struct EnemyProjectileStats
{
    public float Damage;
    public float Speed;
    public float Range;
    public Vector3 Direction;
}
public class EnemyProjectile : MonoBehaviour
{
    private EnemyAttack enemyAttack;

    [SerializeField] private LayerMask collidableLayers;
    [SerializeField] private float hitboxRadius = 0.5f;

    private EnemyProjectileStats _stats;
    private readonly Collider[] _hits = new Collider[5];

    // Orientation
    private Vector3 _origin;
    private Vector3 _displacement;
    private float _distanceTraveled;
    private float _distanceThisFrame;

    public void Initialize(EnemyRangedAttack attack, Transform spawn, Transform target)
    {
        enemyAttack = attack;

        // Spawn position
        transform.position = spawn.position;
        _origin = transform.position;

        // Initialize stats
        _stats.Damage = attack.Damage();
        _stats.Speed = attack.ProjectileSpeed();
        _stats.Range = attack.Range();

        // Calculate target direction
        var direction = (Vector3.ProjectOnPlane(target.position, Vector3.up) - Vector3.ProjectOnPlane(spawn.position, Vector3.up)).normalized;
        _stats.Direction = direction;
    }

    // OverlapSphere for collisions
    void Update()
    {
        var hits = Physics.OverlapSphereNonAlloc
        (
            transform.position,
            hitboxRadius,
            _hits,
            collidableLayers
        );

        // "If a hit is detected..."
        if (hits > 0)
        {
            var hit = _hits.FirstOrDefault(c => c != null);

            enemyAttack.HandleHit(hit, _stats.Damage);

            EnemyProjectilePool.Instance.Release(gameObject);
        }
    }

    // Travels in a given direction
    void FixedUpdate()
    {
        // Update distance to travel this frame
        _distanceThisFrame = _stats.Speed * Time.deltaTime;

        // Travel forward
        transform.position += _stats.Direction * _distanceThisFrame;
        
        // Return to object pool after travelling a certain distance
        _displacement = transform.position - _origin;
        _distanceTraveled = Vector3.Dot(_displacement, _stats.Direction);
        if (_distanceTraveled >= _stats.Range) {
            EnemyProjectilePool.Instance.Release(gameObject);
        }
    }
}
