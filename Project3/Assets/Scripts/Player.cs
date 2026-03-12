///
/// * This script is responsible for handling player input
///   and updating functions related to the player character
///   in response to input
/// 
using UnityEngine;
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [SerializeField] private Camera mainCamera;
    [Space]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerAnimationController animationController;
    [SerializeField] private Transform cameraTarget;

    private PlayerInput _inputActions;

    void Awake() => Instance = this;

    void Start()
    {
        _inputActions = new PlayerInput();
        _inputActions.Enable();

        playerMovement.Initialize();
        animationController.Initialize();

        mainCamera.transform.SetPositionAndRotation(cameraTarget.position, cameraTarget.rotation);
    }

    void Update()
    {
        var input = _inputActions.Movement;
        var movementInput = new MovementInput
        {
            Movement    = input.Move.ReadValue<Vector2>(),
            Dodge       = input.Dodge.WasPressedThisFrame()
        };
        playerMovement.UpdateInput(movementInput);
    }

    void LateUpdate()
    {
        // Update camera to follow player
        mainCamera.transform.SetPositionAndRotation(cameraTarget.position, cameraTarget.rotation);

        // Update Animations
        animationController.UpdateAnimation();
    }

    void FixedUpdate()
    {
        var deltaTime = Time.deltaTime;
        playerMovement.UpdateMovement(deltaTime);
    }

    void OnDisable()
    {
        _inputActions.Dispose();
    }
}
