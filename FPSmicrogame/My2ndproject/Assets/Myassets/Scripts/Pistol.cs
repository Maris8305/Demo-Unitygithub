using System.Collections;
using UnityEngine;

/// <summary>
/// Complete pistol shooting system with unlimited ammo
/// Easy animation timing adjustment via Inspector
/// </summary>
public class Pistol : MonoBehaviour
{
    [Header("=== COMBAT SETTINGS ===")]
    [Tooltip("Damage dealt per bullet")]
    public int damager = 10;

    [Tooltip("Maximum shooting distance")]
    public float shootRange = 100f;

    [Tooltip("Time between each shot (seconds)")]
    public float shootCooldown = 0.5f;

    [Header("=== ANIMATION TIMING (ADJUST HERE!) ===")]
    [Tooltip("Delay before bullet fires - ADJUST THIS to sync with animation!")]
    [Range(0f, 0.5f)]
    public float fireDelay = 0.15f;

    [Tooltip("If TRUE, effects happen instantly. If FALSE, effects wait for fireDelay")]
    public bool instantEffects = false;

    [Header("=== VISUAL EFFECTS ===")]
    [Tooltip("Particle effect shown when gun fires")]
    public ParticleSystem muzzleFlash;

    [Tooltip("Light that flashes when gun fires")]
    public GameObject muzzleFlashLight;

    [Tooltip("Particle effect shown where bullet hits")]
    public ParticleSystem impactEffect;

    [Header("=== AUDIO ===")]
    [Tooltip("Sound played when shooting")]
    public AudioSource shootSound;

    [Header("=== CARTRIDGE EJECTION ===")]
    [Tooltip("Transform position where empty shell ejects from")]
    public Transform cartridgeEjectionPoint;

    [Tooltip("Empty bullet shell prefab")]
    public GameObject cartridgePrefab;

    [Tooltip("Force applied to ejected shell")]
    public float cartridgeEjectionForce = 5f;

    [Header("=== ANIMATION ===")]
    [Tooltip("Gun's Animator component")]
    public Animator gun;

    [Header("=== DEBUG INFO ===")]
    [Tooltip("Show debug UI on screen")]
    public bool showDebugUI = true;

    // ============================================
    // PRIVATE VARIABLES
    // ============================================
    private bool canShoot = true;           // Can we shoot right now?
    private float shootTimer = 0f;          // Tracks cooldown time

    // ============================================
    // UNITY LIFECYCLE METHODS
    // ============================================

    void Start()
    {
        // Initialize shooting state
        canShoot = true;
        shootTimer = 0f;

        // Make sure muzzle flash light is off at start
        if (muzzleFlashLight != null)
        {
            muzzleFlashLight.SetActive(false);
        }

        Debug.Log("Pistol initialized - Unlimited ammo mode");
    }

    void Update()
    {
        // Update shoot cooldown timer
        if (shootTimer > 0f)
        {
            shootTimer -= Time.deltaTime;
        }

        // Check for shooting input
        // Using GetButton (not GetButtonDown) allows holding mouse to shoot continuously
        if (Input.GetButton("Fire1") && canShoot && shootTimer <= 0f)
        {
            Shoot();
        }
    }

    // ============================================
    // SHOOTING SYSTEM
    // ============================================

    /// <summary>
    /// Main shooting function - called when player presses Fire button
    /// </summary>
    void Shoot()
    {
        // Prevent shooting again until cooldown finishes
        canShoot = false;

        // Start animation immediately
        if (gun != null)
        {
            gun.SetTrigger("shoot");
            Debug.Log("Shoot animation triggered");
        }

        // Choose between instant or delayed effects
        if (instantEffects)
        {
            // Fire immediately without waiting for animation
            FireBullet();
        }
        else
        {
            // Wait for animation to reach firing point
            StartCoroutine(DelayedFire());
        }

        // Set cooldown timer
        shootTimer = shootCooldown;

        // Re-enable shooting after cooldown
        StartCoroutine(ResetShootFlag());
    }

    /// <summary>
    /// Coroutine to delay bullet firing until animation reaches the right moment
    /// </summary>
    IEnumerator DelayedFire()
    {
        // Wait for the specified delay time
        // ADJUST fireDelay in Inspector to sync with your animation!
        yield return new WaitForSeconds(fireDelay);

        // Now fire the bullet
        FireBullet();
    }

    /// <summary>
    /// Actually fires the bullet and plays all effects
    /// This is where the "shooting" actually happens
    /// </summary>
    void FireBullet()
    {
        // ===== AUDIO =====
        if (shootSound != null)
        {
            shootSound.Play();
        }

        // ===== MUZZLE FLASH PARTICLE =====
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        // ===== MUZZLE FLASH LIGHT =====
        if (muzzleFlashLight != null)
        {
            muzzleFlashLight.SetActive(true);
            StartCoroutine(DisableMuzzleFlashLight());
        }

        // ===== RAYCAST SHOOTING =====
        // Create a ray from center of camera view
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        // Check if ray hits anything within shoot range
        if (Physics.Raycast(ray, out hit, shootRange))
        {
            Debug.Log($"[Pistol] Hit: {hit.collider.name} at distance {hit.distance:F2}m");

            // ===== DAMAGE ENEMY =====
            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damager);
                    Debug.Log($"[Pistol] Dealt {damager} damage to {hit.collider.name}");
                }
                else
                {
                    Debug.LogWarning($"[Pistol] {hit.collider.name} has 'Enemy' tag but no EnemyHealth component!");
                }
            }

            // ===== IMPACT EFFECT =====
            if (impactEffect != null)
            {
                // Spawn impact particle at hit point, facing hit surface
                Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }

            // Draw debug line to show bullet path (visible in Scene view)
            Debug.DrawLine(ray.origin, hit.point, Color.red, 1f);
        }
        else
        {
            // Didn't hit anything
            Debug.DrawRay(ray.origin, ray.direction * shootRange, Color.yellow, 1f);
        }

        // ===== EJECT EMPTY CARTRIDGE =====
        EjectCartridge();
    }

    /// <summary>
    /// Ejects empty bullet shell from gun
    /// </summary>
    void EjectCartridge()
    {
        if (cartridgePrefab != null && cartridgeEjectionPoint != null)
        {
            // Spawn cartridge at ejection point
            GameObject cartridge = Instantiate(
                cartridgePrefab,
                cartridgeEjectionPoint.position,
                cartridgeEjectionPoint.rotation
            );

            // Add force to make it fly out
            Rigidbody rb = cartridge.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Eject to the right with some upward angle
                Vector3 ejectionDirection = cartridgeEjectionPoint.right;
                rb.AddForce(ejectionDirection * cartridgeEjectionForce, ForceMode.Impulse);

                // Add random spin for realism
                rb.AddTorque(Random.insideUnitSphere * 10f, ForceMode.Impulse);
            }

            // Auto-destroy after 5 seconds to prevent clutter
            Destroy(cartridge, 5f);
        }
    }

    // ============================================
    // COROUTINES
    // ============================================

    /// <summary>
    /// Re-enables shooting after cooldown period
    /// </summary>
    IEnumerator ResetShootFlag()
    {
        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;
    }

    /// <summary>
    /// Disables muzzle flash light after brief moment
    /// </summary>
    IEnumerator DisableMuzzleFlashLight()
    {
        yield return new WaitForSeconds(0.1f);

        if (muzzleFlashLight != null)
        {
            muzzleFlashLight.SetActive(false);
        }
    }

    // ============================================
    // DEBUG UI
    // ============================================

    /// <summary>
    /// Shows debug information on screen
    /// </summary>
    void OnGUI()
    {
        if (!showDebugUI) return;

        // Setup GUI style
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 14;
        style.normal.textColor = Color.white;

        // Background box
        GUI.Box(new Rect(10, 10, 400, 100), "");

        // Display shooting status
        GUI.Label(new Rect(20, 15, 380, 25),
            $"<b>PISTOL STATUS</b>", style);

        GUI.Label(new Rect(20, 35, 380, 25),
            $"Can Shoot: {(canShoot ? "<color=green>YES</color>" : "<color=red>NO</color>")}", style);

        GUI.Label(new Rect(20, 55, 380, 25),
            $"Cooldown: {shootTimer:F2}s", style);

        GUI.Label(new Rect(20, 75, 380, 25),
            $"Fire Delay: {fireDelay:F2}s {(instantEffects ? "(INSTANT MODE)" : "")}", style);

        // Instructions
        style.fontSize = 12;
        style.normal.textColor = Color.yellow;
        GUI.Label(new Rect(20, 115, 500, 25),
            "Hold LEFT MOUSE to shoot | Adjust 'Fire Delay' in Inspector to sync animation", style);
    }

  
    void OnDrawGizmosSelected()
    {
        if (Camera.main != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootRange);
        }
    }
}