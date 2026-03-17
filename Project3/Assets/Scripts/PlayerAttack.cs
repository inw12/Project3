using System;
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

    [Header("Melee Attack")]
    [SerializeField] private Animator animator;
    [Space]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashAcceleration = 15f;
    [SerializeField] private float dashDuration = 0.5f;
    private float _dashTimer;
    [Space]
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
        // "WHERE are we attacking?"
        Ray cursorPosition = Camera.main.ScreenPointToRay(_requestedCursor);
        if (Physics.Raycast(cursorPosition, out RaycastHit hit, Mathf.Infinity)) {
            _state.AttackPosition = hit.point;
        }

        if (movementState.CurrentAction != MovementAction.Dodge)
        {
            // "WHAT attack are we performing?"
            _state.CurrentAttack = _requestedRanged
                                    ? Attack.Ranged : _requestedMelee 
                                    ? Attack.Melee : _comboActive
                                    ? Attack.Melee : Attack.None;         
            
            // "When pressing the Melee button..."
            _comboTimer += Time.deltaTime;
            _dashTimer += Time.deltaTime;
            if (_requestedMelee)
            {
                _comboTimer = 0f;
                _dashTimer = 0f;

                // Animation State (for checking if current animation is complete)
                AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);
                bool animFinished = !animState.loop && animState.normalizedTime >= 1f;

                // Combo START (0 -> 1)
                if (!_comboActive) 
                {
                    PlayerMovement.Instance.DisableMovementInput();

                    // Set combo as Active
                    _comboActive = true;
                    animator.SetBool("ComboActive", _comboActive);

                    // Increment combo counter
                    _comboCounter++;
                    animator.SetInteger("ComboCounter", _comboCounter);

                    // Melee Animation Trigger
                    animator.SetTrigger("MeleeTrigger");

                    // Move Character w/ Attack
                    var direction = (_state.AttackPosition - transform.position).normalized;
                    var targetVelocity = _dashTimer < dashDuration ? dashSpeed * direction : Vector3.zero;
                    PlayerMovement.Instance.UpdateVelocity(targetVelocity, dashAcceleration);
                }                

                // Only read input for the next combo after the current animation is finished
                // *** THIS IS THE COMBO LOOP ***
                if (animFinished)
                {
                    // Increment Combo Counter
                    _comboCounter++;
                    _comboCounter = Math.Clamp(_comboCounter, 0, 3);
                    animator.SetInteger("ComboCounter", _comboCounter);

                    // Activate animation trigger
                    animator.SetTrigger("MeleeTrigger");

                    // Move Character w/ Attack
                    var direction = (_state.AttackPosition - transform.position).normalized;
                    var targetVelocity = _dashTimer < dashDuration ? dashSpeed * direction : Vector3.zero;
                    PlayerMovement.Instance.UpdateVelocity(targetVelocity, dashAcceleration);
                }
            }
            // RANGED Attack
            else if (_state.CurrentAttack is Attack.Ranged)
            {
                // fire projectile
            }

            // Reset combo when timer exceeds input window
            if (_comboTimer > comboBuffer) ResetCombo();
        }

        // Update '_prevState'
        _prevState = _state;
    }

    private void ResetCombo()
    {
        _comboCounter = 0;

        _comboActive = false;
        animator.SetBool("ComboActive", _comboActive);

        PlayerMovement.Instance.EnableMovementInput();
    }

    public AttackState GetState() => _state;
    public AttackState GetPrevState() => _prevState;
}
