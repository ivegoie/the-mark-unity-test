using System;

public class HealthSystem
{
    public event EventHandler OnHealthChanged;

    private int maxHealth;
    private int currentHealth;
    private float regenRate;

    public HealthSystem(int maxHealth, float regenRate = 2f)
    {
        this.maxHealth = maxHealth;
        this.currentHealth = maxHealth;
        this.regenRate = regenRate;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        OnHealthChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        OnHealthChanged?.Invoke(this, EventArgs.Empty);
    }

    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth = maxHealth;

        OnHealthChanged?.Invoke(this, EventArgs.Empty);
    }

    public void IncreaseRegenRate(float amount)
    {
        regenRate += amount;
    }

    public float GetHealthNormalized()
    {
        return (float)currentHealth / maxHealth;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetRegenRate()
    {
        return regenRate;
    }
}
