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
    /// * Referenced by:
    ///     - 'PlayerCombat.cs' (to control movement during certain combat actions)
    public static PlayerMovement Instance { get; private set; }

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float moveAcceleration = 10f;
    [SerializeField] private float moveRotation = 10f;
    [Header("Dodge")]
    [SerializeField] private CapsuleCollider hurtbox;
    [Space]
    [SerializeField] private float dodgeSpeed= 7f;
    [SerializeField] private float dodgeAcceleration = 15f;
    [SerializeField] private float dodgeDuration = 1f;

    private struct DodgeData
    {
        public Vector3 Direction;
        public bool Triggered;
        public float Timer;
    }
    private DodgeData _dodgeData;

    private CharacterController _controller;
    private bool _movementInputEnabled;

    // Requested Inputs
    private Vector3 _requestedMovement;
    private bool _requestedDodge;
    private Vector2 _requestedCursor;

    // State Machine
    private MovementState _state;
    private MovementState _prevState;

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
        // CharacterController
        _controller = GetComponent<CharacterController>();

        // Player Input
        _movementInputEnabled = true;
        
        // State Machine
        _state.CurrentAction = MovementAction.Idle;
        _state.Velocity = Vector3.zero;
        _state.IsGrounded = _controller.isGrounded;
        _prevState = _state;
    }

    // * Read player input *
    //  Called in UPDATE() in 'Player.cs'
    public void UpdateInput(MovementInput input)
    {
        if (_movementInputEnabled)
        {
            // Movement Direction
            _requestedMovement = new Vector3(input.Movement.x, 0f, input.Movement.y).normalized;

            // Dodge Input
            _requestedDodge = input.Dodge;
            TryTriggerDodge();
            
            // Mouse Input
            _requestedCursor = input.MousePosition;
        }
    }

    // Should be called in FIXEDUPDATE() in 'Player'
    public void UpdateMovement(float deltaTime)
    {
        ApplyGravity();        

        // DODGE Movement
        if (_dodgeData.Triggered)
        {
            _state.CurrentAction = MovementAction.Dodge;
            _dodgeData.Timer += deltaTime;

            // Sustain dodge velocity during duration
            var targetVelocity = dodgeSpeed * _dodgeData.Direction;
            _state.Velocity = Vector3.Lerp
            (
                _state.Velocity,
                targetVelocity,
                1f - Mathf.Exp(-dodgeAcceleration * deltaTime)
            );

            // Reset everything once dodge duration reached
            if (_dodgeData.Timer > dodgeDuration)
            {
                _dodgeData.Triggered = false;
                _movementInputEnabled = true;
                hurtbox.enabled = true;
            }
        }
        // REGULAR Movement
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
        // IDLE
        else if (_requestedMovement.sqrMagnitude == 0f && _movementInputEnabled)
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
        Quaternion targetRotation;
        var combatState = PlayerCombat.Instance.GetState();

        // Rotate character towards MOUSE POSITION ------------ (Ranged Attack)
        if (combatState.CurrentAction is CombatAction.Ranged or CombatAction.Melee)
        {
            targetRotation = Quaternion.LookRotation(combatState.Target);
        }
        // Rotate character towards MOUSE POSITION ------------ (Melee Attack)
        else if (combatState.CurrentAction is CombatAction.Melee)
        {
            targetRotation = Quaternion.LookRotation(combatState.Target);
            transform.rotation = targetRotation;
        }
        // Rotate character towards DIRECTION OF MOVEMENT ----- (Basic Movement)
        else if (_requestedMovement.sqrMagnitude > 0f)
        {
            targetRotation = Quaternion.LookRotation(_requestedMovement);
        }
        // No Rotation ---------------------------------------- (No Action)
        else
        {
            targetRotation = Quaternion.LookRotation(transform.forward);
        }

        // * Apply Rotation *
        transform.rotation = Quaternion.Lerp
        (
            transform.rotation,
            targetRotation,
            1f - Mathf.Exp(-moveRotation * deltaTime)
        );
    }

    // Public Methods used by other classes to influence Player movement
    public void EnableMovementInput() => _movementInputEnabled = true;
    public void DisableMovementInput()
    {
        // Disable input
        _movementInputEnabled = false;

        // Stop any character movement
        _requestedMovement = _state.Velocity = Vector3.zero;
        _state.CurrentAction = MovementAction.Idle;
    }
    
    // Velocity Getter/Setter
    public void SetVelocity(Vector3 velocity, float acceleration)
    {
        _state.Velocity = Vector3.Lerp
        (
            _state.Velocity,
            velocity,
            1f - Mathf.Exp(-acceleration * Time.deltaTime)
        );
    }
    public Vector3 GetVelocity() => _state.Velocity;
    public void ResetVelocity() => _state.Velocity = Vector3.zero;

    // State Getters
    public MovementState GetState() => _state;
    public MovementState GetPrevState() => _prevState;

    // Helper Functions
    private void ApplyGravity()
    {
        // Gravity
        _state.IsGrounded = _controller.isGrounded;
        if (_state.IsGrounded)
        {
            if (_prevState.Velocity.y < -2f) {
                _state.Velocity.y = -2f;
            }
        } else {
            _state.Velocity += 2 * Time.deltaTime * Physics.gravity;
        }
    }
    private void TryTriggerDodge()
    {
        if (_requestedDodge && !_dodgeData.Triggered && _requestedMovement.sqrMagnitude > 0f)
        {
            // Update dodge info
            _dodgeData.Triggered = true;
            _dodgeData.Direction = _requestedMovement;
            _dodgeData.Timer = 0f;

            // Disable hurtbox
            hurtbox.enabled = false;

            // Disable player inputs
            _movementInputEnabled = false;
        }
    }
}
