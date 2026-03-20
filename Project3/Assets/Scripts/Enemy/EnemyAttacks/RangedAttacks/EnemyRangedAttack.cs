using UnityEngine;
public abstract class EnemyRangedAttack : EnemyAttack
{
    // Attack ID (identification for state machine and animator control)
    protected int attackID;

    // Attack Stats
    [SerializeField] protected float damage;
    [SerializeField] protected float fireRate;
    [SerializeField] protected float projectileSpeed;
    [SerializeField] protected float range;

    public override abstract void Attack();
}
