using UnityEngine;

[DisallowMultipleComponent]
public class FlowingLava : MonoBehaviour
{
    [Header("Texture")]
    public string textureProperty = "_MainTex";

    [Header("Base Flow")]
    public Vector2 baseFlowDirection = new Vector2(0.05f, 0.03f);
    public float flowSpeed = 1f;

    [Header("Warping (Liquid Motion)")]
    [Tooltip("How strong the UV warping is")]
    public float warpStrength = 0.05f;

    [Tooltip("How fast the warp animation moves")]
    public float warpSpeed = 0.6f;

    [Tooltip("Scale of the noise pattern (larger = smoother blobs)")]
    public float warpScale = 1.5f;

    [Header("Bubbling / Pulsing")]
    public float bubbleStrength = 0.02f;
    public float bubbleSpeed = 2f;

    [Header("Material Usage")]
    public bool useSharedMaterial = false;

    Material _mat;
    Renderer _renderer;
    Vector2 _initialOffset;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer == null)
        {
            Debug.LogError("FlowingLava: No Renderer found.");
            enabled = false;
            return;
        }

        if (useSharedMaterial)
        {
            _mat = _renderer.sharedMaterial;
        }
        else
        {
            _mat = new Material(_renderer.sharedMaterial);
            _renderer.material = _mat;
        }

        if (!_mat.HasProperty(textureProperty))
        {
            Debug.LogWarning($"FlowingLava: Material missing '{textureProperty}' property.");
            enabled = false;
            return;
        }

        _initialOffset = _mat.GetTextureOffset(textureProperty);
    }

    void Update()
    {
        float t = Time.time;

        // ---------- BASE FLOW ----------
        Vector2 flow = baseFlowDirection * flowSpeed * t;

        // ---------- NOISE WARP (SWIRLING MOTION) ----------
        float noiseX = Mathf.PerlinNoise(t * warpSpeed, 0.0f);
        float noiseY = Mathf.PerlinNoise(0.0f, t * warpSpeed);

        Vector2 warp =
            new Vector2(
                noiseX - 0.5f,
                noiseY - 0.5f
            ) * warpStrength;

        // ---------- BUBBLE PULSE ----------
        float bubble = Mathf.Sin(t * bubbleSpeed) * bubbleStrength;

        Vector2 bubbleOffset = new Vector2(bubble, bubble * 0.5f);

        // ---------- FINAL OFFSET ----------
        Vector2 finalOffset =
            _initialOffset +
            flow +
            warp +
            bubbleOffset;

        finalOffset.x = Mathf.Repeat(finalOffset.x, 1f);
        finalOffset.y = Mathf.Repeat(finalOffset.y, 1f);

        _mat.SetTextureOffset(textureProperty, finalOffset);
    }

    void OnDestroy()
    {
        if (!useSharedMaterial && _mat != null)
        {
#if UNITY_EDITOR
            DestroyImmediate(_mat);
#else
            Destroy(_mat);
#endif
        }
    }
}
