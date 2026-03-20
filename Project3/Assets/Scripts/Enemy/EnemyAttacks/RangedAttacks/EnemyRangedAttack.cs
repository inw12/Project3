using UnityEngine;
public abstract class EnemyRangedAttack : EnemyAttack
{
    // Attack Stats
    [SerializeField] protected float damage;
    [SerializeField] protected float fireRate;
    [SerializeField] protected float projectileSpeed;
    [SerializeField] protected float range;

    public override abstract void Attack(Transform target);
}
