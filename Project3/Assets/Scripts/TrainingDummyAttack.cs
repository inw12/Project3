using UnityEngine;
public class TrainingDummyAttack : MonoBehaviour
{
    [SerializeField] private float damage = 1f;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float range = 25f;
    private float _fireTimer;
    [Space]
    [SerializeField] private GameObject projectile;

    void Start()
    {
        _fireTimer = 0f;
    }

    void Update()
    {
        _fireTimer += Time.deltaTime;

        if (_fireTimer >= fireRate)
        {
            // initialize stats
            var stats = new DummyProjectileStats
            {
                Damage = damage,
                Speed = speed,
                Range = range,
                Direction = Vector3.back                
            };

            // spawn bullet
            var bullet = Instantiate(projectile);
            if (bullet.TryGetComponent(out TrainingDummyProjectile p))
            {
                p.Initialize(stats, transform);
            }

            // reset fire rate timer
            _fireTimer = 0f;
        }
    }
}
