using UnityEngine;
using UnityEngine.Pool;
public class EnemyProjectilePool : MonoBehaviour
{
    [SerializeField] private GameObject projectile;
    [Space]
    [SerializeField] private int defaultCapacity = 30;
    [SerializeField] private int maxCapacity = 100;

    private ObjectPool<GameObject> _pool;

    void Awake()
    {
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

    public void Get(EnemyRangedAttack attack, Transform spawn, Vector3 direction)
    {
        GameObject p = _pool.Get();
        if (p.TryGetComponent(out EnemyProjectile q))
        {
            q.Initialize(this, attack, spawn, direction);
            q.gameObject.SetActive(true);
        }
    }
    public void Release(GameObject item)
    {
        item.SetActive(false);
        _pool.Release(item);
    }
}
