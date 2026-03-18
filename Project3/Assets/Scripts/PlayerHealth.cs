using UnityEngine;
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private CapsuleCollider hurtbox;
    [SerializeField] private float health = 10f;
    private float _currentHealth;
    private bool _isAlive;

    public void Initialize()
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
