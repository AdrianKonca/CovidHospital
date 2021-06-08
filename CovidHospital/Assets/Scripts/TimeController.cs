using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class TimeController : MonoBehaviour
{
    public event OnHourIncreaseDelegate OnHourIncrease;

    public event OnMinuteIncreaseDelegate OnMinuteIncrease;

    public event OnDayIncreaseDelegate OnDayIncrease;

    public event EventHandler OnDayEnter;

    public event EventHandler OnNightEnter;

    public delegate void OnHourIncreaseDelegate(int h);

    public delegate void OnMinuteIncreaseDelegate(int m);

    public delegate void OnDayIncreaseDelegate(long d);

    public long dayCount;
    public bool isDay;
    public int hours;
    public int minutes;

    public float dayLength = 15f;
    private float _currentTime;

    public GameObject Light;
    private Light2D _light2D;

    static private TimeController _instance;
    static public TimeController Instance()
    {
        return _instance;
    }
    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError("One time controller already exists.");
        }
        _instance = this;
        _light2D = Light.GetComponent<Light2D>();
    }

    private void FixedUpdate()
    {
        _currentTime += Time.deltaTime / dayLength;
        CheckForNextDay();
        CalculateHour();
        _light2D.intensity = GetLightIntensity();
    }

    private void CalculateHour()
    {
        var OldHours = hours;
        var OldMinutes = minutes;
        hours = (int) (24f * _currentTime);
        minutes = (int) (24f * _currentTime % 1 * 60);

        if (OldHours != hours)
            OnHourIncrease?.Invoke(hours);
        if (OldMinutes != minutes)
            OnMinuteIncrease?.Invoke(minutes);
    }

    private void CheckForNextDay()
    {
        const double dayStart = 0.25;
        const double dayEnd = 0.85;

        if (_currentTime >= 1)
        {
            _currentTime = 0;
            dayCount++;
            OnDayIncrease?.Invoke(dayCount);
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
        return Mathf.Clamp(Intensity, 0.2f, 1f);
    }
}