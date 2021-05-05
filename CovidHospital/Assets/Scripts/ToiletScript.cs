using System;
using System.Collections;
using System.Collections.Generic;
using Entity;
using UnityEngine;

public class ToiletScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("XD");
            collision.gameObject.GetComponent<PawnController>().patientData.ResetToilet();
            collision.gameObject.GetComponent<PawnController>().ReturnToBed();
        }
    }
}