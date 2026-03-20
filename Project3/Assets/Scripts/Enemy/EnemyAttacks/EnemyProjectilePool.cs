///
/// * This is an object pool to manage projectiles fired by the enemy
/// 
using UnityEngine;
using UnityEngine.Pool;
public class EnemyProjectilePool : MonoBehaviour
{
    // Referenced by 'EnemyProjectile' to release object after collision
    public static EnemyProjectilePool Instance { get; private set; }

    [SerializeField] private GameObject projectile;
    [Space]
    [SerializeField] private int defaultCapacity = 30;
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

    private void OnDestroyProjectile(GameObject item) => Destroy(item);

    public void Get(EnemyProjectileStats stats, Transform spawn)
    {
        GameObject p = _pool.Get();
        if (p.TryGetComponent(out EnemyProjectile q)) {
            q.Initialize(stats, spawn);
            q.gameObject.SetActive(true);
        }
    }
    public void Release(GameObject item)
    {
        item.SetActive(false);
        _pool.Release(item);
    }
}
