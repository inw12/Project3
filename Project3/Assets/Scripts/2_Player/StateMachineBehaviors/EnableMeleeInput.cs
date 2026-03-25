using UnityEngine;
public class EnableMeleeInput : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerCombat.Instance.EnableMeleeInput();
    }
}
