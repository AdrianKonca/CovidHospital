using System;
using System.Collections;
using System.Collections.Generic;
using Entity;
using UnityEngine;

public class ShowerScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PatientController>().patientData.ResetHygiene();
            collision.gameObject.GetComponent<PatientController>().ReturnToBed();
        }
    }
}