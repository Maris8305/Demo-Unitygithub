using UnityEngine;

public class Combat : MonoBehaviour
{
    
    [Header("AI Settings")]
    public float moveSpeed = 3f;
    public float attackRange = 2f;
    public float health = 50f;
    public float attackDamage = 10f;
    public float attackCooldown = 2f;

    private Transform player;
    private float lastAttackTime;

    void Start()
    {
        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > attackRange)
        {
            // Move to player
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            transform.LookAt(player);
        }
        else if (distanceToPlayer <= attackRange && Time.time > lastAttackTime + attackCooldown)
        {
            // Attack player
            AttackPlayer();
            lastAttackTime = Time.time;
        }
    }

    void AttackPlayer()
    {
        Debug.Log("Enemy attacks player for " + attackDamage + " damage!");
        // Reduce player's blood
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy died!");
        Destroy(gameObject);
    }
}



