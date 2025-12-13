using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Board Setup")]
    [SerializeField] private float moveSpeed = 3f;

    [Header("Teleport Visuals")]
    [SerializeField] private float fadeDuration = 0.15f;

    [Header("Dice Reference")]
    [SerializeField] private DieRollScript dieRollScript;

    private int currentTileIndex = 0;
    private bool isMoving = false;
    private bool hasMovedThisRoll = false;

    private Renderer[] renderers;

    // Ladder mapping: FROM -> TO
    private Dictionary<int, int> ladders = new Dictionary<int, int>()
    {
        { 1, 16 }, { 6, 12 }, { 13, 23 },
        { 21, 34 }, { 27, 29 }, { 31, 32 }
    };

    // Red -> Green portal mapping
    private Dictionary<int, int> portals = new Dictionary<int, int>()
    {
        { 7, 2 }, { 26, 19 }, { 35, 17 }
    };

    // Access tiles from BoardManager
    private Transform[] tiles => BoardManager.Instance.Tiles;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();

        if (tiles == null || tiles.Length == 0)
        {
            Debug.LogError("[PlayerMovement] Tiles array is EMPTY in BoardManager!");
            return;
        }

        transform.position = tiles[currentTileIndex].position;
    }

    void EnsureDieReference()
    {
        if (dieRollScript != null) return;

        dieRollScript = FindObjectOfType<DieRollScript>();
        if (dieRollScript != null) return;

        GameObject dieObj = GameObject.FindWithTag("Die");
        if (dieObj != null)
        {
            dieRollScript = dieObj.GetComponent<DieRollScript>();
        }

        if (dieRollScript == null)
        {
            Debug.LogWarning("[PlayerMovement] No DieRollScript found in scene. Assign it in inspector or add tag 'Die' to the die GameObject.");
        }
    }

    void Update()
    {
        EnsureDieReference();
        if (dieRollScript == null) return;

        if (dieRollScript.isLanded && !isMoving && !hasMovedThisRoll)
        {
            if (int.TryParse(dieRollScript.diceFaceNum, out int rolledValue))
            {
                Debug.Log("[PlayerMovement] Moving player by " + rolledValue + " steps.");
                StartCoroutine(MovePlayer(rolledValue));
                hasMovedThisRoll = true;
            }
        }

        if (!dieRollScript.isLanded) hasMovedThisRoll = false;
    }

    public IEnumerator MovePlayer(int steps)
    {
        isMoving = true;

        for (int i = 0; i < steps; i++)
        {
            if (currentTileIndex >= tiles.Length - 1) break;

            currentTileIndex++;
            yield return StartCoroutine(MoveToTile(tiles[currentTileIndex]));
        }

        if (ladders.TryGetValue(currentTileIndex, out int ladderTarget))
        {
            currentTileIndex = ladderTarget;
            yield return StartCoroutine(MoveToTile(tiles[currentTileIndex]));
        }

        if (portals.TryGetValue(currentTileIndex, out int portalTarget))
        {
            currentTileIndex = portalTarget;
            yield return StartCoroutine(TeleportVisual(tiles[currentTileIndex]));
        }

        isMoving = false;
    }

    IEnumerator MoveToTile(Transform targetTile)
    {
        while (Vector3.Distance(transform.position, targetTile.position) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetTile.position, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetTile.position;
    }

    IEnumerator TeleportVisual(Transform targetTile)
    {
        yield return StartCoroutine(Fade(1f, 0f));
        transform.position = targetTile.position;
        yield return StartCoroutine(Fade(0f, 1f));
    }

    IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration));
            yield return null;
        }
        SetAlpha(endAlpha);
    }

    void SetAlpha(float alpha)
    {
        foreach (Renderer r in renderers)
        {
            foreach (Material m in r.materials)
            {
                if (m.HasProperty("_Color"))
                {
                    Color c = m.color;
                    c.a = alpha;
                    m.color = c;
                }
            }
        }
    }
}
