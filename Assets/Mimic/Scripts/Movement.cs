using UnityEngine;
using UnityEngine.SceneManagement;

namespace MimicSpace
{
    public class RandomMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [Tooltip("Body Height from ground")]
        [Range(0.5f, 5f)]
        public float height = 0.8f;
        public float speed = 5f;
        public float directionChangeInterval = 2.0f; // Interval to change direction
        public float obstacleDetectionRange = 1.0f;  // Range to detect walls
        public float velocityLerpCoef = 4f;

        private Vector3 velocity = Vector3.zero;
        private Mimic myMimic;
        private float directionChangeTimer = 0f;

        private void Start()
        {
            myMimic = GetComponent<Mimic>();
            ChooseRandomDirection();
        }

        void Update()
        {
            // Update timer to periodically change direction
            directionChangeTimer -= Time.deltaTime;
            if (directionChangeTimer <= 0 || IsObstacleAhead())
            {
                ChooseRandomDirection();
                directionChangeTimer = directionChangeInterval;
            }

            // Move in the current velocity direction
            velocity = Vector3.Lerp(velocity, velocity.normalized * speed, velocityLerpCoef * Time.deltaTime);
            myMimic.velocity = velocity;

            transform.position = transform.position + velocity * Time.deltaTime;

            // Adjust height to follow ground surface
            RaycastHit hit;
            Vector3 destHeight = transform.position;
            if (Physics.Raycast(transform.position + Vector3.up * 5f, -Vector3.up, out hit))
                destHeight = new Vector3(transform.position.x, hit.point.y + height, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, destHeight, velocityLerpCoef * Time.deltaTime);
        }

        // Check if an obstacle is directly ahead and handle Player collision
        bool IsObstacleAhead()
        {
            Vector3 raycastStart = transform.position + Vector3.up * 0.5f; // Start raycast slightly above ground
            RaycastHit hit;

            // Perform the raycast to detect obstacles or the Player
            if (Physics.Raycast(raycastStart, velocity.normalized, out hit, obstacleDetectionRange))
            {
                // Check if the detected object is tagged as "Player"
                if (hit.collider.CompareTag("Player"))
                {
                    SceneManager.LoadScene("Loose"); // Load "Loose" scene if the Player is detected
                    return true; // Consider this an obstacle so it stops moving
                }
                return true; // Obstacle detected
            }
            return false; // No obstacle detected
        }

        // Choose a new random direction for movement
        void ChooseRandomDirection()
        {
            // Generate a random direction in the XZ plane
            float randomAngle = Random.Range(0f, 360f);
            Vector3 randomDirection = new Vector3(Mathf.Cos(randomAngle), 0, Mathf.Sin(randomAngle));
            velocity = randomDirection * speed;
        }
    }
}
