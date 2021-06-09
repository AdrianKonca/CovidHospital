using Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OpenStats : MonoBehaviour
{
    public GameObject manager;
    public GameObject deathMarker;
    PatientController ctrl = null ;
    PatientSpawnerManager ctrlMark = null ;
    Text txt;
    public GameObject stats;
    private void Start()
    {
        if (stats.activeSelf)
            stats.SetActive(false);
        ctrlMark = manager.GetComponent<PatientSpawnerManager>();
    }
    private void Update()
    {
        //Death mark set
        if (ctrlMark.spawnedPatients > 100) 
        {
            SetSliderValue(ctrlMark.deadPatients / ctrlMark.spawnedPatients);
        }

        if (ctrl)
        {
            var hunger = ctrl.patientData.hunger;
            var hygiene = ctrl.patientData.hygiene;
            var toilet = ctrl.patientData.toilet;
            var covid = ctrl.patientData.covidProgress;
            var comfort = ctrl.patientData.comfort;
            SetValue(2, hunger);
            SetValue(5, hygiene);
            SetValue(6, toilet);
            SetValue(3, covid, true);
            SetValue(4, comfort);
        }
        
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit;
            if (hit = Physics2D.Raycast(mousePos2D, Vector2.zero))
            {
                if (hit.rigidbody)
                {
                    openStatistics(hit);
                }
            }
        }
    }

    private void openStatistics(RaycastHit2D hit)
    {
        if (!stats.activeSelf)
            stats.SetActive(true);

        txt = stats.transform.GetChild(1).GetComponent<Text>();

        string name = hit.transform.name;
        ctrl = hit.transform.gameObject.GetComponent<PatientController>();
        var age = ctrl.PawnData.age;
        txt.text = name + " (" + age + ")";
    }

    private void SetSliderValue(float val)
    {
        deathMarker.GetComponent<Slider>().value = val;
        deathMarker.GetComponent<Slider>().maxValue = 0.8f;
        deathMarker.GetComponent<Slider>().minValue = 0.0f;
        val = val * 100;
        deathMarker.gameObject.GetComponentInChildren<Text>().text = val.ToString() + " %";
    }

    private void SetValue(int slider, float value, bool sw = false)
    {

        stats.transform.GetChild(slider).GetChild(1).GetComponent<Slider>().maxValue = 100;
        stats.transform.GetChild(slider).GetChild(1).GetComponent<Slider>().value = value;
        if (sw)
            stats.transform.GetChild(slider).GetChild(1).GetChild(1).GetChild(0).GetComponent<Image>().color = Color.Lerp(Color.green, Color.red, value / 100);
        else 
            stats.transform.GetChild(slider).GetChild(1).GetChild(1).GetChild(0).GetComponent<Image>().color = Color.Lerp(Color.red, Color.green, value/100);
        stats.transform.GetChild(slider).GetChild(1).GetChild(2).GetComponent<Text>().text = ((int)value).ToString() + " %";
    }

    public void CloseWindow()
    {
        ctrl = null;
        stats.SetActive(false);
    }
}

