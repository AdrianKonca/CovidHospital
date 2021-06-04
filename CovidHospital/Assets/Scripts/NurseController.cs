using System;
using Entity;
using Pathfinding;
using UnityEngine;

public class NurseController : MonoBehaviour
{
    private NurseManager nurseManager;
    private GameObject restRoom;
    private AIDestinationSetter _aiDestinationSetter;
    private bool bussy = false;
    private Pawn p;

    public void Initialize(NurseManager nurseManager, GameObject restRoom)
    {
        this.nurseManager = nurseManager;
        this.restRoom = restRoom;
        nurseManager.OnEnqueue += NurseManagerOnEnqueue;
        _aiDestinationSetter = GetComponent<AIDestinationSetter>();
        _aiDestinationSetter.target = restRoom.transform;
    }

    private void NurseManagerOnEnqueue(object sender, EventArgs e)
    {
        if (bussy)
            return;

        p = nurseManager.RemovePawnFromQue();
        if (p)
        {
            _aiDestinationSetter.target = p.transform;
            bussy = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var patient = collision.gameObject.GetComponent<PawnController>();
            if (patient != p || patient == null)
                return;

            patient.patientData.ResetToilet();
            patient.patientData.ResetHygiene();
            patient.patientData.ResetHunger();
            patient.requestForPepeSend = false;
            bussy = false;

            p = nurseManager.RemovePawnFromQue();
            if (p)
            {
                _aiDestinationSetter.target = p.transform;
                bussy = true;
            }
            else
            {
                _aiDestinationSetter.target = restRoom.transform;
                bussy = false;
            }
        }
    }
}