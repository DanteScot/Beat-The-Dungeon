using System.Collections.Generic;
using UnityEngine;

// Classe che gestisce il comportamento del range attack del player
public class BulletController : MonoBehaviour
{
    // lista di poteri che il proiettile possiede
    private List<string> powers;

    public float speed;
    public float damage;
    public float range;

    // variabili utilizzate da eventuali powerups
    #region PowerUps Variables

    public int remainingBounces = 1;
    public Enemy closestEnemy = null;
    public Enemy lastEnemy = null;
    public Color bulletColor = new(0, 0, 0);

    #endregion

    // Imposta i parametri del proiettile ed emette l'evento di sparo
    public void SetBullet(float speed, float damage, float range, Vector2 velocity, List<string> powers)
    {
        this.speed = speed;
        this.damage = damage;
        this.range = range;
        GetComponent<Rigidbody2D>().velocity = velocity;
        this.powers = powers;

        InitList();

        Messenger.Broadcast(GameEvent.BULLET_SHOOT);
    }

    // Se closestenemy è null proseguo dritto, altrimenti mi muovo verso closestenemy (ruotando il proiettile)
    void FixedUpdate()
    {
        if (closestEnemy != null)
        {
            Vector2 direction = (closestEnemy.transform.position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime, Space.World);

            // Rotate the bullet to face the closest enemy
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
        }
        else
        {
            transform.Translate(Vector2.up * speed * Time.deltaTime);
        }

        // Se il proiettile ha raggiunto la sua distanza massima, lo distruggo
        range -= speed * Time.deltaTime;
        if (range <= 0)
        {
            Destroy(gameObject);
        }
    }

    // Logica di collisione del proiettile
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall")) Destroy(gameObject);
        if (other.CompareTag("Obstacle")) {
            // TODO: Gestire proiettili perforanti
            other.GetComponent<ObstacleController>().Hit();
            Destroy(gameObject);
        }

        Enemy enemy =other.GetComponent<Enemy>();
        if(enemy!=null){
            // Se l'enemy è lo stesso dell'ultima collisione, non lo colpisco
            if(lastEnemy!=null && lastEnemy.Id==enemy.Id) return;

            lastEnemy=enemy;

            enemy.TakeDamage(damage);

            // Applico gli effetti dei poteri
            foreach (string power in powers)
            {
                Power.OnHit(power, this, enemy);
            }

            // Si assicura che tutte le condizioni dei poteri siano soddisfatte
            // Il proiettile viene distrutto solo se tutti i poteri hanno soddisfatto le condizioni
            // Se anche solo uno non le soddisfa, il proiettile continua a vivere
            // L'unica eccezione è quando il proiettile eccede il range massimo
            bool destroy = true;
            foreach (string power in powers)
            {
                if (!Power.CanDestroy(power, this))
                {
                    destroy = false;
                    break;
                }
            }

            if (destroy) Destroy(gameObject);
        }
    }

    // Inizializza ogni potere presente nella lista
    void InitList()
    {
        foreach (string power in powers)
        {
            Power.Init(power, this);
        }
    }
}
