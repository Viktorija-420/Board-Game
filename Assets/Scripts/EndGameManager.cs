using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;


public class EndGameManager : MonoBehaviour
{
    public static EndGameManager Instance;

    [SerializeField] private GameObject endScreenPanel;
    [SerializeField] private Text timeText; // Text on END GAME PANEL
    [SerializeField] private Text rollsText; // Text on END GAME PANEL


    [Header("Camera Transition")]
    [SerializeField] private float cameraTransitionDuration = 1.5f;

    private Camera mainCamera;

    // Camera target
    private readonly Vector3 endPosition = new Vector3(0f, 11.3f, -1.12f);
    private readonly Quaternion endRotation = Quaternion.Euler(12.82f, 0f, 0f);

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        mainCamera = Camera.main;

        endScreenPanel.SetActive(false);
    }

    public void ShowEndScreen()
    {
        // ⏱ Stop gameplay timer BEFORE showing UI
        GameTimer.Instance.StopTimer();

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
        // ⏱ Final time
        timeText.text = GameTimer.Instance.GetFormattedFinalTime();

        // 🎲 Total rolls
        rollsText.text = DiceRollCounter.GetTotalRolls().ToString();

        endScreenPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void PlayAgain()
    {
        GameTimer.Instance.ResetTimer();
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }



}
