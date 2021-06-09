using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextManager : MonoBehaviour
{
    public GameObject textPrefabParent;
    float timeToDestroy = 5f;
    // Start is called before the first frame update

    static FloatingTextManager _instance = null;
    private void Awake()
    {
        _instance = this;
    }

    static public FloatingTextManager I() =>
        _instance;
    public void DisplayText(string text, Vector3 position, Color color)
    {
        var prefab = Instantiate(textPrefabParent, position + new Vector3(0, 0, 10), Quaternion.identity);
        var mesh = prefab.GetComponentInChildren<TextMesh>();
        mesh.text = text;
        mesh.text = text;
        mesh.color = color;
        var mr = prefab.GetComponentInChildren<MeshRenderer>();
        mr.sortingOrder = 32001;
        Destroy(prefab, timeToDestroy);

    }
}
