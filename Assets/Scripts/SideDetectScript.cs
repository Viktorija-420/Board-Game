using UnityEngine;

public class SideDetectScript : MonoBehaviour
{
    DieRollScript dieRollScript;

    void Awake()
    {
        dieRollScript = FindFirstObjectByType<DieRollScript>();
    }

    private void OnTriggerStay(Collider sideCollider)
    {
        if (dieRollScript != null)
        {
            if(dieRollScript.GetComponent<Rigidbody>().linearVelocity == Vector3.zero){
                dieRollScript.isLanded = true;
                dieRollScript.diceFaceNum = sideCollider.name;
            }else
                dieRollScript.isLanded = false;
        }
    }
}
