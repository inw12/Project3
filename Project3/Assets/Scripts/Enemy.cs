/// 
/// * This script is responsible for...
///     - State machine control
///     - Determining what actions to do depending on the state
/// 
using UnityEngine;

public struct EnemyState
{
    public EnemyAction CurrentAction;
    public EnemyAttack CurrentAttack;
}
public enum EnemyAction
{
    Idle    = 0,
    Move    = 1,
    Attack  = 2,
    Stagger = 3
}
public enum EnemyAttack
{
    None            = 0,
    Ranged          = 1,
    FocusedRanged   = 2,
    Melee           = 3,
    Zone            = 4
}

public class Enemy : MonoBehaviour
{
    [Header("Enemy Components")]
    [SerializeField] private EnemyMovement enemyMovement;
    [Header("Basic Stats")]
    [SerializeField] private float health = 100f;
    [SerializeField] private float moveSpeed = 15f;

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
        _state.CurrentAttack = EnemyAttack.None;
        _prevState = _state;
    }

    void Update()
    {
        
    }

    void LateUpdate()
    {
        _prevState = _state;
    }

    void FixedUpdate()
    {
        
    }
}
