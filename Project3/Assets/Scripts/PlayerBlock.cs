using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerAttack), typeof(PlayerMovement))]
public class PlayerBlock : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private CapsuleCollider parryBox;
    [Space]
    [SerializeField] private float blockDuration = 1f;
    private float _blockTimer;

    private PlayerMovement _playerMovement;
    private PlayerAttack _playerAttack;
    private bool _requestedBlock;

    // Animator Metrics
    private bool _isBlocking;

    public void Initialize()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _playerAttack = GetComponent<PlayerAttack>();

        parryBox.enabled = false;
    }

    public void UpdateInput(bool input)
    {
        _requestedBlock = input;
        if (_requestedBlock)
        {
            // We can only block if the player is not attacking and not performing a dodge
            if (_playerAttack.GetState().CurrentAttack is Attack.None
                && _playerMovement.GetState().CurrentAction is MovementAction.Idle or MovementAction.Move )
            {
                // Block START
                _blockTimer = 0f;
                _playerMovement.DisableMovementInput();
                _playerAttack.DisableAttackInput();

                _isBlocking = true;
                animator.SetBool("IsBlocking", _isBlocking);
                animator.SetTrigger("BlockTriggered");
            }
        }
    }

    public void UpdateBlock(float deltaTime)
    {
        if (_isBlocking)
        {
            _blockTimer += deltaTime;
            
            // Reset back to normal at the end of block duration
            if (_blockTimer >= blockDuration)
            {
                _isBlocking = false;
                animator.SetBool("IsBlocking", _isBlocking);
                
                _playerMovement.EnableMovementInput();
                _playerAttack.EnableAttackInput();
            }
        }
    }

    public void EnableParryBox() => parryBox.enabled = true;
    public void DisableParryBox() => parryBox.enabled = false;
}
