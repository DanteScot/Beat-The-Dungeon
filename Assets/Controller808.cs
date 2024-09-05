using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Controller808 : MonoBehaviour
{
    public static Controller808 Instance { get; private set; }

    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    private SpriteRenderer spriteRenderer;

    private bool isTalking = false;
    public bool IsTalking
    {
        get => isTalking;
        set {
            isTalking = value;
            animator.SetBool("isTalking", isTalking);
        }
    }

    [SerializeField] private float speed;

    void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);

        animator = GetComponent<Animator>();

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        player = PlayerManager.Instance.GetPlayer();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        agent.speed = speed;

        StartCoroutine(Wait());
    }

    IEnumerator Wait(){
        spriteRenderer.color = new Color(255,255,255,0);
        yield return new WaitUntil(()=>!GameEvent.isInLobby);
        spriteRenderer.color = new Color(255,255,255,1);
    }

    void Update()
    {
        agent.SetDestination(player.position);
        if(player.position.x<transform.position.x)
            transform.localScale = new Vector3(-1,1,1);
        else
            transform.localScale = new Vector3(1,1,1);
    }
}
