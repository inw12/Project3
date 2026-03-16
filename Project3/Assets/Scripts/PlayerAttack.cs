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
    // State Machine
    private AttackState _state;
    private AttackState _prevState;

    // Requested Inputs
    private bool _requestedRanged;
    private bool _requestedMelee;

    public void Initialize()
    {
        _state = AttackState.None;
    }

    public void UpdateInput(AttackInput input)
    {
        _requestedRanged = input.Ranged;
        _requestedMelee = input.Melee;
    }

    public void UpdateAttack() 
    {
        
    }
}
