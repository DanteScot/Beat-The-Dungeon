using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BulletScript : MonoBehaviour
{
    private List<string> powers;
    // public bool isPlayerShooting;
    

    public float speed;
    public float damage;
    public float range;


    #region PowerUps Variables

    public int remainingBounces = 1;
    public Enemy closestEnemy = null;
    public Enemy lastEnemy = null;


    #endregion


    public void SetBullet(float speed, float damage, float range, Vector2 velocity, List<string> powers)
    {
        this.speed = speed;
        this.damage = damage;
        this.range = range;
        GetComponent<Rigidbody2D>().velocity = velocity;
        this.powers = powers;

        InitList();
    }

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

        range -= speed * Time.deltaTime;
        if (range <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var enemy=other.GetComponent<Enemy>();
        if(enemy!=null){
            if(lastEnemy!=null && lastEnemy.Id==enemy.Id) return;

            lastEnemy=enemy;

            enemy.TakeDamage(damage);

            foreach (string power in powers)
            {
                Power.OnHit(power, this, enemy);
            }

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

    void InitList()
    {
        foreach (string power in powers)
        {
            Power.Init(power, this);
        }
    }
}
