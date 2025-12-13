using UnityEngine;

public class EndGameManager : MonoBehaviour
{
    public static EndGameManager Instance;

    [SerializeField] private GameObject endScreenPanel;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        if (endScreenPanel != null)
            endScreenPanel.SetActive(false);
    }

    public void ShowEndScreen()
    {
        if (endScreenPanel != null)
        {
            endScreenPanel.SetActive(true);
            Time.timeScale = 0f; // optional: pause game
        }
    }
}
