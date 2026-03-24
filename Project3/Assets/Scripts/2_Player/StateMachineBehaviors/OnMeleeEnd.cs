using UnityEngine;
public class OnMeleeEnd : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerCombat.Instance.MeleeAnimationEnd();
    }
}
