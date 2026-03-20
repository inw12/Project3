using UnityEngine;
public abstract class EnemyRangedAttack : EnemyAttack
{
    // Attack Stats
    [SerializeField] protected float damage;
    [SerializeField] protected float fireRate;
    [SerializeField] protected float projectileSpeed;
    [SerializeField] protected float range;
    protected float _fireTimer;

    public override abstract void Attack(Transform target);

    // Stat Getters
    public float Damage() => damage;
    public float ProjectileSpeed() => projectileSpeed;
    public float Range() => range;
}
