using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Board Setup")]
    [SerializeField] private Transform[] tiles;
    [SerializeField] private float moveSpeed = 3f;

    [Header("Teleport Visuals")]
    [SerializeField] private float fadeDuration = 0.15f;

    [Header("Dice Reference")]
    private DieRollScript dieRollScript;

    private int currentTileIndex = 0;
    private bool isMoving = false;
    private bool hasMovedThisRoll = false;

    private Renderer[] renderers;

    // Ladder mapping: FROM -> TO
    private Dictionary<int, int> ladders = new Dictionary<int, int>()
    {
        { 1, 16 },
        { 6, 12 },
        { 13, 23 },
        { 21, 34 },
        { 27, 29 },
        { 31, 32 }
    };

    // 🔴➡️🟢 Portal mapping: RED -> GREEN
    private Dictionary<int, int> portals = new Dictionary<int, int>()
    {
        { 7, 2 },
        { 26, 19 },
        { 35, 17 }
    };

    void Start()
    {
        dieRollScript = FindFirstObjectByType<DieRollScript>();
        renderers = GetComponentsInChildren<Renderer>();

        if (dieRollScript == null)
        {
            Debug.LogError("[PlayerMovement] DieRollScript NOT FOUND!");
            return;
        }

        if (tiles == null || tiles.Length == 0)
        {
            Debug.LogError("[PlayerMovement] Tiles array is EMPTY or NOT ASSIGNED!");
            return;
        }

        transform.position = tiles[currentTileIndex].position;
        Debug.Log($"[PlayerMovement] Player placed on Tile ({currentTileIndex})");
    }

    void Update()
    {
        if (dieRollScript == null)
            return;

        if (dieRollScript.isLanded && !isMoving && !hasMovedThisRoll)
        {
            int rolledValue;
            if (int.TryParse(dieRollScript.diceFaceNum, out rolledValue))
            {
                StartCoroutine(MovePlayer(rolledValue));
                hasMovedThisRoll = true;
            }
        }

        if (!dieRollScript.isLanded)
            hasMovedThisRoll = false;
    }

    IEnumerator MovePlayer(int steps)
    {
        isMoving = true;

        for (int i = 0; i < steps; i++)
        {
            if (currentTileIndex >= tiles.Length - 1)
                break;

            currentTileIndex++;
            yield return StartCoroutine(MoveToTile(tiles[currentTileIndex]));
        }

        // 🔼 LADDER CHECK (FINAL TILE ONLY)
        if (ladders.ContainsKey(currentTileIndex))
        {
            int ladderTarget = ladders[currentTileIndex];
            currentTileIndex = ladderTarget;
            yield return StartCoroutine(MoveToTile(tiles[currentTileIndex]));
        }

        // 🌀 RED PORTAL VISUAL TELEPORT
        if (portals.ContainsKey(currentTileIndex))
        {
            int portalTarget = portals[currentTileIndex];
            Debug.Log($"[PlayerMovement] RED PORTAL! Visual teleport to Tile ({portalTarget})");

            currentTileIndex = portalTarget;
            yield return StartCoroutine(TeleportVisual(tiles[currentTileIndex]));
        }

        Debug.Log($"[PlayerMovement] Turn ended on Tile ({currentTileIndex})");
        isMoving = false;
    }

    IEnumerator MoveToTile(Transform targetTile)
    {
        while (Vector3.Distance(transform.position, targetTile.position) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetTile.position,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.position = targetTile.position;
    }

    // 🌀 TELEPORT VISUAL EFFECT (NO LOGIC CHANGE)
    IEnumerator TeleportVisual(Transform targetTile)
    {
        // Fade out
        yield return StartCoroutine(Fade(1f, 0f));

        // Instant reposition (looks like teleport)
        transform.position = targetTile.position;

        // Fade in
        yield return StartCoroutine(Fade(0f, 1f));
    }

    IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            SetAlpha(alpha);
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
