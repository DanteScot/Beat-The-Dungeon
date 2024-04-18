using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Enemy : RythmedObject
{
    [Header("Common Stats")]
    [SerializeField] private float health;
    [SerializeField] private float speed;
    [SerializeField] private float damage;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackSpeed;
    
    public void TakeDamage(float damage){
        health-=damage;
        Debug.Log(damage);

        if(health<=0)
            Die();
    }

    public void Die(){
        Destroy(gameObject);
    }
}
