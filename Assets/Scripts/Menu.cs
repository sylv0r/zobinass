using UnityEngine;
using UnityEngine.SceneManagement; 

public class Menu : MonoBehaviour
{

    [SerializeField] private string scene;

    public void StartGame()
    {
        SceneManager.LoadScene(scene);
    }
}
