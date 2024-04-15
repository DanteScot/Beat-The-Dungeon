using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private float speed;
    private float damage;
    private float range;

    public void SetBullet(float speed, float damage, float range)
    {
        this.speed = speed;
        this.damage = damage;
        this.range = range;
    }

    void Update()
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
        // TODO: Add damage to enemy
    }
}
