using System;
using System.Collections.Generic;
using Entity;
using UnityEngine;

public class NurseManager : MonoBehaviour
{
    public Vector3 nurseSpawnPoint;
    public GameObject nurseSpawner;
    public Queue<Pawn> patientQueue;

    public void Awake()
    {
        patientQueue = new Queue<Pawn>();
        nurseSpawner.transform.position = nurseSpawnPoint;
    }

    public event EventHandler OnEnqueue;

    public void AddPawnToQue(Pawn pawn)
    {
        patientQueue.Enqueue(pawn);
        OnEnqueue?.Invoke(this, EventArgs.Empty);
    }

    public Pawn RemovePawnFromQue()
    {
        if (patientQueue.Count > 0)
        {
            var p = patientQueue.Dequeue();
            return p;
        }

        return null;
    }
}