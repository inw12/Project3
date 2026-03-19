using UnityEngine;
public class TrainingDummyAttack : MonoBehaviour
{
    public enum AttackType
    {
        None,
        Basic,
        Focused,
        Melee,
        Zone
    }
    [SerializeField] private AttackType attackType;
    [Space]
    [SerializeField] private TrainingDummyStats stats;
    private TrainingDummyStats _runtimeStats;

    private Transform _target;
    private float _fireTimer;

    void Start()
    {
        _runtimeStats = Instantiate(stats);
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
        if (_fireTimer >= _runtimeStats.basicRangedFireRate)
        {
            // initialize stats
            var targetDirection = (Vector3.ProjectOnPlane(_target.position, Vector3.up) - Vector3.ProjectOnPlane(transform.position, Vector3.up)).normalized;
            var stats = new DummyProjectileStats
            {
                Damage = _runtimeStats.basicRangedDamage,
                Speed = _runtimeStats.basicRangedSpeed,
                Range = _runtimeStats.basicRangedRange,
                Direction = targetDirection
            };

            // spawn bullet
            var bullet = Instantiate(_runtimeStats.projectile);
            if (bullet.TryGetComponent(out TrainingDummyProjectile p))
            {
                p.Initialize(stats, transform);
            }

            // reset fire rate timer
            _fireTimer = 0f;
        }
    }
}
