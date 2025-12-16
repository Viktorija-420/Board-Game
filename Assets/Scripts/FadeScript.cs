using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeScript : MonoBehaviour
{
    // When true, the next automatic FadeIn in Start() will be skipped and the image hidden immediately.
    public static bool skipNextFadeOnStart = false;

    [Header("Optional Image for fade")]
    [SerializeField] private Image img;

    private Color tempColor;

    void Awake()
    {
        // If not assigned in inspector, try to get Image from this GameObject
        if (img == null)
        {
            img = GetComponent<Image>();
        }

        if (img == null)
        {
            Debug.LogWarning("[FadeScript] No Image component assigned or found on " + gameObject.name);
        }
    }

    void Start()
    {
        if (img != null)
        {
            tempColor = img.color;
            tempColor.a = 1f;
            img.color = tempColor;
            if (skipNextFadeOnStart)
            {
                // hide immediately and reset flag
                tempColor.a = 0f;
                img.color = tempColor;
                img.raycastTarget = false;
                skipNextFadeOnStart = false;
            }
            else
            {
                StartCoroutine(FadeIn(0.2f));
            }
        }
    }

    /// <summary>
    /// Fades in (from opaque to transparent)
    /// </summary>
    public IEnumerator FadeIn(float fadeSpeed)
    {
        if (img == null) yield break;

        img.raycastTarget = true;
        for (float a = 1f; a >= 0f; a -= 0.05f)
        {
            tempColor = img.color;
            tempColor.a = a;
            img.color = tempColor;
            yield return new WaitForSecondsRealtime(fadeSpeed);
        }

        img.raycastTarget = false; // allow clicks through
    }

    /// <summary>
    /// Fades out (from transparent to opaque)
    /// </summary>
    public IEnumerator FadeOut(float fadeSpeed)
    {
        if (img == null) yield break;

        img.raycastTarget = true;
        for (float a = 0f; a <= 1f; a += 0.05f)
        {
            tempColor = img.color;
            tempColor.a = a;
            img.color = tempColor;
            yield return new WaitForSecondsRealtime(fadeSpeed);
        }
    }
}
