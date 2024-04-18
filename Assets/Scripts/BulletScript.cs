using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BulletScript : MonoBehaviour
{
    public bool isContinuos;
    
    private float speed;
    private float damage;
    private float range;
    private float speedX;
    private float speedY;

    private Vector2 direction;

    public void SetBullet(float speed, float damage, float range, bool isContinuos, float speedX, float speedY)
    {
        this.speed = speed+=speedX;
        this.damage = damage;
        this.range = range;
        this.isContinuos=isContinuos;
        this.speedX=speedX;
        this.speedY=speedY;

        direction=Vector2.up;
        direction.y+=speedY;
        Debug.Log("bullet "+direction+" "+speed);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
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
            if(!isContinuos)
                Destroy(gameObject);
        }
    }
}
