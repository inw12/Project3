using System;
using System.Linq;
using UnityEngine;
public struct AttackState
{
    public Attack CurrentAttack;
    public Vector3 AttackPosition;
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
    // Called by 'PlayerAnimationRig' to activate animation rig
    public static PlayerAttack Instance { get; private set; }

    [SerializeField] private LayerMask targetLayer;
    [Header("Ranged Attack")]
    [SerializeField] private float projectileDamage = 1f;
    [SerializeField] private float projectileSpeed = 20f;
    [SerializeField] private float projectileRange = 10f;
    [SerializeField] private float fireRate = 0.2f;
    [Space]
    [SerializeField] private PlayerProjectilePool projectilePool;
    [SerializeField] private Transform projectileSpawn;
    private Vector3 _projectileDirection;
    private float _fireTimer;

    [Header("Melee Attack")]
    [SerializeField] private Animator animator;
    [Space]
    [SerializeField] private LayerMask meleeTarget;
    [SerializeField] private float meleeOuterRange = 8f;
    [SerializeField] private float meleeInnerRange = 2f;
    private readonly Collider[] _enemiesDetected = new Collider[5];
    [Space]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashAcceleration = 15f;
    [SerializeField] private float dashDuration = 0.5f;
    private Vector3 _dashVelocity;
    private float _dashTimer;
    [Space]
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

    public void Initialize()
    {
        Instance = this;
        _state.CurrentAttack = Attack.None;

        // Combo Variables
        _comboActive = false;
        _comboCounter = 0;
        _comboTimer = 0f;
    }

    public void UpdateInput(AttackInput input)
    {
        _requestedCursor = input.MousePosition;

        // Ranged attack should only be available if the button is pressed
        // AND we're not in the middle of a melee attack string
        _requestedRanged = input.Ranged && !_comboActive;

        // Melee attack should only be available if the button is pressed
        // AND we're not firing ranged attacks
        _requestedMelee = input.Melee && !_requestedRanged;
    }

    public void UpdateAttack(MovementState movementState, float deltaTime) 
    {
        // "WHERE are we attacking?"
        Ray cursorPosition = Camera.main.ScreenPointToRay(_requestedCursor);
        if (Physics.Raycast(cursorPosition, out RaycastHit hit, Mathf.Infinity, targetLayer)) {
            _state.AttackPosition = hit.point;
        }

        // "Are there enemies in our personal bubble?"
        var hits = Physics.OverlapSphereNonAlloc
        (
            transform.position,
            meleeOuterRange,
            _enemiesDetected,
            meleeTarget
        );

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

                    // Set combo as Active
                    _comboActive = true;
                    animator.SetBool("ComboActive", _comboActive);

                    // Increment combo counter
                    _comboCounter++;
                    animator.SetInteger("ComboCounter", _comboCounter);

                    // Melee Animation Trigger
                    animator.SetTrigger("MeleeTrigger");

                    // * Melee Movement *
                    _dashTimer = 0f;
                    // When enemy is not in range
                    if (hits == 0)
                    {
                        // Simply dash in the direction of cursor
                        var direction = (_state.AttackPosition - transform.position).normalized;
                        _dashVelocity = dashSpeed * direction;
                    }
                    // When enemy is in range
                    else
                    {
                        // Dash TOWARDS enemy in range
                        var enemyHit = _enemiesDetected.FirstOrDefault(c => c != null);
                        var enemy = enemyHit.gameObject;

                        var targetPos = Vector3.ProjectOnPlane(enemy.transform.position, Vector3.up);
                        var direction = (targetPos - transform.position).normalized;

                        var distance = Vector3.Distance(enemy.transform.position, transform.position) - meleeInnerRange;
                        _dashVelocity = distance / dashDuration * direction;
                    }
                }                

                // Only read input for the next combo after the current animation is finished
                // *** THIS IS THE COMBO LOOP ***
                if (animFinished)
                {
                    // Increment Combo Counter
                    _comboCounter++;
                    _comboCounter = Math.Clamp(_comboCounter, 0, 3);
                    animator.SetInteger("ComboCounter", _comboCounter);

                    // Activate animation trigger
                    animator.SetTrigger("MeleeTrigger");

                    // * Melee Movement *
                    _dashTimer = 0f;
                    // When enemy is not in range
                    if (hits == 0)
                    {
                        // Simply dash in the direction of cursor
                        var direction = (_state.AttackPosition - transform.position).normalized;
                        _dashVelocity = dashSpeed * direction;
                    }
                    // When enemy is in range
                    else
                    {
                        // Dash TOWARDS enemy in range
                        var enemyHit = _enemiesDetected.FirstOrDefault(c => c != null);
                        var enemy = enemyHit.gameObject;

                        var targetPos = Vector3.ProjectOnPlane(enemy.transform.position, Vector3.up);
                        var direction = (targetPos - transform.position).normalized;

                        var distance = Vector3.Distance(enemy.transform.position, transform.position) - meleeInnerRange;
                        _dashVelocity = distance / dashDuration * direction;
                    }
                }
            }
            // "When holding down the Ranged atack button..."
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
                PlayerMovement.Instance.UpdateVelocity(_dashVelocity, dashAcceleration);
            }

            // Reset combo when timer exceeds input window
            if (_comboTimer > comboBuffer) ResetCombo();
        }

        // Update '_prevState'
        _prevState = _state;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeOuterRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, meleeInnerRange);
    }

    private void ResetCombo()
    {
        _comboCounter = 0;

        _comboActive = false;
        animator.SetBool("ComboActive", _comboActive);

        PlayerMovement.Instance.EnableMovementInput();
    }

    public AttackState GetState() => _state;
    public AttackState GetPrevState() => _prevState;
}
