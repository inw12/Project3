///
/// * This script is responsible for handling ALL logic surrounding the player
/// * Any script that will affect the player in any way will have their functions called from this script
/// 
using UnityEngine;
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [SerializeField] private Camera mainCamera;
    [Space]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerAttack playerAttack;
    [Space]
    [SerializeField] private PlayerAnimationController animationController;
    [SerializeField] private PlayerAnimationRig animationRig;
    [Space]
    [SerializeField] private Transform cameraHeight;
    [SerializeField] private Vector3 cameraOffset;

    private PlayerInput _inputActions;

    void Awake() => Instance = this;

    void Start()
    {
        // Player Input
        _inputActions = new PlayerInput();
        _inputActions.Enable();

        // Player Actions (Movement/Attacks)
        playerMovement.Initialize();
        playerAttack.Initialize();

        // Character Animations
        animationController.Initialize();
        animationRig.Initialize();

        // Main Camera
        mainCamera.transform.position = cameraHeight.position;
    }

    void Update()
    {
        // Movement Input
        var input = _inputActions.Movement;
        var movementInput = new MovementInput
        {
            Movement        = input.Move.ReadValue<Vector2>(),
            Dodge           = input.Dodge.WasPressedThisFrame(),
            MousePosition   = input.MousePosition.ReadValue<Vector2>()
        };
        playerMovement.UpdateInput(movementInput);

        // Combat Input
        var attackInput  = new AttackInput
        {
            Ranged          =   _inputActions.Combat.RangedAttack.IsPressed(),
            Melee           =   _inputActions.Combat.MeleeAttack.WasPressedThisFrame(),
            MousePosition   =   input.MousePosition.ReadValue<Vector2>()
        };
        playerAttack.UpdateInput(attackInput);
    }

    void LateUpdate()
    {
        var deltaTime = Time.deltaTime;

        // Rotate character
        playerMovement.UpdateRotation(deltaTime);

        // Trigger Attacks
        playerAttack.UpdateAttack(playerMovement.GetState());

        // Update Animations
        animationController.UpdateAnimation();
        animationRig.UpdateRig();

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
