using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EndGameManager : MonoBehaviour
{
    public static EndGameManager Instance;

    [SerializeField] private GameObject endScreenPanel;
    [SerializeField] private Text timeText; // Legacy UI Text

    [Header("Camera Transition")]
    [SerializeField] private float cameraTransitionDuration = 1.5f;

    private Camera mainCamera;

    // Camera target
    private readonly Vector3 endPosition = new Vector3(0f, 11.3f, -1.12f);
    private readonly Quaternion endRotation = Quaternion.Euler(12.82f, 0f, 0f);

    // ⏱ TIMER START
    private float startTime;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        mainCamera = Camera.main;

        startTime = Time.timeSinceLevelLoad; // ⏱ START TIMER

        endScreenPanel.SetActive(false);
    }

    public void ShowEndScreen()
    {
        StartCoroutine(CameraTransition());
    }

    private IEnumerator CameraTransition()
    {
        Vector3 startPos = mainCamera.transform.position;
        Quaternion startRot = mainCamera.transform.rotation;

        float elapsed = 0f;

        while (elapsed < cameraTransitionDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / cameraTransitionDuration);

            mainCamera.transform.position = Vector3.Lerp(startPos, endPosition, t);
            mainCamera.transform.rotation = Quaternion.Slerp(startRot, endRotation, t);

            yield return null;
        }

        ShowEndUI();
    }

    private void ShowEndUI()
    {
        // ⏱ CALCULATE FINAL TIME
        float totalTime = Time.timeSinceLevelLoad - startTime;

        int minutes = Mathf.FloorToInt(totalTime / 60f);
        int seconds = Mathf.FloorToInt(totalTime % 60f);

        timeText.text = "Time Pld: " +
                        minutes.ToString("00") + ":" +
                        seconds.ToString("00");

        endScreenPanel.SetActive(true);
        Time.timeScale = 0f;
    }
}
