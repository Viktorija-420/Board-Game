using UnityEngine;
using System.IO;

public class PlayerScript : MonoBehaviour
{
    public GameObject[] playerPrefabs;
    public GameObject spawnPoint;
    private const string textFileName = "PlayerNames";

    [HideInInspector] public PlayerMovement mainPlayerMovement; // runtime instance reference

    void Start()
    {
        // Spawn main player
        int characterIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        GameObject mainCharacter = Instantiate(
            playerPrefabs[characterIndex], spawnPoint.transform.position, Quaternion.identity);

        mainCharacter.GetComponent<NameScript>().SetName(
            PlayerPrefs.GetString("PlayerName", "John Doe"));

        // Get the PlayerMovement component for runtime reference
        mainPlayerMovement = mainCharacter.GetComponent<PlayerMovement>();

        // Ensure TurnManager exists and mark main player
        if (TurnManager.Instance == null)
        {
            GameObject tm = new GameObject("TurnManager");
            tm.AddComponent<TurnManager>();
        }
        if (mainPlayerMovement != null)
        {
            mainPlayerMovement.MarkAsPlayer();
            TurnManager.Instance.RegisterPlayer(mainPlayerMovement);
        }
        // Spawn other players
        int playerCount = PlayerPrefs.GetInt("PlayerCount", 2);
        string[] nameArray = ReadLinesFromFile(textFileName);

        for (int i = 0; i < playerCount - 1; i++)
        {
            spawnPoint.transform.position += new Vector3(0.2f, 0, 0.08f);
            int index = Random.Range(0, playerPrefabs.Length);
            GameObject otherPlayer = Instantiate(
                playerPrefabs[index], spawnPoint.transform.position, Quaternion.identity);

            otherPlayer.GetComponent<NameScript>().SetName(
                nameArray[Random.Range(0, nameArray.Length)]);

            // register NPC player movement with TurnManager
            var pm = otherPlayer.GetComponent<PlayerMovement>();
            if (pm != null) TurnManager.Instance.RegisterPlayer(pm);
        }

    }

    string[] ReadLinesFromFile(string fileName)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(fileName);
        
        if (textAsset != null)
        {
            return textAsset.text.Split(new[] { '\r', '\n' }, 
                System.StringSplitOptions.RemoveEmptyEntries);
        }
        else
        {
            Debug.LogWarning("File not found: " + fileName);
            return new string[0];
        }
    }
}