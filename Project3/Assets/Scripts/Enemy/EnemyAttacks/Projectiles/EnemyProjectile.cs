using System.Linq;
using UnityEngine;
public abstract class EnemyProjectile : MonoBehaviour
{
    [SerializeField] protected LayerMask collidableLayers;
    [Space]
    [SerializeField] protected float hitboxRadius;

    protected EnemyAttack _enemyAttack;
    protected EnemyProjectilePool _source;
    protected readonly Collider[] _hits = new Collider[5];

    // Projectile Stats
    protected float _damage;
    protected float _range;
    protected float _projectileSpeed;
    protected Vector3 _direction;

    // Orientation
    protected float _distanceTraveled;
    protected float _distanceThisFrame;

    public virtual void Initialize(EnemyProjectilePool pool, EnemyRangedAttack attack, Transform spawn, Vector3 direction)
    {
        // Object Pool
        _source = pool;

        // Spawn position
        transform.position = spawn.position;

        // Initialize stats
        _enemyAttack = attack;
        _damage = attack.Damage();
        _projectileSpeed = attack.ProjectileSpeed();
        _range = attack.Range();
        _direction = direction;

        // Reset range
        _distanceTraveled = 0f;
    }

    // Collision Detection
    protected virtual void Update()
    {
        // OverlapSphere to detect collisions
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
            _enemyAttack.HandleHit(hit, _damage);
            OnProjectileEnd();
            _source.Release(gameObject);
        }
    }

    // Travels in a given direction
    protected virtual void FixedUpdate()
    {
        // Update distance to travel this frame
        _distanceThisFrame = _projectileSpeed * Time.fixedDeltaTime;

        Move();
        
        // Return to object pool after travelling a certain distance
        _distanceTraveled += _distanceThisFrame;
        if (_distanceTraveled >= _range)
        {
            OnProjectileEnd();
            _source.Release(gameObject);
        }
    }

    protected abstract void Move();
    protected virtual void OnProjectileEnd() {}
}
