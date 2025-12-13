using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;
    public Transform[] Tiles;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
}
