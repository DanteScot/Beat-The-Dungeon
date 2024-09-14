using UnityEngine;
using UnityEngine.AI;

// Gestisce il comportamento del nemico volante
public class FlyingEnemyController : Enemy
{
    private Transform player;
    private NavMeshAgent agent;

    [SerializeField] private float speed;

    // Inizializza l'agente di navigazione ed il bersaglio (il giocatore)
    void Start()
    {
        player = PlayerManager.Instance.GetPlayer();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        agent.speed = speed;
    }

    // Aggiorna la posizione bersaglio del nemico in base alla posizione del giocatore
    // Imposta anche la direzione dello sprite del nemico
    void Update()
    {
        if(!isActive) return;

        agent.SetDestination(player.position);
        if(player.position.x<transform.position.x)
            transform.localScale = new Vector3(-1,1,1);
        else
            transform.localScale = new Vector3(1,1,1);
    }

    // Veniva usato per far attaccare appena il nemico toccava il giocatore
    // Ho preferito mantenere il fatto che il nemico attacchi solo quando parte il beat
    //
    // void OnCollisionEnter2D(Collision2D other)
    // {
    //     if(other.gameObject.CompareTag("Player")){
    //         other.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
    //         SetBeatToWait(1);
    //     }
    // }

    // Se il nemico è in contatto con il giocatore e può attaccare, infligge danno al giocatore
    void OnCollisionStay2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Player") && canAttack){
            other.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
            SetBeatToWait(1);
        }
    }
}
