using UnityEngine;

public class DieRollScript : MonoBehaviour
{
    Rigidbody rBody;
    Vector3 position;
    [SerializeField] private float maxRandForcVal, startRollongForce;
    float forceX, forceY, forceZ;
    public string diceFaceNum;
    public bool isLanded = false;
    public bool firstThrow = false;
    public bool acceptInput = true;
    // Prevent consuming the same landed result multiple times
    public bool resultConsumed = false;
    private int originalLayer;
    // Event fired when the die is detected as landed. Argument is the face number string.
    public event System.Action<string> OnLanded;

    void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        rBody = GetComponent<Rigidbody>();
        rBody.isKinematic = true;
        position = transform.position;
        transform.rotation = new Quaternion(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360), 0);
        originalLayer = gameObject.layer;
    }

    private void RollDice()
    {
        rBody.isKinematic = false;
        forceX = Random.Range(0, maxRandForcVal);
        forceY = Random.Range(0, maxRandForcVal);
        forceZ = Random.Range(0, maxRandForcVal);
        rBody.AddForce(Vector3.up * Random.Range(800, startRollongForce));
        rBody.AddTorque(forceX, forceY, forceZ);
    }

    // Public wrapper so other scripts (NPC/TurnManager) can trigger a visual roll
    public void StartRoll()
    {
        if (!firstThrow) firstThrow = true;
        isLanded = false;
        Debug.Log("[DieRollScript] StartRoll invoked (visual roll)");
        // allow detection of a fresh result
        resultConsumed = false;
        // Attribute this roll to the currently active player (if any)
        var currentPlayer = FindCurrentPlayer();
        if (currentPlayer != null)
        {
            currentPlayer.IncrementDiceRolls();
            // If the current player is the human player, update the dice counter UI
            if (currentPlayer.IsPlayer)
            {
                if (DiceRollCounter.Instance != null) DiceRollCounter.Instance.ForceRefresh();
            }
        }
        RollDice();
    }

    // Called by SideDetectScript when a face has been determined
    public void ReportLanded(string face)
    {
        // Ignore land reports if the die wasn't actually rolled this turn.
        // This prevents a previously-settled die from being consumed immediately when the player's
        // turn starts (PrepareForPlayer resets firstThrow to false).
        if (!firstThrow)
        {
            Debug.Log($"[DieRollScript] Ignoring ReportLanded {face} because firstThrow==false");
            return;
        }

        diceFaceNum = face;
        isLanded = true;
        resultConsumed = false;
        Debug.Log($"[DieRollScript] ReportLanded called: {face}");
        try { OnLanded?.Invoke(face); } catch (System.Exception e) { Debug.LogException(e); }
    }

    // Prepare die state for the human player so they must click to roll
    public void PrepareForPlayer()
    {
        isLanded = false;
        firstThrow = false;
        acceptInput = true;
        resultConsumed = false;
    }

    public void SetAcceptInput(bool allow)
    {
        acceptInput = allow;
        // Toggle raycastability by moving die hierarchy to/from the Ignore Raycast layer
        int ignoreLayer = LayerMask.NameToLayer("Ignore Raycast");
        if (ignoreLayer == -1) ignoreLayer = 2; // fallback to default Unity Ignore Raycast layer id
        int targetLayer = allow ? originalLayer : ignoreLayer;
        SetLayerRecursively(gameObject, targetLayer);
        Debug.Log($"[DieRollScript] acceptInput set to {allow} (hierarchy layer set to {targetLayer})");
    }

    void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform t in obj.transform)
        {
            SetLayerRecursively(t.gameObject, layer);
        }
    }

    public void ResetDice()
    {
        Debug.Log("[DieRollScript] ResetDice called");
        transform.position = position;
        firstThrow = false;
        isLanded = false;
        Initialize();
    }

    void Update()
    {
        if (rBody == null) return;
        // Die is rolled via UI button (see UIDieButton.OnClickRoll) or via StartRoll() for NPCs.
    }

    // Called by a UI Button to perform the player's manual roll.
    public void PlayerRoll()
    {
        bool isPlayersTurn = TurnManager.Instance == null || TurnManager.Instance.IsPlayersMove;
        if (!acceptInput || !isPlayersTurn || firstThrow) return;
        firstThrow = true;
        Debug.Log("[DieRollScript] Player roll via UI button");
        // Attribute this roll to the currently active player
        var currentPlayer = FindCurrentPlayer();
        if (currentPlayer != null)
        {
            currentPlayer.IncrementDiceRolls();
            if (DiceRollCounter.Instance != null) DiceRollCounter.Instance.ForceRefresh();
        }
        RollDice();
    }

    private PlayerMovement FindCurrentPlayer()
    {
        var players = FindObjectsOfType<PlayerMovement>();
        foreach (var p in players)
        {
            if (p.IsTurn) return p;
        }
        return null;
    }
}
