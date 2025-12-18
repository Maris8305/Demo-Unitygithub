using UnityEngine;

public class KillPlayer : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damageAmount = 10;
    public float damageInterval = 1f;

    private float lastDamageTime;
    private PlayerHealth playerHealth;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
                lastDamageTime = Time.time;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && playerHealth != null)
        {
            if (Time.time >= lastDamageTime + damageInterval)
            {
                playerHealth.TakeDamage(damageAmount);
                lastDamageTime = Time.time;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerHealth = null;
        }
    }
}