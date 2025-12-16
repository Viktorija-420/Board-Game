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
                if (!dieRollScript.isLanded)
                {
                    // If the collider the trigger reports is the Board, determine which die child collider
                    // is contacting the board (choose the lowest collider on the die).
                    if (sideCollider != null && sideCollider.name == "Board")
                    {
                        GameObject dieObj = dieRollScript.gameObject;
                        Collider[] dieColliders = dieObj.GetComponentsInChildren<Collider>();
                        Collider best = null;
                        float bestMinY = float.MaxValue;
                        foreach (var c in dieColliders)
                        {
                            // skip triggers on the die itself
                            if (c.isTrigger) continue;
                            // check intersection with board bounds
                            try
                            {
                                if (c.bounds.Intersects(sideCollider.bounds))
                                {
                                    float minY = c.bounds.min.y;
                                    if (minY < bestMinY)
                                    {
                                        bestMinY = minY;
                                        best = c;
                                    }
                                }
                            }
                            catch (System.Exception) { /* ignore bounds issues */ }
                        }

                        if (best != null)
                        {
                            string faceName = best.gameObject.name;
                            int parsed = 0;
                            if (int.TryParse(faceName, out parsed))
                            {
                                dieRollScript.ReportLanded(parsed.ToString());
                                Debug.Log($"[SideDetectScript] Die landed on face {dieRollScript.diceFaceNum} (detected via Board collision)");
                            }
                            else
                            {
                                // extract digits
                                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                                foreach (char c in faceName)
                                {
                                    if (char.IsDigit(c)) sb.Append(c);
                                }
                                if (sb.Length > 0 && int.TryParse(sb.ToString(), out parsed))
                                {
                                    dieRollScript.ReportLanded(parsed.ToString());
                                    Debug.Log($"[SideDetectScript] Die landed on face {dieRollScript.diceFaceNum} (extracted from '{faceName}')");
                                }
                                else
                                {
                                    Debug.LogWarning($"[SideDetectScript] Board collision detected but die child '{faceName}' contains no digits");
                                }
                            }
                        }
                        else
                        {
                            Debug.LogWarning("[SideDetectScript] Board collision detected but no die child collider intersects the board bounds.");
                        }
                    }
                    else
                    {
                        // Only accept face names that contain a number
                        string faceName = sideCollider.name;
                        int parsed = 0;
                        // try direct parse
                        if (int.TryParse(faceName, out parsed))
                        {
                            dieRollScript.ReportLanded(parsed.ToString());
                            Debug.Log($"[SideDetectScript] Die landed on face {dieRollScript.diceFaceNum}");
                        }
                        else
                        {
                            // try to extract digits
                            System.Text.StringBuilder sb = new System.Text.StringBuilder();
                            foreach (char c in faceName)
                            {
                                if (char.IsDigit(c)) sb.Append(c);
                            }
                            if (sb.Length > 0 && int.TryParse(sb.ToString(), out parsed))
                            {
                                dieRollScript.ReportLanded(parsed.ToString());
                                Debug.Log($"[SideDetectScript] Die landed on face {dieRollScript.diceFaceNum} (extracted from '{faceName}')");
                            }
                            else
                            {
                                Debug.LogWarning($"[SideDetectScript] Ignoring non-face collider '{faceName}' when die stopped.");
                            }
                        }
                    }
                }
            }
            else
            {
                if (dieRollScript.isLanded)
                {
                    Debug.Log("[SideDetectScript] Die no longer stationary");
                }
                dieRollScript.isLanded = false;
            }
        }
    }
}
