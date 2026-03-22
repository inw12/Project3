using UnityEngine;
public abstract class EnemyRangedAttack : EnemyAttack
{
    // Attack Stats
    [Space]
    [SerializeField] protected float damage;
    [SerializeField] protected float fireRate;
    [SerializeField] protected float projectileSpeed;
    [SerializeField] protected float range;
    [Space]
    [SerializeField] protected Transform projectileSpawn;
    [SerializeField] protected ProjectilePool projectilePool;
    protected float _fireTimer;

    public override abstract void Attack(Transform target);

    // Stat Getters
    public float Damage() => damage;
    public float ProjectileSpeed() => projectileSpeed;
    public float Range() => range;
}