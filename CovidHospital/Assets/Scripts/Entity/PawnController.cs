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
        public PatientData patientData;

        private void CovidRegress(float delta)
        {
            float AgeOffSet = 50f;

            float AgeMultiplayer = (float) Math.Tanh((PawnData.age - AgeOffSet) / 80) * -1 + 1; //<0.4,1.4>
            float NeedsMultiplayer = (float) Math.Max(patientData.GetNormalizedAverageNeeds(), 0.2);

            var ProgressToSubtract = delta * AgeMultiplayer * NeedsMultiplayer;

            patientData.AddCovidProgress(ProgressToSubtract);
            slider.value = patientData.covidProgress;
        }


        private void CovidProgress(float delta)
        {
            float ImmunityDecrease = patientData.GetSumOfImmunityDecrease();
            float AgeOffSet = 40f;

            float AgeMultiplayer = (float) Math.Tanh((PawnData.age - AgeOffSet) / 50) + 1f; //<0.6,1.8>

            float ImmunityMultiplayer = (float) Math.Tanh(ImmunityDecrease / 30) + 1f; //<1,1.8>

            float NeedsMultiplayer = Math.Min(1 / patientData.GetNormalizedAverageNeeds() / 2 + 0.5f, 2f); //<1,2>

            var ProgressToAdd = delta * AgeMultiplayer * ImmunityMultiplayer * NeedsMultiplayer;

            patientData.AddCovidProgress(ProgressToAdd);
            slider.value = patientData.covidProgress;
        }

        private void Awake()
        {
            PawnData = ScriptableObject.CreateInstance<PawnData>();

            patientData = ScriptableObject.CreateInstance<PatientData>();
        }

        private void Update()
        {
            CovidRegress(-0.02f);
            CovidProgress(0.02f);
        }
    }
}