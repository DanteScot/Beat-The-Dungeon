using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class BeatManager : MonoBehaviour
{
    [SerializeField] private float _bpm;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Intervals[] _intervals;
    [SerializeField] private TextMeshProUGUI textMeshPro;
    private int tmp=0;

    private void Update()
    {
        foreach(Intervals interval in _intervals)
        {
            float sampledTime = _audioSource.timeSamples / (_audioSource.clip.frequency * interval.GetIntervalLength(_bpm));
            interval.CheckForNewInterval(sampledTime);
        }
    }

    public void DebugLog()
    {
        tmp++;
        textMeshPro.text=((tmp%4)+1).ToString();
    }
}

[System.Serializable]
public class Intervals
{
    [SerializeField] private float _steps;
    [SerializeField] private UnityEvent _trigger;

    private int _LastInterval;

    public float GetIntervalLength(float bpm)
    {
        return 60 / (bpm * _steps);
    }

    public void CheckForNewInterval(float interval)
    {
        if(Mathf.FloorToInt(interval) != _LastInterval)
        {
            _LastInterval = Mathf.FloorToInt(interval);
            _trigger.Invoke();
        }
    }
}