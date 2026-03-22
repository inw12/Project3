using UnityEngine;
public abstract class Projectile : MonoBehaviour, IHitbox
{
    [SerializeField] protected float hitboxRadius;
    [Space]
    [SerializeField] protected LayerMask hitLayers;
    public LayerMask HittableLayers => hitLayers;

    protected ProjectileStats _stats;
    protected ProjectilePool _pool;
    protected readonly Collider[] _hits = new Collider[10];

    // Range Management
    protected float _distanceThisFrame;
    protected float _distanceTraveled;

    public virtual void Initialize(ProjectilePool pool, ProjectileStats stats, Transform spawn)
    {
        // Object Pool
        _pool = pool;

        // Projectile Stats
        _stats = stats;

        // Spawn Position
        transform.position = spawn.position;

        _distanceTraveled = 0f;
    }

    // Collision Detection
    protected virtual void Update()
    {
        // OverlapSphere to detect collisions in 'HittableLayers'
        var hits = Physics.OverlapSphereNonAlloc
        (
            transform.position,
            hitboxRadius,
            _hits,
            HittableLayers
        );

        // Handle hit if detected
        if (hits > 0)
        {
            OnHit(_hits[0]);
        }
    }

    // Projectile Movement
    protected virtual void FixedUpdate() => Move();
    
    public virtual void OnHit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IDamageable e))
        {
            e.DecreaseHealth(_stats.Damage);
            _pool.Release(gameObject);
        }
    }

    protected abstract void Move();
}
