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
    Idle,
    Move,
    Attack,
    Stagger
}
public enum EnemyAttack
{
    None,
    Ranged,
    FocusedRanged,
    Melee,
    Zone
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
