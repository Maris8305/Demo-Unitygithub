using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    private Animator animator;
    private UnityEngine.AI.NavMeshAgent agent;
    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return; 

        currentHealth -= damageAmount;
        Debug.Log($"Enemy took {damageAmount} damage. Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

   
    public bool IsDead()
    {
        return isDead;
    }

    private void Die()
    {
        if (isDead) return; 

        isDead = true;
        Debug.Log("Enemy died!");

      
        if (animator != null)
        {
            animator.SetBool("Death", true);
        }

     
        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            agent.enabled = false;
        }

        
        EnemyController controller = GetComponent<EnemyController>();
        if (controller != null)
        {
            controller.enabled = false;
        }

      
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.enabled = false;
        }

       
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        
        Destroy(gameObject, 3f);
    }
}