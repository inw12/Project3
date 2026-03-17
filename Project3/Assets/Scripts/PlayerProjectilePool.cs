using UnityEngine;
using UnityEngine.Pool;
public class PlayerProjectilePool : MonoBehaviour
{
    public static PlayerProjectilePool Instance { get; private set; }

    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform spawnPoint;
    [Space]
    [SerializeField] private int defaultCapacity = 20;
    [SerializeField] private int maxCapacity = 100;

    private ObjectPool<GameObject> _pool;

    void Awake()
    {
        // Instance Control
        if (!Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
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
    private void OnDestroyProjectile(GameObject item)
    {
        
    }

    public void Get()
    {
        GameObject p = _pool.Get();
        p.transform.position = spawnPoint.position;
        p.SetActive(true);
    }
    public void Release(GameObject item)
    {
        item.SetActive(false);
        _pool.Release(item);
    }
}
