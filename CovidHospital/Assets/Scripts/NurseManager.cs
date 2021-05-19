using System;
using System.Collections.Generic;
using Entity;
using UnityEngine;

public class NurseManager : MonoBehaviour
{
    public Queue<Pawn> _patientQueue;
    public event EventHandler OnEnqueue;

    public NurseManager()
    {
        _patientQueue = new Queue<Pawn>();
    }

    public void AddPawnToQue(Pawn pawn)
    {
        _patientQueue.Enqueue(pawn);
        OnEnqueue?.Invoke(this, EventArgs.Empty);
    }

    public Pawn RemovePawnFromQue()
    {
        
        return _patientQueue.Dequeue();
    }
}