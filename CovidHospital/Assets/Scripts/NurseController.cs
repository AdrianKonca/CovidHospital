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

        Pawn p = nurseManager.RemovePawnFromQue();
        Debug.Log(p);
        if (p)
        {
            Debug.Log(p + transform.name);
            _aiDestinationSetter.target = p.transform;
            bussy = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PawnController>().patientData.ResetToilet();
            collision.gameObject.GetComponent<PawnController>().patientData.ResetHygiene();
            collision.gameObject.GetComponent<PawnController>().patientData.ResetHunger();
            collision.gameObject.GetComponent<PawnController>().requestForPepeSend = false;
            _aiDestinationSetter.target = restRoom.transform;
            bussy = false;
        }
    }
}