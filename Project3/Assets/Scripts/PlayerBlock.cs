using UnityEngine;
public class PlayerBlock : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerAttack playerAttack;
    [Space]
    [SerializeField] private Animator animator;
    [Space]
    [SerializeField] private CapsuleCollider parryBox;
    [Space]
    [SerializeField] private float blockDuration = 0.6f;
    private float _blockTimer;

    private bool _requestedBlock;

    // Animator Metrics
    private bool _isBlocking;
    private bool _parryTriggered;

    public void Initialize()
    {
        parryBox.enabled = false;
    }

    public void UpdateInput(bool input)
    {
        _requestedBlock = input;
        if (_requestedBlock)
        {
            // We can only block if the player is not attacking and not performing a dodge
            if (playerAttack.GetState().CurrentAttack is Attack.None
                && playerMovement.GetState().CurrentAction is MovementAction.Idle or MovementAction.Move )
            {
                // Block START
                _blockTimer = 0f;
                playerMovement.DisableMovementInput();
                playerAttack.DisableAttackInput();

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

                playerMovement.EnableMovementInput();
                playerAttack.EnableAttackInput();
            }
        }
    }

    public void EnableParryBox() => parryBox.enabled = true;
    public void DisableParryBox() => parryBox.enabled = false;

    public void TriggerParry() => _parryTriggered = true;
}
