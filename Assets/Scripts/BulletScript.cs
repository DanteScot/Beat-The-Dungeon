using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BulletScript : MonoBehaviour
{
    public bool isPlayerShooting;
    
    private float speed;
    private float damage;
    private float range;

    public void SetBullet(float speed, float damage, float range, bool isPlayerShooting)
    {
        this.speed = speed;
        this.damage = damage;
        this.range = range;
        this.isPlayerShooting=isPlayerShooting;

        if(isPlayerShooting)
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"))/1.25f;
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
        if(isPlayerShooting){
            var enemy=other.GetComponent<Enemy>();
            if(enemy!=null){
                enemy.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
        else{
            var player=other.GetComponent<PlayerManager>();
            if(player!=null){
                player.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
        
    }
}
