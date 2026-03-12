using UnityEngine;

public struct MovementInput
{
    public Vector2 Movement;
    public bool Dodge;
}
public struct MovementState
{
    public CurrentAction Action;
    public Vector3 Velocity;
    public bool IsGrounded;
}
public enum CurrentAction
{
    Idle, Move, Dodge
}

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private struct DodgeInfo
    {
        public Vector3 Direction;
        public bool Triggered;
        public float Timer;
    }

    [Space]
    [SerializeField] private float moveSpeed = 10f;
    [Space]
    [SerializeField] private CapsuleCollider hurtbox;
    [SerializeField] private float dodgeSpeed= 7f;
    [SerializeField] private float dodgeDuration = 1f;
    private DodgeInfo _dodgeInfo;

    private CharacterController _controller;
    private bool _inputEnabled;

    // Requested Inputs
    private Vector3 _requestedMovement;
    private bool _requestedDodge;

    // State Machine
    private MovementState _state;
    private MovementState _prevState;

    public void Initialize()
    {
        // CharacterController
        _controller = GetComponent<CharacterController>();

        // Player Input
        _inputEnabled = true;
        
        // State Machine
        _state.Action = CurrentAction.Idle;
        _state.Velocity = Vector3.zero;
        _state.IsGrounded = _controller.isGrounded;
        _prevState = _state;
    }

    // Should be called in UPDATE() in 'Player'
    public void UpdateInput(MovementInput input)
    {
        if (_inputEnabled)
        {
            // Movement Direction
            _requestedMovement = new Vector3(input.Movement.x, 0f, input.Movement.y).normalized;

            // Dodge Input
            _requestedDodge = input.Dodge;
            // Trigger Dodge (if pressed)
            if (_requestedDodge && !_dodgeInfo.Triggered)
            {
                // Update dodge info
                _dodgeInfo.Triggered = true;
                _dodgeInfo.Direction = _requestedMovement;
                _dodgeInfo.Timer = 0f;

                // Disable hurtbox
                hurtbox.enabled = false;

                // Disable player inputs
                _inputEnabled = false;
            }
        }
    }

    // Should be called in FIXEDUPDATE() in 'Player'
    public void UpdateMovement(float deltaTime)
    {
        // Apply mild gravity force while grounded
        _state.IsGrounded = _controller.isGrounded;
        if (_state.IsGrounded)
        {
            if (_prevState.Velocity.y < -2f) {
                _state.Velocity.y = -2f;
            }
        }

        // Dodge Movement
        if (_dodgeInfo.Triggered)
        {
            _state.Action = CurrentAction.Dodge;
            _dodgeInfo.Timer += deltaTime;

            // Sustain this movement during dodge duration
            var targetVelocity = deltaTime * dodgeSpeed * _dodgeInfo.Direction;
            _controller.Move(targetVelocity);

            // Reset everything once dodge duration reached
            if (_dodgeInfo.Timer > dodgeDuration)
            {
                _dodgeInfo.Triggered = false;
                _inputEnabled = true;
                hurtbox.enabled = true;
            }
        }
        // Regular Movement
        else if (_requestedMovement.sqrMagnitude > 0f)
        {
            _state.Action = CurrentAction.Move;
            var targetVelocity = deltaTime * moveSpeed * _requestedMovement;
            _controller.Move(targetVelocity);
        }
        else
        {
            _state.Action = CurrentAction.Idle;
        }

        // Update State Machine
        _state.Velocity = _controller.velocity;
        _prevState = _state;
    }
}
