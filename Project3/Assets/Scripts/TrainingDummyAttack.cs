using UnityEngine;
public class TrainingDummyAttack : MonoBehaviour
{
    [SerializeField] private float damage = 1f;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float range = 15f;
    private float _fireTimer;
    [Space]
    [SerializeField] private GameObject projectile;

    void Start()
    {
        _fireTimer = 0f;
    }

    void Update()
    {
        _fireTimer += Time.deltaTime;
    }
}
