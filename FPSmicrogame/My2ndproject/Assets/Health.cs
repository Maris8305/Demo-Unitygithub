using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    
    public event Action<float, GameObject> OnDamaged;
    public event Action OnDie;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount, GameObject damageSource)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Remaining: {currentHealth}");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            OnDie?.Invoke();  
        }
        else
        {
            OnDamaged?.Invoke(amount, damageSource);
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }
}

