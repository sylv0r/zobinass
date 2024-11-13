using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject endZone;

    private void Start()
    {
        // Ensure the endZone is enabled at the start (optional)
        if (endZone != null)
        {
            endZone.SetActive(true);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the player touched the endZone
        if (collision.gameObject == player)
        {
            SceneManager.LoadSceneAsync(0);
            // Hide the endZone object
            endZone.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player entered the endZone
        if (other.gameObject == player)
        {
            // Hide the endZone object
            endZone.SetActive(false);
        }
    }
}
