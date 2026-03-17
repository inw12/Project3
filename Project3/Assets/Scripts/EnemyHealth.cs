using UnityEngine;
public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float health;
    [Space]
    [SerializeField] private EnemyDummyHitFeedback hitFeedback;
    private float _currentHealth;
    private bool _isAlive;

    void Start()
    {
        _currentHealth = health;
        _isAlive = _currentHealth > 0f;

        hitFeedback.Initialize();
    }

    void Update()
    {
        hitFeedback.UpdateEnemyModel(_isAlive);
    }

    public void IncreaseHealth(float amount)
    {
        _currentHealth += amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, health);
    }
    public void DecreaseHealth(float amount)
    {
        _currentHealth -= amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, health);
        _isAlive = _currentHealth > 0f;
        hitFeedback.TriggerHitFeedback();
    }
    // 
    public void DecreaseHealth(float amount, Vector3 hit, Vector3 normal)
    {
        _currentHealth -= amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, health);
        _isAlive = _currentHealth > 0f;
    }
}
