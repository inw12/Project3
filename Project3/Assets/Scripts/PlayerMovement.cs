using UnityEngine;

public struct MovementInput
{
    public Vector2 Movement;
    public bool Dodge;
    public Vector2 MousePosition;
}
public struct MovementState
{
    public MovementAction CurrentAction;
    public Vector3 Velocity;
    public bool IsGrounded;
}
public enum MovementAction
{
    Idle  = 0, 
    Move  = 1, 
    Dodge = 2
}

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; }

    private struct DodgeInfo
    {
        public Vector3 Direction;
        public bool Triggered;
        public float Timer;
    }

    [Space]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float moveAcceleration = 10f;
    [SerializeField] private float moveRotation = 10f;
    [Space]
    [SerializeField] private CapsuleCollider hurtbox;
    [SerializeField] private float dodgeSpeed= 7f;
    [SerializeField] private float dodgeAcceleration = 15f;
    [SerializeField] private float dodgeDuration = 1f;
    private DodgeInfo _dodgeInfo;

    private CharacterController _controller;
    private bool _inputEnabled;

    // Requested Inputs
    private Vector3 _requestedMovement;
    private bool _requestedDodge;
    private Vector2 _requestedCursor;

    // State Machine
    private MovementState _state;
    private MovementState _prevState;

    public void Initialize()
    {
        Instance = this;

        // CharacterController
        _controller = GetComponent<CharacterController>();

        // Player Input
        _inputEnabled = true;
        
        // State Machine
        _state.CurrentAction = MovementAction.Idle;
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
            if (_requestedDodge && !_dodgeInfo.Triggered && _requestedMovement.sqrMagnitude > 0f)   // Trigger Dodge (if pressed)
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

            // Mouse Input
            _requestedCursor = input.MousePosition;
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
            _state.CurrentAction = MovementAction.Dodge;
            _dodgeInfo.Timer += deltaTime;

            // Sustain this movement during dodge duration
            var targetVelocity = dodgeSpeed * _dodgeInfo.Direction;
            _state.Velocity = Vector3.Lerp
            (
                _state.Velocity,
                targetVelocity,
                1f - Mathf.Exp(-dodgeAcceleration * deltaTime)
            );

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
            _state.CurrentAction = MovementAction.Move;

            var targetVelocity = moveSpeed * _requestedMovement;
            _state.Velocity = Vector3.Lerp
            (
                _state.Velocity,
                targetVelocity,
                1f - Mathf.Exp(-moveAcceleration * deltaTime)
            );
        }
        // Idle
        else
        {
            _state.CurrentAction = MovementAction.Idle;

            _state.Velocity = Vector3.Lerp
            (
                _state.Velocity,
                Vector3.zero,
                1f - Mathf.Exp(-moveAcceleration * deltaTime)
            );
        }

        // Apply Movement
        _controller.Move(_state.Velocity * deltaTime);

        // Update State Machine
        _prevState = _state;
    }

    public void UpdateRotation(float deltaTime)
    {
        // Rotate player towards cursor (if attacking)
        if (PlayerAttack.Instance.GetState().CurrentAttack != Attack.None)
        {
            Ray cursorPosition = Camera.main.ScreenPointToRay(_requestedCursor);
            if (Physics.Raycast(cursorPosition, out RaycastHit hit, Mathf.Infinity))
            {
                var targetDirection = (hit.point - transform.position).normalized;
                targetDirection.y = 0f;

                var targetRotation = Quaternion.LookRotation(targetDirection);
                transform.rotation = Quaternion.Lerp
                (
                    transform.rotation,
                    targetRotation,
                    1f - Mathf.Exp(-moveRotation * 2f * deltaTime)
                );
            }
        }
        // Rotate player towards direction of movement
        else if (_requestedMovement.sqrMagnitude > 0f)
        {
            var targetRotation = Quaternion.LookRotation(_requestedMovement);
            transform.rotation = Quaternion.Lerp
            (
                transform.rotation,
                targetRotation,
                1f - Mathf.Exp(-moveRotation * deltaTime)
            );
        }
    }

    public void EnableMovementInput() => _inputEnabled = true;
    public void DisableMovementInput()
    {
        _inputEnabled = false;
        _state.Velocity = Vector3.zero;
    } 
    
    public MovementState GetState() => _state;
    public MovementState GetPrevState() => _prevState;

    public Vector3 GetMovementDirection() => _requestedMovement;
}
