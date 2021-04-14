using Entity;
using UnityEngine;
using UnityEditor;
 
public class COnditionDataAsset
{
    [MenuItem("Assets/Create/DiseaseData")]
    public static void CreateAsset ()
    {
        ScriptableObjectUtility.CreateAsset<ConditionData> ();
    }
}