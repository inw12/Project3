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

    [SerializeField] private Animator animator;
    [Header("Melee Attack")]
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

    public void UpdateAttack(MovementState movementState) 
    {
        // ** "Where are we attacking?" **
        Ray cursorPosition = Camera.main.ScreenPointToRay(_requestedCursor);
        if (Physics.Raycast(cursorPosition, out RaycastHit hit, Mathf.Infinity)) {
            _state.AttackPosition = hit.point;
        }

        if (movementState.CurrentAction != MovementAction.Dodge)
        {
            // "What attack are we performing?"
            _state.CurrentAttack = _requestedRanged
                                    ? Attack.Ranged : _requestedMelee 
                                    ? Attack.Melee : _comboActive
                                    ? Attack.Melee : Attack.None;
            
            // Perform ranged attack
            if (_state.CurrentAttack is Attack.Ranged)
            {
                // fire projectile
            }
            // Melee combo
            else if (_state.CurrentAttack is Attack.Melee)
            {
                // begin melee routine
                if (!_comboActive) _comboActive = true;
                _comboCounter++;
                _comboTimer = 0f;


            }
        }

        // Update '_prevState'
        _prevState = _state;
    }

    private void MeleeCombo()
    {
        
    }

    public AttackState GetState() => _state;
    public AttackState GetPrevState() => _prevState;
}
