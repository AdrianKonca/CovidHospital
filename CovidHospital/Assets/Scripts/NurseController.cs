using System;
using Entity;
using Pathfinding;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class NurseController : Pawn
{
    private NurseManager nurseManager;
    private GameObject sofa;
    private AIDestinationSetter _aiDestinationSetter;
    public bool bussy = false;
    private Pawn _pawn;
    private TimeController _timeController;

    public PatientController patient;

    public void Initialize(NurseManager nurseManager, TimeController timeController)
    {
        Initialize(Role.Nurse);
        this.nurseManager = nurseManager;
        nurseManager.OnEnqueue += NurseManagerOnEnqueue;
        _timeController = timeController;

        _timeController.OnHourIncrease += OnHourIncrease;
        _timeController.OnMinuteIncrease += OnMinuteIncrease;
        MapController.Instance().OnFurnitureBuilt += OnFurnitureBuilt;
        _aiDestinationSetter = GetComponent<AIDestinationSetter>();
    }

    private void OnMinuteIncrease(int m)
    {
        if (bussy)
            return;

        _pawn = nurseManager.RemovePawnFromQue();
        if (_pawn)
        {
            _aiDestinationSetter.target = _pawn.transform;
            bussy = true;
        }
    }

    private void OnFurnitureBuilt(GameObject furniture)
    {
        if (furniture == null)
            return;

        if (sofa == null)
            SelectFurniture(MapController.Instance().GetClosestFreeFurniture("Sofa", transform.position));
    }

    private void OnHourIncrease(int h)
    {
        if (sofa == null)
            SelectFurniture(MapController.Instance().GetClosestFreeFurniture("Sofa", transform.position));

        if (bussy)
            return;

        _pawn = nurseManager.RemovePawnFromQue();
        if (_pawn)
        {
            _aiDestinationSetter.target = _pawn.transform;
            bussy = true;
        }
    }

    private void SelectFurniture(GameObject furniture)
    {
        if (furniture == null)
            return;

        if (furniture.name == "Sofa" && sofa == null)
        {
            sofa = furniture;
        }
    }

    private void NurseManagerOnEnqueue(object sender, EventArgs e)
    {
        if (bussy)
            return;

        _pawn = nurseManager.RemovePawnFromQue();
        if (_pawn)
        {
            _aiDestinationSetter.target = _pawn.transform;
            bussy = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            patient = collision.gameObject.GetComponent<PatientController>();
            if (patient != _pawn || patient == null)
                return;

            patient.patientData.ResetToilet();
            patient.patientData.ResetHygiene();
            patient.patientData.ResetHunger();
            patient.requestForPepeSend = false;
            bussy = false;

            if (sofa)
                _aiDestinationSetter.target = sofa.transform;
        }
    }

    private void Update()
    {
        if (transform == _aiDestinationSetter.target)
        {
            Debug.Log("XD");
        }
    }
}