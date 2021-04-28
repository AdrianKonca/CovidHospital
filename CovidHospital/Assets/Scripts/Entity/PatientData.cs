using System;
using UnityEngine;

namespace Entity
{
    [Serializable]
    public class PatientData : ScriptableObject
    {
        public int[] conditionsId;

        public float covidProgress;

        //<0,1> 
        private float MaxNeedsValue = 100;

        public float comfort;
        public float hunger;
        public float hygiene;
        public float toilet;

        public PatientData()
        {
            covidProgress = 50;

            comfort = MaxNeedsValue;
            hunger = MaxNeedsValue;
            hygiene = MaxNeedsValue;
            toilet = MaxNeedsValue;
        }

        public float GetSumOfImmunityDecrease()
        {
            //todo : sum all immunityDecrease fields from db
            return 0;
        }

        public void AddCovidProgress(float progressToAdd)
        {
            covidProgress += progressToAdd;
        }

        public float GetNormalizedAverageNeeds()
        {
            return (comfort + hunger + hygiene + toilet) / (4 * MaxNeedsValue);
        }

        public void AddComfort(float delta)
        {
            var NewComfort = comfort + delta;
            comfort = Mathf.Clamp(NewComfort, 0, 1);
        }

        public void AddHunger(float delta)
        {
            var NewHunger = comfort + delta;
            hunger = Mathf.Clamp(NewHunger, 0, 1);
        }

        public void AddHygiene(float delta)
        {
            var NewHygiene = comfort + delta;
            hygiene = Mathf.Clamp(NewHygiene, 0, 1);
        }

        public void AddToilet(float delta)
        {
            var NewToilet = comfort + delta;
            toilet = Mathf.Clamp(NewToilet, 0, 1);
        }
    }
}