using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public SaveLoadScript saveLoadScript;
    public FadeScript fadeScript;

    // Quit the game
    public void CloseGame()
    {
        StartCoroutine(Delay("quit", -1, ""));
    }

    // Go to main menu
    public void GoToMenu()
    {
        StartCoroutine(Delay("menu", -1, ""));
    }

    // ➜ Go to settings scene (NEW)
    public void GoToSettings()
    {
        StartCoroutine(Delay("settings", -1, ""));
    }

    public IEnumerator Delay(string command, int characterIndex, string characterName)
    {
        // QUIT GAME
        if (string.Equals(command, "quit", System.StringComparison.OrdinalIgnoreCase))
        {
            yield return fadeScript.FadeOut(0.1f);
            PlayerPrefs.DeleteAll();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        // START GAME
        else if (string.Equals(command, "play", System.StringComparison.OrdinalIgnoreCase))
        {
            yield return fadeScript.FadeOut(0.1f);
            saveLoadScript.SaveGame(characterIndex, characterName);
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }

        // MAIN MENU
        else if (string.Equals(command, "menu", System.StringComparison.OrdinalIgnoreCase))
        {
            yield return fadeScript.FadeOut(0.1f);
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }

        // ➜ SETTINGS SCENE (NEW)
        else if (string.Equals(command, "settings", System.StringComparison.OrdinalIgnoreCase))
        {
            yield return fadeScript.FadeOut(0.1f);

            // ⚠️ Change this index if your settings scene has another index in Build Settings
            SceneManager.LoadScene(2, LoadSceneMode.Single);
        }
    }
}
