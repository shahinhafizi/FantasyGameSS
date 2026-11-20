#nullable enable
using System;

public class HealthLogic
{
    public int MaxHealth { get; }
    public int CurrentHealth { get; private set; }
    public bool IsDead => CurrentHealth <= 0;

    public event Action<int, int>? HealthChanged;
    public event Action? Died;

    public HealthLogic(int maxHealth)
    {
        if (maxHealth <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxHealth),
                "MaxHealth must be greater than zero.");
        }

        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Damage amount cannot be negative.");
        }

        if (IsDead || amount == 0)
        {
            return;
        }

        CurrentHealth = Math.Max(0, CurrentHealth - amount);
        HealthChanged?.Invoke(CurrentHealth, MaxHealth);

        if (IsDead)
        {
            Died?.Invoke();
        }
    }
}