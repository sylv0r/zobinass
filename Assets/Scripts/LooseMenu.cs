using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 


public class LooseMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Bot");
    }

    public void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
