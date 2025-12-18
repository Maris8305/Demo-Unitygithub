using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] waypoints; // Để Size = 0 nếu muốn đứng yên
    public float idleTime = 2f;
    public float walkSpeed = 1.4f;

    [Header("Chase & Attack Settings")]
    public float chaseSpeed = 3.5f;
    public float sightDistance = 10f;
    public float attackDistance = 2f; // Khoảng cách tấn công
    public float attackCooldown = 1.5f; // Thời gian giữa các đòn
    public int attackDamage = 10;

    [Header("Audio")]
    public AudioClip idleSound;
    public AudioClip walkingSound;
    public AudioClip chasingSound;
    public AudioClip attackSound;

    private int currentWaypointIndex = 0;
    private NavMeshAgent agent;
    private Animator animator;
    private float idleTimer = 0f;
    private float attackTimer = 0f;
    private Transform player;
    private AudioSource audioSource;
    private EnemyHealth enemyHealth;
    private bool isDead = false;

    private enum EnemyState { Idle, Walk, Chase, Attack }
    private EnemyState currentState = EnemyState.Idle;

   
   
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        enemyHealth = GetComponent<EnemyHealth>();

      
        if (animator != null)
        {
            animator.applyRootMotion = false;
        }

      
        if (agent != null)
        {
            agent.speed = walkSpeed;
            agent.autoBraking = false;
            agent.acceleration = 4;
            agent.angularSpeed = 120;
        }

        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;

            
            Collider[] enemyColliders = GetComponentsInChildren<Collider>();
            Collider[] playerColliders = player.GetComponentsInChildren<Collider>();

            foreach (Collider enemyCol in enemyColliders)
            {
                foreach (Collider playerCol in playerColliders)
                {
                    Physics.IgnoreCollision(enemyCol, playerCol);
                }
            }

            Debug.Log("Enemy and Player ignored collision!");
        }

        if (agent != null && agent.isOnNavMesh && waypoints != null && waypoints.Length > 0)
        {
            SetDestinationToWaypoint();
        }
    }

    private void Update()
    {
        // Kiểm tra chết
        if (CheckIfDead())
            return;

        // Tìm player nếu chưa có
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        if (agent == null)
            return;

        attackTimer += Time.deltaTime;

        switch (currentState)
        {
            case EnemyState.Idle:
                IdleState();
                break;

            case EnemyState.Walk:
                WalkState();
                break;

            case EnemyState.Chase:
                ChaseState();
                break;

            case EnemyState.Attack:
                AttackState();
                break;
        }
    }

    private bool CheckIfDead()
    {
        if (isDead)
            return true;

        if (enemyHealth != null && enemyHealth.GetCurrentHealth() <= 0)
        {
            OnEnemyDeath();
            return true;
        }

        return false;
    }

    private void OnEnemyDeath()
    {
        isDead = true;

        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }

        if (animator != null)
        {
            animator.SetBool("Death", true);
        }

        this.enabled = false;

        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        Debug.Log("Enemy died!");
        Destroy(gameObject, 3f);
    }

    private void IdleState()
    {
        if (agent != null)
            agent.isStopped = false;

        idleTimer += Time.deltaTime;

        if (animator != null)
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsChasing", false);
        }

        PlaySound(idleSound);

        // Chỉ patrol nếu có waypoints
        if (waypoints != null && waypoints.Length > 0 && idleTimer >= idleTime)
        {
            NextWaypoint();
        }

        CheckForPlayerDetection();
    }

    private void WalkState()
    {
        idleTimer = 0f;

        if (agent != null)
        {
            agent.isStopped = false;
            agent.speed = walkSpeed;
        }

        if (animator != null)
        {
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsChasing", false);
        }

        PlaySound(walkingSound);

        // SỬA: Check chặt chẽ hơn khi đến waypoint
        if (agent != null && !agent.pathPending)
        {
            // Nếu đã đến gần waypoint (1 đơn vị) hoặc không còn path
            if (agent.remainingDistance <= agent.stoppingDistance + 0.1f)
            {
                currentState = EnemyState.Idle;
                Debug.Log("Reached waypoint " + currentWaypointIndex);
            }
        }

        CheckForPlayerDetection();
    }

    private void ChaseState()
    {
        idleTimer = 0f;

        if (player == null)
        {
            ReturnToPatrol();
            return;
        }

        if (agent != null)
        {
            agent.isStopped = false;
            agent.speed = chaseSpeed;
        }

        float distToPlayer = Vector3.Distance(transform.position, player.position);

        // Đủ gần để tấn công
        if (distToPlayer <= attackDistance)
        {
            currentState = EnemyState.Attack;
            if (agent != null)
                agent.isStopped = true;
            return;
        }

        // Đuổi theo player
        if (agent != null)
        {
            agent.SetDestination(player.position);
        }

        if (animator != null)
        {
            animator.SetBool("IsChasing", true);
            animator.SetBool("IsWalking", false);
        }

        PlaySound(chasingSound);

        // Mất tầm nhìn
        if (!CanSeePlayer() || distToPlayer > sightDistance)
        {
            ReturnToPatrol();
        }
    }

    private void AttackState()
    {
        if (player == null)
        {
            ReturnToPatrol();
            return;
        }

        // Dừng lại để tấn công
        if (agent != null)
            agent.isStopped = true;

        // Quay mặt về player
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        float distToPlayer = Vector3.Distance(transform.position, player.position);

        // Player đi xa → quay lại Chase
        if (distToPlayer > attackDistance + 0.5f)
        {
            currentState = EnemyState.Chase;
            return;
        }

        // Thực hiện tấn công
        if (attackTimer >= attackCooldown)
        {
            PerformAttack();
            attackTimer = 0f;
        }

        if (animator != null)
        {
            animator.SetBool("IsChasing", true); // Dùng chasing animation khi attack
            animator.SetBool("IsWalking", false);
        }

        PlaySound(attackSound);

        // Mất tầm nhìn
        if (!CanSeePlayer() || distToPlayer > sightDistance)
        {
            ReturnToPatrol();
        }
    }

    private void PerformAttack()
    {
        Debug.Log("Enemy attacks player!");

        if (player != null)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }

    private void ReturnToPatrol()
    {
        if (agent != null)
            agent.isStopped = false;

        if (waypoints != null && waypoints.Length > 0)
        {
            currentState = EnemyState.Walk;
            SetDestinationToWaypoint();
        }
        else
        {
            currentState = EnemyState.Idle;
            if (agent != null)
                agent.ResetPath();
        }
    }

    private bool CanSeePlayer()
    {
        if (player == null)
            return false;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > sightDistance)
            return false;

        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 1.2f;
        Vector3 dir = (player.position + Vector3.up * 1.0f) - origin;

        if (Physics.Raycast(origin, dir.normalized, out hit, sightDistance))
        {
            return hit.collider != null && hit.collider.CompareTag("Player");
        }

        return false;
    }

    private void CheckForPlayerDetection()
    {
        if (CanSeePlayer())
        {
            currentState = EnemyState.Chase;
            Debug.Log("Player detected!");
        }
    }

    private void NextWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            currentState = EnemyState.Idle;
            if (agent != null)
                agent.ResetPath();
            return;
        }

        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        SetDestinationToWaypoint();
    }

    private void SetDestinationToWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            currentState = EnemyState.Idle;
            return;
        }

        if (agent == null || !agent.isOnNavMesh)
            return;

        // DEBUG: Hiển thị waypoint hiện tại
        Debug.Log("Moving to waypoint " + currentWaypointIndex + " at position " + waypoints[currentWaypointIndex].position);

        agent.speed = walkSpeed;
        agent.SetDestination(waypoints[currentWaypointIndex].position);
        currentState = EnemyState.Walk;
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null || audioSource == null)
            return;

        if (audioSource.clip != clip)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}