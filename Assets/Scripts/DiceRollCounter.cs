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

    public void AddRoll()
    {
        rollCount++;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (rollsText != null)
            rollsText.text = rollCount.ToString();
    }

    public int GetRollCount()
    {
        return rollCount;
    }

    public static int GetTotalRolls()
    {
        DiceRollCounter counter = FindFirstObjectByType<DiceRollCounter>();
        return counter != null ? counter.GetRollCount() : 0;
    }

    public void ResetRolls()
    {
        rollCount = 0;
        UpdateUI();
    }


}
