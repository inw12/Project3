using UnityEngine;
public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float health;
    private float _currentHealth;
    private bool _isAlive;

    void Start()
    {
        _currentHealth = health;
        _isAlive = _currentHealth > 0f;
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
    }
}
