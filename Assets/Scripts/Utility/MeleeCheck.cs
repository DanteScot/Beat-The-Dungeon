using UnityEngine;

// Semplice classe che controlla se hai colpito un nemico con un'arma da mischia
public class MeleeCheck : MonoBehaviour
{
    public float damage;

    public void OnTriggerEnter2D(Collider2D other){
        if(other.CompareTag("Enemy")){
            other.GetComponent<Enemy>().TakeDamage(damage);
        } else if(other.CompareTag("Obstacle")){
            other.GetComponent<ObstacleController>().Hit();
        }
    }
}
