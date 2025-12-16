using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Board Setup")]
    [SerializeField] private float moveSpeed = 3f;

    [Header("Teleport Visuals")]
    [SerializeField] private float fadeDuration = 0.15f;

    [Header("Animation")]
    [Tooltip("Assign the Animator from the character prefab (or leave empty to auto-find)")]
    [SerializeField] private Animator animator;

    [Header("Positioning")]
    [Tooltip("Vertical offset applied when placing character on a tile to avoid sinking into the floor")]
    [SerializeField] private float tileYOffset = 0.5f;

    [Header("Dice Reference")]
    [SerializeField] private DieRollScript dieRollScript;

    private int currentTileIndex = 0;
    private bool isMoving = false;
    private bool hasMovedThisRoll = false;
    private bool isTurn = false;
    public bool IsPlayer { get; private set; } = false; // set by PlayerScript for the human player
    public bool IsTurn { get { return isTurn; } }

    // Per-character dice throw counter
    private int diceRollCount = 0;

    public void IncrementDiceRolls()
    {
        diceRollCount++;
    }

    public int GetDiceRolls()
    {
        return diceRollCount;
    }

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

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
            if (animator == null)
            {
                Debug.LogWarning("[PlayerMovement] No Animator found on character. Assign one in the inspector to enable animations.");
            }
        }

        if (tiles == null || tiles.Length == 0)
        {
            Debug.LogError("[PlayerMovement] Tiles array is EMPTY in BoardManager!");
            return;
        }

        var startPos = tiles[currentTileIndex].position;
        startPos.y += tileYOffset;
        transform.position = startPos;

        // registration is handled by PlayerScript to ensure correct ordering
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

        // Only respond to dice when it's this character's turn
        if (!isTurn) return;

        // Only the human player should respond to die-land events here.
        // NPC movement is driven by TurnManager so they won't start moving prematurely.
        if (!IsPlayer) return;

        // Player movement is initiated by GameManager when the die lands.
        // This prevents duplicate movement calls if both GameManager and this component reacted.
    }

    public IEnumerator MovePlayer(int steps)
{
    string cname = GetCharacterName();
    if (isMoving)
    {
        Debug.LogWarning($"[PlayerMovement] {cname} MovePlayer called but already moving. Ignoring duplicate call.");
        yield break;
    }
    Debug.Log($"[PlayerMovement] {cname} MovePlayer START steps={steps} tileIndexBefore={currentTileIndex}");
    isMoving = true;

    // Animation: start walking
    if (animator != null)
    {
        animator.SetBool("Idle", false);
        animator.SetBool("Walk", true);
    }

    for (int i = 0; i < steps; i++)
    {
        if (currentTileIndex >= tiles.Length - 1)
            break;

        currentTileIndex++;
        yield return StartCoroutine(MoveToTile(tiles[currentTileIndex]));
    }

    // LADDER
    if (ladders.TryGetValue(currentTileIndex, out int ladderTarget))
    {
        currentTileIndex = ladderTarget;
        yield return StartCoroutine(MoveToTile(tiles[currentTileIndex]));
    }

    // PORTAL
    if (portals.TryGetValue(currentTileIndex, out int portalTarget))
    {
        // trigger teleport animation and perform a slower fade-out (2 seconds)
        if (animator != null)
        {
            animator.SetBool("Idle", false);
            animator.SetBool("Walk", false);
            animator.SetTrigger("Teleport");
        }

        // play visual teleport first, only update the logical tile index after the visual completes
        yield return StartCoroutine(TeleportVisual(tiles[portalTarget], 2f, fadeDuration));
        currentTileIndex = portalTarget;
    }

    // ✅ END TILE CHECK (Tile 37)
    if (currentTileIndex == 37)
    {
        Debug.Log($"[PlayerMovement] {cname} reached final tile!");
        // notify TurnManager so end screen only shows when all players finish
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.NotifyPlayerFinished(this);
        }
        else
        {
            EndGameManager.Instance.ShowEndScreen();
        }
        // prevent further movement for this character
        isTurn = false;
        isMoving = false;
        yield break;
    }

    // Animation: stop walking, go to idle
    if (animator != null)
    {
        animator.SetBool("Walk", false);
        animator.SetBool("Idle", true);
    }

    isMoving = false;
    Debug.Log($"[PlayerMovement] {cname} MovePlayer END at tileIndex={currentTileIndex}");

    // Notify TurnManager that this player's turn is finished
    if (TurnManager.Instance != null)
    {
        TurnManager.Instance.EndTurn();
    }
}


    IEnumerator MoveToTile(Transform targetTile)
    {
        Vector3 targetPos = new Vector3(targetTile.position.x, targetTile.position.y + tileYOffset, targetTile.position.z);
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
    }

    IEnumerator TeleportVisual(Transform targetTile, float outDuration, float inDuration)
    {
        yield return StartCoroutine(Fade(1f, 0f, outDuration));
        Vector3 tpPos = new Vector3(targetTile.position.x, targetTile.position.y + tileYOffset, targetTile.position.z);
        transform.position = tpPos;
        yield return StartCoroutine(Fade(0f, 1f, inDuration));
    }

    IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(Mathf.Lerp(startAlpha, endAlpha, elapsed / duration));
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

    // Called by TurnManager to assign turn ownership
    public void SetTurn(bool active)
    {
        isTurn = active;
        // reset move state so player can act when their turn begins
        hasMovedThisRoll = false;
    }

    public string GetCharacterName()
    {
        var nameScript = GetComponent<NameScript>();
        if (nameScript != null) return nameScript.GetName();
        return gameObject.name;
    }

    // Called by PlayerScript to mark the real human player
    public void MarkAsPlayer()
    {
        IsPlayer = true;
    }
}
