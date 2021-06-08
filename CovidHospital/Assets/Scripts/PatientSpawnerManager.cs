using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using Random = UnityEngine.Random;

[Serializable]
public class Wave
{
    public enum WaveType
    {
        Admissions,
        Cases
    };

    //[JsonProperty("eighty_min_score")]
    public WaveType Type;
    [JsonProperty("country_name")] public string CountryName;
    [JsonProperty("country_iso")] public string CountryIso;
    [JsonProperty("wave_start")] public DateTime Start;
    [JsonProperty("wave_end")] public DateTime Stop;
    [JsonProperty("cases")] public Dictionary<DateTime, double> Data;
    [JsonProperty("wave_number")] public int WaveNumber;
    [JsonProperty("average")] public float Average;
}

public class PatientSpawnerManager : MonoBehaviour
{
    public TimeController TimeController;
    public PawnFactory PawnFactory;
    public TextAsset HospitalAdmissionsByCountry;
    public TextAsset CasesByCountry;
    public List<Wave> CasesWaves;
    public List<Wave> AdmissionWaves;
    public string CountryIso;
    public int WaveNumber;
    public Wave.WaveType WaveType;
    public Vector3 SpawnPoint;
    private Wave _currentWave;

    public int CasesDenominator = 50;
    public int AdmissionsDenominator = 5;
    public event OnPandemicEndDelegate OnPandemicEnd;

    public delegate void OnPandemicEndDelegate();

    public int spawnedPatients;
    public int deadPatients;
    public int curedPatients;

    public void Awake()
    {
        CasesWaves = JsonConvert.DeserializeObject<List<Wave>>(CasesByCountry.text);
        AdmissionWaves = JsonConvert.DeserializeObject<List<Wave>>(HospitalAdmissionsByCountry.text);
        CasesWaves.ForEach(w => w.Type = Wave.WaveType.Cases);
        AdmissionWaves.ForEach(w => w.Type = Wave.WaveType.Admissions);

        //temp
        _currentWave = GetWave(CountryIso, WaveNumber, Wave.WaveType.Cases);
        TimeController.OnDayIncrease += DailySpawnPatients;
    }

    public void DailySpawnPatients(long d)
    {
        var currentPandemicDay = _currentWave.Start + new TimeSpan((int) d, 0, 0, 0);
        if (currentPandemicDay > _currentWave.Stop)
        {
            OnPandemicEnd?.Invoke();
        }

        var cases = _currentWave.Data[currentPandemicDay];
        var patientsToSpawn = (int) cases / CasesDenominator;
        for (int i = 0; i < patientsToSpawn; i++)
        {
            PawnFactory.Patient(SpawnPoint + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f));
        }

        spawnedPatients += patientsToSpawn;

        CheckStatistics();
    }

    private void CheckStatistics()
    {
        if (spawnedPatients < 100)
        {
            return;
        }

        if (deadPatients / spawnedPatients >= 0.8)
        {
            
        }
    }

    public Wave GetWave(string countryIso, int waveNumber, Wave.WaveType waveType)
    {
        return GetWavesByType(waveType)
            .SingleOrDefault(w => w.CountryIso == countryIso && w.WaveNumber == waveNumber);
    }

    public List<Wave> GetWavesByType(Wave.WaveType waveType)
    {
        switch (waveType)
        {
            case Wave.WaveType.Admissions:
                return AdmissionWaves;
            case Wave.WaveType.Cases:
                return CasesWaves;
        }

        return null;
    }
}