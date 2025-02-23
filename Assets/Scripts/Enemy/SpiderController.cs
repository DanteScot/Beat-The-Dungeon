using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// Gestisce il comportamento del ragno ragno
public class SpiderController : Enemy
{
    private Transform player;
    private NavMeshAgent agent;

    [SerializeField] private float walkRadius;
    [SerializeField] private float speed;

    private Vector2 targetPosition;
    private Animator animator;
    private bool isMoving;

    // Controllo se il giocatore è nel raggio di movimento del ragno
    private bool isPlayerInRange{
        get{
            return Vector2.Distance(player.position, transform.position) < walkRadius;
        }
    }

    // Inizializza l'agente di navigazione ed il bersaglio (il giocatore)
    void Start(){
        player = PlayerManager.Instance.GetPlayer();
        isMoving = false;

        animator = GetComponent<Animator>();
        
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = speed;
    }

    // Se il giocaotre è nel raggio di movimento del ragno, il ragno si muove verso il giocatore
    // Altrimenti si muove in una direzione casuale
    void Update(){
        if(!isActive) return;

        if(!isMoving){
            if(isPlayerInRange){
                targetPosition = player.position;
            }
            else 
            {
                Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
                randomDirection += transform.position;
                NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, walkRadius, 1);
                targetPosition = hit.position;
            }
            agent.SetDestination(targetPosition);
            StartCoroutine(Wait());
            StartCoroutine(Move());
        }
    }

    // Aspetta 2 secondi prima di poter muovere nuovamente il ragno
    private IEnumerator Wait()
    {
        isMoving = true;
        yield return new WaitForSeconds(2);
        isMoving = false;
    }

    // Controlla se il ragno si sta muovendo
    // Se si muove, imposta l'animazione di movimento
    // Se la posizione del ragno non cambia, il ragno viene considerato fermo
    private IEnumerator Move()
    {
        animator.SetBool("isMoving", true);
        Vector3 lastPosition = transform.position;
        yield return new WaitForSeconds(0.1f);
        while(isMoving){
            if(lastPosition == transform.position){
                break;
            }else{
                lastPosition = transform.position;
            }
            yield return new WaitForSeconds(0.1f);
        }
        animator.SetBool("isMoving", false);
    }

    // Veniva usato per far attaccare appena il ragno toccava il giocatore
    // Ho preferito mantenere il fatto che il ragno attacchi solo quando parte il beat
    //
    // void OnCollisionEnter2D(Collision2D other)
    // {
    //     if(other.gameObject.CompareTag("Player")){
    //         other.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
    //         SetBeatToWait(1);
    //     }
    // }

    // Se il ragno è in contatto con il giocatore e può attaccare, infligge danno al giocatore
    void OnCollisionStay2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Player") && canAttack){
            other.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
            SetBeatToWait(1);
        }
    }
}