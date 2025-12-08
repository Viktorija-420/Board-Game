using UnityEngine;
using TMPro;


public class NameScript : MonoBehaviour
{
   TextMeshPro tMP;
    void Start()
    {
        tMP = transform. Find("NameField").gameObject.GetComponent<TextMeshPro>();
    }

    public void SetName(string name)
    {
        tMP.text = name;
        tMP.color = new Color32((byte)Random.Range(0, 256), (byte)Random.Range(0, 256), (byte)Random.Range(0, 256), 255);
    }
}
