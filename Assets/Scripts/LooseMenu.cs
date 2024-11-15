using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 


public class LooseMenu : MonoBehaviour
{

    [SerializeField] private string game;
    [SerializeField] private string menu;


    public void StartGame()
    {
        SceneManager.LoadScene(game);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(menu);
    }

    public void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
