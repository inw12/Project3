using UnityEngine;

public struct MovementInput
{
    public Vector2 Movement;
    public bool Dodge;
}

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;

    private CharacterController _controller;

    // Requested Inputs
    private Vector2 _requestedMovement;
    private bool _requestedDodge;

    public void Initialize()
    {
        _controller = GetComponent<CharacterController>();
    }

    public void UpdateInput(MovementInput input)
    {
        _requestedMovement = input.Movement;

        _requestedDodge = input.Dodge;
    }

    // Should be called in FIXEDUPDATE() in 'Player'
    public void UpdateMovement()
    {
        
    }
}
