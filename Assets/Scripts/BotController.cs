using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotController : MonoBehaviour
{
    public Transform target; // Target for the bot to follow
    public float moveSpeed = 2.0f; // Movement speed of the bot
    public float rotationSpeed = 5.0f; // Rotation speed toward the target

    void Start()
    {
        // Ensure the bot has a target set
        if (target == null)
        {
            Debug.LogWarning("BotController: No target assigned.");
        }
    }

    void Update()
    {
        if (target != null)
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
    }

}
