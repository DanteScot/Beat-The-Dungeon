using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Il cuore di tutto il gioco, il BeatManager è il manager che si occupa di gestire il tempo e i beat del gioco.
// Il BeatManager è un Singleton, quindi è presente in una sola istanza per tutto il gioco.
// OGNI oggetto che ha bisogno di essere sincronizzato con il tempo del gioco DEVE passare per il BeatManager.
// Per evitare problemi nei vari Awake questa classe è stat impostata nei settaggi di unity come la prima ad essere eseguita.
public class BeatManager : MonoBehaviour
{
    public static BeatManager Instance { get; private set; }

    public float BPM;
    public AudioSource AudioSource { get; private set; }
    public float TimeBetweenBeats { get {return 60/BPM;} }

    private UnityEvent beatEvent;
    private int lastInterval = 0;

    #region Debug
    // Numero in alto, giusto come debug per vedere il tempo
    [Header("Debug")]
    [SerializeField] private bool debug=true;
    [SerializeField] private TextMeshProUGUI bpmText;
    int tmp;

    // Funzione per aggiornare il testo di debug
    public void UpdateBPMText()
    {
        bpmText.text = ((tmp%4)+1).ToString();
        tmp++;
    }
    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            AudioSource = GetComponent<AudioSource>();
            beatEvent = new UnityEvent();

            if(debug){
                bpmText.gameObject.SetActive(true);
                AddListener(UpdateBPMText);
                tmp = 0;
            }
            else bpmText.gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        AudioSource.Play();
        Application.targetFrameRate = 60;
    }

    // Ogni frame controlla se è passato un beat
    private void Update()
    {
        float interval = AudioSource.timeSamples / (AudioSource.clip.frequency * (60 / BPM));

        if((int)interval != lastInterval)
        {
            lastInterval = (int)interval;
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
















// using TMPro;
// using UnityEngine;
// using UnityEngine.Events;

// // Il cuore di tutto il gioco, il BeatManager è il manager che si occupa di gestire il tempo e i beat del gioco.
// // Il BeatManager è un Singleton, quindi è presente in una sola istanza per tutto il gioco.
// // OGNI oggetto che ha bisogno di essere sincronizzato con il tempo del gioco DEVE passare per il BeatManager.
// // Per evitare problemi nei vari Awake questa classe è stat impostata nei settaggi di unity come la prima ad essere eseguita.
// public class BeatManager : MonoBehaviour
// {
//     public static BeatManager Instance { get; private set; }

//     [SerializeField] private float bpm;
//     public AudioSource AudioSource { get; private set; }

//     // Numero in alto, giusto come debug per vedere il tempo
//     [Header("Debug")]
//     [SerializeField] private bool debug=true;
//     [SerializeField] private TextMeshProUGUI bpmText;
//     int tmp;

//     // Tempo tra un beat e l'altro
//     public float TimeBetweenBeats {get {return 60/bpm;}}
    
//     /* 
//     Tutto il codice relativo a Interval[] è stato commentato, è la versione originale
//     era forse più intuitivo in quanto divideva gli oggetti in base al beat dove venivano chiamati (1/4, 2/4, 4/4)
//     tuttavia questo li rendeva costantemente sincronizzati nello stesso gruppo e non permetteva di fare cose come
//     le spine che si attivano in differita creando così il percorso a tempo.
//     Rimangono commentati per eventuali futuri utilizzi.
//     */
//     // private Interval[] intervals;

//     private Interval interval;

//     private void Awake()
//     {
//         if (Instance == null)
//         {
//             Instance = this;

//             AudioSource = GetComponent<AudioSource>();

//             // intervals = new Interval[3];
//             // intervals[0] = new Interval(1);
//             // intervals[1] = new Interval(0.5f);
//             // intervals[2] = new Interval(0.25f);
//             interval = new Interval(1); // (1) significa ogni beat

//             if(debug){
//                 bpmText.gameObject.SetActive(true);
//                 // intervals[0].AddListener(UpdateBPMText);
//                 interval.AddListener(UpdateBPMText);
//                 tmp = 0;
//             }
//             else
//                 bpmText.gameObject.SetActive(false);
//         }
//         else
//         {
//             Destroy(gameObject);
//         }
//     }

//     private void Start()
//     {
//         AudioSource.Play();
//         // Per semplicità nella gestione del tempo, il gioco è stato impostato a 60fps
//         Application.targetFrameRate = 60;
//     }

//     // Ogni frame controlla se è passato un beat
//     private void Update()
//     {
//         float sampledTime = AudioSource.timeSamples / (AudioSource.clip.frequency * interval.GetIntervalLength(bpm));
//         interval.CheckForNewInterval((int)sampledTime);
//         // foreach(Interval interval in intervals)
//         // {
//         //     float sampledTime = audioSource.timeSamples / (audioSource.clip.frequency * interval.GetIntervalLength(bpm));
//         //     interval.CheckForNewInterval(sampledTime);
//         // }
//     }

//     public Interval GetInterval()
//     {
//         return interval;
//     }
//     /// <summary>
//     /// 0 = 1/4 (4 calls per beat),
//     /// 1 = 2/4 (2 calls per beat),
//     /// 2 = 4/4 (1 call per beat)
//     /// </summary>
//     // public Interval GetInterval(int index)
//     // {
//     //     return intervals[index];
//     // }

//     // Funzione per aggiornare il testo di debug
//     public void UpdateBPMText()
//     {
//         bpmText.text = ((tmp%4)+1).ToString();
//         tmp++;
//     }

//     public void SetBPM(float bpm)
//     {
//         this.bpm = bpm;
//     }
//     public float GetBPM()
//     {
//         return bpm;
//     }
// }

// // Classe Interval, gestisce le classi che devono essere chiamate ad un determinato intervallo di tempo
// public class Interval
// {
//     // Evento che viene chiamato al raggiungimento dell'intervallo
//     private UnityEvent beatEvent;
//     private float steps;
//     private int LastInterval = 0;

//     public Interval(float steps){
//         this.steps = steps;
//         beatEvent = new UnityEvent();
//     }


//     public float GetIntervalLength(float bpm)
//     {
//         return 60 / (bpm * steps);
//     }

//     // Controlla se è passato un intervallo, se è passato chiama l'evento
//     public void CheckForNewInterval(int interval)
//     {
//         if(interval != LastInterval)
//         {
//             LastInterval = interval;
//             // Debug.Log("New Interval: " + interval + "; LastInterval: " + LastInterval);
//             beatEvent.Invoke();
//         }
//     }

//     public void AddListener(UnityAction action)
//     {
//         beatEvent.AddListener(action);
//     }

//     public void RemoveListener(UnityAction action)
//     {
//         beatEvent.RemoveListener(action);
//     }
// }