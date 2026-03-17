using UnityEngine;
using UnityEngine.Pool;
public class PlayerProjectilePool : MonoBehaviour
{
    public static PlayerProjectilePool Instance { get; private set; }

    [SerializeField] private GameObject projectile;
    [Space]
    [SerializeField] private int defaultCapacity = 20;
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
    private void OnGetProjectile(GameObject item)
    {
        
    }
    private void OnReleaseProjectile(GameObject item)
    {
        
    }
    private void OnDestroyProjectile(GameObject item) => Destroy(item);

    public void Get(PlayerProjectileStats stats, Transform spawn)
    {
        GameObject p = _pool.Get();
        if (p.TryGetComponent(out PlayerProjectile q)) {
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
