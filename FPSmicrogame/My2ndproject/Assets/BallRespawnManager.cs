

using System.Collections;
using UnityEngine;

namespace AA0000
{
    public class BallRespawnManager : MonoBehaviour
    {
        public Spawner ballSpawner;
        public float respawnHeight = 10f;
        public float cleanupDelay = 5f; // Waiting time for old ball

        void Awake()
        {
            if (ballSpawner == null)
                ballSpawner = GetComponent<Spawner>();
        }

        public void RespawnBall()
        {
            // Delete old ball 
            StartCoroutine(CleanupAndRespawn());
        }

        IEnumerator CleanupAndRespawn()
        {
            // Delete all old balls
            foreach (GameObject ball in ballSpawner.spawnedObjects)
            {
                if (ball != null)
                    Destroy(ball);
            }
            ballSpawner.spawnedObjects.Clear();

            yield return new WaitForSeconds(0.5f);

            // Reset spawner to spawn again
            ballSpawner.amountOfSpawnedThings = 1;

            // Spawn new ball
            Vector3 spawnLocation = ballSpawner.locationProvider.GetRandomPositionInRing();
            spawnLocation.y = transform.position.y + respawnHeight;

            GameObject newBall = Instantiate(ballSpawner.spanwnedObject, spawnLocation, Quaternion.identity);
            ballSpawner.spawnedObjects.Add(newBall);
        }
    }
}
