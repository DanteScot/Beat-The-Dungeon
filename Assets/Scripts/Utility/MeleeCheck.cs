using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeCheck : MonoBehaviour
{
    public float damage;

    public void OnTriggerEnter2D(Collider2D other){
        if(other.CompareTag("Enemy")){
            other.GetComponent<Enemy>().TakeDamage(damage);
            // Debug.Log("melee");
        }
    }
}
