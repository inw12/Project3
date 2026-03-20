using UnityEngine;
public class ScatterShot : EnemyRangedAttack
{
    public override void Attack(Transform target)
    {
        _fireTimer += Time.deltaTime;
        if (_fireTimer >= fireRate)
        {
            // Get projectile from 'EnemyProjectilePool'
            EnemyProjectilePool.Instance.Get(this, transform, target);

            _fireTimer = 0f;
        }
    }
}