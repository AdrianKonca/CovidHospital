using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NurseController : MonoBehaviour
{
    public NurseManager nurseManager;

    private void Awake()
    {
        nurseManager.OnEnqueue+= NurseManagerOnEnqueue;
    }

    private void NurseManagerOnEnqueue(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }
}


