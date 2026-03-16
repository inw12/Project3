using UnityEngine;

public struct AttackState
{
    public Attack CurrentAttack;
    public Vector2 AttackPosition;
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

    public void UpdateAttack() 
    {
        // Update '_state.AttackPosition'
        Ray cursorPosition = Camera.main.ScreenPointToRay(_requestedCursor);
        if (Physics.Raycast(cursorPosition, out RaycastHit hit, Mathf.Infinity))
        {
            _state.AttackPosition = hit.point;
            _state.AttackPosition.y = 0f;
        }

        // Update '_state.CurrentAttack'
        _state.CurrentAttack = _requestedRanged
                                ? Attack.Ranged : _requestedMelee
                                    ? Attack.Melee : Attack.None;

        // Update '_prevState'
        _prevState = _state;
    }

    public AttackState GetState() => _state;
    public AttackState GetPrevState() => _prevState;
}
