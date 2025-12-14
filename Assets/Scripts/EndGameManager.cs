using UnityEngine;
using System.Collections;

public class EndGameManager : MonoBehaviour
{
    public static EndGameManager Instance;

    [SerializeField] private GameObject endScreenPanel;

    [Header("Camera Transition")]
    [SerializeField] private float cameraTransitionDuration = 1.5f;

    private Camera mainCamera;

    // 🎯 NEW target end-game camera transform
    private readonly Vector3 endPosition = new Vector3(0f, 11.3f, -1.12f);
    private readonly Quaternion endRotation = Quaternion.Euler(12.82f, 0f, 0f);

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        if (endScreenPanel != null)
            endScreenPanel.SetActive(false);

        mainCamera = Camera.main;
    }

    public void ShowEndScreen()
    {
        if (mainCamera != null)
        {
            StartCoroutine(CameraTransition());
        }
        else
        {
            endScreenPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    private IEnumerator CameraTransition()
    {
        Vector3 startPos = mainCamera.transform.position;
        Quaternion startRot = mainCamera.transform.rotation;

        float elapsed = 0f;

        while (elapsed < cameraTransitionDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / cameraTransitionDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            mainCamera.transform.position = Vector3.Lerp(startPos, endPosition, t);
            mainCamera.transform.rotation = Quaternion.Slerp(startRot, endRotation, t);

            yield return null;
        }

        // Snap exactly to final values
        mainCamera.transform.position = endPosition;
        mainCamera.transform.rotation = endRotation;

        // Show UI and pause AFTER movement
        if (endScreenPanel != null)
            endScreenPanel.SetActive(true);

        Time.timeScale = 0f;
    }
}
