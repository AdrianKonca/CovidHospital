using System;
using System.Collections.Generic;
using Entity;
using UnityEngine;

public class NurseManager : MonoBehaviour
{
    public Queue<Pawn> patientQueue;
    public event EventHandler OnEnqueue;

    public void Awake()
    {
        patientQueue = new Queue<Pawn>();
    }

    public void AddPawnToQue(Pawn pawn)
    {
        if (patientQueue.Contains(pawn))
            return; 

        patientQueue.Enqueue(pawn);
        OnEnqueue?.Invoke(this, EventArgs.Empty);
    }

    public Pawn RemovePawnFromQue()
    {
        if (patientQueue.Count > 0)
            return patientQueue.Dequeue();

        return null;
    }
}