using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public struct AttackState
{
    public Attack CurrentAttack;
    public Vector3 AttackPosition;
    public Vector3 MeleeTarget;
}
public enum Attack
{
    None,
    Ranged,
    Melee
}
public struct AttackInput
{
    public bool Ranged;
    public bool Melee;
    public Vector2 MousePosition;
}

public class PlayerAttack : MonoBehaviour
{
    // Called by 'PlayerMovement' to rotate character appropriately to attacks
    // Called by 'PlayerAnimationRig' to activate animation rig
    public static PlayerAttack Instance { get; private set; }

    [Header("Ranged Attack")]
    [SerializeField] private LayerMask rangedTarget;
    [SerializeField] private PlayerProjectilePool projectilePool;
    [SerializeField] private Transform projectileSpawn;
    [Space]
    [SerializeField] private float projectileDamage = 1f;
    [SerializeField] private float projectileSpeed = 20f;
    [SerializeField] private float projectileRange = 10f;
    [SerializeField] private float fireRate = 0.2f;
    private Vector3 _projectileDirection;
    private float _fireTimer;

    [Header("Melee Attack | Game Components")]
    [SerializeField] private LayerMask meleeTarget;
    [SerializeField] private LayerMask enemyHurtbox;
    [SerializeField] private Animator animator;
    [Header("Melee Attack | Hitbox")]
    [SerializeField] private float meleeDamage = 2f;
    [SerializeField] private float sweepRadius = 0.1f;
    [SerializeField] private Transform meleeBase;
    [SerializeField] private Transform meleeTip;
    [Header("Melee Attack | Range")]
    [SerializeField] private float meleeOuterRange = 8f;
    [SerializeField] private float meleeInnerRange = 2f;
    private readonly Collider[] _outerHits = new Collider[5];
    private readonly Collider[] _innerHits = new Collider[5];
    private bool _hasMeleeTarget;
    [Header("Melee Attack | Movement Speed")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashAcceleration = 15f;
    [SerializeField] private float dashDuration = 0.5f;
    [SerializeField] [Range(1f, 2f)] private float targetedDashSpeedMultiplier = 1.5f;
    private Vector3 _dashVelocity;
    private float _dashTimer;
    [Header("Melee Attack | Combo")]
    [SerializeField] private float comboBuffer = 0.7f;
    private bool _comboActive;
    private int _comboCounter;
    private float _comboTimer;

    // State Machine
    private AttackState _state;
    private AttackState _prevState;

    // Requested Inputs
    private bool _requestedRanged;
    private bool _requestedMelee;
    private Vector2 _requestedCursor;

    // Hitbox Scanning
    private Vector3 _prevTipPosition;
    private Vector3 _prevBasePosition;
    private bool _hitboxActive;
    private readonly HashSet<Collider> _meleeHits = new();

    private bool _inputEnabled;

    public void Initialize()
    {
        Instance = this;
        _state.CurrentAttack = Attack.None;

        // Combo Variables
        _comboActive = false;
        _comboCounter = 0;
        _comboTimer = 0f;

        _inputEnabled = true;
    }

    public void UpdateInput(AttackInput input)
    {
        if (_inputEnabled)
        {
            _requestedCursor = input.MousePosition;

            // Ranged attack should only be available if the button is pressed
            // AND we're not in the middle of a melee attack string
            _requestedRanged = input.Ranged && !_comboActive;

            // Melee attack should only be available if the button is pressed
            // AND we're not firing ranged attacks
            _requestedMelee = input.Melee && !_requestedRanged;
        }
    }

    public void UpdateAttack(MovementState movementState, float deltaTime) 
    {
        // "WHERE are we attacking?"
        Ray cursorPosition = Camera.main.ScreenPointToRay(_requestedCursor);
        if (Physics.Raycast(cursorPosition, out RaycastHit hit, Mathf.Infinity, rangedTarget)) {
            _state.AttackPosition = hit.point;
        }

        // "Are there enemies within the outer bubble?"
        var outerHits = Physics.OverlapSphereNonAlloc
        (
            transform.position,
            meleeOuterRange,
            _outerHits,
            meleeTarget,
            QueryTriggerInteraction.Ignore
        );
        // "Are there enemies within the inner bubble?"
        var innerHits = Physics.OverlapSphereNonAlloc
        (
            transform.position,
            meleeInnerRange,
            _innerHits,
            meleeTarget,
            QueryTriggerInteraction.Ignore
        );
        _hasMeleeTarget = outerHits > 0;

        if (movementState.CurrentAction != MovementAction.Dodge)
        {
            // "WHAT attack are we performing?"
            _state.CurrentAttack = _requestedRanged
                                    ? Attack.Ranged : _requestedMelee 
                                    ? Attack.Melee : _comboActive
                                    ? Attack.Melee : Attack.None;         
            

            // Melee Attack Timers
            _comboTimer += deltaTime;
            _dashTimer += deltaTime;

            // Ranged Attack Timers
            _fireTimer += deltaTime;

            // "When pressing the Melee button..."
            if (_requestedMelee)
            {
                _comboTimer = 0f;

                // Animation State (for checking if current animation is complete)
                AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);
                bool animFinished = !animState.loop && animState.normalizedTime >= 1f;

                // Combo START (0 -> 1)
                if (!_comboActive) 
                {
                    PlayerMovement.Instance.DisableMovementInput();
                    EnableHitbox();

                    // Set combo as Active
                    _comboActive = true;
                    animator.SetBool("ComboActive", _comboActive);

                    // Increment combo counter
                    _comboCounter = 1;
                    animator.SetInteger("ComboCounter", _comboCounter);

                    // Melee Animation Trigger
                    animator.SetTrigger("MeleeTrigger");

                    // * Melee Movement *
                    _dashTimer = 0f;
                    // When enemy is not in range
                    if (!_hasMeleeTarget)
                    {
                        // Simply dash in the direction of cursor
                        var direction = (_state.AttackPosition - transform.position).normalized;
                        _dashVelocity = dashSpeed * direction;
                    }
                    // When enemy is in range
                    else
                    {
                        // Dash TOWARDS enemy in range
                        var enemyHit = _outerHits.FirstOrDefault(c => c != null);
                        var enemy = enemyHit.gameObject;

                        _state.MeleeTarget = enemy.transform.position;

                        var targetPos = Vector3.ProjectOnPlane(enemy.transform.position, Vector3.up);
                        var direction = (targetPos - transform.position).normalized;

                        var distance = Vector3.Distance(enemy.transform.position, transform.position) - meleeInnerRange;
                        _dashVelocity = distance / dashDuration * direction * targetedDashSpeedMultiplier;
                    }
                }                

                // Only read input for the next combo after the current animation is finished
                // *** THIS IS THE COMBO LOOP ***
                if (animFinished)
                {
                    _meleeHits.Clear();

                    if (_comboCounter == 3) _comboCounter = 0;

                    // Increment Combo Counter
                    _comboCounter++;
                    _comboCounter = Math.Clamp(_comboCounter, 0, 3);
                    animator.SetInteger("ComboCounter", _comboCounter);

                    // Activate animation trigger
                    animator.SetTrigger("MeleeTrigger");

                    // * Melee Movement *
                    _dashTimer = 0f;
                    // When enemy is not in range
                    if (!_hasMeleeTarget)
                    {
                        // Simply dash in the direction of cursor
                        var direction = (_state.AttackPosition - transform.position).normalized;
                        _dashVelocity = dashSpeed * direction;
                    }
                    // When enemy is in range
                    else
                    {
                        // Dash TOWARDS enemy in range
                        var enemyHit = _outerHits.FirstOrDefault(c => c != null);
                        var enemy = enemyHit.gameObject;

                        _state.MeleeTarget = enemy.transform.position;

                        var targetPos = Vector3.ProjectOnPlane(enemy.transform.position, Vector3.up);
                        var direction = (targetPos - transform.position).normalized;

                        var distance = Vector3.Distance(enemy.transform.position, transform.position) - meleeInnerRange;
                        _dashVelocity = distance / dashDuration * direction * targetedDashSpeedMultiplier;
                    }
                }
            }
            // "When holding down the Ranged attack button..."
            else if (_state.CurrentAttack is Attack.Ranged)
            {
                // calculate projectile direction
                var source = Vector3.ProjectOnPlane(projectileSpawn.position, Vector3.up);
                _projectileDirection = (_state.AttackPosition - source).normalized;

                // only shoot when timer is within fire rate interval
                if (_fireTimer >= fireRate)
                {
                    var stats = new PlayerProjectileStats
                    {
                        Damage = projectileDamage,
                        Speed = projectileSpeed,
                        Range = projectileRange,
                        Direction = _projectileDirection
                    };
                    projectilePool.Get(stats, projectileSpawn);

                    _fireTimer = 0f;
                }
            }

            // Trigger melee movement
            if (_comboActive)
            {
                _dashVelocity = _dashTimer < dashDuration ? _dashVelocity : Vector3.zero;
                _dashVelocity = innerHits == 0 ? _dashVelocity : Vector3.zero;
                PlayerMovement.Instance.UpdateVelocity(_dashVelocity, dashAcceleration);
            }

            // Reset combo when timer exceeds input window
            if (_comboTimer > comboBuffer && _comboActive) 
            {
                ResetCombo();
                DisableHitbox();
            }
        }

        // Update '_prevState'
        _prevState = _state;
    }

    public void UpdateMeleeHitbox()
    {
        if (!_hitboxActive) return;

        {
            var direction = meleeTip.position - _prevTipPosition;
            var distance = direction.magnitude;

            if (distance < 0.001f) return;  // skip if sword hasn't moved

            RaycastHit[] hits = Physics.SphereCastAll
            (
                _prevTipPosition,
                sweepRadius,
                direction.normalized,
                distance,
                enemyHurtbox
            );

            foreach (var hit in hits)
            {
                // skip if we've already hit
                if (_meleeHits.Contains(hit.collider)) continue;   
                
                // update hit list
                _meleeHits.Add(hit.collider);

                // ** Damage Effect Goes Here **
            }
        }

        _prevBasePosition = meleeBase.position;
        _prevTipPosition = meleeTip.position;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeOuterRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, meleeInnerRange);

        if (meleeTip == null || meleeBase == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(meleeBase.position, meleeTip.position);
        Gizmos.DrawWireSphere(meleeTip.position, sweepRadius);
    }

    #region Melee Hitbox Shenanigans
    private void EnableHitbox()
    {
        _meleeHits.Clear();
        _prevBasePosition = meleeBase.position;
        _prevTipPosition = meleeTip.position;
        _hitboxActive = true;
    }
    private void DisableHitbox() => _hitboxActive = false;
    #endregion

    private void ResetCombo()
    {
        _comboCounter = 0;

        _comboActive = false;
        animator.SetBool("ComboActive", _comboActive);

        PlayerMovement.Instance.EnableMovementInput();
    }

    public AttackState GetState() => _state;
    public AttackState GetPrevState() => _prevState;

    public void EnableAttackInput() => _inputEnabled = true;
    public void DisableAttackInput() => _inputEnabled = false;

    public bool HasMeleeTarget() => _hasMeleeTarget;
}
