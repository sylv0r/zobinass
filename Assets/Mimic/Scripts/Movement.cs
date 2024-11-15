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
        public float directionChangeInterval = 2.0f;
        public float obstacleDetectionRange = 1.0f;
        public float velocityLerpCoef = 4f;

        private Vector3 velocity = Vector3.zero;
        private Mimic myMimic;
        private float directionChangeTimer = 0f;

        public Transform target;

        private void Start()
        {
            target = GameObject.Find("Player(Clone)").transform;
            myMimic = GetComponent<Mimic>();
            ChooseRandomDirection();
        }

        void Update()
        {
            directionChangeTimer -= Time.deltaTime;
            if (directionChangeTimer <= 0 || IsObstacleAhead())
            {
                ChooseRandomDirection();
                directionChangeTimer = directionChangeInterval;
            }

            velocity = Vector3.Lerp(velocity, velocity.normalized * speed, velocityLerpCoef * Time.deltaTime);
            myMimic.velocity = velocity;

            transform.position = transform.position + velocity * Time.deltaTime;

            RaycastHit hit;
            Vector3 destHeight = transform.position;
            if (Physics.Raycast(transform.position + Vector3.up * 5f, -Vector3.up, out hit))
                destHeight = new Vector3(transform.position.x, hit.point.y + height, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, destHeight, velocityLerpCoef * Time.deltaTime);

                        // if bot touches the target, go to scene Loose
            if (Vector3.Distance(transform.position, target.position) < 1.0f)
            {
                SceneManager.LoadScene("Loose");
            }
        }

        bool IsObstacleAhead()
        {
            Vector3 raycastStart = transform.position + Vector3.up * 0.5f;
            RaycastHit hit;

            if (Physics.Raycast(raycastStart, velocity.normalized, out hit, obstacleDetectionRange))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    SceneManager.LoadScene("Loose");
                    return true;
                }
                return true;
            }
            return false;
        }

        void ChooseRandomDirection()
        {
            float randomAngle = Random.Range(0f, 360f);
            Vector3 randomDirection = new Vector3(Mathf.Cos(randomAngle), 0, Mathf.Sin(randomAngle));
            velocity = randomDirection * speed;
        }
    }
}
