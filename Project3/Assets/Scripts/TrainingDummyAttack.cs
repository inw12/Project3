using System.Collections.Generic;
using System.Linq;
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
    [Space]
    [SerializeField] private float meleeSpeed = 10f;
    [SerializeField] private float sweepRadius = 0.1f;
    [SerializeField] private float meleeDuration = 0.5f;
    private float _meleeTimer;
    [SerializeField] private GameObject sword;
    [SerializeField] private Transform swordPivot;
    [SerializeField] private Transform swordHitbox;
    [SerializeField] private float startRotation;
    [SerializeField] private float endRotation;
    private readonly HashSet<Collider> _meleeHits = new();
    private Vector3 _prevSweep;
    [Space]
    [SerializeField] private float attackCooldown = 2f;

    private Transform _target;
    private float _fireTimer;       // Basic Ranged Fire Rate Timer
    private float _chargeTimer;     // Charge Timer for focused shot and zone attack

    private float _stateTimer; 

    private readonly Collider[] _hitBuffer = new Collider[5];

    // State Machine Control
    private AttackType _current;
    private AttackType _previous;

    void Start()
    {
        _runtimeStats = Instantiate(stats);
        _fireTimer = 0f;
        _chargeTimer = 0f;

        // Melee attack stuff
        sword.SetActive(false);
        swordHitbox.gameObject.SetActive(false);
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
            // Melee Atttack
            case AttackType.Melee:
                MeleeAttack();
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

    private void SwitchState(AttackType attack)
    {
        _stateTimer += Time.deltaTime;
        if (_stateTimer >= attackCooldown)
        {
            attackType = attack;
            _stateTimer = 0f;
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

    private void MeleeAttack()
    {
        // Melee Attack START
        if (_previous != _current)
        {
            _meleeHits.Clear();
            _meleeTimer = 0f;

            swordPivot.localEulerAngles = startRotation * Vector3.up;
            _prevSweep = swordHitbox.position;

            sword.SetActive(true);
            swordHitbox.gameObject.SetActive(true);
        }

        _meleeTimer += Time.deltaTime;

        // Melee Attack Swing
        swordPivot.localRotation = Quaternion.Lerp
        (
            swordPivot.localRotation,
            Quaternion.Euler(endRotation * Vector3.up),
            1f - Mathf.Exp(-meleeSpeed * Time.deltaTime)
        );

        // Detect hits
        var direction = swordHitbox.position - _prevSweep;
        var distance = direction.magnitude;

        if (distance < 0.001f) return;

        var hits = Physics.SphereCastAll
        (
            _prevSweep,
            sweepRadius,
            direction.normalized,
            distance,
            _runtimeStats.targetLayer
        );
        foreach (var hit in hits)
        {
            if (_meleeHits.Contains(hit.collider) || !hit.collider) continue;

            _meleeHits.Add(hit.collider);

            // Look for player hurtbox/parrybox
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("PlayerHurtbox"))
            {
                if (hit.collider.TryGetComponent(out PlayerHealth player))
                    player.DecreaseHealth(_runtimeStats.meleeDamage);
            }
        }
        _prevSweep = swordHitbox.position;

        if (_meleeTimer > meleeDuration)
        {
            attackType = AttackType.None;
            sword.SetActive(false);
            swordHitbox.gameObject.SetActive(false);
        }
    }

    private void ZoneAttack()
    {
        // Zone Attack START
        if (_previous != _current)
        {
            var targetPosition = transform.position;
            targetPosition.y = 0f;
            attackIndicator.Initialize(_runtimeStats, targetPosition);
            attackIndicator.Show();
        }

        // Update timer + attack indicator
        _chargeTimer += Time.deltaTime;
        var p = _chargeTimer / _runtimeStats.zoneAttackChargeTime;
        attackIndicator.UpdateIndicator(p);

        // Execute attack once charge time reached
        if (p >= 1)
        {
            // "Is the player within range?"
            var hits = Physics.OverlapSphereNonAlloc
            (
                transform.position,
                _runtimeStats.zoneAttackRadius,
                _hitBuffer,
                _runtimeStats.targetLayer
            );
            var hit = _hitBuffer.FirstOrDefault(c => c != null);

            // Trigger damaging effects
            if (hit && hit.gameObject.layer == LayerMask.NameToLayer("PlayerHurtbox"))
            {
                if (hit.TryGetComponent(out PlayerHealth player))
                    player.DecreaseHealth(_runtimeStats.zoneAttackDamage);
            }

            // Exit 'Zone Attack' State
            attackType = AttackType.None;

            // Reset everything
            _chargeTimer = 0f;
            attackIndicator.Hide();
        }
    }
}
