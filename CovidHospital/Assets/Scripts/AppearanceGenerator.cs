using Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

public enum BodyPart { Hair, Head, Body }
public enum Direction { Front, Back, Right, Left }

public class AppearanceGenerator : MonoBehaviour
{
    public GameObject PatientPrefab;
    public GameObject DoctorPrefab;
    public GameObject NursePrefab;

    public TimeController TimeController;
    public NurseManager NurseManager;

    // Update is called once per frame
    private bool _loaded = false;
    void Update()
    {
        if (SpriteManager.AllSpritesLoaded && _loaded == false)
        {
            _loaded = true;
            var patient = Instantiate(PatientPrefab);
            var pc = patient.GetComponent<PawnController>();
            pc.Initialize(Role.Patient, TimeController, NurseManager);
        }
    }
}
