using UnityEngine;
public class EnemyAttackTrigger : StateMachineBehaviour
{
    private Enemy _enemy;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_enemy) _enemy = animator.GetComponentInParent<Enemy>();
        if (_enemy) _enemy.ActivateAttack();
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_enemy) _enemy.SetToIdle();
    }
}