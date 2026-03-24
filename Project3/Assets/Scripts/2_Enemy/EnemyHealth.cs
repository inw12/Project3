using UnityEngine;
[RequireComponent(typeof(CapsuleCollider))]
public class EnemyHealth : MonoBehaviour, IDamageable
{
    private float _maxHealth;
    private float _currentHealth;
    private CapsuleCollider _hurtbox;

    public float MaxHealth => _maxHealth;
    public float CurrentHealth => _currentHealth;

    public void Initialize(float health)
    {
        _hurtbox = GetComponent<CapsuleCollider>();

        _maxHealth = health;
        _currentHealth = _maxHealth;
    }

    public void DecreaseHealth(float amount)
    {
        _currentHealth -= amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, _maxHealth);
        
        Debug.Log("Enemy HP: " + CurrentHealth + "/" + MaxHealth);
    }
    public void IncreaseHealth(float amount)
    {
        _currentHealth += amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, _maxHealth);
        
        Debug.Log("Enemy HP: " + CurrentHealth + "/" + MaxHealth);
    }
    public void Death() {}
}
