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
    [Space]
    [SerializeField] private Transform cameraHeight;
    [SerializeField] private Vector3 cameraOffset;

    private PlayerInput _inputActions;

    void Awake() => Instance = this;

    void Start()
    {
        _inputActions = new PlayerInput();
        _inputActions.Enable();

        playerMovement.Initialize();
        animationController.Initialize();

        mainCamera.transform.position = cameraHeight.position;
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
        // Rotate character
        playerMovement.UpdateRotation(Time.deltaTime);

        // Update Animations
        animationController.UpdateAnimation();

        // Update Camera Position
        mainCamera.transform.position = cameraHeight.position + cameraOffset;
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
