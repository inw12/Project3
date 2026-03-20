using System.Linq;
using UnityEngine;
public abstract class EnemyProjectile : MonoBehaviour
{
    [SerializeField] protected LayerMask collidableLayers;
    [Space]
    [SerializeField] protected float hitboxRadius;

    protected EnemyAttack _enemyAttack;

    // Projectile Stats
    protected float _damage;
    protected float _range;
    protected float _projectileSpeed;
    protected Vector3 _direction;

    protected readonly Collider[] _hits = new Collider[5];

    // Orientation
    protected Vector3 _origin;
    protected Vector3 _displacement;
    protected float _distanceTraveled;
    protected float _distanceThisFrame;

    public virtual void Initialize(EnemyRangedAttack attack, Transform spawn, Vector3 direction)
    {
        _enemyAttack = attack;

        // Spawn position
        transform.position = spawn.position;
        _origin = transform.position;

        // Initialize stats
        _damage = attack.Damage();
        _projectileSpeed = attack.ProjectileSpeed();
        _range = attack.Range();
        _direction = direction;
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
            EnemyProjectilePool.Instance.Release(gameObject);
        }
    }

    // Travels in a given direction
    protected virtual void FixedUpdate()
    {
        // Update distance to travel this frame
        _distanceThisFrame = _projectileSpeed * Time.fixedDeltaTime;

        Move();
        
        // Return to object pool after travelling a certain distance
        _displacement = transform.position - _origin;
        _distanceTraveled = Vector3.Dot(_displacement, _direction);
        if (_distanceTraveled >= _range)
        {
            OnProjectileEnd();
            EnemyProjectilePool.Instance.Release(gameObject);
        }
    }

    protected abstract void Move();
    protected virtual void OnProjectileEnd() {}
}
