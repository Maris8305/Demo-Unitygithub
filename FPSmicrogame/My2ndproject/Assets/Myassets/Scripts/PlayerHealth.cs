using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("UI")]
    public HealthBar healthBar;
    public DamageEffect damageEffect;

    [Header("Death Settings")]
    public string deathSceneName = "DeathScene"; 
    public float deathDelay = 2.5f;

    private void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(currentHealth);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(currentHealth, 0);
        Debug.Log("Player took " + damageAmount + " damage! Current health: " + currentHealth);

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        if (damageEffect != null && currentHealth > 0)
        {
            damageEffect.ShowDamageEffect();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        Debug.Log("Player healed " + healAmount + ". Current health: " + currentHealth);

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    private void Die()
    {
        Debug.Log("Player died!");
        if (damageEffect != null)
        {
            damageEffect.ShowDeathEffect();
        }
        Invoke("LoadGameOverScene", deathDelay);
    }

    private void LoadGameOverScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(deathSceneName); 
    }
}