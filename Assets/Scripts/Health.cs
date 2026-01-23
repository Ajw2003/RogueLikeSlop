using UnityEngine;
using System;
using Interfaces;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    private float _currentHealth;

    public event Action OnDeath;
    public event Action<float> OnHealthChanged;
    public event Action<float> OnDamageTaken;

    public float CurrentHealth => _currentHealth;
    public float MaxHealth => maxHealth;

    private void Awake()
    {
        _currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (_currentHealth <= 0) return;

        _currentHealth -= amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth);
        
        OnHealthChanged?.Invoke(_currentHealth);
        OnDamageTaken?.Invoke(amount);

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (_currentHealth > 0)
        {
            _currentHealth = 0;
            OnHealthChanged?.Invoke(_currentHealth);
        }
        OnDeath?.Invoke();
    }

    public void Heal(float amount)
    {
        _currentHealth += amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(_currentHealth);
    }

    public void ResetHealth()
    {
        _currentHealth = maxHealth;
        OnHealthChanged?.Invoke(_currentHealth);
    }
}