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
    public static PlayerAttack Instance { get; private set; }

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
    }

    public void UpdateInput(AttackInput input)
    {
        _requestedRanged = input.Ranged;
        _requestedMelee = input.Melee;
        _requestedCursor = input.MousePosition;
    }

    public void UpdateAttack(MovementState movementState) 
    {
        // Check to see if we can attack
        bool canAttack = movementState.CurrentAction != MovementAction.Dodge;

        if (canAttack)
        {
            // * Update '_state.AttackPosition'
            // * WHERE we are attacking
            Ray cursorPosition = Camera.main.ScreenPointToRay(_requestedCursor);
            if (Physics.Raycast(cursorPosition, out RaycastHit hit, Mathf.Infinity)) {
                _state.AttackPosition = hit.point;
            }

            // * Update '_state.CurrentAttack'
            // * Determine WHAT attack we're performing
            _state.CurrentAttack = _requestedRanged ? Attack.Ranged : Attack.Melee;
        }
        else
        {
            _state.CurrentAttack = Attack.None;
        }

        // Update '_prevState'
        _prevState = _state;
    }

    public AttackState GetState() => _state;
    public AttackState GetPrevState() => _prevState;
}
