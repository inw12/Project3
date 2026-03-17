using UnityEngine;

public struct PlayerProjectileStats
{
    public float Damage;
    public float Speed;
    public float Range;
    public Vector3 Direction;
}

public class PlayerProjectile : MonoBehaviour
{
    [SerializeField] private LayerMask collidableLayers;
    [SerializeField] private float hitboxRadius = 2f;

    private PlayerProjectileStats _stats;

    // Orientation
    private Vector3 _origin;
    private Vector3 _displacement;
    private float _distanceTraveled;
    private float _distanceThisFrame;

    // SphereCast Variables
    private Vector3 _prev;

    public void Initialize(PlayerProjectileStats stats, Transform spawn)
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

    // SphereCast for collisions
    void Update()
    {
        var current = transform.position;
        var direction = current - _prev;
        var distance = direction.magnitude;

        if (distance < 0.001f) return;

        if (Physics.SphereCast(
            _prev,
            hitboxRadius,
            direction.normalized,
            out RaycastHit hit,
            distance,
            collidableLayers))
        {
            if (hit.collider.gameObject.TryGetComponent(out EnemyHealth e)) {
                e.DecreaseHealth(_stats.Damage);
            }

            PlayerProjectilePool.Instance.Release(gameObject);
        }
    }

    // Travels in a given direction
    void FixedUpdate()
    {
        // Update distance to travel this frame
        _distanceThisFrame = _stats.Speed * Time.deltaTime;

        // Travel forward
        transform.position += _stats.Direction * _distanceThisFrame;
        
        // Return to object pool after travelling a certain distance;
        _displacement = transform.position - _origin;
        _distanceTraveled = Vector3.Dot(_displacement, _stats.Direction);
        if (_distanceTraveled >= _stats.Range) {
            PlayerProjectilePool.Instance.Release(gameObject);
        }
    }
}
