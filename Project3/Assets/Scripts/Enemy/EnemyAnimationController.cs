using UnityEngine;
public struct EnemyAnimatorVariables
{
    public int CurrentAction;
    public int CurrentAttack;
    public int AttackID;
}
[RequireComponent(typeof(Animator))]
public class EnemyAnimationController : MonoBehaviour
{
    private Animator _animator;

    public void Initialize(Enemy enemy)
    {
        _animator = GetComponent<Animator>();
    }

    public void UpdateAnimator(EnemyAnimatorVariables enemy)
    {
        _animator.SetInteger("CurrentAction", enemy.CurrentAction);
        _animator.SetInteger("CurrentAttack", enemy.CurrentAttack);
        _animator.SetInteger("AttackID", enemy.AttackID);
    }
}
