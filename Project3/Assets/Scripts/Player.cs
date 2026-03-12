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
    [SerializeField] private Transform cameraTarget;

    void Awake() => Instance = this;

    void Start()
    {
        playerMovement.Initialize();

        mainCamera.transform.SetPositionAndRotation(cameraTarget.position, cameraTarget.rotation);
    }

    void Update() 
    {

    }

    void LateUpdate()
    {

    }

    void FixedUpdate()
    {

    }
}
