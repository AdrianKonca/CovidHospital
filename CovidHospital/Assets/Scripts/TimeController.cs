using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class TimeController : MonoBehaviour
{
    public long dayCount;
    public bool isDay;
    public int hours;
    public int minutes;
    public float debugLight;

    private const float DayLength = 120f;
    private float _currentTime;

    public GameObject Light;
    private Light2D _light2D;

    private void Awake()
    {
        _light2D = Light.GetComponent<Light2D>();
    }

    private void Update()
    {
        _currentTime += Time.deltaTime / DayLength;
        CheckForNextDay();
        CalculateHour();
        _light2D.intensity = GetLightIntensity();
    }

    private void CalculateHour()
    {
        hours = (int) (24f * _currentTime);
        minutes = (int) (24f * _currentTime % 1 * 60);
    }

    private void CheckForNextDay()
    {
        if (_currentTime >= 1)
        {
            _currentTime = 0;
            dayCount++;
            NextDayAction();
        }

        if (0.25 < _currentTime & _currentTime < 0.85)
            isDay = true;
        else 
            isDay = false;
    }

    private float GetLightIntensity()
    {
        float Intensity = (float) Math.Pow(Math.Sin((_currentTime - 0.05) * Math.PI), 2) * 1.1f;
        debugLight = Intensity;
        return Mathf.Clamp(Intensity, 0.2f, 1f);
    }

    private void NextDayAction()
    {
    }
}