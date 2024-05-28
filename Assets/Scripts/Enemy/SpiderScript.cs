using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpiderScript : Enemy
{
    private Transform player;
    private NavMeshAgent agent;

    [SerializeField] private float walkRadius;
    [SerializeField] private float speed;

    private Vector2 targetPosition;
    private Animator animator;
    private bool isMoving;

    private bool isPlayerInRange{
        get{
            return Vector2.Distance(player.position, transform.position) < walkRadius;
        }
    }

    new void Start(){
        base.Start();

        player = PlayerManager.Instance.GetPlayer();
        isMoving = false;

        animator = GetComponent<Animator>();
        
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = speed;
    }

    void Update(){
        if(!isMoving){
            if(isPlayerInRange){
                targetPosition = player.position;
            }
            else 
            {
                Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
                randomDirection += transform.position;
                NavMeshHit hit;
                NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
                targetPosition = hit.position;
            }
            agent.SetDestination(targetPosition);
            StartCoroutine(Wait());
            StartCoroutine(Move());
        }
    }

    private IEnumerator Wait()
    {
        isMoving = true;
        yield return new WaitForSeconds(2);
        isMoving = false;
    }

    private IEnumerator Move()
    {
        animator.SetBool("isMoving", true);
        var lastPosition = transform.position;
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

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Player")){
            other.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
            SetBeatToWait(1);
        }
    }

    void OnCollisionStay2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Player") && canAttack){
            other.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
        }
    }
}