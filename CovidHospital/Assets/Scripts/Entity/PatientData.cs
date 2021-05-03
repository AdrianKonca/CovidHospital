using System;
using UnityEngine;

namespace Entity
{
    [Serializable]
    public class PatientData : ScriptableObject
    {
        public int[] conditionsId;
        public float comfort;
        public float covidProgress;

        public float GetSumOfImmunityDecrease()
        {
            //todo : sum all immunityDecrease fields from db
            return 0;
        }
    }
}