using System;
using Entity;
using Pathfinding;
using UnityEngine;

public class NurseController : MonoBehaviour
{
    public NurseManager nurseManager;
    public GameObject restRoom;
    private AIDestinationSetter _aiDestinationSetter;
    private bool bussy = false;


    private void Awake()
    {
        nurseManager.OnEnqueue += NurseManagerOnEnqueue;
        _aiDestinationSetter = GetComponent<AIDestinationSetter>();
        _aiDestinationSetter.target = restRoom.transform;
    }

    private void NurseManagerOnEnqueue(object sender, EventArgs e)
    {
        if (bussy)
            return;
        bussy = true;

        getPawnFromQueAndSetAsATarget();
    }

    private bool getPawnFromQueAndSetAsATarget()
    {
        Pawn pawn = nurseManager.RemovePawnFromQue();

        if (pawn)
        {
            _aiDestinationSetter.target = pawn.transform;
            return true;
        }

        return false;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Pacjent załatwiony");
            collision.gameObject.GetComponent<PawnController>().patientData.ResetToilet();
            collision.gameObject.GetComponent<PawnController>().patientData.ResetHygiene();
            collision.gameObject.GetComponent<PawnController>().patientData.ResetHunger();

            if (!getPawnFromQueAndSetAsATarget())
            {
                _aiDestinationSetter.target = restRoom.transform;
                bussy = false;
            }
        }
    }
}