///
/// * This script is responsible for handling ALL logic surrounding the player
/// * Any script that will affect the player in any way will have their functions called from this script
/// 
using UnityEngine;
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerCombat playerCombat;
    [Space]
    [SerializeField] private PlayerAnimationController animationController;
    [SerializeField] private PlayerAnimationRig animationRig;
    [Space]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform cameraHeight;
    [SerializeField] private Vector3 cameraOffset;
    [SerializeField] private float cameraAcceleration = 5f;

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
        playerCombat.Initialize();

        // Character Animations
        animationController.Initialize();
        animationRig.Initialize();

        // Main Camera
        mainCamera.transform.position = cameraHeight.position;
    }

    void Update()       // Read player INPUT
    {
        // Read Movement Input
        var moveInputActions = _inputActions.Movement;
        var movementInput = new MovementInput
        {
            Movement        = moveInputActions.Move.ReadValue<Vector2>(),
            Dodge           = moveInputActions.Dodge.WasPressedThisFrame(),
            MousePosition   = moveInputActions.MousePosition.ReadValue<Vector2>()
        };
        playerMovement.UpdateInput(movementInput);

        // Read Combat Input
        var combatInputActions = _inputActions.Combat;
        var combatInput = new CombatInput
        {
            Ranged          = combatInputActions.RangedAttack.IsPressed(),
            Melee           = combatInputActions.MeleeAttack.WasPressedThisFrame(),
            Parry           = combatInputActions.Parry.WasPressedThisFrame(),
            MousePosition   = moveInputActions.MousePosition.ReadValue<Vector2>()        
        };
        playerCombat.UpdateInput(combatInput);
    }

    void LateUpdate()   // Update components IN RESPONSE to player action
    {
        var deltaTime = Time.deltaTime;

        // Update Combat Actions
        playerCombat.UpdateCombatAction(deltaTime);

        // Rotate character
        playerMovement.UpdateRotation(deltaTime);

        // Update Animations
        animationController.UpdateAnimation();
        animationRig.UpdateRig();

        // Update Camera Position
        var cameraTarget = cameraHeight.position + cameraOffset;
        mainCamera.transform.position = Vector3.Lerp
        (
            mainCamera.transform.position,
            cameraTarget,
            1f - Mathf.Exp(-cameraAcceleration * deltaTime)
        );
    }

    void FixedUpdate()  // Trigger CHARACTER MOVEMENT
    {
        var deltaTime = Time.fixedDeltaTime;
        playerMovement.UpdateMovement(deltaTime);
    }

    void OnDisable()
    {
        _inputActions.Dispose();
    }
}
