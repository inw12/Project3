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
    [SerializeField] private float moveSpeed = 10f;
    [Space]
    [SerializeField] private float dodgeSpeed= 7f;
    [SerializeField] private float dodgeDuration = 1f;

    private CharacterController _controller;

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
        
        // State Machine
        _state.Action = CurrentAction.Idle;
        _state.Velocity = Vector3.zero;
        _state.IsGrounded = _controller.isGrounded;
        _prevState = _state;
    }

    // Should be called in UPDATE() in 'Player'
    public void UpdateInput(MovementInput input)
    {
        // Movement Direction
        _requestedMovement = new Vector3(input.Movement.x, 0f, input.Movement.y).normalized;

        // Dodge Input
        _requestedDodge = input.Dodge;
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

        // Apply Movement
        var targetVelocity = deltaTime * moveSpeed * _requestedMovement;
        _controller.Move(targetVelocity);

        // Update State Machines
        _state.Velocity = _controller.velocity;

        // Update Previous State
        _prevState = _state;
    }
}
