using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DoorPosition
{
    UP,
    DOWN,
    LEFT,
    RIGHT
}


[RequireComponent(typeof(BoxCollider2D))]
public class DoorController : MonoBehaviour
{
    [SerializeField] private Sprite closedDoor;
    [SerializeField] private Sprite openDoor;
    [SerializeField] private DoorController linkedDoor;

    private bool isOpen = true;

    public void OpenDoor(){
        GetComponent<SpriteRenderer>().sprite = openDoor;
        isOpen = true;
    }

    public void CloseDoor(){
        GetComponent<SpriteRenderer>().sprite = closedDoor;
        isOpen = false;
    }

    public Vector3 GetSpawnPoint(){
        return transform.position+(transform.up*-2);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && isOpen)
        {
            other.transform.position = linkedDoor.GetSpawnPoint();
        }
    }
}
