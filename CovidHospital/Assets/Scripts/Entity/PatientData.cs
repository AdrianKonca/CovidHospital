using System;
using UnityEngine;

namespace Entity
{
    [Serializable]
    public class PatientData : ScriptableObject
    {
        public int[] conditionsId;

        public float mentalHealth;
        public float comfort;
        public float covidProgress;
    }
}