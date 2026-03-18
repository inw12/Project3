using UnityEngine;

public struct DummyProjectileStats
{
    public float Damage;
    public float Speed;
    public float Range;
    public Vector3 Direction;
}

public class TrainingDummyProjectile : MonoBehaviour
{
    [SerializeField] private LayerMask collidableLayers;
    [SerializeField] private float hitboxRadius = 2f;

    private DummyProjectileStats _stats;

    // Orientation
    private Vector3 _origin;
    private Vector3 _displacement;
    private float _distanceTraveled;
    private float _distanceThisFrame;

    // SphereCast Variables
    private Vector3 _prev;

    public void Initialize(DummyProjectileStats stats, Transform spawn)
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
            if (hit.collider.gameObject.TryGetComponent(out PlayerHealth p)) {
                p.DecreaseHealth(_stats.Damage);
            }

            Destroy(gameObject);
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
            Destroy(gameObject);
        }
    }
}
