using UnityEngine;
public class PlayerProjectile : MonoBehaviour
{
    [SerializeField] private LayerMask collidableLayers;

    private float _speed;
    private float _range;
    private Vector3 _direction;

    // Orientation
    private Vector3 _origin;
    private Vector3 _displacement;
    private float _distanceTraveled;
    private float _distanceThisFrame;

    public void Initialize()
    {
        _origin = transform.position;
    }

    // Raycasts forward for collisions
    void Update()
    {
        if (Physics.Raycast(transform.position, _direction, out RaycastHit hitInfo, _distanceThisFrame, collidableLayers))
        {
            
        }
    }

    // Travels in a given direction
    void FixedUpdate()
    {
        // Update distance to travel this frame
        _distanceThisFrame = _speed * Time.deltaTime;

        // Travel forward
        transform.position += _direction * _distanceThisFrame;
        
        // Destroy after travelling a certain distance;
        _displacement = transform.position - _origin;
        _distanceTraveled = Vector3.Dot(_displacement, _direction);
        if (_distanceTraveled >= _range)
        {
            
        }
    }
}
