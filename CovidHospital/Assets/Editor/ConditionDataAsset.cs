using Entity;
using UnityEngine;
using UnityEditor;
 
public class ConditionDataAsset
{
    [MenuItem("Assets/Create/DiseaseData")]
    public static void CreateAsset ()
    {
        ScriptableObjectUtility.CreateAsset<ConditionData> ();
    }
}