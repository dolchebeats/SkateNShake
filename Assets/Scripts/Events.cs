using UnityEngine.SceneManagement;
using UnityEngine;

public class Events : MonoBehaviour
{
    public GameObject gameOverPanel;
    public void ReplayGame() {
        SceneManager.LoadScene("Level");
        Time.timeScale = 1.0f;
        gameOverPanel.SetActive(false);
    }

    public void QuitToMenu() { 
        Application.Quit();
    }
}
