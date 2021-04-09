using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Entity
{
    //nonserializable representation of data, contains sprites and other nonserializable stuff constructed based on PawnData
    public class PawnController : Pawn
    {
        private PatientData _patientData;

        public void ChangeCovidProgress(float delta)
        {
            _patientData.covidProgress += delta * 100 / _patientData.comfort;
        }

        private void Awake()
        {
            _pawnData = ScriptableObject.CreateInstance<PawnData>();
            _pawnData.role = Role.Patient;
            _pawnData.sex = (Sex) Random.Range(0, 1);

            //Todo : add Normal distr to age generation;
            _pawnData.age = Random.Range(18, 100);
            _pawnData.alive = true;

            _patientData = ScriptableObject.CreateInstance<PatientData>();
            _patientData.comfort = 100;
            _patientData.mentalHealth = 100;
            _patientData.covidProgress = 0;

            Debug.Log(_patientData.comfort);
            Debug.Log(_pawnData.age); 
        }
    }
}