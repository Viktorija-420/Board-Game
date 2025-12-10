using UnityEngine;

/// <summary>
/// Simple script to animate a texture offset on a material so it looks like flowing lava.
/// Attach to the GameObject with the Renderer (mesh, quad, plane, etc.).
/// </summary>
[DisallowMultipleComponent]
public class FlowingLava : MonoBehaviour
{
    [Tooltip("Texture property to animate. Common: _MainTex (Standard), _BaseMap (URP/HDRP).")]
    public string textureProperty = "_MainTex";

    [Tooltip("Offset speed in texture UV units per second (X, Y).")]
    public Vector2 speed = new Vector2(0.08f, 0.05f);

    [Tooltip("Multiplies the base speed for more/less flow.")]
    public float speedMultiplier = 1f;

    [Tooltip("If true, modifies the shared material (affects all objects using this material). " +
             "If false, a temporary instance is created for this Renderer (safer for per-object variation).")]
    public bool useSharedMaterial = false;

    // runtime
    Material _instanceMaterial;     // the material we're writing to
    Vector2 _initialOffset;
    Renderer _renderer;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer == null)
        {
            Debug.LogError($"[{nameof(FlowingLava)}] No Renderer found on GameObject '{gameObject.name}'. Disabling script.");
            enabled = false;
            return;
        }

        // pick material: sharedMaterial or instance
        if (useSharedMaterial)
        {
            _instanceMaterial = _renderer.sharedMaterial;
        }
        else
        {
            // create an instance so we don't modify the original asset
            _instanceMaterial = new Material(_renderer.sharedMaterial);
            _renderer.material = _instanceMaterial;
        }

        // get initial offset (if property exists)
        if (_instanceMaterial.HasProperty(textureProperty))
        {
            _initialOffset = _instanceMaterial.GetTextureOffset(textureProperty);
        }
        else
        {
            Debug.LogWarning($"[{nameof(FlowingLava)}] Material does not have a texture property named '{textureProperty}'. " +
                             "Check shader property name (e.g. _MainTex or _BaseMap). Disabling script.");
            enabled = false;
        }
    }

    void Update()
    {
        if (_instanceMaterial == null) return;

        // compute offset
        Vector2 offset = _initialOffset + speed * speedMultiplier * Time.time;

        // keep offset in 0..1 range for numeric stability
        offset.x = Mathf.Repeat(offset.x, 1f);
        offset.y = Mathf.Repeat(offset.y, 1f);

        _instanceMaterial.SetTextureOffset(textureProperty, offset);
    }

    void OnDestroy()
    {
        // If we created an instance material, destroy it to avoid leaks
        if (!useSharedMaterial && _instanceMaterial != null)
        {
            // If running in editor, DestroyImmediate may be appropriate, but Destroy is fine at runtime.
#if UNITY_EDITOR
            DestroyImmediate(_instanceMaterial);
#else
            Destroy(_instanceMaterial);
#endif
        }
    }

    // Optionally expose a method to change speed at runtime
    public void SetSpeed(Vector2 newSpeed)
    {
        speed = newSpeed;
    }
}
