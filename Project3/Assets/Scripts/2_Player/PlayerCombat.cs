using UnityEngine;
public struct CombatInput
{
    public bool Ranged;
    public bool Melee;
    public bool Parry;
    public Vector2 MousePosition;
}
public struct CombatState
{
    public CombatAction CurrentAction;
    public Vector3 Target;
}
public enum CombatAction
{
    None    = 0,
    Ranged  = 1,
    Melee   = 2,
    Parry   = 3
}
public class PlayerCombat : MonoBehaviour
{
    /// * Referenced by:
    ///     - 'PlayerMovement.cs'       (handles character rotation depending on current attack)
    ///     - 'PlayerAnimationRig.cs'   (triggers ranged attack animation rig)
    public static PlayerCombat Instance { get; private set; }

    [SerializeField] private PlayerAttackRanged rangedAttack;
    [SerializeField] private PlayerAttackMelee meleeAttack;
    [SerializeField] private PlayerParry parry;

    private bool _combatInputEnabled;

    // Requested Inputs
    private bool _requestedRanged;
    private bool _requestedMelee;
    private bool _requestedParry;
    private Vector2 _requestedMousePos;

    // State Machine
    private CombatState _state;
    private CombatState _prevState;

    // Melee Variables
    private bool _comboActive;

    void Awake()
    {
        // Singleton Initialization
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Initialize()
    {
        // Enable Combat Inputs
        _combatInputEnabled = true;

        // State Machine Initialization
        _state.CurrentAction = CombatAction.None;
        _state.Target = Vector3.zero;
        _prevState = _state;

        // Initialize Combat Actions
        rangedAttack.Initialize();

        _comboActive = false;
    }

    public void UpdateInput(CombatInput input)
    {
        if (_combatInputEnabled)
        {
            _requestedMousePos = input.MousePosition;

            // Parry should only be available if the button is pressed
            // AND we're not performing a melee attack
            // AND we're not performing a dodge
            _requestedParry = input.Parry && !_comboActive && PlayerMovement.Instance.GetState().CurrentAction is not MovementAction.Dodge;

            // Ranged attack should only be available if the button is pressed
            // AND we're not in the middle of a melee attack string
            _requestedRanged = input.Ranged && !_comboActive && PlayerMovement.Instance.GetState().CurrentAction is not MovementAction.Dodge;

            // Melee attack should only be available if the button is pressed
            // AND we're not firing ranged attacks
            _requestedMelee = input.Melee && !_requestedRanged && PlayerMovement.Instance.GetState().CurrentAction is not MovementAction.Dodge;
        }       
    }

    public void UpdateCombatAction(float deltaTime)
    {
        if (_requestedRanged)
        {
            _state.CurrentAction = CombatAction.Ranged;
            rangedAttack.Attack(ref _state, _requestedMousePos, deltaTime);
        }
        else if (_requestedMelee)
        {
            _state.CurrentAction = CombatAction.Melee;
        }
    }

    public void CancelCurrentAction()
    {
        _state.CurrentAction = CombatAction.None;
    }

    // Public methods to enable/disable combat inputs
    public void EnableCombatInput() => _combatInputEnabled = true;
    public void DisableCombatInput() => _combatInputEnabled = false;

    // State Getters
    public CombatState GetState() => _state;
    public CombatState GetPrevState() => _prevState;
}
