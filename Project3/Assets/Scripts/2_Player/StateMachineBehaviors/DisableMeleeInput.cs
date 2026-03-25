using UnityEngine;
public class DisableMeleeInput : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerCombat.Instance.DisableMeleeInput();
    }
}
