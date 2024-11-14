using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 


public class RandomBotController : MonoBehaviour
{
    public Transform target; // Target for the bot to follow

    public float moveSpeed = 2.0f; // Speed of the bot
    public float directionChangeInterval = 2.0f; // How often the bot changes direction
    public float detectionRange = 1.0f; // Range for wall detection
    public float rotationSpeed = 5.0f; // Rotation speed toward the target
    private float lookTimer = 0.0f;


    private Vector3 movementDirection; // Current movement direction
    private float directionChangeTimer; // Timer to control direction change

    void Start()
    {
        target = GameObject.Find("Player").transform;
        ChooseNewDirection(); // Initialize with a random direction
    }

    void Update()
    {
        // Move in the current direction
        if (Vector3.Distance(transform.position, target.position) > 5.0f) {
            MoveInDirection();
        }

        // Count down the timer and choose a new direction when it reaches zero

        directionChangeTimer -= Time.deltaTime;
        if (directionChangeTimer <= 0 && Vector3.Distance(transform.position, target.position) > 5.0f)
        {
            ChooseNewDirection();
        }

        // If target is nearby, move towards it
        if (target != null && Vector3.Distance(transform.position, target.position) < 5.0f)
        {
            MoveTowardsTarget();
        }


    }


    void MoveTowardsTarget()
    {
        // Calculate the direction vector from the bot to the target
        Vector3 direction = (target.position - transform.position).normalized;
        

        // Ensure the direction vector is not zero
        if (direction != Vector3.zero)
        {
            // Move the bot forward in the direction of the target
            transform.position += direction * moveSpeed * Time.deltaTime;

            // Smoothly rotate the bot to face the target
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // if bot touches the target, go to scene Loose
        if (Vector3.Distance(transform.position, target.position) < 1.0f)
        {
            SceneManager.LoadScene("Loose");
        }

        // Check if the player is looking at the bot
        if (IsPlayerLookingAtBot())
        {
            // Increment the timer
            lookTimer += Time.deltaTime;

            // If the player has been looking at the bot for more than 10 seconds, destroy the bot
            if (lookTimer > 10.0f)
            {
            Destroy(gameObject);
            }
        }
        else
        {
            // Reset the timer if the player is not looking at the bot
            lookTimer = 0.0f;
        }
    }


    bool IsPlayerLookingAtBot()
    {
        Vector3 directionToBot = (transform.position - target.position).normalized;
        float dotProduct = Vector3.Dot(target.forward, directionToBot);

        // Check if the dot product is greater than a threshold (e.g., 0.9 for a narrow field of view)
        return dotProduct > 0.9f;
    }
    

    void MoveInDirection()
    {
        // Check for walls in the direction the bot is moving
        if (!IsObstacleAhead())
        {
            // Move the bot in the current direction
            transform.position += movementDirection * moveSpeed * Time.deltaTime;
        }
        else
        {
            // If there's an obstacle, choose a new direction immediately
            ChooseNewDirection();
        }
    }

    bool IsObstacleAhead()
    {
        // Cast a ray forward to detect walls
        return Physics.Raycast(transform.position, movementDirection, detectionRange);
    }

    void ChooseNewDirection()
    {
        // Reset the timer
        directionChangeTimer = directionChangeInterval;

        // Choose a new random direction in the XZ plane (ignoring Y for flat ground movement)
        float randomAngle = Random.Range(0f, 360f);
        movementDirection = new Vector3(Mathf.Cos(randomAngle), 0, Mathf.Sin(randomAngle)).normalized;
        
        // Rotate the bot to face the new direction
        transform.rotation = Quaternion.LookRotation(movementDirection);
    }
}

