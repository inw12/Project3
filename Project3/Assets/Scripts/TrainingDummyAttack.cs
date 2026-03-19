using UnityEngine;
public class TrainingDummyAttack : MonoBehaviour
{
    #region Attack Types
    public enum AttackType
    {
        None,
        Basic,
        Focused,
        Melee,
        Zone
    }
    [SerializeField] private AttackType attackType;
    #endregion
    // "Who are we attacking?"
    private Transform _target;

    [SerializeField] private GameObject projectile;
    [Header("Basic Ranged")]
    [SerializeField] private float damage = 1f;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float range = 25f;
    private float _fireTimer;

    void Start()
    {
        _fireTimer = 0f;
    }

    void Update()
    {
        // Get player position
        _target = Player.Instance ? Player.Instance.transform : _target;

        // Attack State Machine
        switch (attackType)
        {
            // Basic Ranged Attack
            case AttackType.Basic:
                BasicRangedAttack();
                break;
            // No Attack
            default:
                break;
        }
    }

    private void BasicRangedAttack()
    {
        _fireTimer += Time.deltaTime;
        if (_fireTimer >= fireRate)
        {
            // initialize stats
            var targetDirection = (Vector3.ProjectOnPlane(_target.position, Vector3.up) - Vector3.ProjectOnPlane(transform.position, Vector3.up)).normalized;
            var stats = new DummyProjectileStats
            {
                Damage = damage,
                Speed = speed,
                Range = range,
                Direction = targetDirection
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
