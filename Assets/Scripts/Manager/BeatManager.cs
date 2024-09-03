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

    public float TimeBetweenBeats {get {return 60/bpm;}}
    
    // private Interval[] intervals;
    private Interval interval;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // intervals = new Interval[3];
            // intervals[0] = new Interval(1);
            // intervals[1] = new Interval(0.5f);
            // intervals[2] = new Interval(0.25f);
            interval = new Interval(1);

            if(debug){
                bpmText.gameObject.SetActive(true);
                // intervals[0].AddListener(UpdateBPMText);
                interval.AddListener(UpdateBPMText);
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
        Application.targetFrameRate = 60;
    }

    private void Update()
    {
        float sampledTime = audioSource.timeSamples / (audioSource.clip.frequency * interval.GetIntervalLength(bpm));
        interval.CheckForNewInterval((int)sampledTime);
        // foreach(Interval interval in intervals)
        // {
        //     float sampledTime = audioSource.timeSamples / (audioSource.clip.frequency * interval.GetIntervalLength(bpm));
        //     interval.CheckForNewInterval(sampledTime);
        // }
    }

    /// <summary>
    /// 0 = 1/4 (4 calls per beat),
    /// 1 = 2/4 (2 calls per beat),
    /// 2 = 4/4 (1 call per beat)
    /// </summary>
    public Interval GetInterval()
    {
        return interval;
    }
    // public Interval GetInterval(int index)
    // {
    //     return intervals[index];
    // }

    public void UpdateBPMText()
    {
        bpmText.text = ((tmp%4)+1).ToString();
        tmp++;
    }

    public void SetBPM(float bpm)
    {
        this.bpm = bpm;
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

    public void CheckForNewInterval(int interval)
    {
        if(interval != LastInterval)
        {
            LastInterval = interval;
            // Debug.Log("New Interval: " + interval + "; LastInterval: " + LastInterval);
            beatEvent.Invoke();
        }
    }

    public void AddListener(UnityAction action)
    {
        beatEvent.AddListener(action);
    }

    public void RemoveListener(UnityAction action)
    {
        beatEvent.RemoveListener(action);
    }
}