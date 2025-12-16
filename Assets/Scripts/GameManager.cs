using UnityEngine;

public class GameManager : MonoBehaviour
{
    public DieRollScript dieRoll;
    public PlayerScript playerScript; // reference the PlayerScript, not a prefab

    void Update()
    {
        // TurnManager now centrally handles consuming die results and starting movement.
        // GameManager no longer auto-consumes die landings to ensure the human player must roll manually.
    }
}
