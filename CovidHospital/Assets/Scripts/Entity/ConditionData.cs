using System;
using UnityEngine;

namespace Entity
{
    [Serializable]
    public class ConditionData : ScriptableObject
    {
        public int id;
        public float immunityDecrease;
        public string diseaseName;
    }
}