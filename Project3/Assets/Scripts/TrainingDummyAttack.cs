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
    [SerializeField] private TrainingDummyProjectilePool projectilePool;
    private TrainingDummyStats _runtimeStats;

    private Transform _target;
    private float _fireTimer;       // Basic Ranged Fire Rate Timer
    private float _chargeTimer;     // Focused Ranged Charge Timer

    void Start()
    {
        _runtimeStats = Instantiate(stats);
        _fireTimer = 0f;
        _chargeTimer = 0f;
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
            case AttackType.Focused:
                FocusedRangedAttack();
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

            // get bullet from pool
            projectilePool.Get(stats, transform);

            // reset fire rate timer
            _fireTimer = 0f;
        }
    }

    public void FocusedRangedAttack()
    {
        _chargeTimer += Time.deltaTime;

        ///
        /// ** Focused shot buildup implementation here **
        /// 

        // Fire Focused Shot
        if (_chargeTimer >= stats.focusedRangedChargeTime)
        {
            // initialize stats
            var targetDirection = (Vector3.ProjectOnPlane(_target.position, Vector3.up) - Vector3.ProjectOnPlane(transform.position, Vector3.up)).normalized;
            var stats = new DummyProjectileStats
            {
                Damage = _runtimeStats.focusedRangedDamage,
                Speed = _runtimeStats.focusedRangedSpeed,
                Range = _runtimeStats.focusedRangedRange,
                Direction = targetDirection
            };

            // get bullet from pool
            projectilePool.Get(stats, transform);

            // reset charge timer
            _chargeTimer = 0f;
        }
    }
}
