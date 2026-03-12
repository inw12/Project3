using UnityEngine;
public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Animator animator;

    private static readonly int currentAction = Animator.StringToHash("CurrentAction");
    private MovementState _prevState;

    public void Initialize() => animator.SetInteger(currentAction, (int)playerMovement.GetState().CurrentAction);

    public void UpdateAnimation()
    {
        var state = playerMovement.GetState();

        var input = playerMovement.GetMovementDirection();
        animator.SetFloat("xInput", input.x);
        animator.SetFloat("yInput", input.z);

        if (_prevState.CurrentAction != state.CurrentAction) {
            animator.SetInteger(currentAction, (int)state.CurrentAction);
        }

        _prevState = state;
    }
}
