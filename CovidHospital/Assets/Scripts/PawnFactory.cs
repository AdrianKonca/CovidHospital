using Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

public enum BodyPart
{
    Hair,
    Head,
    Body
}

public enum Direction
{
    Front,
    Back,
    Right,
    Left
}

public class PawnFactory : MonoBehaviour
{
    public GameObject PatientPrefab;
    public GameObject DoctorPrefab;
    public GameObject NursePrefab;

    public MapController MapController;
    public TimeController TimeController;
    public NurseManager NurseManager;
    public PatientSpawnerManager PatientSpawnerManager;

    private GameObject _pawns;
    private GameObject _patients;
    private GameObject _doctors;
    private GameObject _nurses;

    public void Awake()
    {
        _pawns = new GameObject("Pawns");
        _patients = new GameObject("Patients");
        _doctors = new GameObject("Doctors");
        _nurses = new GameObject("Nurses");

        _patients.transform.parent = _pawns.transform;
        _doctors.transform.parent = _pawns.transform;
        _nurses.transform.parent = _pawns.transform;
    }

    public void Patient(Vector3 coordinates)
    {
        var patient = Instantiate(PatientPrefab);
        var pc = patient.GetComponent<PatientController>();

        pc.Initialize(Role.Patient, TimeController, NurseManager,PatientSpawnerManager);
        patient.transform.position = coordinates;

        patient.transform.parent = _patients.transform;
    }

    public void Patient() =>
        Patient(Vector3.zero);

    public void Nurse()
    {
        var nurse = Instantiate(NursePrefab);
        var nurseController = nurse.GetComponent<NurseController>();
        nurseController.Initialize(NurseManager, MapController.GetClosestFurniture("Sofa", transform.position));
        nurse.transform.parent = _nurses.transform;
    }
}