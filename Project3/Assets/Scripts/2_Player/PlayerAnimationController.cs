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
        var velocity = transform.InverseTransformDirection(state.Velocity.normalized);

        animator.SetFloat("xVelocity", velocity.x);
        animator.SetFloat("yVelocity", velocity.z);

        if (_prevState.CurrentAction != state.CurrentAction)
        {
            animator.SetInteger(currentAction, (int)state.CurrentAction);
        }

        _prevState = state;
    }
}
