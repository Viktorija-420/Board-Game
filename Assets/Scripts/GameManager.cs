// using UnityEngine;

// public class GameManager : MonoBehaviour
// {
//     public DieRollScript dieRoll;
//     public PlayerScript playerScript; // reference the PlayerScript, not a prefab

//     void Update()
//     {
//         // Check if dice landed and player exists
//         if (dieRoll != null && playerScript != null && playerScript.mainPlayerMovement != null && dieRoll.isLanded)
//         {
//             if (!string.IsNullOrEmpty(dieRoll.diceFaceNum))
//             {
//                 int steps = int.Parse(dieRoll.diceFaceNum); // convert dice result to int
//                 playerScript.mainPlayerMovement.MovePlayer(steps);
//                 dieRoll.isLanded = false; // reset dice state
//             }
//         }
//     }
// }
