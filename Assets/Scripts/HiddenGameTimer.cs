using UnityEngine;

public class HiddenGameTimer : MonoBehaviour
{
    public static HiddenGameTimer Instance;

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

    public void StartTimer()
    {
        startTime = Time.time;
        isRunning = true;
    }

    public void StopTimer()
    {
        if (!isRunning) return;
        finalTime = Time.time - startTime;
        isRunning = false;
    }

    public float GetFinalTime()
    {
        return finalTime;
    }

    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(finalTime / 60f);
        int seconds = Mathf.FloorToInt(finalTime % 60f);
        return minutes.ToString("00") + ":" + seconds.ToString("00");
    }
}
