using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SuvBotController : MonoBehaviour
{
    public Transform player;


    public float moveSpeed = 2.0f;           // Speed of the bot
    public float rotationSpeed = 5.0f;       // Rotation speed for smooth turning
    public float stopDuration = 5.0f;        // How long to stop when obstacle is detected
    public float collisionBackoffDistance = 0.1f; // Distance to move back after hitting a wall

    private Vector3 movementDirection;       // Current movement direction
    private bool isStopped = false;          // Flag to indicate if the car is stopped

    void Start()
    {
        player = GameObject.Find("Player").transform;
        // Initialize with a forward direction
        movementDirection = transform.forward;
    }

    void Update()
    {
        if (!isStopped)
        {
            // Move the car forward
            transform.Translate(movementDirection * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the car has collided with a wall
        if (collision.gameObject.CompareTag("Wall"))
        {
            // Move the car slightly backward to prevent staying within the wall
            transform.position -= movementDirection * collisionBackoffDistance;

            // Log collision for debugging
            Debug.Log("Collision detected with wall!");

            // Stop and wait, then choose a new direction
            StartCoroutine(StopAndChooseNewDirection());
        }

        // Check if the car has collided with the player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Restart the scene when the player is hit
            SceneManager.LoadScene("Loose");
        }
    }

    IEnumerator StopAndChooseNewDirection()
    {
        // Set isStopped to true to prevent movement
        isStopped = true;

        // Wait for the specified stop duration
        yield return new WaitForSeconds(stopDuration);

        // Choose a new direction after stopping
        ChooseNewDirection();

        // Resume movement by setting isStopped back to false
        isStopped = false;
    }

    void ChooseNewDirection()
    {
        // Try to rotate 90 degrees to the right as a starting direction change
        movementDirection = Quaternion.Euler(0, 90, 0) * movementDirection;
        transform.rotation = Quaternion.LookRotation(movementDirection);

        // If there's still an obstacle immediately after turning, try alternative directions
        if (IsObstacleAhead())
        {
            movementDirection = Quaternion.Euler(0, -180, 0) * movementDirection;
            transform.rotation = Quaternion.LookRotation(movementDirection);
        }

        if (IsObstacleAhead())
        {
            movementDirection = Quaternion.Euler(0, -90, 0) * movementDirection;
            transform.rotation = Quaternion.LookRotation(movementDirection);
        }
    }

    bool IsObstacleAhead()
    {
        // Short raycast to detect walls just in front of the car
        Vector3 raycastStart = transform.position + transform.forward * 0.5f;
        Debug.DrawRay(raycastStart, movementDirection * 0.5f, Color.red);

        // Perform a raycast from a slightly offset position to check for obstacles
        return Physics.Raycast(raycastStart, movementDirection, 0.5f);
    }
}
