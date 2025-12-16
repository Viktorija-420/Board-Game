using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance { get; private set; }

    private string filePath;
    private float gameStartTime;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
        DontDestroyOnLoad(gameObject);
        filePath = Path.Combine(Application.persistentDataPath, "leaderboard.txt");
        gameStartTime = Time.time;
    }

    public void ResetTimer()
    {
        gameStartTime = Time.time;
    }

    // Called when a player finishes the game to log their stats
    public void RecordFinish(PlayerMovement pm)
    {
        if (pm == null) return;
        float timePlayed = Time.time - gameStartTime; // seconds
        int diceThrows = pm.GetDiceRolls();
        int points = Mathf.CeilToInt(1000f - (timePlayed * diceThrows / 10f));
        if (points < 0) points = 0;

        var entry = new LeaderboardEntry()
        {
            Name = pm.GetCharacterName().Replace('|', '_'),
            TimeSeconds = timePlayed,
            DiceThrows = diceThrows,
            Points = points,
            Timestamp = DateTime.UtcNow
        };

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.AppendAllLines(filePath, new[] { entry.ToLine() });
            Debug.Log($"[LeaderboardManager] Recorded finish: {entry}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[LeaderboardManager] Failed to write leaderboard: {e}");
        }
    }

    public List<LeaderboardEntry> ReadAll()
    {
        var list = new List<LeaderboardEntry>();
        if (!File.Exists(filePath)) return list;
        try
        {
            foreach (var line in File.ReadAllLines(filePath))
            {
                var e = LeaderboardEntry.FromLine(line);
                if (e != null) list.Add(e);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[LeaderboardManager] Failed to read leaderboard: {ex}");
        }
        return list;
    }

    public List<LeaderboardEntry> Top(int take = 7)
    {
        return ReadAll().OrderByDescending(e => e.Points).ThenBy(e => e.TimeSeconds).Take(take).ToList();
    }
}

[Serializable]
public class LeaderboardEntry
{
    public string Name;
    public float TimeSeconds;
    public int DiceThrows;
    public int Points;
    public DateTime Timestamp;

    // line format: Name|TimeSeconds|DiceThrows|Points|TimestampUtc
    public string ToLine()
    {
        return string.Join("|", new[] {
            Name,
            TimeSeconds.ToString(CultureInfo.InvariantCulture),
            DiceThrows.ToString(),
            Points.ToString(),
            Timestamp.ToString("o", CultureInfo.InvariantCulture)
        });
    }

    public static LeaderboardEntry FromLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line)) return null;
        var parts = line.Split('|');
        if (parts.Length < 5) return null;
        var e = new LeaderboardEntry();
        e.Name = parts[0];
        if (!float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float t)) t = 0f;
        e.TimeSeconds = t;
        if (!int.TryParse(parts[2], out int d)) d = 0;
        e.DiceThrows = d;
        if (!int.TryParse(parts[3], out int p)) p = 0;
        e.Points = p;
        if (DateTime.TryParse(parts[4], null, DateTimeStyles.RoundtripKind, out DateTime ts)) e.Timestamp = ts;
        return e;
    }

    public override string ToString()
    {
        return $"{Name} | {TimeSeconds}s | {DiceThrows} throws | {Points} pts";
    }
}
