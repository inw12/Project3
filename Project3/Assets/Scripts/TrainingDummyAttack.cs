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
    [Space]
    [SerializeField] private AttackIndicator attackIndicator;

    private Transform _target;
    private float _fireTimer;       // Basic Ranged Fire Rate Timer
    private float _chargeTimer;     // Charge Timer for focused shot and zone attack

    private readonly Collider[] _hitBuffer = new Collider[5];

    // State Machine Control
    private AttackType _current;
    private AttackType _previous;

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
        _current = attackType;
        switch (_current)
        {
            // Basic Ranged Attack
            case AttackType.Basic:
                BasicRangedAttack();
                break;
            // Focused Ranged Attack
            case AttackType.Focused:
                FocusedRangedAttack();
                break;
            // Zone Attack
            case AttackType.Zone:
                ZoneAttack();
                break;
            // No Attack
            default:
                break;
        }
    }

    void LateUpdate()
    {
        _previous = _current;
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
        if (_chargeTimer >= _runtimeStats.focusedRangedChargeTime)
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

    private void ZoneAttack()
    {
        if (_previous != _current)
        {
            var targetPosition = transform.position;
            targetPosition.y = 0f;
            attackIndicator.Initialize(_runtimeStats, targetPosition);
            attackIndicator.Show();
        }

        _chargeTimer += Time.deltaTime;
        var p = _chargeTimer / _runtimeStats.zoneAttackChargeTime;
        attackIndicator.UpdateIndicator(p);

        if (p >= 1)
        {
            // * trigger attack * 

            attackIndicator.Hide();
        }
        
    }
}
