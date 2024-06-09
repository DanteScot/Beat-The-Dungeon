using System.Collections;
using UnityEngine;



public class Enemy : RythmedObject
{
    [Header("Common Stats")]
    [SerializeField] protected float health;
    [SerializeField] protected float damage;

    protected bool canAttack = false;
    protected bool isAttacking = false;

    public bool isActive=false;

    private int beatToWait = 0;

    public new void Start()
    {
        base.Start();
    }

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

        StartCoroutine(Blink());

        if(health<=0)
            Die();
    }

    IEnumerator Blink(){
        for(int i=0; i<3; i++){
            GetComponent<SpriteRenderer>().color = Color.red;
            yield return new WaitForSeconds(0.1f);
            GetComponent<SpriteRenderer>().color = Color.white;
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
