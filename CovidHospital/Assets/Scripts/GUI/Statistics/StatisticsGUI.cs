using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatisticsGUI : MonoBehaviour
{
    public GameObject statistics;



    public void Exit()
    {
        statistics.SetActive(false);
    }
}
