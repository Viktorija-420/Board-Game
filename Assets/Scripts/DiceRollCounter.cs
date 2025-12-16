using UnityEngine;
using UnityEngine.UI;

public class DiceRollCounter : MonoBehaviour
{
    public static DiceRollCounter Instance;

    [SerializeField] private Text rollsText; // UIObjects/Rolls/Times

    private int rollCount = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        UpdateUI();
    }

    // UI now reads the human player's dice count directly from their PlayerMovement.
    private void UpdateUI()
    {
        if (rollsText == null) return;
        var players = FindObjectsOfType<PlayerMovement>();
        foreach (var p in players)
        {
            if (p.IsPlayer)
            {
                rollsText.text = p.GetDiceRolls().ToString();
                return;
            }
        }
        // fallback
        rollsText.text = "0";
    }

    // Force a refresh call from other scripts when the active player's count changes
    public void ForceRefresh()
    {
        UpdateUI();
    }

    public static int GetTotalRolls()
    {
        // Deprecated for global counting; attempt to sum all players as a fallback
        int total = 0;
        var players = FindObjectsOfType<PlayerMovement>();
        foreach (var p in players) total += p.GetDiceRolls();
        return total;
    }

    public void ResetRolls()
    {
        var players = FindObjectsOfType<PlayerMovement>();
        foreach (var p in players)
        {
            // no direct reset method on PlayerMovement; skipping
        }
        UpdateUI();
    }


}
