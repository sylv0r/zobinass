using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SuvBotController : MonoBehaviour
{
    public Transform player;

    public float moveSpeed = 2.0f;
    public float rotationSpeed = 5.0f;
    public float stopDuration = 5.0f;
    public float collisionBackoffDistance = 0.1f;
    public AudioSource endSound;

    private Vector3 movementDirection;
    private bool isStopped = false;


    public AudioSource accelerationSound;

    void Start()
    {
        endSound.enabled = false;
        accelerationSound.enabled = false;
        player = GameObject.Find("Player(Clone)").transform;
        movementDirection = transform.forward;

    }

    void Update()
    {
        if (!isStopped)
        {
            accelerationSound.enabled = true;
            transform.Translate(movementDirection * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    IEnumerator HandleEnd()
    {
        endSound.enabled = true;
        player.LookAt(transform);
        player.GetComponent<PlayerController>().enabled = false;
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("Loose");
    }

    void OnCollisionEnter(Collision collision)
    {
        accelerationSound.enabled = false;
        
        if (collision.gameObject.CompareTag("Wall"))
        {
            StartCoroutine(StopAndChooseNewDirection());
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(HandleEnd());
        }
    }

    IEnumerator StopAndChooseNewDirection()
    {
        isStopped = true;
        yield return new WaitForSeconds(stopDuration);
        ChooseNewDirection();
        isStopped = false;
    }

    void ChooseNewDirection()
    {
        movementDirection = Quaternion.Euler(0, 30, 0) * movementDirection;
        transform.rotation = Quaternion.LookRotation(movementDirection);
        if (IsObstacleAhead())
        {
            movementDirection = Quaternion.Euler(0, -30, 0) * movementDirection;
            transform.rotation = Quaternion.LookRotation(movementDirection);
        }

        if (IsObstacleAhead())
        {
            movementDirection = Quaternion.Euler(0, -60, 0) * movementDirection;
            transform.rotation = Quaternion.LookRotation(movementDirection);
        }
    }

    bool IsObstacleAhead()
    {
        Vector3 raycastStart = transform.position + transform.forward * 0.5f;
        Debug.DrawRay(raycastStart, movementDirection * 0.5f, Color.red);
        return Physics.Raycast(raycastStart, movementDirection, 0.5f);
    }
}
