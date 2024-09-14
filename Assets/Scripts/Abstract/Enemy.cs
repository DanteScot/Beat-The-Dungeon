using System;
using System.Collections;
using UnityEngine;

// Classe astratta per i nemici, tutti i nemici devono ereditare da questa classe
public class Enemy : RythmedObject
{
    [Header("Common Stats")]
    [SerializeField] protected float health;
    [SerializeField] protected float damage;
    public bool isActive=false;

    // Id usato per identificare il nemico, utile per il sistema di rimbalzo dei proiettili
    protected Guid id = Guid.NewGuid();
    public Guid Id { get => id; }

    protected bool canAttack = false;
    protected bool isAttacking = false;
    protected bool isDying = false;

    // Semplice sistema per far si che il nemico sia leggermente rosso quando è in stato di "fuoco"
    private Color currentColor = Color.white;
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


    // Metodo che viene chiamato ogni beat
    // Usa beatToWait per evitare che il nemico attacchi troppo spesso
    public override void Trigger()
    {
        if(beatToWait>0){
            beatToWait--;
            return;
        }

        StartCoroutine(AttackWindow());
    }

    // Verrà usato dai singoli nemici per settare il tempo di attesa tra un attacco e l'altro
    // Utile per gestire nemici più forti che attaccano meno frequentemente
    protected void SetBeatToWait(int beat){
        canAttack = false;
        beatToWait = beat;
    }

    // Il nemico attacca solo nel frame dove parte il beat
    private IEnumerator AttackWindow()
    {
        canAttack = true;
        yield return new WaitForFixedUpdate();
        canAttack = false;
    }
    
    public virtual void TakeDamage(float damage){
        if(isDying) return;

        health-=damage;

        Messenger.Broadcast(GameEvent.ENEMY_HIT);

        if(health<=0) Die();

        StartCoroutine(Blink());
    }

    // Metodo per far lampeggiare il nemico quando viene colpito
    IEnumerator Blink(){
        for(int i=0; i<3; i++){
            GetComponent<SpriteRenderer>().color = Color.red;
            yield return new WaitForSeconds(0.1f);
            GetComponent<SpriteRenderer>().color = currentColor;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public virtual void Die(){
        StopAllCoroutines();
        Destroy(gameObject);
    }
}
