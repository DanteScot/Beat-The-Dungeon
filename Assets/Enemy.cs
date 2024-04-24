using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Enemy : RythmedObject
{
    [Header("Common Stats")]
    [SerializeField] private float health;
    [SerializeField] private float damage;

    protected bool canAttack = false;
    protected bool isAttacking = false;

    public new void Start()
    {
        base.Start();
    }

    public override void Trigger()
    {
        StartCoroutine(AttackWindow());
    }

    private IEnumerator AttackWindow()
    {
        canAttack = true;
        yield return new WaitForSeconds(0.1f);
        canAttack = false;
    }
    
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
