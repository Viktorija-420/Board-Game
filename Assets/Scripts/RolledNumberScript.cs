using UnityEngine;
using UnityEngine.UI;

public class RolledNumberScript : MonoBehaviour
{
    DieRollScript dieRollScript;
    [SerializeField] Text rolledNumberText;

    void Awake()
    {
        dieRollScript = FindFirstObjectByType<DieRollScript>();
    }

    void Update()
    {
        if (dieRollScript != null)
        {
            // Color the rolled number green when it's the human player's turn
            bool isPlayersTurn = TurnManager.Instance != null && TurnManager.Instance.IsPlayersMove;
            rolledNumberText.color = isPlayersTurn ? Color.green : Color.white;

            if (dieRollScript.isLanded)
                rolledNumberText.text = dieRollScript.diceFaceNum;
            else
                rolledNumberText.text = "?";
        }else
            Debug.Log("DieRollScript not found");
    }
}
