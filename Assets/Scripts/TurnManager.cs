using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    private List<PlayerMovement> players = new List<PlayerMovement>();
    private int currentIndex = 0;
    private HashSet<PlayerMovement> finishedPlayers = new HashSet<PlayerMovement>();

    private Text turnText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        CreateTurnUI();
    }

    void CreateTurnUI()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject cobj = new GameObject("TurnCanvas");
            canvas = cobj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            cobj.AddComponent<CanvasScaler>();
            cobj.AddComponent<GraphicRaycaster>();
        }

        GameObject t = new GameObject("TurnText");
        t.transform.SetParent(canvas.transform);
        turnText = t.AddComponent<Text>();
        turnText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        turnText.fontSize = 24;
        turnText.alignment = TextAnchor.UpperLeft;
        RectTransform rt = turnText.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = new Vector2(10f, -10f);
        rt.sizeDelta = new Vector2(400f, 60f);
        turnText.color = Color.white;
        UpdateTurnText();
    }

    public void RegisterPlayer(PlayerMovement pm)
    {
        if (pm == null) return;
        if (players.Contains(pm))
        {
            Debug.LogWarning($"[TurnManager] Attempted to register player twice: {pm.GetCharacterName()}");
            return;
        }
        players.Add(pm);
        Debug.Log($"[TurnManager] Registered player: {pm.GetCharacterName()} (IsPlayer={pm.IsPlayer}). Total players: {players.Count}");
        // Validate there is at most one human player marked
        int humanCount = 0;
        foreach (var p in players) if (p.IsPlayer) humanCount++;
        if (humanCount == 0)
            Debug.LogWarning("[TurnManager] No human player registered yet.");
        else if (humanCount > 1)
            Debug.LogWarning($"[TurnManager] Multiple human players registered: {humanCount}");
        if (players.Count == 1)
        {
            StartTurn(0);
        }
    }

    void StartTurn(int index)
    {
        StartCoroutine(StartTurnCoroutine(index));
    }

    IEnumerator StartTurnCoroutine(int index)
    {
        if (players.Count == 0) yield break;

        // find next non-finished player starting at index
        int start = index % players.Count;
        int found = -1;
        for (int offset = 0; offset < players.Count; offset++)
        {
            int i = (start + offset) % players.Count;
            if (!finishedPlayers.Contains(players[i])) { found = i; break; }
        }
        if (found == -1) yield break; // all finished

        currentIndex = found;
        for (int i = 0; i < players.Count; i++)
        {
            players[i].SetTurn(i == currentIndex);
        }
        UpdateTurnText();
        Debug.Log($"[TurnManager] Starting turn for {players[currentIndex].GetCharacterName()} (IsPlayer={players[currentIndex].IsPlayer}) at index {currentIndex}");

        // Log full players list status for diagnostics
        string listStatus = "[TurnManager] Players status:";
        for (int i = 0; i < players.Count; i++)
        {
            listStatus += $" \n  [{i}] {players[i].GetCharacterName()} IsPlayer={players[i].IsPlayer} IsTurn={players[i].IsTurn}";
        }
        Debug.Log(listStatus);

        var die = FindObjectOfType<DieRollScript>();

        // If the next player is the human player, prepare the die and wait for the player's manual roll (event-driven)
        if (players[currentIndex].IsPlayer)
        {
            if (die != null)
            {
                // Prepare the die for the player's manual roll (do not reposition it)
                die.PrepareForPlayer();
                die.SetAcceptInput(true);
            }

            bool landed = false; string face = null;
            System.Action<string> handler = (f) => { landed = true; face = f; };
            if (die != null) die.OnLanded += handler;

            // Wait until the player clicks the die and it lands
            while (!landed) yield return null;

            if (die != null) die.OnLanded -= handler;
            if (die != null) die.SetAcceptInput(false);

            int steps = 0;
            if (!int.TryParse(face, out steps)) steps = Random.Range(1, 7);
            Debug.Log($"[TurnManager] Player {players[currentIndex].GetCharacterName()} rolled: {steps}");

            // Execute player movement and rely on PlayerMovement to call EndTurn/NotifyPlayerFinished
            yield return StartCoroutine(players[currentIndex].MovePlayer(steps));
            yield break;
        }

        // Otherwise it's an NPC: autoplay using event-driven landing as well
        yield return StartCoroutine(AutoPlayNpc(players[currentIndex]));
    }

    IEnumerator AutoPlayNpc(PlayerMovement npc)
    {
        // disable player input on die
        var die = FindObjectOfType<DieRollScript>();
        if (die != null) die.SetAcceptInput(false);

        yield return new WaitForSeconds(0.75f);

        // Trigger visual die roll and wait for it to land
        if (die != null)
        {
            Debug.Log($"[TurnManager] NPC {npc.GetCharacterName()} auto-roll starting (visual die)");
            bool landed = false; string face = null;
            System.Action<string> handler = (f) => { landed = true; face = f; };
            die.OnLanded += handler;
            die.StartRoll();
            yield return new WaitUntil(() => landed);
            die.OnLanded -= handler;
            int steps = 0;
            if (!int.TryParse(face, out steps)) steps = Random.Range(1, 7);
            Debug.Log($"[TurnManager] NPC {npc.GetCharacterName()} roll result: {steps}");

            // Wait for the NPC to complete its movement before re-enabling input and advancing turn.
            Debug.Log($"[TurnManager] NPC {npc.GetCharacterName()} starting MovePlayer({steps})");
            yield return StartCoroutine(npc.MovePlayer(steps));
            Debug.Log($"[TurnManager] NPC {npc.GetCharacterName()} finished moving");

            die.SetAcceptInput(true);
            // Note: npc.MovePlayer will call TurnManager.EndTurn() or NotifyPlayerFinished as appropriate.
            yield break;
        }
        else
        {
            int steps = Random.Range(1, 7);
            Debug.Log($"[TurnManager] NPC {npc.GetCharacterName()} fallback roll result: {steps}");
            yield return StartCoroutine(npc.MovePlayer(steps));
            Debug.Log($"[TurnManager] NPC {npc.GetCharacterName()} finished moving (fallback)");
            yield break;
        }
    }

    public void EndTurn()
    {
        if (players.Count == 0) return;
        int next = (currentIndex + 1) % players.Count;
        StartTurn(next);
    }

    public void NotifyPlayerFinished(PlayerMovement pm)
    {
        if (!finishedPlayers.Contains(pm)) finishedPlayers.Add(pm);
        // Record the finished player's stats to the leaderboard (per-player dice count)
        if (LeaderboardManager.Instance != null)
        {
            LeaderboardManager.Instance.RecordFinish(pm);
        }
        // if all players finished, show end screen
        if (finishedPlayers.Count >= players.Count)
        {
            EndGameManager.Instance.ShowEndScreen();
        }
        else
        {
            // advance to next still-active player
            EndTurn();
        }
    }

    void UpdateTurnText()
    {
        if (turnText == null) return;
        if (players.Count == 0)
        {
            turnText.text = "";
            return;
        }
        var cur = players[currentIndex];
        string name = cur.GetCharacterName();
        if (finishedPlayers.Contains(cur)) { turnText.text = name + " - Finished"; return; }
        if (cur.IsPlayer) turnText.text = name + " - Your turn";
        else turnText.text = name + " - Their turn";
    }

    // True when the currently active turn belongs to the human player
    public bool IsPlayersMove
    {
        get
        {
            if (players == null || players.Count == 0) return false;
            if (currentIndex < 0 || currentIndex >= players.Count) return false;
            var cur = players[currentIndex];
            return cur != null && cur.IsPlayer && cur.IsTurn;
        }
    }


}