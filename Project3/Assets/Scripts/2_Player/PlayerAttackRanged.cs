using UnityEngine;
public class PlayerAttackRanged : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float damage = 1f;
    [SerializeField] private float fireRate = 0.24f;
    [SerializeField] private float projectileSpeed = 25f;
    [SerializeField] private float projectileRange = 50f;
    private float _fireTimer;
    private Vector3 _projectileDirection;

    [Header("Components")]
    [SerializeField] private ProjectilePool projectilePool;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform projectileSpawn;
}
