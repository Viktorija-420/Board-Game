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
            if(dieRollScript.isLanded)
                rolledNumberText.text = dieRollScript.diceFaceNum;
            
            else
                rolledNumberText.text = "?";
        }else
            Debug.Log("DieRollScript not found");
    }
}
