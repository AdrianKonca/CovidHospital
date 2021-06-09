using UnityEngine;

public class FloatingTextManager : MonoBehaviour
{
    // Start is called before the first frame update

    private static FloatingTextManager _instance;
    public GameObject textPrefabParent;
    private readonly float timeToDestroy = 5f;

    private void Awake()
    {
        _instance = this;
    }

    public static FloatingTextManager I()
    {
        return _instance;
    }

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