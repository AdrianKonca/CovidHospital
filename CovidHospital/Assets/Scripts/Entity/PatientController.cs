using System;
using Pathfinding;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Random;

namespace Entity
{
    //nonserializable representation of data, contains sprites and other nonserializable stuff constructed based on PawnData
    public class PatientController : Pawn
    {
        public TimeController timeController;
        public PatientData patientData;
        public float covidRegressMultiplier;
        public float covidProgressMultiplier;
        public float covidUnableToMoveAfter;

        public NurseManager nurseManager;
        public GameObject toilet { get; set; }
        public GameObject bed { get; set; }
        public GameObject canteen { get; set; }
        public GameObject shower { get; set; }

        public bool requestForPepeSend = false;

        private AIDestinationSetter _aiDestinationSetter;

        public PatientController(PawnData data) : base()
        {
        }

        private void CovidRegress(float delta)
        {
            float AgeOffSet = 50f;

            float AgeMultiplier = (float) Math.Tanh((PawnData.age - AgeOffSet) / 80) * -1 + 1; //<0.4,1.4>
            float NeedsMultiplier = (float) Math.Max(patientData.GetNormalizedAverageNeeds(), 0.2);

            var ProgressToSubtract = delta * AgeMultiplier * NeedsMultiplier;

            patientData.AddCovidProgress(ProgressToSubtract * covidRegressMultiplier);
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
        }

        public void Initialize(Role role, TimeController timeController, NurseManager nurseManager)
        {
            Initialize(role);
            patientData = ScriptableObject.CreateInstance<PatientData>();
            this.nurseManager = nurseManager;
            this.timeController = timeController;
            timeController.OnDayIncrease += TimeControllerOnOnDayIncrease;
            timeController.OnHourIncrease += TimeControllerOnOnHourIncrease;

            covidRegressMultiplier = 1;
            covidProgressMultiplier = 1;
            covidUnableToMoveAfter = 50;

            patientData.OnLowComfort += PatientDataOnLowComfort;
            patientData.OnLowHunger += PatientDataOnLowHunger;
            patientData.OnLowHygiene += PatientDataOnLowHygiene;
            patientData.OnLowToilet += PatientDataOnLowToilet;
            MapController.Instance().OnFurnitureBuilt += OnFurnitureBuilt;
            TimeController.Instance().OnHourIncrease += OnHourIncrease;

            _aiDestinationSetter = GetComponent<AIDestinationSetter>();
        }

        private void OnHourIncrease(int h)
        {
            if (bed == null)
                SelectFurniture(MapController.Instance().GetClosestFreeFurniture("Bed", transform.position));
            //if (toilet == null && bed != null)
            //    SelectFurniture(MapController.Instance().GetClosestFreeFurniture("Toilet", bed.transform.position));
            //if (shower == null && bed != null)
            //    SelectFurniture(MapController.Instance().GetClosestFreeFurniture("Shower", bed.transform.position));
        }

        private void OnFurnitureBuilt(GameObject furniture)
        {
            SelectFurniture(furniture);
        }

        private void SelectFurniture(GameObject furniture)
        {
            if (furniture == null)
                return;
            var furnitureOwner = furniture.GetComponent<FurnitureController>().owner;
            if (furniture.name == "Bed" && bed == null && furnitureOwner == null)
            {
                bed = furniture;
                bed.GetComponent<FurnitureController>().owner = this;
                ReturnToBed();
            }
            //else if (furniture.name == "Toilet" && bed == null && furnitureOwner == null)
            //{
            //    toilet = furniture;
            //    toilet.GetComponent<FurnitureController>().owner = this;
            //}
            //else if (furniture.name == "Shower" && bed == null && furnitureOwner == null)
            //{
            //    shower = furniture;
            //    shower.GetComponent<FurnitureController>().owner = this;
            //}
        }

        public void ReturnToBed()
        {
            _aiDestinationSetter.target = bed.transform;
        }

        private void PatientDataOnLowHygiene(object sender, EventArgs e)
        {
            if (patientData.covidProgress < covidUnableToMoveAfter)
                _aiDestinationSetter.target = shower.transform;
            // else if (requestForPEPESend)
            //     nurseManager.AddPawnToQue(this);
        }

        private void PatientDataOnLowHunger(object sender, EventArgs e)
        {
            if (patientData.covidProgress < covidUnableToMoveAfter)
                _aiDestinationSetter.target = canteen.transform;
            // else if (requestForPEPESend)
            //     nurseManager.AddPawnToQue(this);
        }

        private void PatientDataOnLowToilet(object sender, EventArgs e)
        {
            if (patientData.covidProgress < covidUnableToMoveAfter)
                _aiDestinationSetter.target = toilet.transform;
            else if (!requestForPepeSend)
            {
                nurseManager.AddPawnToQue(this);
                requestForPepeSend = true;
            }
        }

        private void PatientDataOnLowComfort(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }


        private void TimeControllerOnOnHourIncrease(int h)
        {
            CovidRegress(-0.7f);
            CovidProgress(0.7f);
            if (timeController.isDay)
            {
                patientData.AddHunger(Range(-3f, -1f));
                patientData.AddToilet(Range(-5f, -1f));
                patientData.AddHygiene(Range(-5f, -1f));
            }
            else
            {
                patientData.AddToilet(Range(-2f, -1f));
                patientData.AddHygiene(Range(-2f, -1f));
                patientData.AddHunger(Range(-2f, -1f));
            }
        }

        private void TimeControllerOnOnDayIncrease(long d)
        {
            covidRegressMultiplier += 0.025f;
        }
    }
}