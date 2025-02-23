using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using HeneGames.DialogueSystem;
using UnityEngine;

// TODO: RIMUOVERE FRASI INADATTE

// Lista di discorsi
[System.Serializable]
public class NPC_CentenceList
{
    public List<NPC_Centence> sentences = new();
}

// Classe che gestisce il comportamento di 808 nella lobby
public class Lobby808Controller : MonoBehaviour
{
    public static Lobby808Controller Instance { get; private set; }

    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float speed = 2;

    // Primo avvio del gioco
    [Header("First Time Sentences")]
    [SerializeField] private NPC_CentenceList firstTimeSentences = new();

    // Frasi casuali mostrate a partire dal secondo avvio del gioco / dopo aver fatto almeno una partita
    [Header("Random Sentences")]
    [SerializeField] private List<NPC_CentenceList> randomSentences = new();

    // Giusto per ridere, non ripete le frasi le frasi randomiche
    [Header("Can't Repeat Sentences")]
    [SerializeField] private List<NPC_CentenceList> doNotRepeatSentences = new();


    private Vector3 targetPosition;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCollider;

    private bool canMove = false;
    private int hasTalked;

    // Atuomaticamente settato quando si parla con 808
    private bool isTalking = false;
    public bool IsTalking
    {
        get => isTalking;
        set {
            isTalking = value;
            animator.SetBool("isTalking", isTalking);
        }
    }

    // All'inizio è invisibile
    void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);

        circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.enabled = false;

        animator = GetComponent<Animator>();
        
        targetPosition = new Vector3(8f,-4,0);

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(255,255,255,0);

    }
    
    // Animazione per la prima volta
    public void StartAnimation()
    {
        StartCoroutine(FadeIn());
    }

    // In caso non si voglia l'animazione (dopo il primo avvio)
    public void NoAnimation()
    {
        transform.localScale = new Vector3(-transform.localScale.x,transform.localScale.y,1);
        transform.position = targetPosition;
        spriteRenderer.color = new Color(255,255,255,1);
        circleCollider.enabled = true;
    }

    // Se è la prima volta setta le frasi di default, altrimenti setta frasi randomiche
    void Start()
    {
        hasTalked = 0;

        if(GameEvent.ifFirstTime) GetComponent<DialogueManager>().sentences = firstTimeSentences.sentences;
        else {
            GetComponent<DialogueManager>().sentences = randomSentences[Random.Range(0, randomSentences.Count)].sentences;
            StartCoroutine(Paganini());
        }
    }

    // Si dirige verso la posizione target
    void Update()
    {
        if(!canMove) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed*Time.deltaTime);

        if(Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            transform.localScale = new Vector3(-transform.localScale.x,transform.localScale.y,1);
            virtualCamera.Follow = PlayerManager.Instance.GetPlayer();
            canMove = false;
            GameEvent.canMove = true;
            circleCollider.enabled = true;
        }
    }

    // Animazione di fade in
    IEnumerator FadeIn()
    {
        virtualCamera.Follow = transform;
        for(float i=0; i<=1; i+=0.01f)
        {
            spriteRenderer.color = new Color(255,255,255,i);
            yield return new WaitForSeconds(0.01f);
        }
        canMove = true;
    }

    // Non ripete :)
    IEnumerator Paganini()
    {
        // Aspetta che abbia finito di parlare la prima volta
        yield return new WaitUntil(() => hasTalked == 1 && !isTalking);
        for(int i=0; i<doNotRepeatSentences.Count; i++)
        {
            // Dice una frase randomica le volte dopo
            yield return new WaitUntil(() => hasTalked == i+1);
            GetComponent<DialogueManager>().sentences = doNotRepeatSentences[Random.Range(0, doNotRepeatSentences.Count)].sentences;
            // SetSentences(doNotRepeatSentences[Random.Range(0, doNotRepeatSentences.Count)]);
        }
    }

    // Chiamato dall'evento di fine dialogo
    public void UpdateHasTalked()
    {
        hasTalked++;
    }

    // TODO: RIMUOVERE
    // Setta le frasi da dire automatizzando l'evento di fine dialogo
    // void SetSentences(NPC_CentenceList list)
    // {
    //     list.sentences[list.sentences.Count-1].sentenceEvent.AddListener(() => UpdateHasTalked());

    //     GetComponent<DialogueManager>().sentences = list.sentences;
    // }
}
