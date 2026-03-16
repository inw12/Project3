using UnityEngine;

public enum AttackState
{
    None,
    Ranged,
    Melee
}
public struct AttackInput
{
    public bool Ranged;
    public bool Melee;
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

    public void Initialize()
    {
        Instance = this;

        _state = AttackState.None;
    }

    public void UpdateInput(AttackInput input)
    {
        _requestedRanged = input.Ranged;
        _requestedMelee = input.Melee;
    }

    public void UpdateAttack() 
    {
        _state = _requestedRanged
                ? AttackState.Ranged
                : _requestedMelee
                    ? AttackState.Melee 
                    : AttackState.None;
        _prevState = _state;
    }

    public AttackState GetState() => _state;
    public AttackState GetPrevState() => _prevState;
}
