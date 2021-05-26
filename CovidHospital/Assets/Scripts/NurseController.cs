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
    private Pawn p;

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
            if (collision.gameObject.GetComponent<PawnController>() != p)
                return;


            PawnController dupa = collision.gameObject.GetComponent<PawnController>();
            dupa.patientData.ResetToilet();
            dupa.patientData.ResetHygiene();
            dupa.patientData.ResetHunger();
            dupa.requestForPepeSend = false;
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