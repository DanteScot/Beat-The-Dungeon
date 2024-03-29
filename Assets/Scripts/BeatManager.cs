using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class BeatManager : MonoBehaviour
{
    [SerializeField] private float bpm;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Intervals[] intervals;
    [SerializeField] private TextMeshProUGUI textMeshPro;
    private int tmp=0;

    private bool isStarted=false;

    private void Update()
    {
        foreach(Intervals interval in intervals)
        {
            float sampledTime = audioSource.timeSamples / (audioSource.clip.frequency * interval.GetIntervalLength(bpm));
            interval.CheckForNewInterval(sampledTime);
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(isStarted)
            {
                audioSource.Pause();
            }
            else
            {
                audioSource.Play();
            }
            isStarted = !isStarted;
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
    [SerializeField] private float steps;
    [SerializeField] private UnityEvent trigger;

    private int LastInterval;

    public float GetIntervalLength(float bpm)
    {
        return 60 / (bpm * steps);
    }

    public void CheckForNewInterval(float interval)
    {
        if(Mathf.FloorToInt(interval) != LastInterval)
        {
            LastInterval = Mathf.FloorToInt(interval);
            trigger.Invoke();
        }
    }
}