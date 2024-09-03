using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlyingEnemy : Enemy
{
    private Transform player;
    private NavMeshAgent agent;

    [SerializeField] private float speed;

    void Start()
    {
        player = PlayerManager.Instance.GetPlayer();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        agent.speed = speed;
    }

    void Update()
    {
        if(!isActive) return;

        agent.SetDestination(player.position);
        if(player.position.x<transform.position.x)
            transform.localScale = new Vector3(-1,1,1);
        else
            transform.localScale = new Vector3(1,1,1);
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
