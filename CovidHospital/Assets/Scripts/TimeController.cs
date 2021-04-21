using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.PlayerLoop;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Tilemaps;


public class TimeController : MonoBehaviour
{
    private float _dayLength = 120f;
    private float _currentTime;
    private long dayCount;
    private bool _isDay;


    private void Update()
    {
        _currentTime += Time.deltaTime / _dayLength;
        Debug.Log(_currentTime);
    }
}