using System;
using Pathfinding;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Random;

namespace Entity
{
    //nonserializable representation of data, contains sprites and other nonserializable stuff constructed based on PawnData
    public class PawnController : Pawn
    {
        public TimeController timeController;
        public Slider slider;
        public PatientData patientData;
        public float covidRegressMultiplier = 1;
        public float covidProgressMultiplier = 1;
        //public static int Range(float minInclusive, float maxExclusive);

        public GameObject toilet;
        public GameObject bed;
        public GameObject canteen;
        public GameObject shower;

        private AIDestinationSetter _aiDestinationSetter;

        private void CovidRegress(float delta)
        {
            float AgeOffSet = 50f;

            float AgeMultiplier = (float) Math.Tanh((PawnData.age - AgeOffSet) / 80) * -1 + 1; //<0.4,1.4>
            float NeedsMultiplier = (float) Math.Max(patientData.GetNormalizedAverageNeeds(), 0.2);

            var ProgressToSubtract = delta * AgeMultiplier * NeedsMultiplier;

            patientData.AddCovidProgress(ProgressToSubtract * covidRegressMultiplier);
            slider.value = patientData.covidProgress;
        }

        private void CovidProgress(float delta)
        {
            float ImmunityDecrease = patientData.GetSumOfImmunityDecrease();
            float AgeOffSet = 40f;

            float AgeMultiplier = (float) Math.Tanh((PawnData.age - AgeOffSet) / 50) + 1f; //<0.6,1.8>

            float ImmunityMultiplier = (float) Math.Tanh(ImmunityDecrease / 30) + 1f; //<1,1.8>
            if (ImmunityDecrease > 100)
            {
                ImmunityMultiplier *= 2;
            }

            float NeedsMultiplier = Math.Min(1 / patientData.GetNormalizedAverageNeeds() / 2 + 0.5f, 2f); //<1,2>

            var ProgressToAdd = delta * AgeMultiplier * ImmunityMultiplier * NeedsMultiplier;

            patientData.AddCovidProgress(ProgressToAdd * covidProgressMultiplier);
            slider.value = patientData.covidProgress;
        }

        private void Awake()
        {
            PawnData = ScriptableObject.CreateInstance<PawnData>();
            patientData = ScriptableObject.CreateInstance<PatientData>();

            timeController.OnDayIncrease += TimeControllerOnOnDayIncrease;
            timeController.OnHourIncrease += TimeControllerOnOnHourIncrease;

            patientData.OnLowComfort += PatientDataOnLowComfort;
            patientData.OnLowHunger += PatientDataOnLowHunger;
            patientData.OnLowHygiene += PatientDataOnLowHygiene;
            patientData.OnLowToilet += PatientDataOnLowToilet;

            _aiDestinationSetter = GetComponent<AIDestinationSetter>();
            _aiDestinationSetter.target = bed.transform;
        }

        public void ReturnToBed()
        {
            _aiDestinationSetter.target = bed.transform;
        }

        private void PatientDataOnLowHygiene(object sender, EventArgs e)
        {
            _aiDestinationSetter.target = shower.transform;
        }

        private void PatientDataOnLowHunger(object sender, EventArgs e)
        {
            _aiDestinationSetter.target = canteen.transform;
        }

        private void PatientDataOnLowComfort(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void PatientDataOnLowToilet(object sender, EventArgs e)
        {
            _aiDestinationSetter.target = toilet.transform;
        }
        

        private void TimeControllerOnOnHourIncrease(int h)
        {
            //todo poprawic potrzeby
            
            CovidRegress(-0.7f);
            CovidProgress(0.7f);
            patientData.AddToilet(Range(-5f,-1f));
            patientData.AddHygiene(Range(-5f, -1f));
        }

        private void TimeControllerOnOnDayIncrease(long d)
        {
            covidRegressMultiplier += 0.025f;
        }
    }
}