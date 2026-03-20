using UnityEngine;
public class ScatterShot : EnemyRangedAttack
{
    public override void Attack(Transform target)
    {
        _fireTimer += Time.deltaTime;
        if (_fireTimer >= fireRate)
        {
            // Get Random Direction
            Vector2 randomCircle = Random.insideUnitCircle;
            Vector3 randomPoint = new(randomCircle.x, 0f, randomCircle.y);
            randomPoint = randomPoint.normalized;

            // Get projectile from 'EnemyProjectilePool'
            EnemyProjectilePool.Instance.Get(this, projectileSpawn, randomPoint);

            _fireTimer = 0f;
        }
    }
}