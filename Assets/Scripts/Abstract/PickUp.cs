using UnityEngine;

// Classe astratta per gli oggetti che possono essere raccolti dal giocatore
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public abstract class PickUp : PulseToBeat
{
    public new void Start()
    {
        base.Start();
    }

    public void OnValidate()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
        GetComponent<BoxCollider2D>().size = new Vector2(0.5f, 0.5f);
    }

    // Se il giocatore entra in collisione con l'oggetto, lo raccoglie
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponentInChildren<Animator>().SetTrigger("PickUp");
            PickUpItem();
            Destroy(gameObject);
        }
    }

    // Metodo che verrà chiamato quando il giocatore raccoglie l'oggetto
    public abstract void PickUpItem();
}