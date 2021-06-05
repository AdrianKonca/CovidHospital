using Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OpenStats : MonoBehaviour
{
    PatientController ctrl = null ;
    Text txt;
    public GameObject stats;
    private void Start()
    {
        if (stats.activeSelf)
            stats.SetActive(false);
    }
    private void Update()
    {
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
            SetValue(3, covid);
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

    private void SetValue(int slider, float value)
    {
        stats.transform.GetChild(slider).GetChild(1).GetComponent<Slider>().maxValue = 100;
        stats.transform.GetChild(slider).GetChild(1).GetComponent<Slider>().value = value;
        stats.transform.GetChild(slider).GetChild(1).GetChild(1).GetChild(0).GetComponent<Image>().color = Color.Lerp(Color.red, Color.green, value/100);
        stats.transform.GetChild(slider).GetChild(1).GetChild(2).GetComponent<Text>().text = ((int)value).ToString() + " %";
    }

    public void CloseWindow()
    {
        ctrl = null;
        stats.SetActive(false);
    }
}

