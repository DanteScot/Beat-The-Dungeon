using System;
using System.Collections;
using UnityEngine;



public class Enemy : RythmedObject
{
    [Header("Common Stats")]
    [SerializeField] protected float health;
    [SerializeField] protected float damage;

    protected Guid id = Guid.NewGuid();
    public Guid Id { get => id; }

    protected bool canAttack = false;
    protected bool isAttacking = false;

    private Color currentColor;
    private bool isOnFire = false;
    public bool IsOnFire {
        get => isOnFire;
        set {
            isOnFire = value;
            if (isOnFire) currentColor = new Color(1,.7f,.7f);
            else currentColor = Color.white;

            GetComponent<SpriteRenderer>().color = currentColor;
        }
    }

    public bool isActive=false;

    public override void Trigger()
    {
        if(beatToWait>0){
            beatToWait--;
            return;
        }

        StartCoroutine(AttackWindow());
    }

    private IEnumerator AttackWindow()
    {
        canAttack = true;
        yield return new WaitForFixedUpdate();
        canAttack = false;
    }
    
    public void TakeDamage(float damage){
        health-=damage;
        Debug.Log(damage);

        if(health<=0) Die();

        StartCoroutine(Blink());
    }

    IEnumerator Blink(){
        for(int i=0; i<3; i++){
            GetComponent<SpriteRenderer>().color = Color.red;
            yield return new WaitForSeconds(0.1f);
            GetComponent<SpriteRenderer>().color = currentColor;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void Die(){
        Destroy(gameObject);
    }

    protected void SetBeatToWait(int beat){
        beatToWait = beat;
    }
}
