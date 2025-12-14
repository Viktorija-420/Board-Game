using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance;

    [Header("Gameplay UI Root")]
    [SerializeField] private GameObject gameplayUIRoot; // Parent of ALL gameplay UI

    [Header("Timer UI")]
    [SerializeField] private Text timerText;

    private float startTime;
    private float finalTime;
    private bool isRunning = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        startTime = Time.time;
        isRunning = true;

        // Enable gameplay UI at start
        if (gameplayUIRoot != null)
            gameplayUIRoot.SetActive(true);
    }

    void Update()
    {
        if (!isRunning) return;

        float elapsedTime = Time.time - startTime;

        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);

        timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    // ⏱ Stops timer, disables ALL gameplay UI, saves final time
    public void StopTimer()
    {
        if (!isRunning) return;

        finalTime = Time.time - startTime;
        isRunning = false;

        // Disable ALL gameplay UI
        if (gameplayUIRoot != null)
            gameplayUIRoot.SetActive(false);
    }

    // ⏱ Used by EndGameManager
    public string GetFormattedFinalTime()
    {
        int minutes = Mathf.FloorToInt(finalTime / 60f);
        int seconds = Mathf.FloorToInt(finalTime % 60f);

        return minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    public void ResetTimer()
    {
        startTime = Time.time;
        finalTime = 0f;
        isRunning = true;
    }

}
