/// 
/// * This script is responsible for...
///     - State machine control
///     - Determining what actions to do depending on the state
/// 
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
public struct EnemyState
{
    public EnemyAction CurrentAction;       // what action is CURRENTLY happening
    public EnemyAttackType CurrentAttack;   // what ATTACK is the enemy currently performing?
}
public enum EnemyAction
{
    Idle    = 0,
    Move    = 1,
    Attack  = 2,
    Stagger = 3
}
public enum EnemyAttackType
{
    None            = 0,
    Ranged          = 1,
    FocusedRanged   = 2,
    Melee           = 3,
    Zone            = 4
}

public class Enemy : MonoBehaviour
{
    // Called by all 'EnemyAttack' variants to exit attack state
    public static Enemy Instance { get; private set; }

    [SerializeField] private bool stateMachineActive;
    [SerializeField] private float attackCooldown;
    private float _cooldownTimer;
    [Space]
    [SerializeField] private EnemyAction currentAction;
    [SerializeField] private EnemyAttackType currentAttack;
    [Space]
    [SerializeField] private LayerMask playerLayer;

    [Header("Enemy Components")]
    [SerializeField] private EnemyMovement enemyMovement;
    [SerializeField] private EnemyAnimationController animationController;
    [SerializeField] private EnemyHealth enemyHealth;

    [Header("Basic Stats")]
    [SerializeField] private float health = 100f;
    [SerializeField] private float moveSpeed = 15f;

    [Header("Attacks")]
    [SerializeField] private List<EnemyAttack> rangedAttacks;
    [SerializeField] private List<EnemyAttack> focusAttacks;
    [SerializeField] private List<EnemyAttack> meleeAttacks;
    [SerializeField] private List<EnemyAttack> zoneAttacks;

    // "Who are we fighting?"
    private Transform _target;    

    // "What action do we want to do?"
    private EnemyAttackType _requestedAttack;
    private EnemyAttack _currentAttack;
    private bool _attackActive;
    private bool _attackSelected;

    // State Machine Control
    private EnemyState _state;
    private EnemyState _prevState;

    // Player Detection
    private readonly Collider[] _hitBuffer = new Collider[10];

    /// Animation Controller Variables
    /// * CurrentAction (int)
    /// * CurrentAttack (int)
    /// * AttackID      (int)

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Initialize Enemy Components
        enemyHealth.Initialize(health);
        enemyMovement.Initialize(moveSpeed);
        animationController.Initialize(this);

        // Initialize State Machine
        SetToIdle();
        _prevState = _state;

        // Manual state change from the editor
        _state.CurrentAction = currentAction;
        _state.CurrentAttack = currentAttack;
    }

    void Update()
    {
        currentAction = _state.CurrentAction;
        currentAttack = _state.CurrentAttack;

        if (stateMachineActive)
        {
            _cooldownTimer += Time.deltaTime;
            if (_cooldownTimer >= attackCooldown)
            {
                // Request the next attack
                // _requestedAttack = (EnemyAttackType)Random.Range(1, 5);
                
                // Transition from Idle -> Attack
                if (_state.CurrentAction is EnemyAction.Idle)
                {
                    _state.CurrentAction = EnemyAction.Attack;
                    _state.CurrentAttack = EnemyAttackType.Ranged;
                }
            }

            // Update Attack State
            if (_state.CurrentAction is EnemyAction.Attack)
            {
                switch(_state.CurrentAttack)
                {
                    case EnemyAttackType.Ranged:
                        GetRangedAttack();
                        break;
                    default:
                        break;
                };
            }

            // Update Animator Controller
            // * only update the animator on state change
            if (_prevState.CurrentAction != _state.CurrentAction || _prevState.CurrentAttack != _state.CurrentAttack)
            {
                var current = new EnemyAnimatorVariables
                {
                    CurrentAction = (int)_state.CurrentAction,
                    CurrentAttack = (int)_state.CurrentAttack,
                    AttackID = _currentAttack ? _currentAttack.GetAttackID() : 0
                };
                animationController.UpdateAnimator(current);
            }
        }        

        //#region Randomized State Machine Control
        //// Request a new attack after duration
        //_stateTimer += Time.deltaTime;
        //if (_stateTimer >= stateSwitchCooldown && _state.CurrentAction is EnemyAction.Idle)
        //{
        //    // 1. Request an attack type
        //    _requestedAttack = (EnemyAttackType)Random.Range(1, 5);
        //    // 2. Check to see if this attack type is doable
        //    // 3. Select an attack from that attack type
        //}
        //#endregion
    }

    void LateUpdate()
    {
        _prevState = _state;
    }

    void FixedUpdate()
    {
        
    }

    private void GetRangedAttack()
    {
        if (!_target) ScanForPlayer(100);
        //_attackActive = true;

        // Randomly select ranged attack type
        if (!_attackSelected)
        {
            _currentAttack = rangedAttacks[Random.Range(0, rangedAttacks.Count)];
            _attackSelected = true;
        }

        // Only trigger attack effects when '_attackActive' is true
        // '_attackActive' only toggled to 'true' when attack animation plays
        if (_attackActive)
        {
            _currentAttack.Attack(_target);
        }
    }

    public void ActivateAttack() => _attackActive = true;

    // Called @ the end of attack or movement functions to reset enemy state
    public void SetToIdle()
    {
        _state.CurrentAction = EnemyAction.Idle;
        _state.CurrentAttack = EnemyAttackType.None;

        _attackActive = false;
        _attackSelected = false;

        _cooldownTimer = 0f;
    }

    public void DeactivateAttack() => _attackActive = false;

    public void ScanForPlayer(float detectionRadius)
    {
        int hitCount = Physics.OverlapSphereNonAlloc(
            transform.position,
            detectionRadius,
            _hitBuffer,
            playerLayer
        );
        if (hitCount > 0)
        {
            var hit = _hitBuffer.FirstOrDefault(c => c != null);
            _target = hit.transform;
        }
    }
}
