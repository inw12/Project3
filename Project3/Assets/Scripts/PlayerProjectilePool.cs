using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
public class PlayerProjectilePool : MonoBehaviour
{
    public static PlayerProjectilePool Instance { get; private set; }

    [SerializeField] private GameObject projectile;
    [Space]
    [SerializeField] private int defaultCapacity = 20;
    [SerializeField] private int maxCapacity = 100;

    private ObjectPool<PlayerProjectile> _pool;

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
        _pool = new ObjectPool<PlayerProjectile>
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

    private PlayerProjectile CreateProjectile()
    {
        var p = Instantiate(projectile, transform);
        return p.TryGetComponent(out PlayerProjectile q) ? q : null;
    }
    private void OnGetProjectile(PlayerProjectile p)
    {
        
    }
    private void OnReleaseProjectile(PlayerProjectile p)
    {
        
    }
    private void OnDestroyProjectile(PlayerProjectile p)
    {
        
    }
}
