using UnityEngine;
public class EnableMeleeInputOnExit : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerCombat.Instance.EnableMeleeInput();
    }
}
