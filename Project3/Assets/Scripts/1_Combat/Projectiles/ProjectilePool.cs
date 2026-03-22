using UnityEngine;
using UnityEngine.Pool;
public class ProjectilePool : MonoBehaviour
{
    /// * Referenced by:
    ///     - 'Projectile.cs'   (release)
    ///     - 'PlayerAttack.cs' (get)
    ///     - 'EnemyAttack.cs'  (get)
    public static ProjectilePool Instance { get; private set; }

    [SerializeField] private GameObject projectile;
    [Space]
    [SerializeField] private int defaultCapacity = 50;
    [SerializeField] private int maxCapacity = 100;

    private ObjectPool<GameObject> _pool;

    void Awake()
    {
        Instance = this;

        // Initialize Pool
        _pool = new ObjectPool<GameObject>
        (
            CreateProjectile,
            OnGetProjectile,
            OnReleaseProjectile,
            OnDestroyProjectile,
            true,
            defaultCapacity,
            maxCapacity
        );
    }

    private GameObject CreateProjectile()
    {
        var p = Instantiate(projectile, transform);
        return p;
    }

    private void OnGetProjectile(GameObject item) {}

    private void OnReleaseProjectile(GameObject item) {}

    private void OnDestroyProjectile(GameObject item) => DestroyImmediate(item);

    public void Get(ProjectileStats stats, Transform spawn)
    {
        GameObject item = _pool.Get();
        if (item.TryGetComponent(out Projectile p))
        {
            p.Initialize(this, stats, spawn);
            p.gameObject.SetActive(true);
        }
    }
    
    public void Release(GameObject item)
    {
        item.SetActive(false);
        _pool.Release(item);
    }
}
