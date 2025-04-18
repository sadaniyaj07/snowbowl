using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public void RestartGame()
    {
        Time.timeScale = 1f; // Unpause the game, if paused
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene
    }
}
