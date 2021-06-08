using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public GameObject textPrefabParent;
    float timeToDestroy = 1.3f;
    // Start is called before the first frame update


    void DisplayText(string text, int x, int y, Color color)
    {
        var prefab = Instantiate(textPrefabParent, new Vector3(x, y), Quaternion.identity);
        var mesh = textPrefabParent.GetComponentInChildren<TextMesh>();
        mesh.text = text;
        mesh.color = color;

    }
    void DeleteFloatingText()
    {
        Destroy(textPrefabParent, timeToDestroy);
    }
}
