using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [Header("Dependencies")]
    public SaveLoadScript saveLoadScript;
    public FadeScript fadeScript;

    // -----------------------------
    // PUBLIC BUTTON METHODS
    // -----------------------------

    // Quit the game
    public void CloseGame()
    {
        StartCoroutine(Delay("quit"));
    }

    // Go to Main Menu
    public void GoToMenu()
    {
        StartCoroutine(Delay("menu"));
    }

    // Go to Settings
    public void GoToSettings()
    {
        StartCoroutine(Delay("settings"));
    }

    // Start/Play Level1 from MainMenu or ChooseCharacter
    public void PlayGame()
    {
        StartCoroutine(Delay("play"));
    }

    // -----------------------------
    // CORE DELAY METHOD
    // Supports optional save parameters
    // -----------------------------
    public IEnumerator Delay(string command, int characterIndex = -1, string characterName = "")
    {
        // Save game if SaveLoadScript is assigned
        if (saveLoadScript != null && characterIndex >= 0)
        {
            saveLoadScript.SaveGame(characterIndex, characterName);
        }

        // Fade out if FadeScript assigned
        if (fadeScript != null)
        {
            yield return fadeScript.FadeOut(0.1f);
        }

        // Reset time scale before scene change
        Time.timeScale = 1f;

        // Handle scene loading
        switch (command.ToLower())
        {
            case "play":
                SceneManager.LoadScene("Level1", LoadSceneMode.Single);
                break;

            case "menu":
                SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
                break;

            case "settings":
                SceneManager.LoadScene("SettingsScene", LoadSceneMode.Single);
                break;

            case "quit":
                PlayerPrefs.DeleteAll();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                break;

            default:
                Debug.LogWarning("[SceneChanger] Unknown command: " + command);
                break;
        }
    }
}
