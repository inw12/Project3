/// 
/// * This script is responsible for...
///     - State machine control
///     - Determining what actions to do depending on the state
/// 
using UnityEngine;

public struct EnemyState
{
    public EnemyAction CurrentAction;       // what action is CURRENTLY happening
    public EnemyAction IntendedAction;      // what action the enemy WANTS to do
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
    [SerializeField] private EnemyAction currentAction;
    [SerializeField] private EnemyAttackType currentAttack;
    [SerializeField] private float stateSwitchCooldown = 3f;
    private float _stateTimer;
    [Header("Enemy Components")]
    [SerializeField] private EnemyMovement enemyMovement;
    [Header("Basic Stats")]
    [SerializeField] private float health = 100f;
    [SerializeField] private float moveSpeed = 15f;
    [Header("Attacks")]
    [SerializeField] private EnemyAttack[] rangedAttacks;
    [SerializeField] private EnemyAttack[] focusAttacks;
    [SerializeField] private EnemyAttack[] meleeAttacks;
    [SerializeField] private EnemyAttack[] zoneAttacks;

    // "Who are we fighting?"
    private Transform _target;    

    // State Machine Control
    private EnemyState _state;
    private EnemyState _prevState;

    void Start()
    {
        // Set target to be the player
        if (Player.Instance) _target = Player.Instance.transform;

        // Initialize Enemy Components
        enemyMovement.Initialize(moveSpeed);

        // Initialize State Machine
        _state.CurrentAction = EnemyAction.Idle;
        _state.CurrentAttack = EnemyAttackType.None;
        _prevState = _state;
    }

    void Update()
    {
        _state.CurrentAction = currentAction;
        _state.CurrentAttack = currentAttack;
    }

    void LateUpdate()
    {
        _prevState = _state;
    }

    void FixedUpdate()
    {
        
    }
}
