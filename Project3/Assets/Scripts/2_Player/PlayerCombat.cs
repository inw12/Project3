using UnityEngine;
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
public struct CombatInput
{
    public bool Ranged;
    public bool Melee;
    public bool Parry;
    public Vector2 MousePosition;
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
        meleeAttack.Initialize();
    }

    public void UpdateInput(CombatInput input)
    {
        if (_combatInputEnabled)
        {
            _requestedMousePos = input.MousePosition;

            // Only allow combat actions when NOT DODGING
            if (PlayerMovement.Instance.GetState().CurrentAction is not MovementAction.Dodge)
            {
                // Parry should only be available if the button is pressed
                //  AND we're not performing a melee attack
                _requestedParry = input.Parry && _state.CurrentAction is not CombatAction.Melee;

                // Melee attack should only be available if the button is pressed
                //  AND we're not performing a parry
                _requestedMelee = input.Melee && _state.CurrentAction is not CombatAction.Parry;

                // Ranged attack should only be available if the button is pressed
                //  AND we're not performing a melee attack
                //  AND we're not performing a parry
                _requestedRanged = input.Ranged && _state.CurrentAction is not CombatAction.Melee or CombatAction.Parry;
            }
            else
            {
                _requestedParry = _requestedMelee = _requestedRanged = false;
            }
        }       
    }

    public void UpdateCombatAction(float deltaTime)
    {
        // State Machine Control
        switch (_state.CurrentAction)
        {
            case CombatAction.Parry:
                OnParry();
                break;
            case CombatAction.Melee:
                OnMeleeAttack(deltaTime);
                break;
            case CombatAction.Ranged:
                OnRangedAttack(deltaTime);
                break;
            default:
                TryEnterNewState();
                break;
        };

        // Debug Message
        if (_prevState.CurrentAction != _state.CurrentAction) {
            Debug.Log(_state.CurrentAction);
        }        

        // Update previous state
        _prevState = _state;
    }

    private void OnParry()
    {
        
    }
    private void OnMeleeAttack(float deltaTime)
    {
        if (_requestedMelee)
        {
            meleeAttack.TriggerAttack();
        }

        meleeAttack.UpdateMeleeAttack(ref _state, deltaTime);
    }
    private void OnRangedAttack(float deltaTime)
    {
        rangedAttack.Attack(ref _state, _requestedMousePos, deltaTime);

        if (!_requestedRanged)
        {
            _state.CurrentAction = CombatAction.None;
        }
    }
    private void TryEnterNewState()
    {
        _state.CurrentAction = _requestedParry ? CombatAction.Parry
                                : _requestedMelee ? CombatAction.Melee
                                    : _requestedRanged ? CombatAction.Ranged : CombatAction.None;
    }

    // Public methods to enable/disable combat inputs
    public void EnableCombatInput() => _combatInputEnabled = true;
    public void DisableCombatInput() => _combatInputEnabled = false;

    // State Getters
    public CombatState GetState() => _state;
    public CombatState GetPrevState() => _prevState;
}
