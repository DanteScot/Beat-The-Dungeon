using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class BeatManager : MonoBehaviour
{
    public static BeatManager Instance { get; private set; }

    [SerializeField] private float bpm;
    [SerializeField] private AudioSource audioSource;

    [Header("Debug")]
    [SerializeField] private bool debug=true;
    [SerializeField] private TextMeshProUGUI bpmText;
    int tmp;
    
    private Interval[] intervals;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            intervals = new Interval[3];
            intervals[0] = new Interval(1);
            intervals[1] = new Interval(0.5f);
            intervals[2] = new Interval(0.25f);

            if(debug){
                bpmText.gameObject.SetActive(true);
                intervals[0].AddListener(UpdateBPMText);
                tmp = 0;
            }
            else
                bpmText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("BeatManager already exists, destroying new one");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        audioSource.Play();
    }

    private void Update()
    {
        foreach(Interval interval in intervals)
        {
            float sampledTime = audioSource.timeSamples / (audioSource.clip.frequency * interval.GetIntervalLength(bpm));
            interval.CheckForNewInterval(sampledTime);
        }
    }

    /// <summary>
    /// 0 = 1/4 (4 calls per beat),
    /// 1 = 2/4 (2 calls per beat),
    /// 2 = 4/4 (1 call per beat)
    /// </summary>
    public Interval GetInterval(int index)
    {
        return intervals[index];
    }

    public void UpdateBPMText()
    {
        bpmText.text = ((tmp%4)+1).ToString();
        tmp++;
    }
}


public class Interval
{
    private UnityEvent beatEvent;
    private float steps;
    private int LastInterval = 0;

    public Interval(float steps){
        this.steps = steps;
        beatEvent = new UnityEvent();
    }


    public float GetIntervalLength(float bpm)
    {
        return 60 / (bpm * steps);
    }

    public void CheckForNewInterval(float interval)
    {
        if(Mathf.FloorToInt(interval) != LastInterval)
        {
            LastInterval = Mathf.FloorToInt(interval);
            beatEvent.Invoke();
        }
    }

    public void AddListener(UnityAction action)
    {
        beatEvent.AddListener(action);
    }
}