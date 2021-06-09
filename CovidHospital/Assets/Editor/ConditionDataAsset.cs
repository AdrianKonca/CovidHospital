using Entity;
using UnityEditor;

public class ConditionDataAsset
{
    [MenuItem("Assets/Create/DiseaseData")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<ConditionData>();
    }
}