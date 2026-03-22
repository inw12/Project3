using UnityEngine;
using System.Linq;
public abstract class Projectile : MonoBehaviour, IHitbox
{
    [SerializeField] protected LayerMask hitLayers;
    public LayerMask HittableLayers => hitLayers;

    protected ProjectileStats _stats;
    protected readonly Collider[] _hits = new Collider[10];

    public virtual void Initialize(ProjectileStats stats, Transform spawn)
    {
        // Initial Spawn Position
        transform.position = spawn.position;

        // Initialize Bullet Stats
        _stats = stats;
    }

    // Collision Detection
    protected virtual void Update()
    {
        // OverlapSphere to detect collisions
        var hits = Physics.OverlapSphereNonAlloc
        (
            transform.position,
            _stats.HitboxRadius,
            _hits,
            HittableLayers
        );

        // Handle hit if detected
        if (hits > 0)
        {
            var hit = _hits.FirstOrDefault(c => c != null);
            OnHit(hit);
        }
    }

    // Projectile Movement
    protected virtual void FixedUpdate() => Travel();

    // * MUST be implemented by child classes
    public abstract void OnHit(Collider other); // "what happens when this projectile hits something?"
    public abstract void Travel();              // "how does this projectile travel?"
}
