using System;
using UnityEngine;
public class PlayerAttackMelee : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float damage;
    [SerializeField] private float meleeOuterRange;
    [SerializeField] private float meleeInnerRange;

    [Header("Dash Movement")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashAcceleration;
    [SerializeField] private float dashDuration;
    [SerializeField] [Range(0f, 5f)] private float targetedDashSpeedMultiplier;
    private Vector3 _dashVelocity;
    private float _dashTimer;

    [Header("Combo")]
    [SerializeField] private float hitstunDuration;
    [SerializeField] private float comboBuffer;
    private int _comboCounter;
    private float _comboTimer;
    private bool _hitstunActive;
    private float _hitstunTimer;

    [Header("Unity Components")]
    [SerializeField] private PlayerAnimationController animationController;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Transform melee1Hitbox;
    [SerializeField] private Transform melee2Hitbox;
    [SerializeField] private Transform melee3Hitbox;
    [SerializeField] private float hitboxRadius;
    [SerializeField] private float hitboxOffset;
    private bool _hitboxActive;

    // For enemy tracking/targeting
    private readonly Collider[] _meleeHits = new Collider[5];
    private readonly Collider[] _outerHits = new Collider[5];
    private readonly Collider[] _innerHits = new Collider[5];
    private Vector3 _target;

    private bool _meleeInputEnabled;
    private Vector3 _meleeGizmos;

    public void Initialize()
    {
        ResetMeleeCombo();
        _meleeInputEnabled = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, meleeOuterRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, meleeInnerRange);
        Gizmos.color = Color.yellow;
        if (_meleeGizmos != null) Gizmos.DrawWireSphere(_meleeGizmos, hitboxRadius);
    }

    // Called every frame in "PlayerCombat" when in the "Melee" state
    public void UpdateMeleeAttack(ref CombatState state, ref bool meleeStarted, float deltaTime)
    {
        // Exit Melee State once timer exceeds combo input buffer
        if (_comboTimer > comboBuffer) 
        {
            state.CurrentAction = CombatAction.None;
            meleeStarted = false;
            ResetMeleeCombo();
            PlayerMovement.Instance.EnableMovementInput();
        }

        // Input buffers should only update when able to input
        if (_meleeInputEnabled)
        {
            // Increment Timers
            _comboTimer += deltaTime;
        }
        _dashTimer += deltaTime;

        // Scan for enemies
        var outerHits = Physics.OverlapSphereNonAlloc
        (
            transform.position,
            meleeOuterRange,
            _outerHits,
            enemyLayer
        );
        var innerHits = Physics.OverlapSphereNonAlloc
        (
            transform.position,
            meleeInnerRange,
            _innerHits,
            enemyLayer
        );
        
        // Melee Targeting 
        _target = outerHits > 0 ? _outerHits[0].transform.position : Vector3.zero;
        var directionToTarget = (Vector3.ProjectOnPlane(_target, Vector3.up) - Vector3.ProjectOnPlane(transform.position, Vector3.up)).normalized;
        var directionToCursor = (Vector3.ProjectOnPlane(state.Target, Vector3.up) - Vector3.ProjectOnPlane(transform.position, Vector3.up)).normalized;
        _target = Vector3.Dot(directionToCursor, directionToTarget) > 0f ? _target : Vector3.zero;

        // Calculate target velocity for melee dash
        var direction = _target == Vector3.zero ? (state.Target - transform.position).normalized    // move towards cursor
                                                : (_target - transform.position).normalized;        // or move towards target
        _dashVelocity = direction * dashSpeed;        

        // Reset velocity if dash duration ended
        _dashVelocity = _dashTimer < dashDuration ? _dashVelocity : Vector3.zero;
        // Multiply velocity if dashing towards target
        _dashVelocity = outerHits > 0 ? _dashVelocity * targetedDashSpeedMultiplier : _dashVelocity;
        // Reset velocity if target is in "inner" range
        _dashVelocity = innerHits == 0 ? _dashVelocity : Vector3.zero;
        PlayerMovement.Instance.SetVelocity(_dashVelocity, dashAcceleration);
    }

    public void UpdateHitbox(float deltaTime)
    {
        // Get hitbox location
        var source = _comboCounter switch
        {
            2 => melee2Hitbox,
            3 => melee3Hitbox,
            _ => melee1Hitbox
        };
        var offset = transform.forward * hitboxOffset;

        _meleeGizmos = source.position + offset;

        if (_hitboxActive)
        {
            // Scan for hits
            var hits = Physics.OverlapSphereNonAlloc
            (
                source.position + offset,
                hitboxRadius,
                _meleeHits,
                enemyLayer
            );

            // Trigger hit
            if (hits > 0 && !_hitstunActive)
            {
                _hitstunActive = true;
                _hitstunTimer = 0f;

                animationController.UpdateMeleeAnimation(_hitstunActive);

                var hit = _meleeHits[0];
                if (hit.TryGetComponent(out IDamageable e))
                {
                    e.DecreaseHealth(damage);
                    _hitboxActive = false;
                }
            }
        }

        // Update hitstun timer
        if (_hitstunActive)
        {
            _hitstunTimer += deltaTime;
            if (_hitstunTimer >= hitstunDuration)
            {
                _hitstunActive = false;
                animationController.UpdateMeleeAnimation(_hitstunActive);
            }
        }
    }

    // Called whenever "melee" button is pressed
    public void TriggerAttack()
    {
        if (_meleeInputEnabled)
        {
            _comboTimer = 0f;
            _dashTimer = 0f;

            if (_comboCounter == 3) _comboCounter = 0;
            _comboCounter++;
            _comboCounter = Math.Clamp(_comboCounter, 0, 3);

            animationController.UpdateMeleeAnimation(_comboCounter);
        }
    }

    private void ResetMeleeCombo()
    {
        _comboCounter = 0;
        _comboTimer = 0f;
        _dashTimer = 0f;
    }

    public void EnableMeleeInput() => _meleeInputEnabled = true;
    public void DisableMeleeInput() => _meleeInputEnabled = false;

    public void EnableMeleeHitbox() => _hitboxActive = true;
    public void DisableMeleeHitbox() => _hitboxActive = false;
}