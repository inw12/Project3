using UnityEngine;
[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : MonoBehaviour
{
    private struct AnimatorParameters
    {
        private float xVelocity;
        private float yVelocity;
        private int CurrentAction;
        private int ComboCounter;
        private bool MeleeTrigger;  // is "Trigger", not bool
        private bool HitstunActive;
        private bool BlockTrigger;  // is "Trigger", not bool
        private bool IsBlocking;
        private bool ParryActive;
    }
    private AnimatorParameters _parameter;

    [Header("Components Requiring Animation Control")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerCombat playerCombat;
    
    private Animator _animator;

    private static readonly int currentAction = Animator.StringToHash("CurrentAction");
    private MovementState _prevState;

    public void Initialize()
    {
        _animator = GetComponent<Animator>();
        _animator.SetInteger(currentAction, (int)playerMovement.GetState().CurrentAction);
    }

    public void UpdateAnimation()
    {
        var state = playerMovement.GetState();
        var velocity = transform.InverseTransformDirection(state.Velocity.normalized);

        _animator.SetFloat("xVelocity", velocity.x);
        _animator.SetFloat("yVelocity", velocity.z);

        if (_prevState.CurrentAction != state.CurrentAction)
        {
            _animator.SetInteger(currentAction, (int)state.CurrentAction);
        }

        _prevState = state;
    }
}
