using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public Transform[] waypoints;
    public float idleTime = 2f;
    public float walkSpeed = 2f; // Walking speed.
    public float chaseSpeed = 4f; // Chasing speed.
    public float sightDistance = 10f;
    public AudioClip idleSound;
    public AudioClip walkingSound;
    public AudioClip chasingSound;

    private int currentWaypointIndex = 0;
    private NavMeshAgent agent;
    private Animator animator;
    private float idleTimer = 0f;
    private Transform player;
    private AudioSource audioSource;

    private enum EnemyState { Idle, Walk, Chase }
    private EnemyState currentState = EnemyState.Idle;

    private bool isChasingAnimation = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // Try to find player, but don't assume it's found immediately
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;

        // Ensure agent exists and is on the navmesh before setting destinations
        if (agent != null && agent.isOnNavMesh)
            SetDestinationToWaypoint();
        else if (agent == null)
            Debug.LogWarning("EnemyController: NavMeshAgent component missing.");
    }

    private void Update()
    {
        // Try to recover player reference if still null (in case player is created at runtime)
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
        }

        if (agent == null)
            return;

        switch (currentState)
        {
            case EnemyState.Idle:
                idleTimer += Time.deltaTime;
                if (animator) { animator.SetBool("IsWalking", false); animator.SetBool("IsChasing", false); }
                PlaySound(idleSound);

                if (idleTimer >= idleTime)
                {
                    NextWaypoint();
                }

                CheckForPlayerDetection();
                break;

            case EnemyState.Walk:
                idleTimer = 0f;
                if (animator) { animator.SetBool("IsWalking", true); animator.SetBool("IsChasing", false); }
                PlaySound(walkingSound);

                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    currentState = EnemyState.Idle;
                }

                CheckForPlayerDetection();
                break;

            case EnemyState.Chase:
                idleTimer = 0f;
                agent.speed = chaseSpeed; // Set the chase speed.

                if (player != null)
                    agent.SetDestination(player.position);

                isChasingAnimation = true;
                if (animator) animator.SetBool("IsChasing", true);

                PlaySound(chasingSound);

                if (player != null)
                {
                    if (Vector3.Distance(transform.position, player.position) > sightDistance)
                    {
                        currentState = EnemyState.Walk;
                        agent.speed = walkSpeed; // Restore walking speed.
                    }
                }
                else
                {
                    // If we lost the player transform, fall back to walk or idle
                    currentState = (waypoints != null && waypoints.Length > 0) ? EnemyState.Walk : EnemyState.Idle;
                }
                break;
        }
    }

    private void CheckForPlayerDetection()
    {
        if (player == null)
            return;

        // First check distance to avoid unnecessary raycasts
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > sightDistance)
            return;

        // Raycast from slightly above the enemy's pivot to avoid hitting the floor
        Vector3 origin = transform.position + Vector3.up * 1.2f;
        Vector3 dir = (player.position + Vector3.up * 1.0f) - origin;

        RaycastHit hit;
        if (Physics.Raycast(origin, dir.normalized, out hit, sightDistance))
        {
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                currentState = EnemyState.Chase;
                Debug.Log("Player detected!");
            }
        }
    }

    private void PlaySound(AudioClip soundClip)
    {
        if (audioSource == null || soundClip == null)
            return;

        if (!audioSource.isPlaying || audioSource.clip != soundClip)
        {
            audioSource.clip = soundClip;
            audioSource.Play();
        }
    }

    private void NextWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            // No waypoints: go to idle
            currentState = EnemyState.Idle;
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

        if (agent == null)
            return;

        agent.SetDestination(waypoints[currentWaypointIndex].position);
        currentState = EnemyState.Walk;
        agent.speed = walkSpeed; // Set the walking speed.
        if (animator) animator.enabled = true;
    }

    // Draw a green raycast line at all times and switch to red when the player is detected.
    private void OnDrawGizmos()
    {
        // Safety: OnDrawGizmos runs in editor before Start, so guard player against null
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
        }

        if (player == null)
            return;

        Gizmos.color = currentState == EnemyState.Chase ? Color.red : Color.green;

        // draw from slightly above enemy to player's centre
        Vector3 from = transform.position + Vector3.up * 1.2f;
        Vector3 to = player.position + Vector3.up * 1.0f;
        Gizmos.DrawLine(from, to);
    }
}