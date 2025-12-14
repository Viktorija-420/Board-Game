using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject uiObjects;   // UIObjects parent
    [SerializeField] private GameObject pausePanel;  // PausePanel

    private bool isPaused = false;

    void Start()
    {
        pausePanel.SetActive(false);
    }

    // Called by PAUSE button
    public void PauseGame()
    {
        if (isPaused) return;

        isPaused = true;
        uiObjects.SetActive(false);
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // Optional: call this from Resume button
    public void ResumeGame()
    {
        if (!isPaused) return;

        isPaused = false;
        pausePanel.SetActive(false);
        uiObjects.SetActive(true);
        Time.timeScale = 1f;
    }
}
