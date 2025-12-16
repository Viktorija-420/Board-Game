using UnityEngine;
using TMPro;

public class NameScript : MonoBehaviour
{
    TextMeshPro tMP;


    void Awake()
    {
      tMP = transform.Find("NameField").gameObject.GetComponent<TextMeshPro>();
    }

    public void SetName(string name)
    {
      tMP.text = name;
      currentName = name;
        tMP.color = new Color32(
        (byte)Random.Range(0, 256), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
    }

    private string currentName = "";

    public string GetName()
    {
        if (!string.IsNullOrEmpty(currentName)) return currentName;
        if (tMP != null) return tMP.text;
        return gameObject.name;
    }
}
