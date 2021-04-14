using System;
using UnityEngine;

namespace Entity
{
    [Serializable]
    public class DiseaseData : ScriptableObject
    {
        public int id;
        public float immunityDecrease;
        public string diseaseName;
    }
}
