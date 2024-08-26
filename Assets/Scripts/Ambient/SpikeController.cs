using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeController : RythmedObject
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private float damage;

    private bool canGoNextPhase;
    private int phase;

    public new void Start()
    {
        base.Start();
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
        
        // if (phase == 0) canGoNextPhase = false;
        canGoNextPhase = false;
    }
}
