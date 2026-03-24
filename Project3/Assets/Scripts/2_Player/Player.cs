///
/// * This script is responsible for handling ALL logic surrounding the player
/// * Any script that will affect the player in any way will have their functions called from this script
/// 
using UnityEngine;
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [SerializeField] private PlayerMovement playerMovement;
    [Space]
    [SerializeField] private PlayerAnimationController animationController;
    [SerializeField] private PlayerAnimationRig animationRig;
    [Space]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform cameraHeight;
    [SerializeField] private Vector3 cameraOffset;

    private PlayerInput _inputActions;

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

    void Start()
    {
        // Player Input
        _inputActions = new PlayerInput();
        _inputActions.Enable();

        // Player Actions
        playerMovement.Initialize();

        // Character Animations
        animationController.Initialize();
        animationRig.Initialize();

        // Main Camera
        mainCamera.transform.position = cameraHeight.position;
    }

    void Update()
    {
        // Read Movement Input
        var input = _inputActions.Movement;
        var movementInput = new MovementInput
        {
            Movement        = input.Move.ReadValue<Vector2>(),
            Dodge           = input.Dodge.WasPressedThisFrame(),
            MousePosition   = input.MousePosition.ReadValue<Vector2>()
        };
        playerMovement.UpdateInput(movementInput);
    }

    void LateUpdate()
    {
        var deltaTime = Time.deltaTime;

        // Rotate character
        playerMovement.UpdateRotation(deltaTime);

        // Update Animations
        animationController.UpdateAnimation();
        animationRig.UpdateRig();

        // Update Camera Position
        mainCamera.transform.position = cameraHeight.position + cameraOffset;
    }

    void FixedUpdate()
    {
        var deltaTime = Time.fixedDeltaTime;
        playerMovement.UpdateMovement(deltaTime);
    }

    void OnDisable()
    {
        _inputActions.Dispose();
    }
}
