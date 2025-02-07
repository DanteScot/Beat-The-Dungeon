using UnityEngine;
using UnityEngine.AI;

// Classe che gestisce il comportamento di 808 al difuori della lobby
public class Controller808 : MonoBehaviour, Minion
{
    public static Controller808 Instance { get; private set; }

    [SerializeField] private RequiredNavMesh requiredNavMesh;
    public RequiredNavMesh RequiredNavMesh { get => requiredNavMesh; set => requiredNavMesh = value; }

    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    // Autmoaticamente setta l'animazione di 808 in base a se sta parlando o meno
    private bool isTalking = false;
    public bool IsTalking
    {
        get => isTalking;
        set {
            isTalking = value;
            if(animator!=null) animator.SetBool("isTalking", isTalking);
        }
    }


    [SerializeField] private float speed;

    // Imposta l'istanza di 808 attaccanfola al BeatManager
    void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);

        BeatManager.Instance.GetInterval().AddListener(OnBeat);

        animator = GetComponent<Animator>();
    }

    // Permette ad 808 di "brillare" al ritmo della musica
    void OnBeat()
    {
        try{
            if(!isTalking)
                animator.SetTrigger("Beat");
        } catch (System.Exception){}
    }

    void Start()
    {
        player = PlayerManager.Instance.GetPlayer();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        agent.speed = speed;
    }

    // Segue il player e si gira verso di lui
    void Update()
    {
        if(player == null){
            player = PlayerManager.Instance.GetPlayer();
            return;
        }

        if(agent.isOnNavMesh) agent.SetDestination(player.position);
        
        if(player.position.x<transform.position.x)
            transform.localScale = new Vector3(-1,1,1);
        else
            transform.localScale = new Vector3(1,1,1);
    }

    void OnDestroy()
    {
        BeatManager.Instance.GetInterval().RemoveListener(OnBeat);
    }
}
