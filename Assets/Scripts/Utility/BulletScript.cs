using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BulletScript : MonoBehaviour
{
    public bool isPlayerShooting;
    
    private float speed;
    private float damage;
    private float range;


    public void SetBullet(float speed, float damage, float range, Vector2 velocity)
    {
        this.speed = speed;
        this.damage = damage;
        this.range = range;
        GetComponent<Rigidbody2D>().velocity = velocity;
    }

    void FixedUpdate()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
        range -= speed * Time.deltaTime;
        if(range <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var enemy=other.GetComponent<Enemy>();
        if(enemy!=null){
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
