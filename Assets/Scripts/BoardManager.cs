using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public Transform[] tiles;

    void Awake()
    {
        tiles = new Transform[38]; // Tile 0 to Tile 37

        for (int i = 0; i < tiles.Length; i++)
        {
            string tileName = i == 0 ? "Tile" : $"Tile ({i})";
            GameObject tileObj = GameObject.Find(tileName);
            if (tileObj != null)
                tiles[i] = tileObj.transform;
            else
                Debug.LogWarning("Tile not found: " + tileName);
        }
    }
}
