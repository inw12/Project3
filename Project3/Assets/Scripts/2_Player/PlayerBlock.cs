using UnityEngine;
public class PlayerBlock : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerAttack playerAttack;
    [Space]
    [SerializeField] private Animator animator;
    [Space]
    [SerializeField] private CapsuleCollider parryBox;
    [SerializeField] private CapsuleCollider hurtbox;
    [Space]
    [SerializeField] private float blockDuration = 0.6f;
    [SerializeField] private float parryStartup = 0.2f;
    private float _blockTimer;

    private bool _requestedBlock;

    // Animator Metrics
    private bool _isBlocking;
    private bool _parryTriggered;

    public void Initialize()
    {
        DisableParryBox();
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
            
            if (_blockTimer >= parryStartup) EnableParryBox();

            // Reset back to normal at the end of block duration
            if (_blockTimer >= blockDuration)
            {
                _isBlocking = false;
                animator.SetBool("IsBlocking", _isBlocking);

                playerMovement.EnableMovementInput();
                playerAttack.EnableAttackInput();

                DisableParryBox();
            }
        }
    }

    public void EnableParryBox()
    {
        parryBox.enabled = true;
        hurtbox.enabled = false;
    }
    public void DisableParryBox()
    {
        parryBox.enabled = false;
        hurtbox.enabled = true;
    }

    public void TriggerParry() => _parryTriggered = true;
}
