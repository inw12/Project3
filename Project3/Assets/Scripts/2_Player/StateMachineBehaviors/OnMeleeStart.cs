using UnityEngine;
public class OnMeleeStart : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerCombat.Instance.DisableMeleeInput();
    }
}
