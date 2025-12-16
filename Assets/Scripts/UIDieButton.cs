using UnityEngine;
using UnityEngine.UI;

// Attach this to a UI Button and assign the die GameObject's DieRollScript
public class UIDieButton : MonoBehaviour
{
    public Button rollButton;
    public DieRollScript die;

    void Start()
    {
        if (rollButton != null) rollButton.onClick.AddListener(OnClickRoll);
        if (die == null) die = FindObjectOfType<DieRollScript>();
    }

    void OnDestroy()
    {
        if (rollButton != null) rollButton.onClick.RemoveListener(OnClickRoll);
    }

    public void OnClickRoll()
    {
        if (die != null) die.PlayerRoll();
    }
}
