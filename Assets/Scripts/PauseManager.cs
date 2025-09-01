using UnityEngine;


public class PauseManager : MonoBehaviour
{
    public GameObject gameplayPanel;
    public GameObject pausePanel;
    public void PauseGame() {
        Time.timeScale = 0f;
        gameplayPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void UnpauseGame() {
        Time.timeScale = 1f;
        gameplayPanel.SetActive(true );
        pausePanel.SetActive(false);
    }
}
