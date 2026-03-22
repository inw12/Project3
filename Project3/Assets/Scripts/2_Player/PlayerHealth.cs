using UnityEngine;
public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private CapsuleCollider hurtbox;
    [Space]
    [SerializeField] private float health = 10f;
    private float _currentHealth;
    private bool _isAlive;

    public float MaxHealth => health;
    public float CurrentHealth => _currentHealth;

    public void Initialize()
    {
        _currentHealth = health;
        _isAlive = _currentHealth > 0f;
    }

    public void IncreaseHealth(float amount)
    {
        _currentHealth += amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, health);
        
        Debug.Log("Current HP: " + CurrentHealth);
    }

    public void DecreaseHealth(float amount)
    {
        _currentHealth -= amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, health);
        _isAlive = _currentHealth > 0f;

        Debug.Log("Current HP: " + CurrentHealth);
    }

    public void Death()
    {
        throw new System.NotImplementedException();
    }
}
