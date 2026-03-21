/// * Objects that:
///     - Have a HP value
///     - HP value can increase/decrease
///     - Have a "death effect" once HP reaches 0
public interface IDamageable
{
    float MaxHealth { get; }
    float CurrentHealth { get; }

    void DecreaseHealth(float amount);
    void IncreaseHealth(float amount);
    void Death();
}
