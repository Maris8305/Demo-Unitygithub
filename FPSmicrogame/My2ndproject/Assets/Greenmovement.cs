using UnityEngine;

public class Greenmovement : MonoBehaviour
{

    [Header("Defense Turret Movement Settings")]
    public float moveSpeed = 3f;
    public float moveDistance = 6f;
    public bool alternateColumns = true;

    private TurretData[] turrets;

    void Start()
    {
        SetupDefenseTurrets();
    }

    void SetupDefenseTurrets()
    {
        int turretCount = 0;

        // Count turrets
        foreach (Transform child in transform)
        {
            if (child.name.Contains("Defense turret mini"))
            {
                turretCount++;
            }
        }

        // Create array to save turrets
        turrets = new TurretData[turretCount];
        int turretIndex = 0;

        foreach (Transform child in transform)
        {
            if (child.name.Contains("Defense turret mini"))
            {
                turrets[turretIndex] = new TurretData();
                turrets[turretIndex].transform = child;
                turrets[turretIndex].startPosition = child.position;
                turrets[turretIndex].moveDistance = moveDistance;
                turrets[turretIndex].movingRight = true;

                // Alternate direction for each turret
                if (alternateColumns && turretIndex % 2 == 1)
                {
                    turrets[turretIndex].moveDistance = -moveDistance;
                    turrets[turretIndex].movingRight = false;
                }

                turretIndex++;
            }
        }
    }

    void Update()
    {
        if (turrets == null) return;

        // Move all turrets
        for (int i = 0; i < turrets.Length; i++)
        {
            if (turrets[i].transform == null) continue;

            MoveTurret(turrets[i]);
        }
    }

    void MoveTurret(TurretData turret)
    {
        // Estimate target position
        Vector3 targetPos;
        if (turret.movingRight)
        {
            targetPos = turret.startPosition + Vector3.right * Mathf.Abs(turret.moveDistance);
        }
        else
        {
            targetPos = turret.startPosition - Vector3.right * Mathf.Abs(turret.moveDistance);
        }

        // Move to target
        turret.transform.position = Vector3.MoveTowards(
            turret.transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );

        // Change position when reach target
        if (Vector3.Distance(turret.transform.position, targetPos) < 0.1f)
        {
            turret.movingRight = !turret.movingRight;
        }
    }

    [System.Serializable]
    public class TurretData
    {
        public Transform transform;
        public Vector3 startPosition;
        public float moveDistance;
        public bool movingRight;
    }
}

