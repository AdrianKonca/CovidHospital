using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Entity
{
    //nonserializable representation of data, contains sprites and other nonserializable stuff constructed based on PawnData
    public class PawnController : Pawn
    {
        public Slider slider;
        private PatientData _patientData;

        private void CovidProgress(float delta)
        {
            //pobrac wszystkie zarazy ich wagi
            float ImmunityDecrease = _patientData.GetSumOfImmunityDecrease();
            float AgeOffSet = 40f;

            //<0.6,1.8>
            float AgeMultiplayer = (float) Math.Tanh((PawnData.age - AgeOffSet) / 50) + 1f;

            //<1,1.8>
            float ImmunityMultiplayer = (float) Math.Tanh(ImmunityDecrease / 30) + 1f;

            var ProgressToAdd = delta * AgeMultiplayer * ImmunityMultiplayer;
            
            AddCovidProgress(ProgressToAdd);
        }

        private void AddCovidProgress(float ProgressToAdd)
        {
            _patientData.covidProgress += ProgressToAdd;
            slider.value = _patientData.covidProgress;
        }

        private void Awake()
        {
            PawnData = ScriptableObject.CreateInstance<PawnData>();
            PawnData.role = Role.Patient;
            PawnData.sex = (Sex) Random.Range(0, 1);

            //Todo : add Normal distr to age generation
            PawnData.age = Random.Range(18, 100);
            PawnData.alive = true;

            _patientData = ScriptableObject.CreateInstance<PatientData>();

            _patientData.comfort = 100;

            //todo: randomize covid progress
            _patientData.covidProgress = 50;

            // Debug.Log("Wiek " + PawnData.age);

            AddCovidProgress(0);
        }

        private void Update()
        {
            CovidProgress(0.02f);
        }
    }
}