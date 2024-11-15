using UnityEngine;
using UnityEngine.SceneManagement; 

public class Menu : MonoBehaviour
{

    [SerializeField] private string scene;

    public void StartGame()
    {
        SceneManager.LoadScene(scene);
    }
    
    public void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}