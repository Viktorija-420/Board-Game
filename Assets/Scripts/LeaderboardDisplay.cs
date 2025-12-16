using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Attach to a GameObject in the scoreboard scene. Provide a parent with a VerticalLayoutGroup
// that contains exactly 7 Text children (or fewer — the script will populate available slots).
public class LeaderboardDisplay : MonoBehaviour
{
    [Tooltip("Parent that contains Text children to fill (VerticalLayoutGroup recommended)")]
    [SerializeField] private RectTransform entriesParent;

    [Tooltip("Line format using string.Format placeholders: {0}=rank {1}=name {2}=time {3}=throws {4}=points")]
    [SerializeField] private string lineFormat = "{0}.\t{1}\t{2}\t{3} throws\t{4} pts";

    private const int SlotCount = 7;

    void Start()
    {
        if (entriesParent == null)
        {
            Debug.LogWarning("[LeaderboardDisplay] entriesParent not assigned. Drag your VerticalLayoutGroup parent into this component.");
            return;
        }

        EnsureSlots();
        Refresh();
    }

    // Ensure there are exactly SlotCount TextMeshProUGUI children under entriesParent
    private void EnsureSlots()
    {
        // Do not modify existing children. Only ensure there are at least SlotCount child objects
        int existing = entriesParent.childCount;
        for (int i = existing; i < SlotCount; i++)
        {
            GameObject entry = new GameObject($"Entry_{i + 1}");
            var rt = entry.AddComponent<RectTransform>();
            entry.transform.SetParent(entriesParent, false);
        }
    }

    public void Refresh()
    {
        if (entriesParent == null) return;

        var top = LeaderboardManager.Instance != null ? LeaderboardManager.Instance.Top(SlotCount) : new List<LeaderboardEntry>();
        int childCount = entriesParent.childCount;
        for (int i = 0; i < SlotCount; i++)
        {
            if (i >= childCount) break;
            var child = entriesParent.GetChild(i);
            var tmp = child.GetComponent<TextMeshProUGUI>();
            var legacy = child.GetComponent<Text>();
            if (i < top.Count)
            {
                var e = top[i];
                string timeStr = FormatTime(e.TimeSeconds);
                string line = string.Format(lineFormat, i + 1, e.Name, timeStr, e.DiceThrows, e.Points);
                if (tmp != null) tmp.text = line;
                else if (legacy != null) legacy.text = line;
            }
            else
            {
                if (tmp != null) tmp.text = "";
                else if (legacy != null) legacy.text = "";
            }
        }
    }

    private string FormatTime(float seconds)
    {
        int mins = Mathf.FloorToInt(seconds / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);
        return string.Format("{0:00}:{1:00}", mins, secs);
    }
}
