using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Entity
{
    [Serializable]
    public class PatientData : ScriptableObject
    {
        public List<int> conditionsId;
        public int conditionsValue;

        public float covidProgress;

        public float comfort;
        public float hunger;
        public float hygiene;
        public float toilet;

        //<0,1> 
        private float MaxNeedsValue = 100;

        public PatientData()
        {
            covidProgress = 50;

            comfort = MaxNeedsValue;
            hunger = MaxNeedsValue;
            hygiene = MaxNeedsValue;
            toilet = MaxNeedsValue;

            conditionsId = new List<int>();
            conditionsValue = 0;
        }

        private void OnEnable()
        {
            //hiv
            if (Random.Range(0, 99) < 2)
            {
                Debug.Log("HIV");
                conditionsId.Add(1);
                conditionsValue += 7;
            }

            //diabetes
            if (Random.Range(0, 99) < 15)
            {
                Debug.Log("Diab");
                conditionsId.Add(2);
                conditionsValue += 5;
            }

            //obesity
            if (Random.Range(0, 99) < 40)
            {
                Debug.Log("Obe");
                conditionsId.Add(3);
                conditionsValue += 2;
            }

            //lung_cancer
            if (Random.Range(0, 99) < 10)
            {
                Debug.Log("Canc");
                conditionsId.Add(4);
                conditionsValue += 100;
            }

            //asthma
            if (Random.Range(0, 99) < 5)
            {
                Debug.Log("Asth");
                conditionsId.Add(5);
                conditionsValue += 4;
            }

            //nicotineAddiction
            if (Random.Range(0, 99) < 22)
            {
                Debug.Log("Nico");
                conditionsId.Add(6);
                conditionsValue += 2;
            }

            //alcoholAddiction
            if (Random.Range(0, 99) < 25)
            {
                Debug.Log("Alco");
                conditionsId.Add(7);
                conditionsValue += 2;
            }
        }

        public event EventHandler OnLowComfort;
        public event EventHandler OnLowHunger;
        public event EventHandler OnLowHygiene;
        public event EventHandler OnLowToilet;

        public float GetSumOfImmunityDecrease()
        {
            return conditionsValue;
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
            comfort = Mathf.Clamp(NewComfort, 0, MaxNeedsValue);

            if (comfort < MaxNeedsValue / 2f) OnLowComfort?.Invoke(this, EventArgs.Empty);
        }

        public void AddHunger(float delta)
        {
            var NewHunger = hunger + delta;
            hunger = Mathf.Clamp(NewHunger, 0, MaxNeedsValue);

            if (hunger < MaxNeedsValue / 2f) OnLowHunger?.Invoke(this, EventArgs.Empty);
        }

        public void AddHygiene(float delta)
        {
            var NewHygiene = hygiene + delta;
            hygiene = Mathf.Clamp(NewHygiene, 0, MaxNeedsValue);

            if (hygiene < MaxNeedsValue / 2f) OnLowHygiene?.Invoke(this, EventArgs.Empty);
        }

        public void AddToilet(float delta)
        {
            var NewToilet = toilet + delta;
            toilet = Mathf.Clamp(NewToilet, 0, MaxNeedsValue);

            if (toilet < MaxNeedsValue / 2f) OnLowToilet?.Invoke(this, EventArgs.Empty);
        }

        public void ResetToilet()
        {
            toilet = MaxNeedsValue;
        }

        public void ResetHygiene()
        {
            hygiene = MaxNeedsValue;
        }

        public void ResetHunger()
        {
            hunger = MaxNeedsValue;
        }

        public void ResetComfort()
        {
            comfort = MaxNeedsValue;
        }

        public void SetComfortNormalized(float scale)
        {
            comfort = MaxNeedsValue * scale;
        }
    }
}