using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance;

    [SerializeField] private Text timerText; // UI Text shown during gameplay

    private float startTime;
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
        timerText.gameObject.SetActive(true);
    }

    void Update()
    {
        if (!isRunning) return;

        float elapsedTime = Time.time - startTime;

        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);

        timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    public float StopTimer()
    {
        isRunning = false;
        timerText.gameObject.SetActive(false);
        return Time.time - startTime;
    }
}
