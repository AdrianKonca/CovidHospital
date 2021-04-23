using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class TimeController : MonoBehaviour
{
    public event OnHourIncrease OnHourEvent;

    public event OnMinuteIncrease OnMinuteEvent;

    public event OnDayIncrease OnDayEvent;

    public event EventHandler OnDayEnter;

    public event EventHandler OnNightEnter;

    public delegate void OnHourIncrease(int h);

    public delegate void OnMinuteIncrease(int m);

    public delegate void OnDayIncrease(long d);

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

        OnHourEvent?.Invoke(hours);
        OnMinuteEvent?.Invoke(minutes);
    }

    private void CheckForNextDay()
    {
        var dayStart = 0.25;
        var dayEnd = 0.85;

        if (_currentTime >= 1)
        {
            _currentTime = 0;
            dayCount++;
            OnDayEvent?.Invoke(dayCount);
        }

        if (dayStart < _currentTime & _currentTime < dayEnd & !isDay)
        {
            isDay = true;
            OnDayEnter?.Invoke(this, EventArgs.Empty);
        }

        if (dayEnd < _currentTime & isDay)
        {
            isDay = false;
            OnNightEnter?.Invoke(this, EventArgs.Empty);
        }
    }

    private float GetLightIntensity()
    {
        float Intensity = (float) Math.Pow(Math.Sin((_currentTime - 0.05) * Math.PI), 2) * 1.1f;
        debugLight = Intensity;
        return Mathf.Clamp(Intensity, 0.2f, 1f);
    }
}