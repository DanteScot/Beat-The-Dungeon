using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public abstract class PickUp : MonoBehaviour
{
    public void OnValidate()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
        GetComponent<BoxCollider2D>().size = new Vector2(0.5f, 0.5f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PickUpItem();
            Destroy(gameObject);
        }
    }

    public abstract void PickUpItem();
}