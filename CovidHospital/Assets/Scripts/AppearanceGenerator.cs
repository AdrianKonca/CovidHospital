using Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

public enum BodyPart
{
    Hair,
    Head,
    Body
}

public enum Direction
{
    Front,
    Back,
    Right,
    Left
}

public class AppearanceGenerator : MonoBehaviour
{
    public GameObject PatientPrefab;
    public GameObject DoctorPrefab;
    public GameObject NursePrefab;

    public MapController MapController;
    public TimeController TimeController;
    public NurseManager NurseManager;

    // Update is called once per frame
    private bool _loaded = false;

    void Update()
    {
        if (SpriteManager.AllSpritesLoaded && _loaded == false)
        {
            _loaded = true;
            Patient();
        }
    }

    public void Patient()
    {
        var patient = Instantiate(PatientPrefab);
        var pc = patient.GetComponent<PawnController>();
        
        pc.Initialize(Role.Patient, TimeController, NurseManager);
        pc.bed = MapController.GetClosestFreeFurniture("Bed", transform.position);
        pc.bed.GetComponent<FurnitureController>().owner = pc;
        pc.ReturnToBed();

        // todo activate this elements when map building supports misc. items
        //##
        // pc.toilet = MapController.GetClosestFurniture("Toilet", pc.bed.transform.position);
        // pc.canteen = MapController.GetClosestFurniture("???", pc.bed.transform.position);
        // pc.shower = MapController.GetClosestFurniture("Shower", pc.bed.transform.position);
        //##
    }

    public void Nurse()
    {
        var nurse = Instantiate(NursePrefab);
        var nurseController = nurse.GetComponent<NurseController>();
        nurseController.Initialize(NurseManager, MapController.GetClosestFurniture("Sofa", transform.position));
    }
}