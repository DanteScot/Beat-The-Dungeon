using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Classe per il controllo delle trappole spinose
public class SpikeController : RythmedObject
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private float damage;

    private bool canGoNextPhase;
    private int phase;

    public void Start()
    {
        GetComponent<SpriteRenderer>().sprite = sprites[0];
        phase = 0;
        canGoNextPhase = false;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().TakeDamage(damage);
        }
    }

    // Gestisce il cambio di fase della trappola
    // La trappola ha 3 fasi:
    // 0: trappola chiusa
    // 1: trappola in fase di apertura
    // 2: trappola aperta (danneggia il giocatore)
    // La trappola passa da una fase all'altra ogni due beat
    public override void Trigger()
    {
        if (!canGoNextPhase){
            canGoNextPhase = true;
            return;
        }

        phase = (phase + 1) % 3;

        GetComponent<SpriteRenderer>().sprite = sprites[phase];

        if (phase == 2) GetComponent<BoxCollider2D>().enabled = true;
        else GetComponent<BoxCollider2D>().enabled = false;
        
        // if (phase == 0) canGoNextPhase = false; // all'inizio solo la prima fase durava 2 beat
        canGoNextPhase = false;
    }
}
