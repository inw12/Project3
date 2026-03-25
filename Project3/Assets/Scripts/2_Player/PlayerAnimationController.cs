using UnityEngine;
/// ************************
/// * Animator Metrics:
///     - xVelocity
///     - yVelocity
///     - MovementAction
///     - CombatAction
///     - MeleeTrigger
///     - ComboCount
///     - HitstunActive
/// ************************
[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : MonoBehaviour
{
    [Header("Components Requiring Animation Control")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerCombat playerCombat;

    private Animator _animator;

    // Action States
    private MovementState _movementState;
    private MovementState _prevMovementState;
    private CombatState _combatState;
    private CombatState _prevCombatState;

    public void Initialize()
    {
        _animator = GetComponent<Animator>();

        // Initialize Animator Values
        _animator.SetInteger("MovementAction", (int)playerMovement.GetState().CurrentAction);
        _animator.SetInteger("CombatAction", (int)playerCombat.GetState().CurrentAction);
    }

    public void UpdateAnimation()
    {
        _movementState = playerMovement.GetState();
        _combatState = playerCombat.GetState();


        // * Velocity (x/y)
        var velocity = transform.InverseTransformDirection(_movementState.Velocity.normalized);
        _animator.SetFloat("xVelocity", velocity.x);
        _animator.SetFloat("yVelocity", velocity.z);

        // * Movement Action
        if (_prevMovementState.CurrentAction != _movementState.CurrentAction)
        {
            _animator.SetInteger("MovementAction", (int)_movementState.CurrentAction);
        }

        // * Combat Action
        if (_prevCombatState.CurrentAction != _combatState.CurrentAction)
        {
            _animator.SetInteger("CombatAction", (int)_combatState.CurrentAction);
        }


        _prevMovementState = _movementState;
        _prevCombatState = _combatState;
    }

    public void UpdateMeleeAnimation(int comboCount)
    {
        _animator.SetTrigger("MeleeTrigger");
        _animator.SetInteger("ComboCount", comboCount);
    }
    public void UpdateMeleeAnimation(bool hitstunActive)
    {
        _animator.SetBool("HitstunActive", hitstunActive);
    }
}
