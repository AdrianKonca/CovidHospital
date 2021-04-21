using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.PlayerLoop;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Tilemaps;


public class TimeController : MonoBehaviour
{
    private float _dayLength = 120f;
    private float _currentTime;
    private long _dayCount;
    private bool _isDay;
    public GameObject Light;

    private void Awake()
    {
    }

    private void Update()
    {
        _currentTime += Time.deltaTime / _dayLength;
        Light.GetComponent<Light2D>().intensity = _currentTime * 10;
        Debug.Log(_currentTime);
    }
}