using UnityEngine;

// Classe per il controllo delle trappole spinose
public class SpikeController : RythmedObject
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private float damage;

    private int phase;

    public void Start()
    {
        GetComponent<SpriteRenderer>().sprite = sprites[0];
        phase = 0;
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
    public override void Trigger()
    {
        phase = (phase + 1) % 3;

        GetComponent<SpriteRenderer>().sprite = sprites[phase];

        if (phase == 2) GetComponent<BoxCollider2D>().enabled = true;
        else GetComponent<BoxCollider2D>().enabled = false;
    }
}
