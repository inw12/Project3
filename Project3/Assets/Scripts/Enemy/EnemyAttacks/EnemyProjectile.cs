using System.Linq;
using UnityEngine;
public struct EnemyProjectileStats
{
    public float Damage;
    public float Speed;
    public float Range;
    public Vector3 Direction;
}
public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private LayerMask collidableLayers;
    [SerializeField] private float hitboxRadius = 0.5f;

    private EnemyProjectileStats _stats;
    private readonly Collider[] _hits = new Collider[5];

    // Orientation
    private Vector3 _origin;
    private Vector3 _displacement;
    private float _distanceTraveled;
    private float _distanceThisFrame;

    public void Initialize(EnemyProjectileStats stats, Transform spawn)
    {
        // Spawn position
        transform.position = spawn.position;
        _origin = transform.position;

        // Initialize stats
        _stats.Damage = stats.Damage;
        _stats.Speed = stats.Speed;
        _stats.Range = stats.Range;
        _stats.Direction = stats.Direction;
    }

    // OverlapSphere for collisions
    void Update()
    {
        var hits = Physics.OverlapSphereNonAlloc
        (
            transform.position,
            hitboxRadius,
            _hits,
            collidableLayers
        );

        if (hits > 0)
        {
            var hit = _hits.FirstOrDefault(c => c != null);
            
            // ** Collision Logic Here **

            EnemyProjectilePool.Instance.Release(gameObject);
        }
    }

    // Travels in a given direction
    void FixedUpdate()
    {
        // Update distance to travel this frame
        _distanceThisFrame = _stats.Speed * Time.deltaTime;

        // Travel forward
        transform.position += _stats.Direction * _distanceThisFrame;
        
        // Return to object pool after travelling a certain distance
        _displacement = transform.position - _origin;
        _distanceTraveled = Vector3.Dot(_displacement, _stats.Direction);
        if (_distanceTraveled >= _stats.Range) {
            EnemyProjectilePool.Instance.Release(gameObject);
        }
    }
}
