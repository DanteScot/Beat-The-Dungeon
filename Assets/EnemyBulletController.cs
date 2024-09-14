using UnityEngine;

// """SEMPLICE""" classe che si occupa di gestire i proiettili sparati dai nemici
// Per i dettagli sulla sua semplicità, vedere il commento in JerryController.cs
public class EnemyBulletController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float damage;

    // Imposta la velocità, il danno e la grandezza del proiettile in base ai parametri passati e lo fa puntare verso il giocatore
    public void SetBullet(float speed, float damage, float sizeMultiplier, Vector3 player)
    {
        this.speed = speed;
        this.damage = damage;

        transform.localScale = Vector3.one * sizeMultiplier;

        Vector2 direction = (player - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
    }

    void FixedUpdate()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
