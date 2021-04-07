using Entity;
using UnityEngine;
using UnityEditor;
 
public class DiseaseDataAsset
{
    [MenuItem("Assets/Create/DiseaseData")]
    public static void CreateAsset ()
    {
        ScriptableObjectUtility.CreateAsset<DiseaseData> ();
    }
}