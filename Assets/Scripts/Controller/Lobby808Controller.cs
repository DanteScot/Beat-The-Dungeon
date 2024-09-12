using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using HeneGames.DialogueSystem;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class NPC_CentenceList
{
    public List<NPC_Centence> sentences = new List<NPC_Centence>();
}

public class Lobby808Controller : MonoBehaviour
{
    public static Lobby808Controller Instance { get; private set; }

    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float speed = 2;


    [Header("First Time Sentences")]
    [SerializeField] private NPC_CentenceList firstTimeSentences = new NPC_CentenceList();
    [Header("Random Sentences")]
    [SerializeField] private List<NPC_CentenceList> randomSentences = new List<NPC_CentenceList>();
    [Header("Can't Repeat Sentences")]
    [SerializeField] private List<NPC_CentenceList> doNotRepeatSentences = new List<NPC_CentenceList>();


    private Vector3 targetPosition;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool canMove = false;
    private int hasTalked;

    private bool isTalking = false;
    public bool IsTalking
    {
        get => isTalking;
        set {
            isTalking = value;
            animator.SetBool("isTalking", isTalking);
        }
    }


    void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);

        animator = GetComponent<Animator>();
        
        targetPosition = new Vector3(8f,-4,0);

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(255,255,255,0);

    }
    
    public void StartAnimation()
    {
        StartCoroutine(FadeIn());
    }

    public void NoAnimation()
    {
        transform.localScale = new Vector3(-transform.localScale.x,transform.localScale.y,1);
        transform.position = targetPosition;
        spriteRenderer.color = new Color(255,255,255,1);
    }

    void Start()
    {
        hasTalked = 0;

        if(GameEvent.ifFirstTime) SetSentences(firstTimeSentences);
        else {
            SetSentences(randomSentences[Random.Range(0, randomSentences.Count)]);
            StartCoroutine(Paganini());
        }
    }

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
        }
    }

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
        yield return new WaitUntil(() => hasTalked == 1 && !isTalking);
        for(int i=0; i<doNotRepeatSentences.Count; i++)
        {
            yield return new WaitUntil(() => hasTalked == i+1);
            SetSentences(doNotRepeatSentences[Random.Range(0, doNotRepeatSentences.Count)]);
        }
    }

    public void UpdateHasTalked()
    {
        hasTalked++;
    }

    void SetSentences(NPC_CentenceList list)
    {
        list.sentences[list.sentences.Count-1].sentenceEvent.AddListener(() => UpdateHasTalked());

        GetComponent<DialogueManager>().sentences = list.sentences;
    }
}
